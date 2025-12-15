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
        {"AW", new []{"AX", "BB"}},
        {"AX", new []{"AY"}},
        {"AY", new []{"AZ"}},
        {"AZ", new []{"BA"}},
        {"BA", new []{"AA"}},
        {"BB", new []{"BC"}},
        {"BC", new []{"BD"}},
        {"BD", new []{"BE"}},
        {"BE", new []{"AZ"}},
    };

    private readonly Dictionary<string, FieldEvent> fieldEvents = new()
    {
    };

    private readonly Dictionary<string, FieldEvent> interfieldEvents = new()
    {
        {"AO__AP", new VendorEventField()},
        {"AC__AD", new VendorEventField()},
        {"AA__AB", new CoinGivingEvent()},
        {"BC__BD", new CoinGivingEvent()},
    };

    public ReadOnlyDictionary<string, FieldObject> FieldDictionary { get; private set; }
    public ReadOnlyDictionary<string, List<string>> BidirectionalGraph { get; private set; }
    private ReadOnlyDictionary<string, List<Piece>> PiecesAtFields => new(Pieces.DistinctBy(x => x.Position).Select(x => new KeyValuePair<string, List<Piece>>(x.Position, Pieces.Where(y => y.Position == x.Position).ToList())).ToDictionary(x => x.Key, x => x.Value));
    public List<Piece> Pieces { get; } = new();
    public static int NumberOfPieces = -1;

    [SerializeField] private Transform fields;
    [SerializeField] private Float exclamationPointPrefab;
    [SerializeField] private Material fieldEventMaterial;
    [SerializeField] private AnimationCurve movingPieceCurve;
    [SerializeField] private float movingPieceMaxHeight = 2;
    [SerializeField] private LoadableSet[] loadableSets;

    public void Init()
    {
        Dictionary<string, FieldObject> fieldDictionary = new();
        for (int i = 0; i < fields.childCount; i++)
        {
            var child = fields.GetChild(i);
            fieldDictionary[child.name] = child.GetComponent<FieldObject>();

            if (fieldEvents.TryGetValue(child.name, out _))
                child.GetComponentInChildren<MeshRenderer>().material = fieldEventMaterial;
        }
        FieldDictionary = new(fieldDictionary);

        Dictionary<string, List<string>> bidirectionalGraph = new();
        foreach (var node in graph)
        {
            if (!bidirectionalGraph.TryGetValue(node.Key, out _))
                bidirectionalGraph[node.Key] = new();
            bidirectionalGraph[node.Key].AddRange(node.Value);

            foreach (var neighbor in node.Value)
            {
                if (!bidirectionalGraph.TryGetValue(neighbor, out _))
                    bidirectionalGraph[neighbor] = new();
                bidirectionalGraph[neighbor].Add(node.Key);
            }
        }
        BidirectionalGraph = new(bidirectionalGraph);

        foreach (KeyValuePair<string, FieldEvent> pair in interfieldEvents)
        {
            var keys = pair.Key.Split("__");
            var field1 = FieldDictionary[keys[0]];
            var field2 = FieldDictionary[keys[1]];

            var exclamationPoint = Instantiate(exclamationPointPrefab, transform);
            exclamationPoint.transform.position = (field1.transform.position + field2.transform.position) / 2;
            exclamationPoint.transform.position = exclamationPoint.transform.position + Vector3.up * exclamationPoint.MaxDistance / 2;
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

    public IEnumerator MovePieceForward(int pieceId, int selectedPath, PieceController pieceController, PlayerUIController playerUiController)
    {
        var haveRunInterfieldEvent = interfieldEvents.TryGetValue($"{Pieces[pieceId].Position}__{graph[Pieces[pieceId].Position][selectedPath]}", out var interfieldEvent);
        var field = graph[Pieces[pieceId].Position][selectedPath];
        Pieces[pieceId].Position = field;
        var positions = GetPiecesLocalPositionsAtField(field);

        if (haveRunInterfieldEvent)
        {
            yield return MoveToPosition(pieceId, Pieces[pieceId].transform.position / 2 + positions.Last() / 2);

            var faceEuler = Quaternion.LookRotation(positions.Last() - Pieces[pieceId].transform.position).eulerAngles;
            Pieces[pieceId].transform.eulerAngles = new Vector3(0, 0, faceEuler.z);
            yield return interfieldEvent?.Execute(pieceController, playerUiController, loadableSets);
        }

        yield return MoveToPosition(pieceId, positions.Last());

        Pieces[pieceId].transform.position = positions.Last();
        Pieces[pieceId].transform.localEulerAngles = Vector3.zero;

        if (fieldEvents.TryGetValue(pieceController.Piece.Position, out var value))
            yield return value?.Execute(pieceController, playerUiController, loadableSets);
    }

    private IEnumerator MoveToPosition(int pieceId, Vector3 targetPosition)
    {
        var timer = 0f;
        var startPosition = Pieces[pieceId].transform.position;

        while (timer < movingPieceTime)
        {
            var y = movingPieceCurve.Evaluate(timer / movingPieceTime) * movingPieceMaxHeight * Vector3.up;
            Pieces[pieceId].transform.position = Vector3.Lerp(startPosition, targetPosition, timer / movingPieceTime) + y;
            var faceEuler = Quaternion.LookRotation(targetPosition - Pieces[pieceId].transform.position).eulerAngles;
            Pieces[pieceId].transform.eulerAngles = new Vector3(0, 0, faceEuler.z);
            // Pieces[pieceId].transform.localEulerAngles += Vector3.right * -13;

            timer += BoardTime.DeltaTime;
            yield return 0;
        }
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
            var collider = FieldDictionary[field].BoxCollider;
            var x = collider.transform.position.x + collider.bounds.extents.x * Mathf.Cos(rad);
            var z = collider.transform.position.z + collider.bounds.extents.x * Mathf.Sin(rad);
            var y = collider.transform.position.y + collider.bounds.size.y;

            result.Add(new Vector3(x, y, z));
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

    public bool IsForkAheadOfPiece(int pieceId, out FieldObject[] fieldsAhead)
    {
        return IsForkAheadOfPiece(Pieces[pieceId].Position, out fieldsAhead);
    }

    public bool IsForkAheadOfPiece(string position, out FieldObject[] fieldsAhead)
    {
        var result = graph[position].Skip(1).Any();
        fieldsAhead = graph[position].Select(x => FieldDictionary[x]).ToArray();
        return result;
    }
}
