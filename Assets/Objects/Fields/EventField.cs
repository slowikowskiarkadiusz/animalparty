using System.Collections;
using UnityEngine;

public abstract class EventField : MonoBehaviour
{
    public abstract IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController);
}
