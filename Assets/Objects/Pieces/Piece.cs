using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Piece : MonoBehaviour
{
    public string Position { get; set; }
    public BoardGraph BoardGraph { get; set; }
    public CapsuleCollider CapsuleCollider { get; private set; }
    public Dice[] Dices { get; private set; } = new[] { new DefaultDice() };
    public List<Card> Cards { get; set; } = new() { BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie };

    private void Start()
    {
        CapsuleCollider = GetComponent<CapsuleCollider>();
    }
}
