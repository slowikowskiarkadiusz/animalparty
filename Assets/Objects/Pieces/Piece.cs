using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string Position { get; set; }
    public BoardGraph BoardGraph { get; set; }
    public Transform PiecePrefab { get; set; }
    public Dice[] Dices { get; private set; } = new[] { Dice.Default, Dice.FukkedUp };
    public List<Card> Cards { get; set; } = new() { BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie };

    [SerializeField] private Transform defaultPiecePrefab;

    private float pieceSpawnHeight = 0.24f;
    private Transform piece;

    private void Start()
    {
        //TODO
        // var standMeshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        // pieceSpawnHeight = (standMeshRenderer.bounds.center.y + standMeshRenderer.bounds.extents.y) * standMeshRenderer.transform.lossyScale.y;

        piece = Instantiate(defaultPiecePrefab ?? PiecePrefab);
        piece.SetParent(transform);
        piece.localPosition = new Vector3(0, pieceSpawnHeight, 0);
    }
}
