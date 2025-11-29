using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    public Card Card { get; set; }
    [SerializeField] private TextMeshProUGUI text;

    public void Start()
    {
        text.text = Card.Name;
    }
}
