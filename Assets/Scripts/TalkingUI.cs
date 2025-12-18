using System;
using System.Collections;
using UnityEngine;

public class TalkingUI : MonoBehaviour
{
    private static TalkingUI instance;

    [SerializeField] private RectTransform bubbleParent;
    [SerializeField] private SpeechBubbleObject bubblePrefab;
    [SerializeField] private float bubbleOffset;

    void Awake()
    {
        instance = this;
    }

    public static void Show(Transform actor, string text, Action then)
    {
        instance.StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            var bubble = Instantiate(instance.bubblePrefab, instance.bubbleParent);
            yield return bubble.Run(actor.position + bubble.Size.y * 1.2f * Vector3.up, text);
            then();
        }
    }
}
