using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardGraph))]
public class BoardConductor : MonoBehaviour
{
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private PieceController pieceControllerPrefab;
    [SerializeField] private int noOfPlayersToSpawn = 4;
    [SerializeField] private PlayerUIController playerUiController;
    private BoardGraph boardGraph;
    private readonly List<PieceController> pieceControllers = new();

    public List<(BirdCard card, int turnsLeft)>[] appliedBirdCards;

    private void Awake()
    {
        StartCoroutine(Game());
    }

    private IEnumerator Game()
    {
        yield return BeginGame();

        while (true)
        {
            for (int i = 0; i < boardGraph.Pieces.Count; i++)
            {
                SwitchToPlayer(i);
                yield return PlayersTurn(i);
            }

            yield return 0;
        }
    }

    private IEnumerator BeginGame()
    {
        boardGraph = GetComponent<BoardGraph>();
        appliedBirdCards = new List<(BirdCard, int)>[noOfPlayersToSpawn];

        for (int i = 0; i < noOfPlayersToSpawn; i++)
        {
            appliedBirdCards[i] = new();
            boardGraph.AddPiece(Instantiate(piecePrefab, boardGraph.transform));
            pieceControllers.Add(new PieceController(i, this, boardGraph.Pieces[i]));
        }

        yield return null;
    }

    private IEnumerator PlayersTurn(int playerId)
    {
        var controller = pieceControllers[playerId];

        int? diceRoll;

        while (!(diceRoll = controller.PopDiceRoll()).HasValue)
            yield return 0;

        while (diceRoll-- > 0)
        {
            if (boardGraph.IsForkAheadOfPiece(playerId, out var fieldsAhead))
            {
                playerUiController.ShowPathSelectionMenu(fieldsAhead);

                int? selectedPathIndex;

                while (!(selectedPathIndex = controller.PopSelectedPathIndex()).HasValue)
                    yield return 0;

                var selectedPath = selectedPathIndex.Value;
                yield return boardGraph.MovePieceForward(playerId, selectedPath);
            }

            yield return boardGraph.MovePieceForward(playerId, 0);
        }
    }

    public void ApplyCard(BirdCard card, int playerId)
    {
        if (card.Turns.HasValue)
            appliedBirdCards[playerId].Add((card, card.Turns.Value));
    }

    private void SwitchToPlayer(int playerId)
    {
        foreach (var pieceController in pieceControllers)
            pieceController.SetActive(false);

        pieceControllers[playerId].SetActive(true);
        playerUiController.ConnectToPlayer(pieceControllers[playerId]);
    }
}