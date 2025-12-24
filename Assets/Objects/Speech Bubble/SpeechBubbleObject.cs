using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeechBubbleObject : FrameResizer
{
    // public readonly Vector2 Size = new Vector2(4f, 1.8f);
    [SerializeField] private AnimationCurve buttonResizeCurve = new(new(0, 0, 0, 0, 0, 0.5f), new(1, 1, 0, 0, 0.5f, 0));
    [SerializeField] private float buttonResizeInterval = 1f;
    [SerializeField] private float buttonResizeMax = 1.2f;
    [SerializeField] private Color buttonResizeColor = Color.orangeRed;
    [SerializeField] private TextMeshProUGUI textMesh;
    private Vector2 startSize = new Vector2(0, 1);
    private float letterInterval = 0.01f;
    private float blinkInterval = 0.4f;
    private MeshRenderer[] meshRenderers;
    private Color[] originalColors;


    void Awake()
    {
        Init();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalColors = meshRenderers.Select(x => x.material.GetColor("_TintColor")).ToArray();
    }

    public IEnumerator RunText(Vector3 at, string text, Vector3 size)
    {
        yield return Run(at, text, size);

        StartCoroutine(BlinkWaitingToBeClosed());
        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        Destroy(gameObject);
    }

    public IEnumerator RunButton(Vector3 at, string text, Vector3 size)
    {
        yield return Run(at, text, size);
    }

    public IEnumerator RunQuestion(Vector3 at, string text, Vector3 size)
    {
        yield return Run(at, text, size);
    }

    private IEnumerator Run(Vector3 at, string text, Vector3 size)
    {
        transform.position = at;
        var textMeshOriginalSize = textMesh.rectTransform.sizeDelta;
        textMesh.gameObject.SetActive(false);
        yield return ResizeCoroutine(startSize, size);
        textMesh.gameObject.SetActive(true);
        textMesh.rectTransform.sizeDelta = textMeshOriginalSize * size / 2;
        textMesh.text = string.Empty;

        var previousPageCount = 1;
        for (int i = 0; i < text.Length; i++)
        {
            textMesh.text += text[i];
            if (previousPageCount != Math.Max(1, textMesh.textInfo.pageCount))
            {
                var cor = StartCoroutine(BlinkWaitingToBeClosed());
                yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                SetColors(true);
                StopCoroutine(cor);
                textMesh.pageToDisplay = previousPageCount + 1;
            }
            previousPageCount = Math.Max(1, textMesh.textInfo.pageCount);
            yield return new WaitForSeconds(letterInterval);
        }
    }

    public void Focus()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkWaitingToBeChosen());
    }

    public void Blur()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.one;

        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.SetColor("_TintColor", originalColors[i]);
    }

    public void Select(Action then)
    {
        StopAllCoroutines();
        StartCoroutine(ButtonChosen(then));
    }

    private IEnumerator ButtonChosen(Action then)
    {
        var timer = 0f;
        var goalColor = buttonResizeColor;
        goalColor.a = 0;
        while (timer < buttonResizeInterval)
        {
            transform.localScale = Vector3.Lerp(buttonResizeMax * Vector3.one, buttonResizeMax * 2 * Vector3.one, timer / buttonResizeInterval);

            for (int i = 0; i < meshRenderers.Length; i++)
                meshRenderers[i].material.SetColor("_TintColor", Color.Lerp(buttonResizeColor, goalColor, timer / buttonResizeInterval));

            timer += BoardTime.DeltaTime;

            yield return 0;
        }

        then();
    }

    private IEnumerator BlinkWaitingToBeChosen()
    {
        var isOn = true;
        var timer = 0f;
        while (true)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, buttonResizeMax * Vector3.one, BoardTime.DeltaTime);

            for (int i = 0; i < meshRenderers.Length; i++)
                meshRenderers[i].material.SetColor("_TintColor", Color.Lerp(originalColors[i], buttonResizeColor, buttonResizeCurve.Evaluate(timer / buttonResizeInterval)));

            timer += BoardTime.DeltaTime;

            if (timer > buttonResizeInterval)
            {
                timer = 0;
                isOn = !isOn;
            }

            yield return 0;
        }
    }

    private IEnumerator BlinkWaitingToBeClosed()
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

    private void SetColors(bool on)
    {
        foreach (var renderer in meshRenderers)
        {
            var color = renderer.material.GetColor("_TintColor");
            color.a = on ? 1 : 0.5f;
            renderer.material.SetColor("_TintColor", color);
        }
    }
}
