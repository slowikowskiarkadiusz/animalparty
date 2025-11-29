using System;

public abstract class Dice
{
    private static readonly Random rng = new();
    public abstract int[] Faces { get; }

    public int Roll()
    {
        var faceIndex = rng.Next(0, Faces.Length);
        return Faces[faceIndex];
    }
}