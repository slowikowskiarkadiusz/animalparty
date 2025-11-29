using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Piece : MonoBehaviour
{
    public string Position { get; set; }
    public BoardGraph BoardGraph { get; set; }
    public CapsuleCollider CapsuleCollider { get; private set; }
    public Dice[] Dices { get; private set; } = new[] { new DefaultDice() };
    public List<Card> Cards { get; set; } = new() { Card.BirdMagpie, Card.TotemPerpetualVelocity, Card.BirdMagpie, Card.TotemPerpetualVelocity, Card.BirdMagpie, Card.TotemPerpetualVelocity, Card.BirdMagpie, Card.TotemPerpetualVelocity, };

    private void Start()
    {
        CapsuleCollider = GetComponent<CapsuleCollider>();
    }
}
