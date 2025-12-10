using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Cameraman : MonoBehaviour
{
    private static Cameraman instance;

    private Vector3 originalPosition;
    private Vector3 offset;
    private float originalSize;
    public const float FocusedOnPieceSize = 2;

    private new Camera camera;

    public float cameraMovementDuration = 0.5f;
    public float cameraMovementSpeed = 5f;

    public static Vector3 CurrentPosition => instance.transform.position;

    private Coroutine followingCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        originalSize = camera.orthographicSize;
        originalPosition = transform.position;
        var isHit = Physics.Raycast(originalPosition, transform.forward, out var hit, 1000, 1 << LayerMask.NameToLayer("Board Base"));
        if (isHit)
            offset = hit.point - originalPosition;
    }

    public static void Zoom(float size)
    {
        instance.StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            var timer = 0f;
            var startSize = instance.camera.orthographicSize;
            var destinationSize = size;
            while (timer <= instance.cameraMovementDuration)
            {
                instance.camera.orthographicSize = Mathf.Lerp(startSize, destinationSize, timer / instance.cameraMovementDuration);

                timer += Time.deltaTime;
                yield return 0;
            }

            instance.camera.orthographicSize = destinationSize;
        }
    }

    public static void Follow(Func<Vector3> func)
    {
        if (instance.followingCoroutine != null)
            instance.StopCoroutine(instance.followingCoroutine);
        instance.followingCoroutine = instance.StartCoroutine(instance.FollowVector3Coroutine(func));
    }

    public static void Follow(Transform t)
    {
        if (instance.followingCoroutine != null)
            instance.StopCoroutine(instance.followingCoroutine);
        instance.followingCoroutine = instance.StartCoroutine(instance.FollowTransformCoroutine(t));
    }

    private IEnumerator FollowVector3Coroutine(Func<Vector3> func)
    {
        while (true)
        {
            Vector3 position;

            try
            {
                position = func();
            }
            catch
            {
                break;
            }

            var destination = position - offset;

            if (Vector3.Distance(position, destination) > 0.1)
                transform.position += cameraMovementSpeed * Time.deltaTime * (destination - transform.position);

            yield return 0;
        }
    }

    private IEnumerator FollowTransformCoroutine(Transform t)
    {
        while (true)
        {
            if (!t) break;

            var destination = t.position - offset;
            var distance = Vector3.Distance(t.position, destination);
            if (distance > 0.1)
                transform.position += cameraMovementSpeed * Time.deltaTime * (destination - transform.position);

            yield return 0;
        }
    }

    public void ResetToBoardView()
    {

    }
}
