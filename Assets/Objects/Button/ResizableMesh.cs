using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ResizableMesh : MonoBehaviour
{
    [Range(0.0001f, 5)][SerializeField] private float x = 1;
    [Range(0.0001f, 5)][SerializeField] private float y = 1;
    [Range(0.0001f, 5)][SerializeField] private float z = 1;
    protected Vector3 Size => new(x, y, z);
    [SerializeField] private bool isRunInEditor;

    [SerializeField] protected List<MeshFilter> meshFilters;
    protected List<Vector3[]> originalVertices;
    protected Vector3? initialDiff;

    void Awake()
    {
        if (Application.isPlaying)
            Preinit();
    }

    protected void Preinit()
    {
        if (meshFilters.Count == 0)
            meshFilters = GetComponentsInChildren<MeshFilter>().ToList();

        if (originalVertices == null)
        {
            originalVertices = meshFilters.Select(meshFilter => new Vector3[meshFilter.sharedMesh.vertices.Length]).ToList();

            for (int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter meshFilter = meshFilters[i];
                var mesh = meshFilter.mesh;
                meshFilter.sharedMesh = mesh;

                for (int ii = 0; ii < originalVertices[i].Length; ii++)
                    originalVertices[i][ii] = meshFilter.sharedMesh.vertices[ii];
            }
        }

        if (!initialDiff.HasValue)
            initialDiff = new Vector3(originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[0])),
                originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[1])),
                originalVertices.SelectMany(x => x).Max(x => Mathf.Abs(x[2])));
    }

    void Update()
    {
        if (isRunInEditor)
        {
            Debug.Log("A");
            if (!Application.isPlaying)
            {
                Preinit();
                Debug.Log("B");
                var to = new Vector3(x, y, z);
                for (int i = 0; i < 3; i++)
                    SnapResize(i, to, meshFilters, originalVertices, initialDiff.Value);
            }
        }
    }

    public void SnapResize(int axis, Vector3 to)
    {
        Preinit();

        SnapResize(axis, to, meshFilters, originalVertices, initialDiff.Value);
    }

    public void SnapResize(Vector3 to)
    {
        Preinit();

        for (int axis = 0; axis < 3; axis++)
            SnapResize(axis, to, meshFilters, originalVertices, initialDiff.Value);
    }

    public static void SnapResize(int axis,
        Vector3 to,
        List<MeshFilter> meshFilters,
        List<Vector3[]> originalVertices,
        Vector3 initialDiff)
    {
        to[axis] /= 2;

        var otherAxis = axis == 0 ? 1 : 0;
        var toDiff = Math.Abs(to.x - to.y);

        for (int h = 0; h < meshFilters.Count; h++)
        {
            var meshFilter = meshFilters[h];
            var mesh = meshFilter.mesh;
            meshFilter.sharedMesh = mesh;
            var newVertices = new Vector3[meshFilter.sharedMesh.vertices.Length];
            for (int i = 0; i < meshFilter.sharedMesh.vertices.Length; i++)
                if (originalVertices[h][i][axis] != 0)
                {
                    var position = meshFilter.sharedMesh.vertices[i];
                    var sign = (originalVertices[h][i][axis] > 0) ? 1 : -1;

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