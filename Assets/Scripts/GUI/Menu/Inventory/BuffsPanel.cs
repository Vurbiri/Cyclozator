using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsPanel : MonoBehaviour
{
    [SerializeField] private BuffGUI _itemPrefab;

    private Transform _content;
    private ToggleGroup _toggleGroup;

    private readonly List<BuffGUI> _buffs = new();

    public event Action<Item> EventBuffSelected;
    public event Action<Item> EventBuffDeselected;

    private void OnEnable()
    {
        foreach (BuffGUI buff in _buffs)
            buff.ToggleIsOn = false;
    }

    public void Initialize(ToggleGroup toggleGroup)
    {
        _content = this.transform;
        _toggleGroup = toggleGroup;

        foreach (var b in BuffStorage.Inst.Buffs)
            _buffs.Add(CreateItem(b));

        BuffStorage.Inst.EventChangeBuffs += Refresh;
        BuffStorage.Inst.EventChangeCount += ReInitialize;
    }

    private void ReInitialize()
    {
        if (_content == null || _toggleGroup == null) return;

        List<Buff> buff = BuffStorage.Inst.Buffs;
        int i = 0;
        for ( ; i < buff.Count; i++)
        {
            if (isChild())
                _buffs[i].Setup(buff[i] as Item);
            else
                _buffs.Add(CreateItem(buff[i]));

            _buffs[i].ToggleIsOn = false;
        }

        while (isChild())
            Destroy(_buffs[i++].gameObject);

        bool isChild() => i < _content.childCount;
    }

    private void Refresh()
    {
        if (_content == null) return;

        for (int i = 0; i < _content.childCount; i++)
        {
            _buffs[i].Setup(BuffStorage.Inst.Buffs[i] as Item);
            _buffs[i].ToggleIsOn = false;
        }

    }

    protected virtual BuffGUI CreateItem(Buff item)
    {
        BuffGUI itemGUI = Instantiate(_itemPrefab, _content);
        itemGUI.Setup(item as Item);
        itemGUI.Group = _toggleGroup;
        itemGUI.EventSelect += (s) => EventBuffSelected?.Invoke(s);
        itemGUI.EventDeselect += (s) => EventBuffDeselected?.Invoke(s);
        return itemGUI;
    }

    private void OnDestroy()
    {
        BuffStorage.Inst.EventChangeBuffs -= Refresh;
        BuffStorage.Inst.EventChangeCount -= ReInitialize;
    }
}
