using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : SelectableItemUI
{
    public Card Card { get; set; } = BirdCard.Magpie;
    [SerializeField] private TextMeshProUGUI text;

    public void Start()
    {
        text.text = Card?.Name;
    }
}
