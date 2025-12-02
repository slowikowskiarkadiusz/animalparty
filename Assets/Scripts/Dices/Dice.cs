using System;

public class Dice
{
    private static readonly Random rng = new();
    public int[] Faces;

    // public static Dice Default => new(new[] { 1, 2, 3, 4, 5, 6 });
    public static Dice Default => new(new[] { 15, 4, 22, 48, 1, 12 });

    public Dice(int[] faces)
    {
        Faces = faces;
    }

    public int Roll()
    {
        var faceIndex = rng.Next(0, Faces.Length);
        return Faces[faceIndex];
    }
}