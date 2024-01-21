using System;
using UnityEngine;

[Serializable]
public class Buff
{
    public ItemType Type { get; }
    public ItemSubType SubType { get; }
    public float Value { get; }
    public ValueType TypeValue { get; }
    public Sprite Sprite { get; private set; }
    private const string _path = "Sprites/";
    public string Description { get; private set; }
   
    private readonly Func<float, float> _func;

    public Buff(ItemType type, ItemSubType subType, float value, ValueType typeValue)
    {
        Type = type; SubType = subType; Value = value; TypeValue = typeValue;

        Resources.LoadAsync<Sprite>(_path + subType.ToString()).completed += (a) => Sprite = (a as ResourceRequest).asset as Sprite;
        
        void setText() => Description = Localization.Inst.GetDescBuff(SubType);
        setText();
        Localization.Inst.EventSwitchLanguage += setText;

        _func = TypeValue switch
        {
            ValueType.Clamp => (x) => x > Value ? Value : x,
            ValueType.Multiplier => (x) => x * Value,
            ValueType.Summand => (x) => x + Value,
            _ => (x) => x
        };
    }

    public float Buffing(float value) => _func(value);
}
