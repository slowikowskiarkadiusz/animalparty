using System;
using System.Collections;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

public class DiceObject : MonoBehaviour
{
    [SerializeField] private Transform dotPrefab;

    private float radius = 2;
    public bool pips = true;
    private Dice dice = Dice.Default;
    public Vector3 minRotateVector;
    public Vector3 maxRotateVector;
    public float rotateChangeEvery = 2;
    public float rotateChangeSpeed = 1;

    private Vector3 randomizedRotateVector;
    private Vector3 rotateVector;
    private Coroutine rollingDiceAnimation;

    readonly Vector3[] rotationDict = new[]
    {
        Vector3.zero,
        new Vector3(0, 90, 0),
        new Vector3(0, 0, 90),
        new Vector3(0, 0, 270),
        new Vector3(0, 270, 0),
        new Vector3(0, 180, 0),
    };

    public void GenerateSides(Dice dice)
    {
        this.dice = dice;
        
        for (int i = 0; i < dice.Faces.Length; i++)
            SpawnSide(i);

        StartCoroutine(ModifyRotateVector());
        StartCoroutine(RandomizeRollVector());

        IEnumerator ModifyRotateVector()
        {
            while (true)
            {
                rotateVector = Vector3.Lerp(rotateVector, randomizedRotateVector, rotateChangeSpeed * Time.deltaTime);
                yield return 0;
            }
        }

        IEnumerator RandomizeRollVector()
        {
            rotateVector = minRotateVector;
            while (true)
            {
                randomizedRotateVector = new Vector3(
                    UnityEngine.Random.Range(minRotateVector.x, maxRotateVector.x),
                    UnityEngine.Random.Range(minRotateVector.y, maxRotateVector.y),
                    UnityEngine.Random.Range(minRotateVector.z, maxRotateVector.z));
                yield return new WaitForSeconds(rotateChangeEvery);
            }
        }
    }

    public IEnumerator StartRollingDice()
    {
        if (rollingDiceAnimation != null)
            StopCoroutine(rollingDiceAnimation);

        while (true)
        {
            transform.Rotate(rotateVector * Time.deltaTime);
            yield return 0;
        }
    }

    public IEnumerator FinishRollingDice(int faceIndex)
    {
        if (rollingDiceAnimation != null)
            StopCoroutine(rollingDiceAnimation);

        transform.eulerAngles = rotationDict[faceIndex];
        yield return LiftDiceAnimation();
    }

    public IEnumerator LiftDiceAnimation()
    {
        const float duration = 1f;
        const float yOffset = 50;

        var timer = 0f;

        var startPosition = transform.position;

        while (timer < duration)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition + yOffset * Vector3.up, Time.deltaTime);

            timer += Time.deltaTime;
            yield return 0;
        }
    }

    private void SpawnSide(int index)
    {
        var placements = dice.GenerateSidePipsPlacements(index);
        var sideTransform = new GameObject().transform;
        sideTransform.SetParent(transform);
        sideTransform.localScale = Vector3.one;
        sideTransform.localPosition = Vector3.zero;

        var width = radius * Mathf.Sqrt(2) / 2.7f;
        foreach (var pip in placements.pips)
        {
            var dot = Instantiate(dotPrefab, sideTransform);
            dot.localScale /= placements.scale;
            dot.localPosition = new Vector3(
                -1.01f,
                pip.X * width,
                pip.Y * width);
        }

        // sideTransform.localEulerAngles = new Vector3(0, (index % 3 + index / 5) * 90, index == 3 ? 90 : index == 4 ? -90 : 0);
        sideTransform.localEulerAngles = rotationDict[index];
    }
}
