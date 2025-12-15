using UnityEngine;

public static class BoardTime
{
    public static float DeltaTime => Time.deltaTime * Modifier;
    public static float Modifier = 1f;
}
