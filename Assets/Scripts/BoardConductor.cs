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

    private void Start()
    {
        StartCoroutine(Game());
    }

    private IEnumerator Game()
    {
        Debug.Log("Beggining the game.");
        yield return BeginGame();

        while (true)
        {
            for (int i = 0; i < boardGraph.Pieces.Count; i++)
            {
                Debug.Log($"Switching to player {i}.");
                SwitchToPlayer(i);
                Debug.Log($"Waiting for the player {i}'s turn.");
                yield return PlayersTurn(i);
            }
        }

        yield return 0;
    }

    private IEnumerator BeginGame()
    {
        boardGraph = GetComponent<BoardGraph>();

        for (int i = 0; i < noOfPlayersToSpawn; i++)
        {
            boardGraph.AddPiece(Instantiate(piecePrefab, boardGraph.transform));
            pieceControllers.Add(new PieceController(i, this, boardGraph.Pieces[i]));
        }

        yield return null;
    }

    private IEnumerator PlayersTurn(int playerId)
    {
        var controller = pieceControllers[playerId];

        Debug.Log($"Waiting for the dice roll.");

        int? diceRoll;

        while (!(diceRoll = controller.PopDiceRoll()).HasValue)
            yield return 0;

        while (diceRoll-- > 0)
        {
            Debug.Log($"{diceRoll + 1} left of the dice");
            if (boardGraph.IsForkAheadOfPiece(playerId))
            {
                playerUiController.ShowPathSelectionMenu();

                var selectedPathIndex = controller.PopSelectedPathIndex();

                while (!selectedPathIndex.HasValue)
                    yield return 0;

                var selectedPath = selectedPathIndex.Value;
                yield return boardGraph.MovePieceForward(playerId, selectedPath);
            }

            yield return boardGraph.MovePieceForward(playerId, 0);
        }
    }

    private void SwitchToPlayer(int playerId)
    {
        foreach (var pieceController in pieceControllers)
            pieceController.SetActive(false);

        pieceControllers[playerId].SetActive(true);
        playerUiController.ConnectToPlayer(pieceControllers[playerId]);
    }
}