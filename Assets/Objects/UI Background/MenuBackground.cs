using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private Vector2 to;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animationTime = 1;
    [SerializeField] private Button closeButton;

    private Action onClose;

    public Transform SelectablesParent => transform.GetChild(0);

    private void Awake()
    {
        Resize(0, 0);
        Resize(1, 0);
    }

    public List<CardUI> ShowCards(CardUI cardPrefab, IEnumerable<Card> cards, Action<CardUI> onClick, Action onClose)
    {
        if (gameObject.activeInHierarchy)
            return null;

        var cardUIs = new List<CardUI>();

        var count = cards.Count();
        for (int i = 0; i < count; i++)
        {
            var card = Instantiate(cardPrefab, transform.GetChild(0));
            card.Card = cards.ElementAt(i);
            card.GetComponent<Button>().onClick.AddListener(() => onClick(card));
            cardUIs.Add(card);
        }

        Open(cardUIs, onClose);

        return cardUIs;
    }

    public void Open(IEnumerable<SelectableItemUI> selectables, Action onClose)
    {
        if (gameObject.activeInHierarchy)
            return;

        gameObject.SetActive(true);
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            this.onClose = onClose;
            var count = selectables.Count();
            for (int i = 0; i < count; i++)
                selectables.ElementAt(i).gameObject.SetActive(false);

            yield return Animation();

            var width = background.sizeDelta.x;

            for (int i = 0; i < count; i++)
            {
                var selectable = selectables.ElementAt(i);
                selectable.gameObject.SetActive(true);
                selectable.transform.SetParent(transform.GetChild(0));
                var posX = ((i + 1f) / (count + 1f) * width) - (width / 2f);
                selectable.transform.localPosition = new Vector3(posX, 0, 0);
                selectable.Float.Offset = (float)i / count;
                selectable.Float.StartRunning();
            }
        }
    }

    public void Close()
    {
        Resize(0, 0);
        Resize(1, 0);

        var childCount = transform.GetChild(0).childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(transform.GetChild(0).GetChild(i).gameObject);

        if (onClose != null)
            onClose();

        gameObject.SetActive(false);
    }

    private IEnumerator Animation()
    {
        closeButton.gameObject.SetActive(false);
        var timer = 0f;

        Resize(1, to[1] / 2f);

        yield return 0;

        while (timer < animationTime)
        {
            Resize(0, curve.Evaluate(timer / animationTime) * to[0]);
            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        timer = 0f;
        var from = to / 2f;

        while (timer < animationTime)
        {
            Resize(1, from[1] + curve.Evaluate(timer / animationTime) * (to[1] - from[1]));
            timer += BoardTime.DeltaTime;
            yield return 0;
        }

        Resize(1, to[1]);
        closeButton.gameObject.SetActive(true);
    }

    public void Resize(int axis, float to)
    {
        var sizeDelta = background.sizeDelta;
        sizeDelta[axis] = to;
        background.sizeDelta = sizeDelta;
    }
}
