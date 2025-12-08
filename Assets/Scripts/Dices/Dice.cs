using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class Dice
{
    private static readonly Random rng = new();
    public int[] Faces { get; }

    public static Dice Default => new(new[] { 9, 9, 9, 9, 9, 9 });
    public static Dice FukkedUp => new(new[] { 9, 9, 9, 9, 9, 9 });
    // public static Dice Default => new(new[] { 1, 2, 3, 4, 5, 6 });
    // public static Dice FukkedUp => new(new[] { 15, 4, 22, 48, 1, 12 });

    public Dice(int[] faces)
    {
        Faces = faces;
    }

    public int GetRandomFaceIndex()
    {
        return rng.Next(0, Faces.Length);
    }

    public (IEnumerable<Vector2> pips, float scale) GenerateSidePipsPlacements(int sideIndex)
    {
        var result = new List<Vector2>();

        var sqrt = UnityEngine.Mathf.Sqrt(Faces[sideIndex]);
        var cols = (int)UnityEngine.Mathf.Ceil(sqrt);
        var perCol = (float)Faces[sideIndex] / cols;
        var rest = UnityEngine.Mathf.Round((perCol - UnityEngine.Mathf.Floor(perCol)) * cols);

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

        var scale = Math.Max(1, Math.Min(UnityEngine.Mathf.Log(cols), UnityEngine.Mathf.Sqrt(cols)));
        for (int i = 0; i < columns.Length; i++)
        {
            for (int ii = 0; ii < columns[i]; ii++)
            {
                var position = new Vector2(columns.Length == 1 ? 0.5f : (float)i / (columns.Length - 1),
                    columns[i] == 1 ? 0.5f : ((float)ii / (columns[i] - 1)));

                result.Add(position);
            }
        }

        return (result, scale);
    }
}