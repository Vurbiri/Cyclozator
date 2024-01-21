using System;

public class Item : Buff
{
    public ItemOverType OverType { get; }
    public CInt Price { get; }
    public Currency Currency { get; }
    public int Count { get => _count; set { _count = value; EventCountChange?.Invoke(_count); } }
    private CInt _count;
    public bool IsBuff => OverType == ItemOverType.Buff;

    public event Action<int> EventCountChange;

    public static Item Empty { get; }

    public Item(ItemOverType overType, int price, Currency currency, int count, ItemType type, ItemSubType subType, float value, ValueType typeValue) 
        : base(type, subType, value, typeValue)
    {
        OverType = overType; Price = price; Currency = currency; _count = count;
           
    }

    static Item() => Empty = new(ItemOverType.None, 0, Currency.None, 0, ItemType.None, ItemSubType.None, 0, ValueType.None);

    public Buff GetBuff()
    {
        if (!IsBuff) return null;

        Count--;
        return this;
    }

    
}
