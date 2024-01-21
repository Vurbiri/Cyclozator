using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewBuffs : ScrollViewPanel<ItemBuffGUI>
{
    [Space]
    [SerializeField] private ButtonAddRemove _buttonAddRemove;
    [SerializeField] private BuffsPanel _buffsPanel;

    private Item _selectItem;
    private Item _selectBuff;

    private readonly List<ItemBuffGUI> _buffs = new();

    protected override void Start()
    {
        base.Start();

        _buffsPanel.Initialize(this);
        _buffsPanel.EventBuffSelected += OnBuffSelected;
        _buffsPanel.EventBuffDeselected += OnBuffDeselected;

        _buttonAddRemove.EventClick += AddRemoveBuff;
    }

    protected override void OnEnable()
    {
        
        base.OnEnable();

        ReInitialize();
    }

    public void Save()
    {
        BuffStorage.Inst.Save(false);
        Inventory.Inst.Save();
    }

    private void AddRemoveBuff(ButtonMode mode)
    {
        if (mode == ButtonMode.Add)
            AddBuff();
        else if (mode == ButtonMode.Remove)
            RemoveBuff();

    }

    private void AddBuff()
    {
        if (_selectItem == null) return;

        if(BuffStorage.Inst.AddBuff(_selectItem))
            _selectItem.Count--;

        ReInitialize();
    }

    private void RemoveBuff()
    {
        if (_selectBuff == null) return;

        if (BuffStorage.Inst.RemoveBuff(_selectBuff))
        {
            Inventory.Inst.AddBuff(_selectBuff);

            ReInitialize();

            SetBuff(null, _hintTextDefault, ButtonMode.None);
        }

    }

    private void CheckingBuffs()
    {
        List<ItemType> types = new();

        foreach (var b in BuffStorage.Inst.Buffs)
            if(b.Type != ItemType.None)
                types.Add(b.Type);

        foreach (var b in _buffs)
           b.Checking(types);
    }

    private void ReInitialize()
    {
        
        int count = _buffs.Count, i = 0;
        foreach (var b in Inventory.Inst.Buffs.Values)
        {
            if (b.Count > 0)
            {
                if (i < count)
                    _buffs[i++].Setup(b);
                else
                    _buffs.Add(CreateItem(b));
            }
        }

        while (i < count)
            _buffs[i++].Deactivate();

        SetAllTogglesOff();

        CheckingBuffs();
        _selectItem = null;
        _selectBuff = null;
        SetElements(_hintTextDefault, ButtonMode.None);
    }

    protected override void OnItemSelected(Item sender) => SetItem(sender, sender.Description, ButtonMode.Add);
    protected override void OnItemDeselected(Item sender)
    {
        if (_selectItem != sender) return;

        SetItem(null, _hintTextDefault, ButtonMode.None);
    }

    private void OnBuffSelected(Item sender) => SetBuff(sender, sender.Description, ButtonMode.Remove);
    private void OnBuffDeselected(Item sender)
    {
        if (_selectBuff != sender) return;

        SetBuff(null, _hintTextDefault, ButtonMode.None);
    }

    private void SetItem(Item item, string hintText, ButtonMode mode)
    {
        _selectItem = item;
        SetElements(hintText, mode);
    }
    private void SetBuff(Item item, string hintText, ButtonMode mode)
    {
        _selectBuff = item;
        SetElements(hintText, mode);
    }
    private void SetElements(string hintText, ButtonMode mode)
    {
        _hint.text = hintText;
        _buttonAddRemove.Switch(mode);
    }

    protected override void GetText()
    {
        base.GetText();
        
        if(_selectBuff != null)
        {
            _hint.text = _selectBuff.Description;
            return;
        }

        if (_selectItem != null)
        {
            _hint.text = _selectItem.Description;
            return;
        }

        _hint.text = _hintTextDefault;
    }

}
