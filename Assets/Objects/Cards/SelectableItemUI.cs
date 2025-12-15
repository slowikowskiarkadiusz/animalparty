using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(FloatUI))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(EventTrigger))]
public class SelectableItemUI : MonoBehaviour
{
    private const float scalingSpeed = 5;
    private static List<SelectableItemUI> instances = new();

    private FloatUI _float;
    public FloatUI Float { get => _float ??= GetComponent<FloatUI>(); }
    private Button _button;
    public Button Button { get => _button ??= GetComponent<Button>(); }
    private RectTransform _rectTransform;
    public RectTransform RectTransform { get => _rectTransform ??= GetComponent<RectTransform>(); }
    private Outline _outline;
    private Outline Outline { get => _outline ??= GetComponent<Outline>(); }
    private EventTrigger _eventTrigger;
    private EventTrigger EventTrigger { get => _eventTrigger ??= GetComponent<EventTrigger>(); }

    private Color outlineColor = new(1f, 205f / 255f, 0, 1f);
    private int? originalChildIndex;
    private Vector3 targetScale = Vector3.one;
    [SerializeField] private float scaleBy = 1.2f;

    private void Awake()
    {
        SetupPointerInteraction();
        instances.Add(this);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scalingSpeed * BoardTime.DeltaTime);
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void CanInteract(bool can)
    {
        foreach (var instance in instances)
        {
            instance.Button.enabled = can;
            instance.EventTrigger.enabled = can;
        }
    }

    private void SetupPointerInteraction()
    {
        Outline.effectColor = outlineColor;

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => OnPointerEnter());
        EventTrigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => OnPointerExit());
        EventTrigger.triggers.Add(pointerExit);
    }

    public IEnumerator RunSelectingAnimation()
    {
        CanInteract(false);
        Float.StopRunning();

        const float duration = 1f;
        const float yOffset = 50;

        var timer = 0f;

        var startPosition = RectTransform.position;

        while (timer < duration)
        {
            RectTransform.position = Vector3.Lerp(RectTransform.position, startPosition + yOffset * Vector3.up, BoardTime.DeltaTime);

            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        yield return 0;
        CanInteract(true);
    }

    public void OnPointerEnter()
    {
        foreach (var instance in instances)
        {
            if (instance == this)
            {
                originalChildIndex = transform.GetSiblingIndex();
                transform.SetSiblingIndex(transform.parent.childCount - 1);
                targetScale = Vector3.one * scaleBy;
                Outline.enabled = true;
            }
            else
            {
                instance.OnPointerExit();
            }
        }
    }

    public void OnPointerExit()
    {
        originalChildIndex ??= transform.GetSiblingIndex();
        transform.SetSiblingIndex(originalChildIndex.Value);
        targetScale = Vector3.one * 1f;
        Outline.enabled = false;
    }
}
