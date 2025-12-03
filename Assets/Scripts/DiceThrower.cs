using UnityEngine;

public class DiceThrower : MonoBehaviour
{
    [SerializeField] private DiceObject diceObjectPrefab;

    private DiceObject currentDiceObject;

    public void ShowDice(Piece piece, Dice dice)
    {
        if (currentDiceObject)
            Destroy(currentDiceObject);

        currentDiceObject = Instantiate(diceObjectPrefab);
        currentDiceObject.GenerateSides(dice);
        currentDiceObject.transform.position = piece.transform.position + 2 * Vector3.up;
        currentDiceObject.StartCoroutine(currentDiceObject.StartRollingDice());

    }

    public void Throw()
    {

    }
}
