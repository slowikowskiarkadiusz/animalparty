using System.Collections;
using UnityEngine;

public static class UIAnimations
{
    public static IEnumerator SelectingUIItem(SelectableItemUI selectable)
    {
        selectable.Float.StopRunning();

        const float duration = 1f;
        const float yOffset = 30;

        var timer = 0f;

        var startPosition = selectable.RectTransform.position;

        while (timer < duration)
        {
            selectable.RectTransform.position = Vector3.Lerp(selectable.RectTransform.position, startPosition + yOffset * Vector3.up, Time.deltaTime);

            timer += Time.deltaTime;
            yield return 0;
        }

        yield return 0;
    }
}