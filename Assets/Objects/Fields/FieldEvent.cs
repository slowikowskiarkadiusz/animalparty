using System;
using System.Collections;

public abstract class FieldEvent
{
    protected static FieldEvent current;

    public abstract IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController);
    protected abstract void Stop();

    public static void StopCurrent()
    {
        current?.Stop();
        current = null;
    }
}
