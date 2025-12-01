using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(FloatUI))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class SelectableItemUI : MonoBehaviour
{
    private FloatUI _float;
    public FloatUI Float { get => _float ??= GetComponent<FloatUI>(); }
    private Button _button;
    public Button Button { get => _button ??= GetComponent<Button>(); }
    private RectTransform _rectTransform;
    public RectTransform RectTransform { get => _rectTransform ??= GetComponent<RectTransform>(); }
}
