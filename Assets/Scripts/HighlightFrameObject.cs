using System.Collections;
using UnityEngine;

public class HighlightFrameObject : MonoBehaviour
{
    private readonly Color highlightColorFrom = new(1f, 0.8f, 0f, 1f);
    private readonly Color highlightColorTo = new(1f, 0.8f, 0f, 0f);
    private readonly float hightlightDuration = 0.5f;
    private readonly AnimationCurve highlightCurve = new(new(0, 0, 0, 0, 0, 0.5f), new(1, 1, 0, 0, 0.5f, 0));

    [SerializeField] private MeshRenderer highlightFrame;

    public void Start()
    {
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            var isFlipped = false;
            while (true)
            {
                var timer = 0f;
                while (timer < hightlightDuration)
                {
                    var curveValue = highlightCurve.Evaluate(timer / hightlightDuration);
                    highlightFrame.material.color = Color.Lerp(highlightColorFrom, highlightColorTo, isFlipped ? (1 - curveValue) : curveValue);
                    timer += Time.deltaTime;
                    yield return 0;
                }

                isFlipped = !isFlipped;
            }
        }
    }
}