using System;
using System.Collections;
using UnityEngine;

public class DiceThrower : MonoBehaviour
{
    [SerializeField] private DiceObject diceObjectPrefab;
    [SerializeField] private AnimationCurve diceShowingSizeCurve;
    [SerializeField] private AnimationCurve diceDisapearingSizeCurve;

    private DiceObject currentDiceObject;
    public Dice Dice { get; private set; }

    public Transform ShowDice(Piece piece, Dice dice)
    {
        if (currentDiceObject)
            Destroy(currentDiceObject);

        currentDiceObject = Instantiate(diceObjectPrefab);
        Dice = dice;
        currentDiceObject.GenerateSides(dice);

        const float abovePieceY = 1.5f;

        currentDiceObject.transform.position = piece.transform.position + abovePieceY * Vector3.up;
        currentDiceObject.StartCoroutine(Coroutine());
        return currentDiceObject.transform;

        IEnumerator Coroutine()
        {
            StartCoroutine(ShowDiceCoroutine(true));
            yield return currentDiceObject.StartRollingDice();
        }
    }

    public IEnumerator FinishRolling(int faceIndex)
    {
        yield return Coroutine();

        StartCoroutine(ShowDiceCoroutine(false));

        IEnumerator Coroutine()
        {
            const float duration = 1f;

            const float yOffset = 1f;

            var timer = 0f;

            var startPosition = currentDiceObject.transform.position;

            currentDiceObject.StopRollingDice();

            while (timer < duration)
            {
                currentDiceObject.transform.position = Vector3.Lerp(currentDiceObject.transform.position, startPosition + yOffset * Vector3.up, BoardTime.DeltaTime);
                currentDiceObject.GenerateSingleSide(faceIndex);
                currentDiceObject.transform.rotation = Quaternion.LookRotation(Cameraman.CurrentPosition - currentDiceObject.transform.position);

                timer += BoardTime.DeltaTime;
                yield return 0;
            }
        }
    }

    private IEnumerator ShowDiceCoroutine(bool show)
    {
        const float duration = 0.3f;

        var timer = 0f;

        currentDiceObject.StopRollingDice();

        var originalScale = currentDiceObject.transform.localScale;

        while (timer < duration)
        {
            var step = timer / duration;
            var curve = show ? diceShowingSizeCurve : diceDisapearingSizeCurve;
            currentDiceObject.transform.localScale = curve.Evaluate(step) * originalScale;
            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        if (!show)
            Destroy(currentDiceObject.gameObject);
    }
}
