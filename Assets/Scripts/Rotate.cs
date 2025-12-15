using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float Offset { get; set; }

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private List<Axis> axes = new();
    [SerializeField] private bool runOnStart = false;
    private Quaternion? originalRotation;
    private List<Coroutine> coroutines = new ();

    private void Start()
    {
        if (runOnStart)
            StartRunning();
    }

    public void StartRunning()
    {
        originalRotation = transform.rotation;
        foreach (var axis in axes)
            coroutines.Add(StartCoroutine(Run(axis)));
    }

    public void ResetPosition()
    {
        if (originalRotation.HasValue)
            transform.rotation = originalRotation.Value;
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

        do
        {
            transform.Rotate(vector * rotationSpeed * BoardTime.DeltaTime);
            yield return 0;
        } while (true);
    }

    public void StopRunning()
    {
        foreach (var coroutine in coroutines)
            StopCoroutine(coroutine);

        coroutines.Clear();
    }
}