using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameResizer : MonoBehaviour
{
    protected List<MeshFilter> meshFilters;
    private List<Vector3[]> originalVertices;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;

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

        SnapResize(0, Vector2.zero);
        SnapResize(1, Vector2.zero);

        toX = initialDiff[0];
        toY = initialDiff[1];
    }

    protected IEnumerator ResizeCoroutine(Vector2 from, Vector2 to)
    {
        var timer = 0f;

        SnapResize(0, from);
        SnapResize(1, from);

        yield return 0;

        if (from.x != to.x)
        {
            while (timer < animationTime)
            {
                SnapResize(0, from + curve.Evaluate(timer / animationTime) * (to - from));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }
        }

        if (from.y != to.y)
        {
            timer = 0f;

            while (timer < animationTime)
            {
                SnapResize(1, from + curve.Evaluate(timer / animationTime) * (to - from));
                timer += BoardTime.DeltaTime;
                yield return 0;
            }

            SnapResize(1, to);
        }
    }

    [Range(0.01f, 4)][SerializeField] private float toX;
    [Range(0.01f, 4)][SerializeField] private float toY;
    [SerializeField] private bool isRunAnimation = false;

    private void Update()
    {
        if (isRunAnimation)
        {
            var to = new Vector2(toX, toY);
            SnapResize(0, to);
            SnapResize(1, to);
            isRunAnimation = false;
        }
    }

    public void SnapResize(int axis, Vector2 to)
    {
        to[axis] /= 2;

        var otherAxis = axis == 0 ? 1 : 0;
        var toDiff = Math.Abs(to.x - to.y);

        for (int h = 0; h < meshFilters.Count; h++)
        {
            var meshFilter = meshFilters[h];
            var newVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];
            for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
                if (originalVertices[h][i][axis] != 0)
                {
                    var position = meshFilter.sharedMesh.vertices[i];
                    var sign = (originalVertices[h][i][axis] > 0) ? 1 : -1;

                    // if (to[axis] >= initialDiff[axis])
                    //     position[axis] = originalVertices[h][i][axis] + sign * (to[axis] - initialDiff[axis]);
                    // else
                    //     position[axis] = originalVertices[h][i][axis] * to[axis] / initialDiff[axis];

                    if (to[otherAxis] < initialDiff[otherAxis])
                    {
                        position[axis] = originalVertices[h][i][axis] * to[otherAxis] / initialDiff[otherAxis] + toDiff * sign;
                    }
                    else if (to[axis] < initialDiff[axis])
                        position[axis] = originalVertices[h][i][axis] * to[axis] / initialDiff[axis];
                    else
                        position[axis] = originalVertices[h][i][axis] + sign * (to[axis] - initialDiff[axis]);

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
