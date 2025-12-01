using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardGraph : MonoBehaviour
{
    private const float movingPieceTime = 0.3f;

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
        {"AM", new []{"AR"}},
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

    private readonly Dictionary<string, CapsuleCollider> fieldDictionary = new();
    private ReadOnlyDictionary<string, List<Piece>> PiecesAtFields => new(Pieces.DistinctBy(x => x.Position).Select(x => new KeyValuePair<string, List<Piece>>(x.Position, Pieces.Where(y => y.Position == x.Position).ToList())).ToDictionary(x => x.Key, x => x.Value));
    public List<Piece> Pieces { get; } = new();
    public static int NumberOfPieces = -1;

    [SerializeField] private Transform fields;

    public void Init()
    {
        for (int i = 0; i < fields.childCount; i++)
        {
            var child = fields.GetChild(i);
            fieldDictionary[child.name] = child.GetComponent<CapsuleCollider>();
        }
    }

    public void MovePieceInstantlyTo(Piece piece, string field)
    {
        if (piece.Position != field)
        {
            if (!DoesFieldExist(field))
                throw new Exception($"Field {field} doesn't exist.");

            piece.Position = field;
            var positions = GetPiecesLocalPositionsAtField(field);

            for (int i = 0; i < PiecesAtFields[field].Count; i++)
            {
                PiecesAtFields[field][i].transform.position = positions[i];
            }
        }
    }

    public IEnumerator MovePieceForward(int pieceId, int selectedPath)
    {
        var timer = 0f;
        var startPosition = Pieces[pieceId].transform.position;
        var field = graph[Pieces[pieceId].Position][selectedPath];
        Pieces[pieceId].Position = field;
        var positions = GetPiecesLocalPositionsAtField(field);

        while (timer < movingPieceTime)
        {
            Pieces[pieceId].transform.position = Vector3.Lerp(startPosition, positions.Last(), timer / movingPieceTime);
            timer += Time.deltaTime;
            yield return 0;
        }

        Pieces[pieceId].transform.position = positions.Last();
    }

    public IEnumerator RunFieldsEvent(PieceController pieceController, PlayerUIController playerUiController)
    {
        var eventField = fieldDictionary[pieceController.PiecesPosition].GetComponent<EventField>();
        if (eventField)
            yield return eventField.Execute(pieceController, playerUiController);
    }

    public int GetNumberOfPiecesAtField(string field)
    {
        return Pieces.Count(x => x.Position == field);
    }

    public Vector3[] GetPiecesLocalPositionsAtField(string field)
    {
        var hasKey = PiecesAtFields.TryGetValue(field, out var pieces);
        var noOfPieces = hasKey ? pieces.Count() : 0;
        var result = new List<Vector3>();

        for (int i = 0; i < noOfPieces; i++)
        {
            var rad = 360 / noOfPieces * i * Mathf.Deg2Rad;
            var collider = fieldDictionary[field];
            var x = collider.transform.position.x + collider.radius * Mathf.Cos(rad);
            var z = collider.transform.position.z + collider.radius * Mathf.Sin(rad);

            result.Add(new Vector3(x, collider.transform.position.y, z));
        }

        return result.ToArray();
    }

    public void AddPiece(Piece piece)
    {
        piece.BoardGraph = this;
        Pieces.Add(piece);
        MovePieceInstantlyTo(piece, "START");
        NumberOfPieces = Pieces.Count;
    }

    private bool DoesFieldExist(string field)
    {
        return graph.TryGetValue(field, out _);
    }

    public bool IsForkAheadOfPiece(int pieceId, out Transform[] fieldsAhead)
    {
        var result = graph[Pieces[pieceId].Position].Skip(1).Any();
        fieldsAhead = graph[Pieces[pieceId].Position].Select(x => fieldDictionary[x].transform).ToArray();
        return result;
    }
}
