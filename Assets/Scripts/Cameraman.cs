using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Cameraman : MonoBehaviour
{
    private static Cameraman instance;

    private Vector3 originalPosition;
    private Vector3 offset;
    private float originalSize;
    private const float focusedOnPieceSize = 3;

    private new Camera camera;

    public float cameraMovementDuration = 0.5f;
    public float cameraMovementSpeed = 5f;

    public static Vector3 CurrentPosition => instance.transform.position;

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

    public static void FocusOnPieceOnPlayersMove(Piece piece)
    {
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.FocusOnPieceOnPlayersMoveCoroutine(piece));
        FollowTransform(piece.transform);
    }

    private IEnumerator FocusOnPieceOnPlayersMoveCoroutine(Piece piece)
    {
        var timer = 0f;
        var startSize = camera.orthographicSize;
        var destinationSize = focusedOnPieceSize;
        while (timer <= cameraMovementDuration)
        {
            camera.orthographicSize = Mathf.Lerp(startSize, destinationSize, timer / cameraMovementDuration);

            timer += Time.deltaTime;
            yield return 0;
        }

        camera.orthographicSize = destinationSize;
    }

    public static void FollowTransform(Transform t)
    {
        instance.StartCoroutine(instance.FollowTransformCoroutine(t));
    }

    private IEnumerator FollowTransformCoroutine(Transform t)
    {
        while (true)
        {
            if (!t) break;

            var destination = t.position - offset;
            if (Vector3.Distance(t.position, destination) > 0.1)
                transform.position += cameraMovementSpeed * Time.deltaTime * (destination - transform.position);

            yield return 0;
        }
    }

    public void ResetToBoardView()
    {

    }
}
