using System;
using System.Collections;

public abstract class FieldEvent
{
    public abstract IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController, LoadableSet[] loadableSets);
}
