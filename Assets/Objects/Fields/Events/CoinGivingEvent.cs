using System.Collections;
using System.Linq;
using UnityEngine;

public class CoinGivingEvent : FieldEvent
{
    private readonly AnimationCurve movingCoinCurve = new(new Keyframe[] { new(0, 0, 2, 4), new(0.5f, 1, 0, 0), new(1, 0, -4, 0), });
    private readonly float movingCoinMaxHeight = 1;

    public override IEnumerator Execute(PieceController pieceController, PlayerUIController playerUiController, LoadableSet[] loadableSets)
    {
        var dataSet = loadableSets.Single(x => x.name == "Coin Giving Loadable Set");

        var coinPrefab = dataSet.Collection[0] as GameObject;

        var coin = Object.Instantiate(coinPrefab).transform;

        yield return MoveCoin(pieceController.Piece, coin);
    }

    private IEnumerator MoveCoin(Piece piece, Transform coin)
    {
        var coinGiver = GameObject.Find("Coin Giver").transform;
        var timer = 0f;
        var duration = 1f;
        var start = coinGiver.position;
        var end = piece.transform.position;
        var direction = end - start;

        while (timer < duration)
        {
            coin.position = Vector3.Lerp(start, end, timer / duration) + movingCoinCurve.Evaluate(timer / duration) * movingCoinMaxHeight * Vector3.up;

            timer += Time.deltaTime;
            yield return 0;
        }
    }
}