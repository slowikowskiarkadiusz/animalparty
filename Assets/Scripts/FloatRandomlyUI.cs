using System;
using System.Collections;
using UnityEngine;

public class FloatRandomlyUI : MonoBehaviour
{
    public float MaxDistance = 20f;
    public float MoveSpeed = 2;
    public float MinDirChangeTime = 0.5f;
    public float MaxDirChangeTime = 2;

    public float Distance;

    private void Awake()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        var originalPosition = transform.position;
        do
        {
            Debug.Log("Change");
            bool isComingBack;
            Vector3 direction;
            if (Vector3.Distance(originalPosition, transform.position) > MaxDistance)
            {
                Debug.Log("Is Coming Back");
                isComingBack = true;
                direction = (originalPosition - transform.position).normalized;
            }
            else
            {
                isComingBack = false;
                direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            }
            Debug.Log(direction);
            var timer = 0f;
            var duration = UnityEngine.Random.Range(MinDirChangeTime, MinDirChangeTime);
            var destination = transform.position + direction * MoveSpeed * duration;
            while (timer < duration)
            {
                var aa = 1 - Math.Clamp(Vector3.Distance(originalPosition, transform.position) / MaxDistance, 0, 0.9f);
                transform.position += direction * MoveSpeed * (isComingBack ? 1 : aa);
                // transform.position = Vector3.Lerp(transform.position, destination, timer / duration);
                timer += Time.deltaTime;
                yield return 0;

                Distance = Vector3.Distance(originalPosition, transform.position);
            }
        } while (true);
    }
}