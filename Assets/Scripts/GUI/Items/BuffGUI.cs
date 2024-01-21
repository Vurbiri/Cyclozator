using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(SelectableItem))]
public class BuffGUI : BuffIcon
{
    protected Toggle _thisToggle;
    protected Item _item;
    private RectTransform _thisTransform;


    public ToggleGroup Group { get => _thisToggle.group; set => _thisToggle.group = value; }
    public bool ToggleIsOn { get => _thisToggle.isOn; set => _thisToggle.isOn = value; }

    public event Action<Item> EventSelect;
    public event Action<Item> EventDeselect;

    public event Action<RectTransform> EventFocus;

    private void Awake() => Initialize();

    protected virtual void Initialize()
    {
        _thisToggle = GetComponent<Toggle>();
        _thisToggle.onValueChanged.AddListener(OnSelect);
        _thisTransform = GetComponent<RectTransform>();
        GetComponent<SelectableItem>().EventSelect += () => EventFocus?.Invoke(_thisTransform);
    }


    public virtual void Setup(Item item)
    {
        if (_item == item) return;
        
        if (_thisToggle == null || _thisTransform == null)
            Initialize();

        _item = item;
        Icon = _item.Sprite;
        SetInteractable(_item.SubType != ItemSubType.None);
    }

    private void OnSelect(bool isOn)
    {
        if (isOn)
            EventSelect?.Invoke(_item);
        else
            EventDeselect?.Invoke(_item);
    }


    protected virtual void SetInteractable(bool isOn)
    {
        _thisToggle.interactable = isOn;
        if (ToggleIsOn && !isOn) ToggleIsOn = isOn;
    }
}
