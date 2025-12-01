using System;
using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class FloatUI : MonoBehaviour
{
    public const float MaxDistance = 5f;
    public const float DirChangeTime = 1.5f;

    public float Offset { get; set; }

    private Vector3? originalPosition;
    private Coroutine coroutine;

    public void StartRunning()
    {
        originalPosition = transform.position;
        coroutine = StartCoroutine(Run());
    }

    public void ResetPosition()
    {
        if (originalPosition.HasValue)
            transform.position = originalPosition.Value;
    }

    private IEnumerator Run()
    {
        yield return 0;
        var originalPosition = transform.position;
        bool isComingBack = false;
        var timer = Offset * DirChangeTime;
        do
        {
            isComingBack = !isComingBack;
            var destination = originalPosition + MaxDistance * Vector3.up * (isComingBack ? -1 : 1);
            var start = transform.position;
            while (timer < DirChangeTime)
            {
                // transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime);
                transform.position = Vector3.Lerp(start, destination, timer / DirChangeTime);
                timer += Time.deltaTime;
                yield return 0;
            }
            timer = 0f;
        } while (true);
    }

    public void StopRunning()
    {
        StopCoroutine(coroutine);
    }
}