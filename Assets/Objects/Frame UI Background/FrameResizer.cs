using System;
using System.Collections;
using UnityEngine;

public class FrameResizer : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    [Range(-300, 300)][SerializeField] private float to;
    [SerializeField] private int axis;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];

        var mesh = meshFilter.mesh;
        meshFilter.sharedMesh = mesh;

        for (int i = 0; i < originalVertices.Length; i++)
            originalVertices[i] = meshFilter.sharedMesh.vertices[i];

        Resize(axis, -Mathf.Infinity);
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        var timer = 0f;

        yield return 0;

        while (timer < animationTime)
        {
            Resize(axis, curve.Evaluate(timer / animationTime) * to);
            timer += Time.deltaTime;
            yield return 0;
        }

        Resize(axis, to);
    }

    private void Update()
    {
        // if (prevto != to)
        // {
        //     Resize(axis, to);
        //     prevto = to;
        // }
    }

    public void Resize(int axis, float to)
    {
        to /= 10000f;
        var newVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];
        for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
            if (originalVertices[i][axis] != 0)
            {
                var vector = originalVertices[i];
                var sign = (vector[axis] > 0) ? 1 : -1;
                vector[axis] += sign * to;
                newVertices[i] = vector;

                var newSign = (newVertices[i][axis] > 0) ? 1 : -1;
                if (sign != newSign)
                {
                    vector[axis] = 0;
                    newVertices[i] = vector;
                }
            }
            else
                newVertices[i] = originalVertices[i];

        meshFilter.sharedMesh.SetVertices(newVertices);
    }

    void OnDestroy()
    {
        for (int i = 0; i < originalVertices.Length; i++)
            meshFilter.sharedMesh.vertices[i] = originalVertices[i];
    }
}
