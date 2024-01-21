using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuffGUI : ItemGUI
{
    private Shadow _iconShadow;
    private Shadow _countShadow;

    protected override void Initialize()
    {
        base.Initialize();

        _iconShadow = _icon.GetComponent<Shadow>();
        _countShadow = _countPanel.GetComponent<Shadow>();
    }

    public override void Setup(Item item)
    {
        gameObject.SetActive(true);
        ToggleIsOn = false;

        if (_item != null && _item.SubType == item.SubType) return;
        
        base.Setup(item);
    }

    public void Checking(List<ItemType> types)
    {
        _textCount.text = _item.Count.ToString();

        if (BuffStorage.Inst.MaxBuff == types.Count)
        {
            SetInteractable(false);
            return;
        }
        
        foreach (ItemType type in types)
        {
            if (_item.Type == type)
            {
                SetInteractable(false);
                return;
            }
        }
       
        SetInteractable(_item.Count > 0);
    }

    public void Deactivate() => gameObject.SetActive(false);

    protected override void SetInteractable(bool isOn)
    {

        base.SetInteractable(isOn);

        _iconShadow.enabled = isOn;
        if (isOn)
            _countShadow.effectDistance = Vector2.one * -3;
        else
            _countShadow.effectDistance = Vector2.zero;
    }

}
