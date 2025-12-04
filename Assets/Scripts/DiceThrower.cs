using System;
using System.Collections;
using UnityEngine;

public class DiceThrower : MonoBehaviour
{
    [SerializeField] private DiceObject diceObjectPrefab;

    private DiceObject currentDiceObject;
    public Dice Dice { get; private set; }

    public Transform ShowDice(Piece piece, Dice dice)
    {
        if (currentDiceObject)
            Destroy(currentDiceObject);

        currentDiceObject = Instantiate(diceObjectPrefab);
        Dice = dice;
        currentDiceObject.GenerateSides(dice);
        currentDiceObject.transform.position = piece.transform.position + 2 * Vector3.up;
        currentDiceObject.StartCoroutine(currentDiceObject.StartRollingDice());
        return currentDiceObject.transform;
    }

    public IEnumerator FinishRolling(int faceIndex)
    {
        const float duration = 1f;
        const float yOffset = 1f;

        var timer = 0f;

        var startPosition = currentDiceObject.transform.position;

        currentDiceObject.StopRollingDice();

        while (timer < duration)
        {
            currentDiceObject.transform.position = Vector3.Lerp(currentDiceObject.transform.position, startPosition + yOffset * Vector3.up, Time.deltaTime);
            currentDiceObject.GenerateSingleSide(faceIndex);
            currentDiceObject.transform.rotation = Quaternion.LookRotation(Cameraman.CurrentPosition - currentDiceObject.transform.position);

            timer += Time.deltaTime;
            yield return 0;
        }

        Destroy(currentDiceObject.gameObject);
    }
}
