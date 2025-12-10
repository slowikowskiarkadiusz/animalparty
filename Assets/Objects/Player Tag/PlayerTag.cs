using TMPro;
using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI victoryPointsText;
    [SerializeField] private TextMeshProUGUI coinText;

    public Piece PieceToFollow { get; set; }

    private void Update()
    {
        playerNameText.text = PieceToFollow.PlayersName;
        victoryPointsText.text = PieceToFollow.VictoryPoints.ToString();
        coinText.text = PieceToFollow.Coins.ToString();
    }
}