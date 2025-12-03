using System.Collections.Generic;
using System.Linq;

public class PieceController
{
    public int Id { get; private set; }
    private int? diceRoll;
    private int? selectedPathIndex;

    private BoardConductor boardConductor;
    public Piece Piece { get; private set; }
    private int selectedDiceIndex = 0;

    public List<Card> PiecesCards => Piece.Cards ?? Enumerable.Empty<Card>().ToList();
    public string PiecesPosition => Piece.Position;

    public PieceController(int id, BoardConductor boardConductor, Piece piece)
    {
        Id = id;
        this.boardConductor = boardConductor;
        this.Piece = piece;
    }

    public void SetActive(bool v)
    {
        // TODO
    }

    public void RollDice(Dice dice)
    {
        boardConductor.DiceThrower.ShowDice(Piece, dice);
        // var dice = Piece.Dices[selectedDiceIndex];
        // var faceIndex = dice.GetRandomFaceIndex();
        // diceRoll = dice.Faces[faceIndex];
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
        Piece.Cards.Remove(card);
        boardConductor.ApplyCard(card, playerId);
    }

    public void UseTotemCard(Card card)
    {
        // TODO
    }
}
