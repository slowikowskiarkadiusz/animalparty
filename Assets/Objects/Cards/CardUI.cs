using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class CardUI : MonoBehaviour
{
    public Card Card { get; set; } = BirdCard.Magpie;
    [SerializeField] private TextMeshProUGUI text;

    public void Start()
    {
        text.text = Card?.Name;
    }
}
