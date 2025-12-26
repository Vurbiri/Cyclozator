using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;


public class Inventory : Singleton<Inventory>, ILoading
{
#if YSDK
    private const string GOODS = "ItemsYSDK/Goods";
#else
    private const string GOODS = "Items/Goods";
#endif
    private const string BUFFS = "Items/Buffs";

    private Dictionary<ItemSubType, Item> _dGoods;
    private Dictionary<ItemSubType, Item> _dBuffs;
        
    public Dictionary<ItemSubType, Item> Goods => _dGoods;
    public Dictionary<ItemSubType, Item> Buffs => _dBuffs;

    private ScoreTotal _score;
    public ScoreTotal TotalScore => _score;

    private const string keyData = "rty";
    private const int keyA = 28;
    private const int keyB = 29;

    public event Action EventLoad;

    protected override void OnUnloaded()
    {
        _score.ResetEvent();
        EventLoad = null;
    }

    public void AddBuff(Item item)
    {
        if (item == null) return;

        _dBuffs[item.SubType].Count++;
    }

    public void SetCell(int count)
    {
        if (count < 0) return;

        _dGoods[ItemSubType.Cell].Count = count;
    }

    public void Save(bool isSaveHard = true, Action<bool> callback = null) => Storage.Save(keyData, ConvertForSave(), isSaveHard, callback);
    public UniTask<bool> SaveAsync(bool isSaveHard = true) => Storage.SaveAsync(keyData, ConvertForSave(), isSaveHard);

    private Dictionary<int, long> ConvertForSave()
    {
        Dictionary<int, long> saveData = new();
        foreach (var b in _dBuffs)
            saveData[b.Key.ToInt()] = b.Value.Count;
        saveData[keyA] = _score.A;
        saveData[keyB] = _score.B;

        return saveData;
    }

    public bool LoadFromResources()
    {
        var (resultG, valueG) = Storage.LoadFromResources<Dictionary<ItemSubType, Item>>(GOODS);
        var (resultB, valueB) = Storage.LoadFromResources<Dictionary<ItemSubType, Item>>(BUFFS);

        if (resultG && resultB)
        {
            _dGoods = valueG;
            _dGoods.TrimExcess();
            _dBuffs = valueB;
            _dBuffs.TrimExcess();
            return true;
        }

        return false;
    }

    public bool Load()
    {
        var(result, value) = Storage.Load<Dictionary<int, long>>(keyData);
        if (!result)
            return false;

        _score = new(value[keyA], value[keyB]);
        foreach (var b in _dBuffs)
            b.Value.Count = (int)value[b.Key.ToInt()];
        InitScore();

        EventLoad?.Invoke();
        return true;
    }

    public void Create()
    {
        _score = new();

        var (resultB, valueB) = Storage.LoadFromResources<Dictionary<ItemSubType, Item>>(BUFFS);
        if (resultB)
            foreach (var b in _dBuffs)
                b.Value.Count = valueB[b.Value.SubType].Count;

        InitScore();
        EventLoad?.Invoke();
    }


    private void InitScore()
    {
#if YSDK
        if (_dGoods.TryGetValue(ItemSubType.Coin_1, out Item item))
            _score.SetConversionRate((int)item.Price);
#endif
    }

}


