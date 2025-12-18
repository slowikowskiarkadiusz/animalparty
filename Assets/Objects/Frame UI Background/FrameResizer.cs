using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameResizer : MonoBehaviour
{
    protected List<MeshFilter> meshFilters;
    private List<Vector3[]> originalVertices;
    // [Range(-300, 300)][SerializeField] private float to;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;
    // [SerializeField] private bool isRunAnimation = false;

    private Vector3 initialDiff = Vector3.zero;

    void Awake()
    {
        Init();
    }

    protected void Init()
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

        initialDiff = new Vector3(originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[0])),
            originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[1])),
            originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[2])));

        SnapResize(0, 0);
        SnapResize(1, 0);
    }

    protected IEnumerator ResizeCoroutine(Vector2 from, Vector2 to)
    {
        var timer = 0f;

        SnapResize(0, from.x);
        SnapResize(1, from.y);

        yield return 0;

        if (from.x != to.x)
        {
            while (timer < animationTime)
            {
                SnapResize(0, from.x + curve.Evaluate(timer / animationTime) * (to.x - from.x));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }
        }

        if (from.y != to.y)
        {
            timer = 0f;

            while (timer < animationTime)
            {
                SnapResize(1, from.y + curve.Evaluate(timer / animationTime) * (to.y - from.y));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }

            SnapResize(1, to.y);
        }
    }

    // private void Update()
    // {
    //     if (isRunAnimation)
    //     {
    //         StartCoroutine(Animation());
    //         isRunAnimation = false;
    //     }
    // }

    public void SnapResize(int axis, float to)
    {
        to /= 2;
        for (int h = 0; h < meshFilters.Count; h++)
        {
            var meshFilter = meshFilters[h];
            var newVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];
            for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
                if (originalVertices[h][i][axis] != 0)
                {
                    var position = meshFilter.sharedMesh.vertices[i];
                    var sign = (originalVertices[h][i][axis] > 0) ? 1 : -1;
                    position[axis] = originalVertices[h][i][axis] + sign * (to - initialDiff[axis]);
                    newVertices[i] = position;

                    var newSign = (newVertices[i][axis] > 0) ? 1 : -1;
                    if (sign != newSign)
                    {
                        position[axis] = 0;
                        newVertices[i] = position;
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
