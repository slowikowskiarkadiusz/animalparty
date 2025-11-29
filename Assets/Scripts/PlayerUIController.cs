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
    [SerializeField] private Button rollDiceButton;
    [SerializeField] private Button pathSelectionButtonPrefab;

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
        rollDiceButton.gameObject.SetActive(true);
        rollDiceButton.onClick.AddListener(() =>
        {
            currentPieceController.RollDice();
            rollDiceButton.gameObject.SetActive(false);
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
}
