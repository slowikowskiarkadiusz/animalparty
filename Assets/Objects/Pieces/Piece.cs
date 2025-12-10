using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string Position { get; set; }
    public BoardGraph BoardGraph { get; set; }
    public Transform PiecePrefab { get; set; }
    public Dice[] Dices { get; set; } = new[] { Dice.Default, Dice.FukkedUp };
    public List<Card> Cards { get; set; } = new() { BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie, BirdCard.Magpie };
    public string PlayersName { get; set; }
    public int VictoryPoints { get; set; } = 0;
    public int Coins { get; set; } = 0;

    [SerializeField] private Transform defaultPiecePrefab;

    private float pieceSpawnHeight = 0.24f;
    private Transform piece;

    public Piece Spawn(PlayerRegistration playerRegistration, Color color)
    {
        //TODO
        // var standMeshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        // pieceSpawnHeight = (standMeshRenderer.bounds.center.y + standMeshRenderer.bounds.extents.y) * standMeshRenderer.transform.lossyScale.y;

        piece = Instantiate(defaultPiecePrefab ?? PiecePrefab);
        piece.SetParent(transform);
        piece.localPosition = new Vector3(0, pieceSpawnHeight, 0);

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (meshRenderer.gameObject.CompareTag("Paintable Piece Part"))
            {
                meshRenderer.materials[0].color = color;
            }
        }

        PlayersName = playerRegistration.Name;

        return this;
    }
}
