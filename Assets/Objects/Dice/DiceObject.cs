using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceObject : MonoBehaviour
{
    [SerializeField] private Transform dotPrefab;

    private float radius = 2;
    public bool pips = true;
    public Dice dice = Dice.Default;
    // public int[] Faces = new[] { 15, 4, 22, 48, 1, 12 };

    readonly Vector3[] dict = new[]
    {
        Vector3.zero,
        new Vector3(0, 90, 0),
        new Vector3(0, 0, 90),
        new Vector3(0, 0, 270),
        new Vector3(0, 270, 0),
        new Vector3(0, 180, 0),
    };

    private void SpawnSide(int index)
    {
        var sideTransform = new GameObject().transform;
        sideTransform.SetParent(transform);
        sideTransform.localScale = Vector3.one;
        sideTransform.localPosition = Vector3.zero;

        var sqrt = Mathf.Sqrt(dice.Faces[index]);
        var cols = (int)Mathf.Ceil(sqrt);
        var perCol = (float)dice.Faces[index] / cols;
        var rest = Mathf.Round((perCol - Mathf.Floor(perCol)) * cols);

        var columns = new int[cols];

        for (int i = 0; i <= columns.Length / 2; i++)
        {
            columns[i] = (int)perCol;
            columns[columns.Length - 1 - i] = (int)perCol;

            if (rest > 1)
            {
                rest -= 2;
                columns[i]++;
                columns[columns.Length - 1 - i]++;
            }
        }

        if (rest == 1)
            columns[columns.Length / 2]++;

        var width = radius * Mathf.Sqrt(2) / 2.7f;
        for (int i = 0; i < columns.Length; i++)
        {
            for (int ii = 0; ii < columns[i]; ii++)
            {
                var dot = Instantiate(dotPrefab, sideTransform);
                dot.localScale /= Math.Max(1, Math.Min(Mathf.Log(cols), Mathf.Sqrt(cols)));
                dot.localPosition = new Vector3(
                    -1.01f,
                    Mathf.Lerp(-width / 2, width / 2, columns[i] == 1 ? 0.5f : ((float)ii / (columns[i] - 1))),
                    Mathf.Lerp(-width / 2, width / 2, columns.Length == 1 ? 0.5f : (float)i / (columns.Length - 1)));
            }
        }

        // sideTransform.localEulerAngles = new Vector3(0, (index % 3 + index / 5) * 90, index == 3 ? 90 : index == 4 ? -90 : 0);
        sideTransform.localEulerAngles = dict[index];
    }

    public void Update()
    {
        if (!pips)
            return;

        for (int i = 1; i < transform.childCount; i++)
        {
            if (i < transform.childCount)
                Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < dice.Faces.Length; i++)
        {
            SpawnSide(i);
        }
        pips = false;
    }
}
