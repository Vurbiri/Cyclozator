using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public abstract class ScrollViewPanel<T> : ToggleGroup where T : BuffGUI
{
    [Space]
    [SerializeField] private T _itemPrefab;
    [Space]
    [SerializeField] protected Text _hint;
    [Space]

    private RectTransform _viewport;
    protected RectTransform _content;
    private string _hintKey;
    protected string _hintTextDefault;
    private float _minSpeedScroll;
    private Coroutine _coroutine;

    protected override void Awake()
    {
        base.Awake();

        ScrollRect rect = GetComponent<ScrollRect>();

        _viewport = rect.viewport;
        _content = rect.content;
        _minSpeedScroll = _itemPrefab.GetComponent<RectTransform>().rect.size.x * 2f;
        _hintKey = _hint.text;
        GetText();
        Localization.Inst.EventSwitchLanguage += GetText;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Localization.Inst.EventSwitchLanguage -= GetText;
    }

    protected virtual T CreateItem(Item item)
    {
        T itemGUI = Instantiate(_itemPrefab, _content);
        itemGUI.Setup(item);
        itemGUI.Group = this;
        itemGUI.EventSelect += OnItemSelected;
        itemGUI.EventDeselect += OnItemDeselected;
        itemGUI.EventFocus += AutoScroll;
        return itemGUI;
    }

   

    private void AutoScroll(RectTransform childTransform)
    {
        Canvas.ForceUpdateCanvases();

        float halfSizeViewport = _viewport.rect.size.x / 2f;
        float maxOffset = _content.rect.size.x - _viewport.rect.size.x;

        float offset = halfSizeViewport - childTransform.localPosition.x;

        if (offset > 0) offset = 0;
        else if (-offset > maxOffset) offset = -maxOffset;

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(SmoothScroll(new(offset, 0)));
    }

    private IEnumerator SmoothScroll(Vector2 endPoint)
    {
        Vector2 current = _content.localPosition;
        float speed;

        while (current != endPoint)
        {
            speed = Mathf.Abs(current.x - endPoint.x);
            if (speed < _minSpeedScroll) speed = _minSpeedScroll;

            _content.localPosition = current.MoveTowards(endPoint, speed * Time.deltaTime);
            yield return null;
        }

        _coroutine = null;
    }


    protected abstract void OnItemSelected(Item sender);
    protected abstract void OnItemDeselected(Item sender);

    protected virtual void GetText()
    {
        _hintTextDefault = Localization.Inst.GetText(_hintKey);
    }
}
