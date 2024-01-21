using System;
using UnityEngine;


public static class Helper
{
    public static Quaternion[] ArrayAngles { get; }
    public static int Seed { get => unchecked(_seed++); }
    private static int _seed = unchecked((int)-DateTime.Now.Ticks);

    static Helper()
    {
        int count = Enum<Turn>.Count;
        float angle = 22.5f;
        ArrayAngles = new Quaternion[count];
        for (int i = 0; i < count; i++)
            ArrayAngles[i] = Quaternion.Euler(0, angle * i, 0);
    }

    public static string RandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        chars += chars.ToLower();

        char[] str = new char[length];
        for(int i = 0; i < length; i++)
            str[i] = chars[URandom.Range(0 , chars.Length)];
        return new string(str);
    }
}
