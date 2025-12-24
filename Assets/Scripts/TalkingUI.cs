using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TalkingUI : MonoBehaviour
{
    private static TalkingUI instance;

    private static readonly Vector3 mainBubbleSize = new(4, 1.5f);
    private static readonly Vector3 optionBubbleSize = new(1.5f, 0.4f);

    [SerializeField] private RectTransform bubbleParent;
    [SerializeField] private SpeechBubbleObject bubblePrefab;
    [SerializeField] private float bubbleOffset;


    void Awake()
    {
        instance = this;
    }

    public static IEnumerator ShowAsCoroutine(Transform actor, string text, Dictionary<string, Action> dialogOptions = null)
    {
        const float gap = 0.1f;
        var mainBubble = Instantiate(instance.bubblePrefab, instance.bubbleParent);
        Cameraman.Follow(() => (actor.position + mainBubble.transform.position) / 2);
        var mainBubblePosition = actor.position + mainBubbleSize.y * 0.8f * Vector3.up;
        if (dialogOptions != null)
        {
            mainBubblePosition -= actor.transform.right * (mainBubbleSize.x / 2 + gap);
            yield return mainBubble.RunQuestion(mainBubblePosition, text, mainBubbleSize);
        }
        else
            yield return mainBubble.RunText(mainBubblePosition, text, mainBubbleSize);


        // bool doYield = false;

        if (dialogOptions != null)
        {
            var optionBubbles = new SpeechBubbleObject[dialogOptions.Count];
            for (int i = 0; i < optionBubbles.Length; i++)
            {
                var dialogOptionPair = dialogOptions.ElementAt(i);
                var optionBubble = Instantiate(instance.bubblePrefab, instance.bubbleParent);
                var optionPosition = mainBubble.transform.position + ((mainBubbleSize.x / 2) + gap + (optionBubbleSize.x / 2)) * mainBubble.transform.right;
                optionPosition.y = mainBubble.transform.position.y + Mathf.Lerp(mainBubbleSize.y / 2 - optionBubbleSize.y, -mainBubbleSize.y / 2 + optionBubbleSize.y, (float)i / (dialogOptions.Count - 1));
                optionBubbles[i] = optionBubble;
                instance.StartCoroutine(optionBubble.RunButton(optionPosition, dialogOptionPair.Key, optionBubbleSize));
                yield return new WaitForSeconds(0.3f);
            }

            var currentlySelected = 0;
            var isClicked = false;

            while (!isClicked)
            {
                foreach (var option in optionBubbles.Where(x => x != optionBubbles[currentlySelected]))
                    option.Blur();

                optionBubbles[currentlySelected].Focus();

                if (Keyboard.current.wKey.wasPressedThisFrame)
                    currentlySelected--;
                if (Keyboard.current.sKey.wasPressedThisFrame)
                    currentlySelected++;

                currentlySelected %= optionBubbles.Count();
                currentlySelected = currentlySelected < 0 ? optionBubbles.Count() - 1 : currentlySelected;

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    foreach (var option in optionBubbles.Where(x => x != optionBubbles[currentlySelected]))
                        Destroy(option.gameObject);

                    Destroy(mainBubble.gameObject);

                    optionBubbles[currentlySelected].Select(() => Destroy(optionBubbles[currentlySelected].gameObject));
                    isClicked = true;
                }

                yield return 0;
            }
        }
    }
}
