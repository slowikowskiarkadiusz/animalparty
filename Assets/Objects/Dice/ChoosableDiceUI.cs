using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ChoosableDiceUI : SelectableItemUI
{
    [SerializeField] private RectTransform sidePrefab;

    public Dice Dice { get; set; }

    private void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        var sideWidth = Math.Min(rectTransform.sizeDelta.y / Dice.Faces.Length, sidePrefab.sizeDelta.x) * 0.8f;

        for (int i = 0; i < Dice.Faces.Length; i++)
        {
            var placements = Dice.GenerateSidePipsPlacements(i);

            var side = Instantiate(sidePrefab, transform);
            var sideRatio = sideWidth / side.sizeDelta.x;
            Destroy(side.GetChild(0).gameObject);
            var from = -rectTransform.sizeDelta.y / 2 + sideWidth / 1.3f;
            side.localPosition = Vector3.Lerp(new Vector3(0, from, 0), new Vector3(0, from * -1, 0), (float)i / (Dice.Faces.Length - 1));
            side.localScale = sideRatio * Vector3.one;

            foreach (var pip in placements.pips)
            {
                var dot = Instantiate(sidePrefab.GetChild(0).GetComponent<RectTransform>(), side.transform);
                dot.localPosition = (new Vector3(pip.X, pip.Y, 0) * side.sizeDelta.x - side.sizeDelta.x / 2 * Vector3.one) * 0.5f;
                dot.localPosition += -20 * Vector3.forward;
                dot.localScale /= placements.scale;
            }
        }
    }
}
