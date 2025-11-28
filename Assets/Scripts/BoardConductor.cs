using System;
using UnityEngine;

[RequireComponent(typeof(BoardGraph))]
public class BoardConductor : MonoBehaviour
{
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private int noOfPlayersToSpawn = 4;
    private BoardGraph boardGraph;

    private void Start()
    {
        boardGraph = GetComponent<BoardGraph>();

        for (int i = 0; i < noOfPlayersToSpawn; i++)
        {
            boardGraph.AddPiece(Instantiate(piecePrefab));
        }
    }
}
