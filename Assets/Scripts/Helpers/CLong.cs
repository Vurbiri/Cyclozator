using System;

[System.Serializable]
public class CLong
{
    private long _value;
    private static readonly long _code;

    private static readonly Random _random = new(Helper.Seed);

    private long Value
    {
        get => ~(_value ^ _code);
        set => _value = (~value) ^ _code;
    }

    static CLong() => _code = _random.Next(int.MinValue, int.MaxValue);
    private CLong(long value) => Value = value;

    public static implicit operator CLong(long value) => new(value);
    public static implicit operator CLong(int value) => new(value);
    public static implicit operator long(CLong obj) => obj.Value;
    public static implicit operator string(CLong obj) => obj.ToString();

    public static CLong operator +(CLong a, CLong b) => new(a.Value + b.Value);
    public static CLong operator -(CLong a, CLong b) => new(a.Value - b.Value);
    public static CLong operator ++(CLong a) => a.Value + 1;
    public static CLong operator --(CLong a) => a.Value - 1;
    public static CLong operator *(CLong a, CLong b) => new(a.Value * b.Value);
    public static CLong operator /(CLong a, CLong b) => new(a.Value / b.Value);
    public static CLong operator %(CLong a, CLong b) => new(a.Value % b.Value);

    public override string ToString() => Value.ToString();
    public override bool Equals(object other) => this == (other as CLong);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(CLong a, CLong b) => a._value == b._value;
    public static bool operator !=(CLong a, CLong b) => a._value != b._value;
    public static bool operator >=(CLong a, CLong b) => a.Value >= b.Value;
    public static bool operator <=(CLong a, CLong b) => a.Value <= b.Value;
    public static bool operator >(CLong a, CLong b) => a.Value > b.Value;
    public static bool operator <(CLong a, CLong b) => a.Value < b.Value;
}
