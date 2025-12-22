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
        instance.StartCoroutine(ShowAsCoroutine(actor, text, then));
    }

    public static IEnumerator ShowAsCoroutine(Transform actor, string text, Action then = null)
    {
        var bubble = Instantiate(instance.bubblePrefab, instance.bubbleParent);
        Cameraman.Follow(() => (actor.position + bubble.transform.position) / 2);
        yield return bubble.Run(actor.position + bubble.Size.y * 0.8f * Vector3.up, text);
    }
}
