
using System;
using System.Collections;
using UnityEngine;

public abstract class Interruption
{
    public string[] BetweenFields { get; }

    public Interruption(string[] betweenFields)
    {
        if (betweenFields.Length != 2)
            throw new ArgumentException($"{nameof(betweenFields)} has to be of length 2.");
        BetweenFields = betweenFields;
    }

    public abstract IEnumerator Execute();
}

public class FullCircleInterruption : Interruption
{
    public FullCircleInterruption(string[] betweenFields) : base(betweenFields) { }

    public override IEnumerator Execute()
    {
        Debug.Log("Worked!");
        yield return 0;
    }
}