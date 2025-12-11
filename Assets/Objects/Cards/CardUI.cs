using TMPro;
using UnityEngine;

public class CardUI : SelectableItemUI
{
    private readonly Vector2 priceTagOffset = Vector2.up * 2;

    public Card Card { get; set; } = BirdCard.Magpie;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform priceTagPrefab;

    private RectTransform currentPriceTag;

    public void Start()
    {
        text.text = Card?.Name;
    }

    public void ShowPrice(int price)
    {
        HidePrice();

        var priceTag = Instantiate(priceTagPrefab, transform);
        priceTag.anchoredPosition = priceTagOffset;
        priceTag.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
    }

    public void HidePrice()
    {
        if (currentPriceTag)
            Destroy(currentPriceTag.gameObject);
    }
}
