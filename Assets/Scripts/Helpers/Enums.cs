using System;
using System.Linq;

public enum FigureType : byte
{
    Off,
    Blue,
    Green,
    Orange,
    Purple,
    Red,
    Teal
}

[Flags]
public enum BridgeType : byte
{
    None = 0,
    East = 1,
    Nord = 2,
    West = 4,
    South = 8
}

public enum DirectRotation : sbyte
{
    Right = 1,
    None = 0,
    Left = -1   
}

public enum Turn : byte
{
    Degree_0,
    Degree_22_5,
    Degree_45,
    Degree_67_5,
    Degree_90,
    Degree_112_5,
    Degree_135,
    Degree_157_5,
    Degree_180,
    Degree_202_5,
    Degree_225,
    Degree_247_5,
    Degree_270,
    Degree_292_5,
    Degree_315,
    Degree_337_5
}

public enum ValueType : byte
{
    None = 0,
    Clamp,
    Multiplier,
    Summand
}

public enum ItemType : sbyte
{
    Coin = -2,
    Cell = -1,
    None = 0,
    Bridge,
    Catch,
    Count,
    MultiSpawn,
    RotSpeed,
    Sector,
    Spawn,
    SpawnOne,
    Speed,
    Points
}

public enum ItemSubType : sbyte
{
#if YSDK
    Coin_Yan = -5,
    Coin_Ads = -4,
#endif
    Coin_10 = -3,
    Coin_1 = -2,
    Cell = -1,
    None = 0,
    Bridge,
    Catch_10,
    Catch_20,
    Catch_30,
    Count,
    MultiSpawnThreeOff,
    MultiSpawn_10,
    MultiSpawn_20,
    MultiSpawn_30,
    RotSpeed_10,
    RotSpeed_20,
    RotSpeed_30,
    Sector,
    Spawn_10,
    Spawn_20,
    Spawn_30,
    SpawnOne_10,
    SpawnOne_20,
    SpawnOne_30,
    Speed_10,
    Speed_20,
    Speed_30,
    Points_x2,
}

public enum ItemOverType : byte
{
    None,
    Buff,
    Goods
}

public enum Currency : byte
{
    None,
    Coins,
    Points,
#if YSDK
    Ad,
    Yan
#endif
}

public enum MixerGroup : byte 
{ 
    Master,
    Music,
    SFX,
    Ambient
}

public enum ButtonMode : byte 
{ 
    None,
    Add,
    Remove
}

public enum GameMode : byte
{
    None,
    Gameplay,
    Initialize
}

public enum GameModeStart : byte
{
    New,
    ContinueSimple,
    Continue
}

public enum ChangingType : byte
{
    None,
    Disappearances,
    Appearances,
}

public enum ModeShowGUI : byte
{
    Default,
    AlwaysShow,
    AlwaysHide,
}

public enum AvatarSize : byte
{
    Small,
    Medium,
    Large
}

public enum MessageType : byte
{
    Normal,
    Warning,
    Error,
    FatalError
}

public enum StatusPurchase : byte
{
    Ready,
    Validate,
    AddAndSave,
    Consume
}

public class Enum<T> where T : Enum
{
    public static int Count => Enum.GetNames(typeof(T)).Length;

    public static T FromInt(int value) => (T)Enum.ToObject(typeof(T), value);

    public static T[] GetValues() => Enum.GetValues(typeof(T)).Cast<T>().ToArray<T>();
}

public static class ExtensionsEnum
{
    public static int ToInt<T>(this T self) where T : Enum => Convert.ToInt32(self);

    public static int Count(this BridgeType self)
    {
        int openBridgesCount = 0;
        BridgeType[] types = Enum<BridgeType>.GetValues();

        foreach (var t in types)
        {
            if (t == BridgeType.None) continue;

            if ((self & t) == t)
                openBridgesCount++;

        }

        return openBridgesCount;
    }

    public static void RandomFillArray<T>(this T[] self) where T : Enum
    {
        int count = self.Length;
        T[] allValues = Enum<T>.GetValues();
        T newValue;

        while (count > 0)
        {
            newValue = allValues.GetRandomElement(1);
            if (Array.IndexOf(self, newValue) == -1)
                self[--count] = newValue;
        }

    }

    #region Turn   
    public static float ToDegree(this Turn self) => 22.5f * self.ToInt();

    public static int Offset(this ref Turn self, int offset)
    {
        int newValue = self.ToInt() + offset;
        int length = Enum<Turn>.Count;
        while (newValue >= length) newValue -= length;
        while (newValue < 0) newValue += length;
        self = (Turn)newValue;
        return newValue;
    }
    public static int Offset(this ref Turn self, DirectRotation offset) => self.Offset(offset.ToInt());
    #endregion
}