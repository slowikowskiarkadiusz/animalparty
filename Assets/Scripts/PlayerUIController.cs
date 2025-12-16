using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(BoardViewerUI))]
public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private RectTransform turnActionsMenu;
    [SerializeField] private Button rollDiceButton;
    [SerializeField] private Button pathSelectionButtonPrefab;
    [SerializeField] private MenuBackground cardMenu;
    [SerializeField] private CardUI cardPrefab;
    [SerializeField] private SelectableItemUI cardTargetSelectionMenuItemPrefab;
    [SerializeField] private SelectableItemUI cardTargetSelectionMenuRandomItemPrefab;
    [SerializeField] private ChoosableDiceUI choosableDicePrefab;
    [SerializeField] private SelectableItemUI finishRollingButton;
    [SerializeField] private RectTransform playersTagsTransform;
    [SerializeField] private PlayerTag playerTagPrefab;
    [SerializeField] private AnimationCurve playerTagFlashingCurve;

    private Camera mainCamera;
    private List<Button> pathSelectionButtons = new();
    private PieceController currentPieceController;
    private (int id, SelectableItemUI selectable)? cardTargetPlayer;
    private List<PlayerTag> playerTags;
    private BoardViewerUI boardViewerUI;

    private void Start()
    {
        mainCamera = Camera.main;
        boardViewerUI = GetComponent<BoardViewerUI>();
    }

    public IEnumerator ConnectToPlayer(PieceController pieceController)
    {
        StopAllCoroutines();

        rollDiceButton.onClick.RemoveAllListeners();

        Cameraman.Reset();

        yield return new WaitForSeconds(1);

        turnActionsMenu.gameObject.SetActive(true);

        yield return FlashPlayerTagName(pieceController.Id);

        Cameraman.Zoom(Cameraman.FocusedOnPieceSize);
        Cameraman.Follow(() => pieceController.Piece.transform.position + Vector3.up * 1);

        currentPieceController = pieceController;
        rollDiceButton.onClick.AddListener(() =>
        {
            turnActionsMenu.gameObject.SetActive(false);
            var items = new List<ChoosableDiceUI>();
            for (int i = 0; i < currentPieceController.Piece.Dices.Length; i++)
            {
                Dice dice = currentPieceController.Piece.Dices[i];
                var item = Instantiate(choosableDicePrefab, cardMenu.SelectablesParent);
                item.Dice = dice;
                item.Button.onClick.AddListener(() =>
                {
                    turnActionsMenu.gameObject.SetActive(false);
                    HideSelectables();
                    currentPieceController.RollDice(dice);

                    //todo
                    finishRollingButton.gameObject.SetActive(true);
                });

                items.Add(item);
            }

            ShowSelectables(items, () => turnActionsMenu.gameObject.SetActive(true));
        });

        boardViewerUI.WaitForZoomOutButton(pieceController,
        () => { turnActionsMenu.gameObject.SetActive(false); },
        () =>
        {
            turnActionsMenu.gameObject.SetActive(true);
            Cameraman.Zoom(Cameraman.FocusedOnPieceSize);
            Cameraman.Follow(() => pieceController.Piece.transform.position + Vector3.up * 1);
        });
    }

    private IEnumerator FlashPlayerTagName(int playerIndex)
    {
        const float flashDuration = 1f;
        const float moveDuration = 0.5f;
        Vector2 flashTextPosition = new Vector2(Screen.width, Screen.height) / 2;
        const float flashStartFontSize = 40f;
        const float flashEndFontSize = 50f;

        var flashOriginalFontSize = playerTags[playerIndex].PlayerNameText.fontSize;
        var moveOriginalTextPosition = playerTags[playerIndex].PlayerNameText.transform.position;

        playerTags[playerIndex].PlayerNameText.transform.position = flashTextPosition;
        playerTags[playerIndex].PlayerNameText.fontSize = flashStartFontSize;

        var timer = 0f;

        while (timer < flashDuration)
        {
            playerTags[playerIndex].PlayerNameText.fontSize = Mathf.LerpUnclamped(flashStartFontSize, flashEndFontSize, playerTagFlashingCurve.Evaluate(timer / flashDuration));

            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(MoveCoroutine());

        yield return new WaitForSeconds(moveDuration / 3);

        IEnumerator MoveCoroutine()
        {
            timer = 0f;
            while (timer < moveDuration)
            {
                playerTags[playerIndex].PlayerNameText.fontSize = Mathf.LerpUnclamped(flashEndFontSize, flashOriginalFontSize, playerTagFlashingCurve.Evaluate(timer / moveDuration));
                playerTags[playerIndex].PlayerNameText.transform.position = Vector3.LerpUnclamped(flashTextPosition, moveOriginalTextPosition, playerTagFlashingCurve.Evaluate(timer / moveDuration));

                timer += BoardTime.DeltaTime;
                yield return 0;
            }

            playerTags[playerIndex].PlayerNameText.fontSize = flashOriginalFontSize;
            playerTags[playerIndex].PlayerNameText.transform.position = moveOriginalTextPosition;
        }
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

    public void ShowSelectables(IEnumerable<SelectableItemUI> selectables, Action onClose = null)
    {
        cardMenu.Open(selectables, onClose);
    }

    public List<CardUI> ShowCards(IEnumerable<Card> cards, Action<CardUI> onClick, Action onClose = null)
    {
        return cardMenu.ShowCards(cardPrefab, cards, onClick, onClose);
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

    public void SpawnPlayersTags(List<Piece> pieces)
    {
        playerTags = new();
        for (int i = 0; i < pieces.Count; i++)
        {
            var item = Instantiate(playerTagPrefab, playersTagsTransform);
            item.transform.localPosition = new Vector3(SpaceAround(i, pieces.Count, playersTagsTransform.sizeDelta.x), playersTagsTransform.position.y);
            item.PieceToFollow = pieces[i];
            playerTags.Add(item);
        }
    }

    private IEnumerator UseBirdCard(CardUI cardUI)
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
