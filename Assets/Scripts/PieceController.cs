using UnityEngine;

public class PieceController
{
    public int Id { get; private set; }
    private int? diceRoll;
    private int? selectedPathIndex;

    private BoardConductor boardConductor;
    private Piece piece;
    private int selectedDiceIndex = 0;

    public PieceController(int id, BoardConductor boardConductor, Piece piece)
    {
        Id = id;
        this.boardConductor = boardConductor;
        this.piece = piece;
    }

    public void SetActive(bool v)
    {
        // TODO
    }

    public void RollDice()
    {
        diceRoll = piece.Dices[selectedDiceIndex].Roll();
    }

    public void SelectPath(int pathIndex)
    {
        selectedPathIndex = pathIndex;
    }

    public int? PopDiceRoll()
    {
        var result = diceRoll;
        diceRoll = null;
        return result;
    }

    public int? PopSelectedPathIndex()
    {
        var result = selectedPathIndex;
        selectedPathIndex = null;
        return result;
    }
}
