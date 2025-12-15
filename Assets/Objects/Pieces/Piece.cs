using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour
{
    private readonly AnimationCurve rotateBackAnimationCurve = new(new Keyframe[] { new(0, 0, 2, 2), new(1, 1, 0, 0) });

    public Rigidbody Rigidbody { get; private set; }
    public string Position { get; set; }
    public BoardGraph BoardGraph { get; set; }
    public Transform PiecePrefab { get; set; }
    public Dice[] Dices { get; set; } = new[] { Dice.Default, Dice.FukkedUp };
    public List<Card> Cards { get; set; } = new() { BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie };
    public string PlayersName { get; set; }
    public int VictoryPoints { get; set; } = 0;
    public int Coins { get; set; } = 0;

    [SerializeField] private Transform defaultPiecePrefab;

    private float pieceSpawnHeight = 0.24f;
    private Transform piece;

    private InputAction submitAction;

    public Piece Spawn(PlayerRegistration playerRegistration, Color color)
    {
        submitAction = new InputAction("Submit");
        submitAction.Enable();

        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.isKinematic = true;

        //TODO
        // var standMeshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        // pieceSpawnHeight = (standMeshRenderer.bounds.center.y + standMeshRenderer.bounds.extents.y) * standMeshRenderer.transform.lossyScale.y;

        piece = Instantiate(defaultPiecePrefab ?? PiecePrefab);
        piece.SetParent(transform);
        piece.localPosition = new Vector3(0, pieceSpawnHeight, 0);

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer.gameObject.CompareTag("Paintable Piece Part"))
            {
                meshRenderer.materials[0].color = color;
            }
        }

        PlayersName = playerRegistration.Name;

        return this;
    }

    public void Kick(int times = 1)
    {
        var originalPosition = Rigidbody.position;
        var originalRotation = Rigidbody.rotation;
        var originalIsKinematic = Rigidbody.isKinematic;
        var originalConstraints = Rigidbody.constraints;

        Rigidbody.isKinematic = false;
        var bounds = piece.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        var point = new Vector3(Rigidbody.position.x, bounds.max.y, Rigidbody.position.z);

        StopAllCoroutines();

        StartCoroutine(Coroutine(times));

        IEnumerator Coroutine(int times)
        {
            const float timeBetweenKicks = 0.35f;

            while (times-- > 0)
            {
                Method();
                yield return new WaitForSeconds(timeBetweenKicks);
            }
        }

        void Method()
        {
            const float horizontalForce = 5;
            const float verticalForce = 80;
            const float angularForce = 5;
            const float comingBackForce = 3f;

            var hortizontalVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            Rigidbody.AddForceAtPosition(hortizontalVector * horizontalForce + transform.up * verticalForce, point);
            Rigidbody.AddTorque(transform.up * angularForce);

            StartCoroutine(RotateBackCoroutine());
            StartCoroutine(GoBackCoroutine());

            IEnumerator RotateBackCoroutine()
            {
                yield return new WaitForSeconds(1);

                var startRotation = Rigidbody.rotation;

                const float duration = 0.3f;
                var timer = 0f;

                while (timer < duration)
                {
                    Rigidbody.MoveRotation(Quaternion.LerpUnclamped(startRotation, originalRotation, rotateBackAnimationCurve.Evaluate(timer / duration)));

                    yield return 0;

                    timer += BoardTime.DeltaTime;
                }
            }

            IEnumerator GoBackCoroutine()
            {
                const float duration = 2;
                var timer = 0f;

                while (timer < duration)
                {
                    Rigidbody.MovePosition(Vector3.Lerp(Rigidbody.position, originalPosition, comingBackForce * BoardTime.DeltaTime));

                    yield return 0;

                    timer += BoardTime.DeltaTime;
                }

                Rigidbody.isKinematic = originalIsKinematic;
                Rigidbody.constraints = originalConstraints;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Kick(2);
        }
    }
}
