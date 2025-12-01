using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopEventField : EventField
{
    // the offer has to be somehow generated. idk if has to be deplatable
    private List<Card> offer = new() { TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity, TotemCard.PerpetualVelocity };
    private bool canYield = false;

    public override IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController)
    {
        playerUiController.ShowCards(offer, card =>
        {
            StartCoroutine(OnCardSelected(card, pieceController, playerUiController));
        });

        // here can be like animations, camera zooming and such

        while (!canYield)
            yield return 0;
    }

    private IEnumerator OnCardSelected(CardUI card, PieceController pieceController, PlayerUIController playerUiController)
    {
        yield return card.RunSelectingAnimation();

        pieceController.PiecesCards.Add(card.Card);
        canYield = true;
        playerUiController.HideSelectables();
    }
}