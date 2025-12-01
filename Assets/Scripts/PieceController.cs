using System.Collections.Generic;
using System.Linq;

public class PieceController
{
    public int Id { get; private set; }
    private int? diceRoll;
    private int? selectedPathIndex;

    private BoardConductor boardConductor;
    private Piece piece;
    private int selectedDiceIndex = 0;

    public List<Card> PiecesCards => piece.Cards ?? Enumerable.Empty<Card>().ToList();
    public string PiecesPosition => piece.Position;

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

    public void UseBirdCard(BirdCard card, int playerId)
    {
        piece.Cards.Remove(card);
        boardConductor.ApplyCard(card, playerId);
    }

    public void UseTotemCard(Card card)
    {
        // TODO
    }
}
