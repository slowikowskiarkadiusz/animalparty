using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorEventField : FieldEvent
{
    // the offer has to be somehow generated. idk if has to be deplatable
    private List<Card> offer = new() { TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity };
    private bool canYield = false;

    public override IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController)
    {
        playerUiController.ShowCards(offer, card =>
        {
            playerUiController.StartCoroutine(OnCardSelected(card, pieceController, playerUiController));
        });

        // here can be like animations, camera zooming and such

        while (!canYield)
            yield return 0;

        Debug.Log("Yields");
    }

    private IEnumerator OnCardSelected(CardObject card, PieceController pieceController, PlayerUIController playerUiController)
    {
        yield return card.RunSelectingAnimation();

        pieceController.PiecesCards.Add(card.Card);
        canYield = true;
        playerUiController.HideSelectables();
    }
}