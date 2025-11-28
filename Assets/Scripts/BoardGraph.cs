using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGraph : MonoBehaviour
{
    private readonly Dictionary<string, string[]> graph = new()
    {
        {"START", new []{"AA"}},
        {"AA", new []{"AB"}},
        {"AB", new []{"AC"}},
        {"AC", new []{"AD"}},
        {"AD", new []{"AE"}},
        {"AE", new []{"AF"}},
        {"AF", new []{"AG"}},
        {"AG", new []{"AH"}},
        {"AH", new []{"AI", "AN"}},
        {"AI", new []{"AJ"}},
        {"AJ", new []{"AK"}},
        {"AK", new []{"AL"}},
        {"AL", new []{"AM"}},
        {"AM", new []{"AN"}},
        {"AN", new []{"AO"}},
        {"AO", new []{"AP"}},
        {"AP", new []{"AR"}},
        {"AR", new []{"AS"}},
        {"AS", new []{"AT"}},
        {"AT", new []{"AU"}},
        {"AU", new []{"AW"}},
        {"AW", new []{"AX"}},
        {"AX", new []{"AY"}},
        {"AY", new []{"AZ"}},
        {"AZ", new []{"BA"}},
        {"BA", new []{"AA"}},
    };

    private readonly Dictionary<string, CapsuleCollider> nodeDictionary = new();
    private readonly Dictionary<string, List<Piece>> piecesAtFields = new();
    public List<Piece> Pieces { get; set; }

    [SerializeField] private Transform fields;

    private void Start()
    {
        for (int i = 0; i < fields.childCount; i++)
        {
            var child = fields.GetChild(i);
            nodeDictionary[child.name] = child.GetComponent<CapsuleCollider>();
        }
    }

    public void MovePieceTo(Piece piece, string field, bool instant)
    {
        if (piece.Position == field)
            return;

        if (!DoesFieldExist(field))
            throw new Exception($"Field {field} doesn't exist.");

        piece.Position = field;
        piecesAtFields.TryAdd(field, new());
        piecesAtFields[field].Add(piece);
        var positions = GetPiecesLocalPositionsAtField(field);

        for (int i = 0; i < piecesAtFields[field].Count; i++)
        {
            piecesAtFields[field][i].transform.position = positions[i];
        }
    }

    public int GetNumberOfPiecesAtField(string field)
    {
        return Pieces.Count(x => x.Position == field);
    }

    public Vector3[] GetPiecesLocalPositionsAtField(string field)
    {
        var noOfPieces = piecesAtFields[field].Count;
        var result = new List<Vector3>();

        for (int i = 0; i < noOfPieces; i++)
        {
            var rad = 360 / noOfPieces * i * Mathf.Deg2Rad;
            var collider = nodeDictionary[field];
            var x = collider.transform.position.x + collider.radius * Mathf.Cos(rad);
            var z = collider.transform.position.z + collider.radius * Mathf.Sin(rad);

            result.Add(new Vector3(x, collider.transform.position.y, z));
        }

        return result.ToArray();
    }

    public void AddPiece(Piece piece)
    {
        piece.BoardGraph = this;
        MovePieceTo(piece, "START", true);
    }

    private bool DoesFieldExist(string field)
    {
        return graph.TryGetValue(field, out _);
    }
}
