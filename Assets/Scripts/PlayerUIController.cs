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

    private Camera mainCamera;
    private List<Button> pathSelectionButtons = new();
    private PieceController currentPieceController;

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
        var width = cardMenu.sizeDelta.x;

        var count = currentPieceController.PiecesCards.Count;
        for (int i = 0; i < count; i++)
        {
            var card = Instantiate(cardPrefab, cardMenu.transform);
            var offset = ((i + 1f) / (count + 1f) * width) - (width / 2f);
            card.transform.localPosition = new Vector3(offset, 0, 0);
        }
    }
}
