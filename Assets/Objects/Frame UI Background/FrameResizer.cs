using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameResizer : MonoBehaviour
{
    private List<MeshFilter> meshFilters;
    private List<Vector3[]> originalVertices;
    [Range(-300, 300)][SerializeField] private float to;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;
    [SerializeField] private bool isRunAnimation = false;

    private Vector3 initialDiff = Vector3.zero;

    private void Awake()
    {
        meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
        originalVertices = meshFilters.Select(meshFilter => new Vector3[meshFilter.sharedMesh.vertices.Length]).ToList();

        for (int i = 0; i < meshFilters.Count; i++)
        {
            MeshFilter meshFilter = meshFilters[i];
            var mesh = meshFilter.mesh;
            meshFilter.sharedMesh = mesh;

            for (int ii = 0; ii < originalVertices[i].Length; ii++)
                originalVertices[i][ii] = meshFilter.sharedMesh.vertices[ii];
        }

        initialDiff = new Vector3(originalVertices.SelectMany(x => x).Select(x => Mathf.Abs(x[0])).Max(),
            originalVertices.SelectMany(x => x).Select(x => Mathf.Abs(x[1])).Max(),
            originalVertices.SelectMany(x => x).Select(x => Mathf.Abs(x[2])).Max());

        Debug.Log(initialDiff.ToString("F8"));

        Resize(0, 0);
        Resize(1, 0);
    }

    private IEnumerator Animation()
    {
        var timer = 0f;

        Resize(1, to / 2f);

        yield return 0;

        while (timer < animationTime)
        {
            Resize(0, curve.Evaluate(timer / animationTime) * to);
            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        timer = 0f;
        var from = to / 2f;

        while (timer < animationTime)
        {
            Resize(1, from + curve.Evaluate(timer / animationTime) * (to - from));
            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        Resize(1, to);
    }

    private void Update()
    {
        if (isRunAnimation)
        {
            StartCoroutine(Animation());
            isRunAnimation = false;
        }
    }

    public void Resize(int axis, float to)
    {
        to /= 1000f;

        for (int h = 0; h < meshFilters.Count; h++)
        {
            var meshFilter = meshFilters[h];
            var newVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];
            for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
                if (originalVertices[h][i][axis] != 0)
                {
                    var vector = meshFilter.sharedMesh.vertices[i];
                    vector[axis] = originalVertices[h][i][axis];
                    var sign = (vector[axis] > 0) ? 1 : -1;
                    vector[axis] += sign * (to - initialDiff[axis]);
                    newVertices[i] = vector;

                    var newSign = (newVertices[i][axis] > 0) ? 1 : -1;
                    if (sign != newSign)
                    {
                        vector[axis] = 0;
                        newVertices[i] = vector;
                    }
                }
                else
                {
                    newVertices[i] = meshFilter.sharedMesh.vertices[i];
                    newVertices[i][axis] = originalVertices[h][i][axis];
                }

            meshFilter.sharedMesh.SetVertices(newVertices);
        }
    }
}
