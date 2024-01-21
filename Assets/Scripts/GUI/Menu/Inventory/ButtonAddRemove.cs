using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAddRemove : MonoBehaviour
{
    [SerializeField] private string _captionAdd = @"\";
    [SerializeField] private string _captionRemove = "/";
    [SerializeField] private string _captionNone = "-";

    private Button _buttonAddRemove;
    private Text _textButton;

    private ButtonMode _mode;

    public event Action<ButtonMode> EventClick;

    private void Awake()
    {
        _buttonAddRemove = GetComponent<Button>();
        _buttonAddRemove.onClick.AddListener(() => EventClick?.Invoke(_mode));
        _textButton = _buttonAddRemove.targetGraphic as Text;

        SwitchButton(_captionNone, false);
        _mode = ButtonMode.None;
    }

    public void Switch(ButtonMode mode)
    {
        if (_mode == mode) return;

        _mode = mode;

        switch (mode)
        {
            case ButtonMode.None: SwitchButton(_captionNone, false); break;
            case ButtonMode.Add: SwitchButton(_captionAdd, true); break;
            case ButtonMode.Remove: SwitchButton(_captionRemove, true); break; 
        };
     
    }


    private void SwitchButton(string caption, bool isOn)
    {
        _textButton.text = caption;
        _buttonAddRemove.interactable = isOn;
    }

}
