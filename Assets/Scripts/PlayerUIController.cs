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
    [SerializeField] private MenuBackground cardMenu;
    [SerializeField] private CardObject cardPrefab;
    [SerializeField] private SelectableItemUI cardTargetSelectionMenuItemPrefab;
    [SerializeField] private SelectableItemUI cardTargetSelectionMenuRandomItemPrefab;
    [SerializeField] private ChoosableDiceUI choosableDicePrefab;
    [SerializeField] private SelectableItemUI finishRollingButton;

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

        Cameraman.Zoom(Cameraman.FocusedOnPieceSize);
        Cameraman.Follow(() => pieceController.Piece.transform.position + Vector3.up * 1);

        currentPieceController = pieceController;
        playerNumberText.text = $"Player {currentPieceController.Id}";
        turnActionsMenu.gameObject.SetActive(true);
        rollDiceButton.onClick.AddListener(() =>
        {
            var items = new List<ChoosableDiceUI>();
            for (int i = 0; i < currentPieceController.Piece.Dices.Length; i++)
            {
                Dice dice = currentPieceController.Piece.Dices[i];
                var item = Instantiate(choosableDicePrefab, cardMenu.SelectablesParent);
                item.Dice = dice;
                item.Button.onClick.AddListener(() =>
                {
                    HideSelectables();
                    currentPieceController.RollDice(dice);

                    //todo
                    finishRollingButton.gameObject.SetActive(true);
                });

                items.Add(item);
            }
            ShowSelectables(items);
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
        cardMenu.Open(selectables, () => { });
    }

    public void ShowCards(IEnumerable<Card> cards, Action<CardObject> onClick)
    {
        cardMenu.ShowCards(cardPrefab, cards, onClick);
    }

    public void HideSelectables()
    {
        cardMenu.Close();
    }

    public void FinishRollingDice()
    {
        finishRollingButton.gameObject.SetActive(false);
        StartCoroutine(currentPieceController.FinishRollingDice());
    }

    private IEnumerator UseBirdCard(CardObject cardUI)
    {
        var birdCard = cardUI.Card as BirdCard;

        yield return cardUI.RunSelectingAnimation();

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
            yield return cardTargetPlayer.Value.selectable.RunSelectingAnimation();
            HideSelectables();
            SetCardMenuActive(true);
        }

        cardTargetPlayer = null;
        yield return 0;
    }

    private IEnumerable<SelectableItemUI> GenerateCardTargetSelectionCollection()
    {
        var result = new List<SelectableItemUI>();
        SelectableItemUI randomItem = null;
        for (int i = 0; i < BoardGraph.NumberOfPieces + 1; i++)
        {
            SelectableItemUI item;
            if (i != BoardGraph.NumberOfPieces)
            {
                item = Instantiate(cardTargetSelectionMenuItemPrefab, cardMenu.SelectablesParent);
                var playerId = i;
                item.Button.onClick.AddListener(() => cardTargetPlayer = (playerId, item));
                result.Add(item);
            }
            else
            {
                randomItem = Instantiate(cardTargetSelectionMenuRandomItemPrefab, cardMenu.SelectablesParent);
                randomItem.Button.onClick.AddListener(() => StartCoroutine(RandomizeCardTargetSelection(result)));
            }
        }

        return result.Append(randomItem);
    }

    private IEnumerator RandomizeCardTargetSelection(List<SelectableItemUI> selectables)
    {
        SelectableItemUI.CanInteract(false);
        var ticks = 50 + UnityEngine.Random.Range(0, BoardGraph.NumberOfPieces);
        int currentSelectableId = 0;

        while (ticks-- >= 0)
        {
            currentSelectableId++;
            currentSelectableId %= selectables.Count;
            selectables[currentSelectableId].OnPointerEnter();
            yield return new WaitForSeconds(0.1f * (ticks < 10 ? 10 - ticks : 1));
        }

        yield return new WaitForSeconds(1);

        cardTargetPlayer = (currentSelectableId, selectables[currentSelectableId]);
        SelectableItemUI.CanInteract(true);
    }

    public static float SpaceAround(float i, float count, float width)
    {
        return ((i + 1f) / (count + 1f) * width) - (width / 2f);
    }
}
