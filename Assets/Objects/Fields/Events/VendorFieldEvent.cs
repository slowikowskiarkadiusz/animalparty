using System.Collections;
using System.Collections.Generic;

public class VendorEventField : FieldEvent
{
    // the offer has to be somehow generated. idk if has to be deplatable
    private List<Card> offer = new() { TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity };
    private bool canYield = false;

    public VendorEventField(int index) : base(index) { }

    public override IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController, FieldEventDataSet[] fieldEventDataSets)
    {
        var dataSet = GetDataSet<VendorFieldEventDataSet>(fieldEventDataSets, FieldEventType.VendorFieldEvent);

        yield return TalkingUI.ShowAsCoroutine(dataSet.ShopActor, "I'm a working shop!");
        Cameraman.Follow(pieceController.Piece.transform);

        var cardsUI = playerUiController.ShowCards(offer, card =>
                {
                    playerUiController.StartCoroutine(OnCardSelected(card, pieceController, playerUiController));
                },
                () => canYield = true);

        foreach (var cardUI in cardsUI)
        {
            cardUI.ShowPrice(2);
        }

        // here can be like animations, camera zooming and such

        while (!canYield)
            yield return 0;

        Reset();
    }

    private IEnumerator OnCardSelected(CardUI card, PieceController pieceController, PlayerUIController playerUiController)
    {
        yield return card.RunSelectingAnimation();

        pieceController.PiecesCards.Add(card.Card);
        canYield = true;
        playerUiController.HideSelectables();
    }

    private void Reset()
    {
        canYield = false;
    }
}