using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z,
}

public class Float : MonoBehaviour
{
    public float Offset { get; set; }
    public float MaxDistance => maxDistance;

    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float dirChangeTime = 1.5f;
    [SerializeField] private List<Axis> axes = new();
    [SerializeField] private bool runOnStart = false;
    [SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1) });
    private Vector3? originalPosition;
    private List<Coroutine> coroutines = new();

    private void Start()
    {
        if (runOnStart)
            StartRunning();
    }

    public void StartRunning()
    {
        originalPosition = transform.position;
        foreach (var axis in axes)
            coroutines.Add(StartCoroutine(Run(axis)));
    }

    public void ResetPosition()
    {
        if (originalPosition.HasValue)
            transform.position = originalPosition.Value;
    }

    private IEnumerator Run(Axis axis)
    {
        var vector = axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => throw new System.NotImplementedException(),
        };

        yield return 0;
        var originalPosition = transform.position;
        bool isComingBack = false;
        var timer = Offset * dirChangeTime;
        do
        {
            isComingBack = !isComingBack;
            var destination = originalPosition + maxDistance * vector * (isComingBack ? -1 : 1);
            var start = transform.position;
            while (timer < dirChangeTime)
            {
                // transform.position = Vector3.Lerp(transform.position, destination, BoardTime.DeltaTime);
                transform.position = Vector3.Lerp(start, destination, curve.Evaluate(timer / dirChangeTime));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }
            timer = 0f;
        } while (true);
    }

    public void StopRunning()
    {
        foreach (var coroutine in coroutines)
            StopCoroutine(coroutine);

        coroutines.Clear();
    }
}