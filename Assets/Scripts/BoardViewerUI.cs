using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardViewerUI : MonoBehaviour
{
    [SerializeField] private FieldArrowObject fieldArrowPrefab;
    [SerializeField] private HighlightFrameObject highlightFramePrefab;

    private BoardGraph board;
    private List<GameObject> arrows = new();
    private HighlightFrameObject highlightFrame;
    private Coroutine navigateCoroutine;

    public void WaitForZoomOutButton(PieceController pieceController, Action onPressed, Action onReleased)
    {
        StopAllCoroutines();

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            while (true)
            {
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    BoardTime.Modifier = 0;
                    board = FindObjectsByType<BoardGraph>(FindObjectsSortMode.None).Single();
                    Cameraman.BeholdBoard(board);

                    navigateCoroutine = StartCoroutine(Navigate(pieceController));

                    onPressed();
                }

                if (Keyboard.current.qKey.wasReleasedThisFrame)
                {
                    BoardTime.Modifier = 1;

                    if (navigateCoroutine != null)
                        StopCoroutine(navigateCoroutine);

                    CleanAfterNavigateCoroutine();

                    onReleased();
                }

                yield return 0;
            }
        }
    }

    private void CleanAfterNavigateCoroutine()
    {
        if (highlightFrame)
            Destroy(highlightFrame.gameObject);

        foreach (var arrow in arrows)
            Destroy(arrow.gameObject);

        arrows.Clear();
    }

    private IEnumerator Navigate(PieceController pieceController)
    {
        var field = pieceController.Piece.Position;

        while (true)
        {
            CleanAfterNavigateCoroutine();

            highlightFrame = Instantiate(highlightFramePrefab);
            highlightFrame.transform.position = board.GetObject(field).transform.position;

            var neighbors = board.BidirectionalGraph[field].Select(x => (board.GetObject(x), x)).ToArray();

            var directions = new Vector3[]{
                new (0, 0, 1),
                new (1, 0, 0),
                new (0, 0, -1),
                new (-1, 0, 0),
            };

            var diffs = new List<Vector3>();

            foreach (var ahead in neighbors)
            {
                var arrow = Instantiate(fieldArrowPrefab, board.transform);
                arrow.transform.position = (ahead.Item1.transform.position + board.GetObject(field).transform.position) / 2 + Vector3.up * 0.1f;
                arrow.transform.rotation = Quaternion.LookRotation(ahead.Item1.transform.position - arrow.transform.position);
                arrow.Blink();
                arrows.Add(arrow.gameObject);

                var diff = Camera.main.WorldToScreenPoint(ahead.Item1.transform.position) - Camera.main.WorldToScreenPoint(board.GetObject(field).transform.position);
                diffs.Add(diff.normalized);
            }

            var fieldsInDirections = new (Transform, string)[directions.Length];

            for (int i = 0; i < directions.Length; i++)
            {
                var minDistance = float.MaxValue;
                var minIndex = -1;
                for (int ii = 0; ii < neighbors.Length; ii++)
                {
                    var distance = Vector3.Distance(diffs[ii], directions[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = ii;
                    }
                }

                fieldsInDirections[i] = neighbors[minIndex];
            }

            Dictionary<UnityEngine.InputSystem.Controls.KeyControl, int> keyDictionary = new()
            {
                {Keyboard.current.wKey, 0},
                {Keyboard.current.dKey, 1},
                {Keyboard.current.sKey, 2},
                {Keyboard.current.aKey, 3},
            };

            while (true)
            {
                var wasSelected = false;
                foreach (var keyPair in keyDictionary)
                {
                    if (keyPair.Key.wasPressedThisFrame && fieldsInDirections[keyPair.Value].Item1 != null)
                    {
                        field = fieldsInDirections[keyPair.Value].Item2;

                        wasSelected = true;
                    }
                }

                yield return 0;

                if (wasSelected)
                    break;
            }
        }
    }
}
