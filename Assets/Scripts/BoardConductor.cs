using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardGraph))]
[RequireComponent(typeof(DiceThrower))]
public class BoardConductor : MonoBehaviour
{
    private readonly List<PlayerRegistration> playerRegistrations = new()
    {
        new("lysy", PieceType.Default),
        new("seksovny", PieceType.Default),
        new("theliver", PieceType.Default),
        new("Domisia", PieceType.Default),
    };

    private readonly Color[] colors = new Color[]
    {
        new(0, 1, 0),
        new(1, 0, 0),
        new(0, 0, 1),
        new(0.5f, 0.5f, 0.5f),
        new(0, 0, 0),
        new(1, 1, 1),
        new(1, 0.5f, 0),
        new(1, 1, 0),
        new(0, 1, 1),
        new(0, 0.5f, 1),
        new(0.5f, 0, 1),
        new(1, 0, 0.5f),
    };

    [SerializeField] private Piece piecePrefab;
    [SerializeField] private PieceController pieceControllerPrefab;
    [SerializeField] private PlayerUIController playerUiController;
    private BoardGraph boardGraph;
    private readonly List<PieceController> pieceControllers = new();

    public DiceThrower DiceThrower { get; private set; }
    public List<(BirdCard card, int turnsLeft)>[] appliedBirdCards;

    private void Awake()
    {
        DiceThrower = GetComponent<DiceThrower>();
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
        boardGraph.Init();
        appliedBirdCards = new List<(BirdCard, int)>[playerRegistrations.Count];

        for (int i = 0; i < playerRegistrations.Count; i++)
        {
            appliedBirdCards[i] = new();
            boardGraph.AddPiece(Instantiate(piecePrefab, boardGraph.transform).Spawn(playerRegistrations[i], colors[i]));
            pieceControllers.Add(new PieceController(i, this, boardGraph.Pieces[i]));
        }

        playerUiController.SpawnPlayersTags(boardGraph.Pieces);

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
                yield return boardGraph.MovePieceForward(playerId, selectedPath, controller, playerUiController);
            }
            else
            {
                yield return boardGraph.MovePieceForward(playerId, 0, controller, playerUiController);
            }
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
        //TODO camera look in the direction of the next field
        playerUiController.ConnectToPlayer(pieceControllers[playerId]);
    }
}