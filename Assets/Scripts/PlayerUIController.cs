using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNumberText;
    [SerializeField] private RectTransform turnActionsMenu;
    [SerializeField] private Button rollDiceButton;
    [SerializeField] private Button pathSelectionButtonPrefab;
    [SerializeField] private RectTransform cardMenu;
    [SerializeField] private CardUI cardPrefab;
    [SerializeField] private RectTransform cardTargetSelectionMenu;
    [SerializeField] private Button cardTargetSelectionMenuItemPrefab;

    private Camera mainCamera;
    private List<Button> pathSelectionButtons = new();
    private PieceController currentPieceController;
    private int? cardTargetPlayerId;

    private void Start()
    {
        mainCamera = Camera.main;
        PrepareCardTargetSelectionMenu();
    }

    public void ConnectToPlayer(PieceController pieceController)
    {
        rollDiceButton.onClick.RemoveAllListeners();

        currentPieceController = pieceController;
        playerNumberText.text = $"Player {currentPieceController.Id}";
        turnActionsMenu.gameObject.SetActive(true);
        rollDiceButton.onClick.AddListener(() =>
        {
            currentPieceController.RollDice();
            turnActionsMenu.gameObject.SetActive(false);
        });
    }

    public void ShowPathSelectionMenu(Transform[] fieldsToSelectFrom)
    {
        for (int i = 0; i < fieldsToSelectFrom.Count(); i++)
        {
            var field = fieldsToSelectFrom[i];
            var button = Instantiate(pathSelectionButtonPrefab, transform);
            pathSelectionButtons.Add(button);
            button.transform.position = mainCamera.WorldToScreenPoint(field.position);
            var pathI = i;

            button.onClick.AddListener(() =>
            {
                currentPieceController.SelectPath(pathI);

                foreach (var button in pathSelectionButtons)
                    DestroyImmediate(button.gameObject);

                pathSelectionButtons.Clear();
            });
        }
    }

    public void SetCardMenuActive(bool active)
    {
        if (cardMenu.gameObject.activeInHierarchy == active)
            return;

        cardMenu.gameObject.SetActive(active);

        if (active)
        {
            var width = cardMenu.sizeDelta.x;

            var count = currentPieceController.PiecesCards.Count;
            for (int i = 0; i < count; i++)
            {
                var card = Instantiate(cardPrefab, cardMenu);
                var offset = ((i + 1f) / (count + 1f) * width) - (width / 2f);
                card.transform.localPosition = new Vector3(offset, 0, 0);
                card.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(UseBirdCard(card)));

                var cardFloat = card.GetComponent<FloatUI>();
                if (cardFloat)
                    cardFloat.Offset = (float)i / count;
            }
        }
        else
        {
            for (int i = 0; i < cardMenu.childCount; i++)
            {
                DestroyImmediate(cardMenu.GetChild(i).gameObject);
            }
        }
    }

    private IEnumerator UseBirdCard(CardUI cardUI)
    {
        var birdCard = cardUI.Card as BirdCard;

        if (birdCard.RequriesTarget)
        {
            cardMenu.gameObject.SetActive(false);
            cardTargetSelectionMenu.gameObject.SetActive(true);
            while (!cardTargetPlayerId.HasValue)
                yield return 0;
            cardTargetSelectionMenu.gameObject.SetActive(false);
            cardMenu.gameObject.SetActive(true);
        }

        PlayerUICoroutines.UsingBirdCard(cardUI.GetComponent<RectTransform>());

        currentPieceController.UseBirdCard(cardUI.Card as BirdCard, cardTargetPlayerId ?? currentPieceController.Id);

        cardTargetPlayerId = null;
        yield return 0;
    }

    private void PrepareCardTargetSelectionMenu()
    {
        for (int i = 0; i < BoardGraph.NumberOfPieces; i++)
        {
            var item = Instantiate(cardTargetSelectionMenuItemPrefab, cardTargetSelectionMenu);
            var offset = ((i + 1f) / (BoardGraph.NumberOfPieces + 1f) * cardTargetSelectionMenu.sizeDelta.x) - (cardTargetSelectionMenu.sizeDelta.x / 2f);
            item.transform.localPosition = offset * Vector3.right;
            var playerId = i;
            item.onClick.AddListener(() => cardTargetPlayerId = playerId);

            var itemFloat = item.GetComponent<FloatUI>();
            if (itemFloat)
                itemFloat.Offset = (float)i / BoardGraph.NumberOfPieces;
        }
    }
}
