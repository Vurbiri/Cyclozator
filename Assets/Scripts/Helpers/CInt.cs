using System;

[System.Serializable]
public class CInt 
{
    private int _value;
    private static readonly int _code;

    private static readonly Random _random = new(Helper.Seed);

    private int Value
    {
        get => ~(_value ^ _code);
        set => _value = (~value) ^ _code;
    }

    static CInt() => _code = _random.Next(int.MinValue, int.MaxValue);
    private CInt(int value) => Value = value;

    public static implicit operator CInt(int value) => new(value);
    public static implicit operator int(CInt obj) => obj.Value;
    public static implicit operator long(CInt obj) => obj.Value;
    public static implicit operator string(CInt obj) => obj.ToString();

    public static CInt operator +(CInt a, CInt b) => new(a.Value + b.Value);
    public static CInt operator -(CInt a, CInt b) => new(a.Value - b.Value);
    public static CInt operator ++(CInt a) => a.Value + 1;
    public static CInt operator --(CInt a) => a.Value - 1;
    public static CInt operator *(CInt a, CInt b) => new(a.Value * b.Value);
    public static CInt operator /(CInt a, CInt b) => new(a.Value / b.Value);
    public static CInt operator %(CInt a, CInt b) => new(a.Value % b.Value);

    public override string ToString() => Value.ToString();
    public override bool Equals(object other) => this == (other as CInt);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(CInt a, CInt b) => a._value == b._value;
    public static bool operator !=(CInt a, CInt b) => a._value != b._value;
    public static bool operator >=(CInt a, CInt b) => a.Value >= b.Value;
    public static bool operator <=(CInt a, CInt b) => a.Value <= b.Value;
    public static bool operator >(CInt a, CInt b) => a.Value > b.Value;
    public static bool operator <(CInt a, CInt b) => a.Value < b.Value;
}


