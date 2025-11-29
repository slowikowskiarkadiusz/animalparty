using System;
using System.Collections;
using UnityEngine;

public static class PlayerUICoroutines
{
    public static IEnumerator UsingBirdCard(RectTransform card)
    {
        const float duration = 2f;
        const float yOffset = 30;

        var timer = 0f;

        var startPosition = card.position;

        while (timer < duration)
        {
            card.position = Vector3.Lerp(card.position, startPosition + yOffset * Vector3.up, timer / duration);

            timer += Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(1f);

        UnityEngine.Object.Destroy(card.gameObject);
    }
}