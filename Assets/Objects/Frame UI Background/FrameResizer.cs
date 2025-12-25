using System.Collections;
using UnityEngine;

public class FrameResizer : ResizableMesh
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;

    void Awake()
    {
        Init();
    }

    protected void Init()
    {
        Preinit();

        SnapResize(0, Vector2.zero);
        SnapResize(1, Vector2.zero);
    }

    protected IEnumerator ResizeCoroutine(Vector2 from, Vector2 to)
    {
        var timer = 0f;

        SnapResize(0, from);
        SnapResize(1, from);

        yield return 0;

        if (from.x != to.x)
        {
            while (timer < animationTime)
            {
                SnapResize(0, from + curve.Evaluate(timer / animationTime) * (to - from));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }
        }

        if (from.y != to.y)
        {
            timer = 0f;

            while (timer < animationTime)
            {
                SnapResize(1, from + curve.Evaluate(timer / animationTime) * (to - from));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }

            SnapResize(1, to);
        }
    }
}