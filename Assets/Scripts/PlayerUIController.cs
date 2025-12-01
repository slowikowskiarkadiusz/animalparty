using System;
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
    [SerializeField] private SelectableItemUI cardTargetSelectionMenuItemPrefab;

    private Camera mainCamera;
    private List<Button> pathSelectionButtons = new();
    private PieceController currentPieceController;
    private (int id, SelectableItemUI selectable)? cardTargetPlayer;

    private void Start()
    {
        mainCamera = Camera.main;
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
                    Destroy(button.gameObject);

                pathSelectionButtons.Clear();
            });
        }
    }

    public void SetCardMenuActive(bool active)
    {
        if (active)
            ShowCards(currentPieceController.PiecesCards, card => StartCoroutine(UseBirdCard(card)));
        else
            HideSelectables();
    }

    public void ShowSelectables(IEnumerable<SelectableItemUI> selectables)
    {
        if (cardMenu.gameObject.activeInHierarchy)
            return;

        cardMenu.gameObject.SetActive(true);
        var width = cardMenu.sizeDelta.x;

        var count = selectables.Count();
        for (int i = 0; i < count; i++)
        {
            var selectable = selectables.ElementAt(i);
            selectable.transform.SetParent(cardMenu.GetChild(0));
            var posX = ((i + 1f) / (count + 1f) * width) - (width / 2f);
            selectable.transform.localPosition = new Vector3(posX, 0, 0);
            selectable.Float.Offset = (float)i / count;
            selectable.Float.StartRunning();
        }
    }

    public void ShowCards(IEnumerable<Card> cards, Action<CardUI> onClick)
    {
        if (cardMenu.gameObject.activeInHierarchy)
            return;

        var cardUIs = new List<CardUI>();

        var count = cards.Count();
        for (int i = 0; i < count; i++)
        {
            var card = Instantiate(cardPrefab, cardMenu.GetChild(0));
            card.Card = cards.ElementAt(i);
            card.GetComponent<Button>().onClick.AddListener(() => onClick(card));
            cardUIs.Add(card);
        }

        ShowSelectables(cardUIs);
    }

    public void HideSelectables()
    {
        var childCount = cardMenu.GetChild(0).childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(cardMenu.GetChild(0).GetChild(i).gameObject);
        }

        cardMenu.gameObject.SetActive(false);
    }

    private IEnumerator UseBirdCard(CardUI cardUI)
    {
        var birdCard = cardUI.Card as BirdCard;

        yield return UIAnimations.SelectingUIItem(cardUI);

        if (birdCard.RequriesTarget)
        {
            SetCardMenuActive(false);
            ShowSelectables(GenerateCardTargetSelectionCollection());
            while (!cardTargetPlayer.HasValue)
                yield return 0;
        }

        currentPieceController.UseBirdCard(cardUI.Card as BirdCard, cardTargetPlayer?.id ?? currentPieceController.Id);

        if (birdCard.RequriesTarget)
        {
            yield return UIAnimations.SelectingUIItem(cardTargetPlayer.Value.selectable);
            HideSelectables();
            SetCardMenuActive(true);
        }

        cardTargetPlayer = null;
        yield return 0;
    }

    // private void PrepareCardTargetSelectionMenu()
    // {
    //     for (int i = 0; i < BoardGraph.NumberOfPieces; i++)
    //     {
    //         var item = Instantiate(cardTargetSelectionMenuItemPrefab, cardTargetSelectionMenu);
    //         var offset = ((i + 1f) / (BoardGraph.NumberOfPieces + 1f) * cardTargetSelectionMenu.sizeDelta.x) - (cardTargetSelectionMenu.sizeDelta.x / 2f);
    //         item.transform.localPosition = offset * Vector3.right;
    //         var playerId = i;
    //         item.onClick.AddListener(() => cardTargetPlayerId = playerId);

    //         var itemFloat = item.GetComponent<FloatUI>();
    //         if (itemFloat)
    //             itemFloat.Offset = (float)i / BoardGraph.NumberOfPieces;
    //     }
    // }

    private IEnumerable<SelectableItemUI> GenerateCardTargetSelectionCollection()
    {
        var result = new List<SelectableItemUI>();
        for (int i = 0; i < BoardGraph.NumberOfPieces; i++)
        {
            var item = Instantiate(cardTargetSelectionMenuItemPrefab, cardMenu);
            var playerId = i;
            item.Button.onClick.AddListener(() => cardTargetPlayer = (playerId, item));

            result.Add(item);
        }

        return result;
    }
}
