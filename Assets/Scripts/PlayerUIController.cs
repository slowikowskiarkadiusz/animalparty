using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNumberText;
    [SerializeField] private Button rollDiceButton;

    public void ConnectToPlayer(PieceController pieceController)
    {
        rollDiceButton.onClick.RemoveAllListeners();

        playerNumberText.text = $"Player {pieceController.Id}";
        rollDiceButton.onClick.AddListener(() => pieceController.RollDice());
    }

    public void ShowPathSelectionMenu()
    {
        throw new NotImplementedException();
    }
}
