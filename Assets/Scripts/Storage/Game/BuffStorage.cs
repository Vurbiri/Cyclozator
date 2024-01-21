using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;


public partial class BuffStorage : Singleton<BuffStorage>, ILoading
{
    public int MaxBuff { get; private set; } = 1;
    public bool IsMaxCellBuff => MaxBuff == _maxBuffsCount;
    public List<Buff> Buffs => _buffs;
    public Buff[] BuffsArray
    {
        get
        {
            List<Buff> result = new(_buffs);
            result.RemoveAll(Item.Empty);
            return result.ToArray();
        }
    }

    private const int _maxBuffsCount = 4;
    private readonly List<Buff> _buffs = new();

    private const string key = "sfb";

    public event Action EventChangeBuffs;
    public event Action EventChangeCount;

    public bool AddBuff(Buff buff)
    {
        if (buff == null || _buffs.ContainsBuff(buff.Type)) return false;

        if (_buffs.RemoveAll(Item.Empty))
        {
            _buffs.Add(buff);
            while (_buffs.Count < MaxBuff) _buffs.Add(Item.Empty);
            EventChangeBuffs?.Invoke();
            return true;
        }
        return false;
    }

    public bool RemoveBuff(Buff buff) 
    {
        if (_buffs.Remove(buff))
        {
            _buffs.Add(Item.Empty);
            EventChangeBuffs?.Invoke();
            return true;
        }
        return false;
    }

    public void ResetBuffs(bool isSaveHard = true)
    {
        _buffs.Fill(Item.Empty, MaxBuff);
        EventChangeBuffs?.Invoke();
        Save(isSaveHard);
    }

    public async UniTask<bool> MaxBuffOut()
    {
        if (MaxBuff == _maxBuffsCount) return false;

        MaxBuff++;
        Inventory.Inst.SetCell(_maxBuffsCount - MaxBuff);
        _buffs.Add(Item.Empty);
        _buffs.TrimExcess();
        EventChangeCount?.Invoke();
        return await SaveBuffs();

        UniTask<bool> SaveBuffs()
        {
            UniTaskCompletionSource<bool> taskCallback = new();
            Save(false, (b) => taskCallback.TrySetResult(b));
            return taskCallback.Task;
        }
    }

    public void Save(bool isSaveHard = true, Action<bool> callback = null)
    {
        Storage.Save(key, ConvertToSaving(), isSaveHard, callback);

        List<int> ConvertToSaving()
        {
            List<int> list = new() { MaxBuff };
            foreach (var b in _buffs)
            {
                ItemSubType st =b.SubType;
                if (st != ItemSubType.None)
                    list.Add(st.ToInt());
            }
            list.TrimExcess();

            return list;
        }
    }

    public bool Load()
    {
        var (result, value) = Storage.Load<List<int>>(key);
        if (result)
            ConvertSaveData(value);
          
        return result;

        void ConvertSaveData(List<int> buffData)
        {
            MaxBuff = buffData[0];
            Inventory.Inst.SetCell(_maxBuffsCount - MaxBuff);

            _buffs.Clear();
            for (int i = 1; i < buffData.Count; i++)
            {
                ItemSubType st = Enum<ItemSubType>.FromInt(buffData[i]);
                _buffs.Add(Inventory.Inst.Buffs[st]);
            }

            while (_buffs.Count < MaxBuff) _buffs.Add(Item.Empty);
            _buffs.TrimExcess();
            EventChangeCount?.Invoke();
        }
    }

    public void Create()
    {
        MaxBuff = 1;
        _buffs.Fill(Item.Empty, MaxBuff);
        EventChangeCount?.Invoke();
        //EventChangeBuffs?.Invoke();
    }

}
