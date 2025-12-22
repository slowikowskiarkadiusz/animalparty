using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SpeechBubbleObject : FrameResizer
{
    // public readonly Vector2 Size = new Vector2(4f, 1.8f);
    public readonly Vector2 Size = new Vector2(4f, 2f);
    [SerializeField] private TextMeshProUGUI textMesh;
    private Vector2 startSize = new Vector2(0, 1);
    private float letterInterval = 0.01f;
    private float blinkInterval = 0.4f;
    private MeshRenderer[] meshRenderers;

    void Awake()
    {
        Init();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public IEnumerator Run(Vector3 at, string text)
    {
        transform.position = at;
        var textMeshOriginalSize = textMesh.rectTransform.sizeDelta;
        textMesh.gameObject.SetActive(false);
        yield return ResizeCoroutine(startSize, Size);
        textMesh.gameObject.SetActive(true);
        textMesh.rectTransform.sizeDelta = textMeshOriginalSize * Size / 2;
        textMesh.text = string.Empty;

        var previousPageCount = 1;
        for (int i = 0; i < text.Length; i++)
        {
            textMesh.text += text[i];
            if (previousPageCount != Math.Max(1, textMesh.textInfo.pageCount))
            {
                var cor = StartCoroutine(Blink());
                yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                SetColors(true);
                StopCoroutine(cor);
                textMesh.pageToDisplay = previousPageCount + 1;
            }
            previousPageCount = Math.Max(1, textMesh.textInfo.pageCount);
            yield return new WaitForSeconds(letterInterval);
        }

        StartCoroutine(Blink());
        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        Destroy(gameObject);

        IEnumerator Blink()
        {
            yield return new WaitForSeconds(blinkInterval);
            var isOn = true;
            while (true)
            {
                SetColors(isOn);

                yield return new WaitForSeconds(blinkInterval);

                isOn = !isOn;
            }
        }

        void SetColors(bool on)
        {
            foreach (var renderer in meshRenderers)
            {
                var color = renderer.material.color;
                color.a = on ? 1 : 0.5f;
                renderer.material.color = color;
            }
        }
    }
}
