using UnityEngine;
using UnityEngine.UI;

public abstract class ItemGUI : BuffGUI
{
    [Space]
    [SerializeField] protected Color _colorOn = Color.yellow;
    [SerializeField] protected Color _colorOff = Color.gray;
    [Space]
    [SerializeField] protected Image _countPanel;
    [SerializeField] protected Text _textCount;

    protected bool _isOn = true;

    protected override void Initialize()
    {
        base.Initialize();

        if (_isOn != _thisToggle.interactable)
            SetInteractable(!_isOn);
    }

    protected override void SetInteractable(bool isOn)
    {

        base.SetInteractable(isOn);

        Color currentColor = isOn ? _colorOn : _colorOff;
        _icon.color = currentColor;
        _countPanel.color = currentColor;

        _isOn = isOn;
    }
}
