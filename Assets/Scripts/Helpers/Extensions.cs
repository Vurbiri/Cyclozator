using System.Collections.Generic;
using UnityEngine;


public static class Extensions
{
    public static Quaternion RotateTowards(this ref Quaternion self, Quaternion to, float maxDegreesDelta) => self = Quaternion.RotateTowards(self, to, maxDegreesDelta);
    public static Vector3 MoveTowards(this ref Vector3 self, Vector3 target, float maxDistanceDelta) => self = Vector3.MoveTowards(self, target, maxDistanceDelta);
    public static Vector2 MoveTowards(this ref Vector2 self, Vector2 target, float maxDistanceDelta) => self = Vector2.MoveTowards(self, target, maxDistanceDelta);

    public static T GetRandomElement<T>(this T[] self, int startIndex = 0)
    {
        URandom.InitState(Helper.Seed);
        return self[URandom.Range(startIndex, self.Length)];
    }
    public static int Left<T>(this T[] self, int index)
    {
        if (--index < 0)
            index += self.Length; ;

        return index;
    }
    public static int Right<T>(this T[] self, int index)
    {
        int length = self.Length;
        if (++index >= length)
            index -= length;

        return index;
    }
    public static int Offset<T>(this T[] self, int index, int offset)
    {
        int length = self.Length;
        index += offset;
        while (index >= length) index -= length;
        while (index < 0) index += length;

        return index;
    }

    public static T Offset<T>(this List<T> self, ref int index, int offset)
    {
        int count = self.Count;
        index += offset;
        while (index >= count) index -= count;
        while (index < 0) index += count;

        return self[index];
    }

    public static void Fill<T>(this List<T> self, T value, int count)
    {
        self.Clear();
        for (int i = 0; i < count; i++)
            self.Add(value);
        self.TrimExcess();
    }

    public static bool RemoveAll<T>(this List<T> self, T value)
    {
        bool removed = false;
        while (self.Remove(value)) removed = true;
        return removed;
    }

    public static bool ContainsBuff(this List<Buff> self, ItemType type)
    {
        bool isContains = false;

        foreach(var buff in self)
            if (buff.Type == type) { isContains = true; break;}

        return isContains;
    }
    public static bool TryGetBuff(this Buff[] self, ItemType type, out Buff buff)
    {
        buff = null;
        bool isContains = false;

        foreach (var b in self)
        {
            if (b.Type == type)
            {
                isContains = true;
                buff = b;
                break;
            }
        }

        return isContains;
    }

    public static int Buffing(this Buff[] self, ItemType type, int value) => (int)self.Buffing(type, (float)value);
    public static float Buffing(this Buff[] self, ItemType type, float value)
    {
        if (self.TryGetBuff(type, out Buff buff))
           value = buff.Buffing(value);
        return value;
    }

    //public static string ZipStr(this string self)
    //{
    //    if (self == string.Empty)
    //        return self;

    //    byte[] buffer = Encoding.UTF8.GetBytes(self);
    //    using MemoryStream stream = new();
    //    using (GZipStream zip = new(stream, CompressionMode.Compress))
    //    {
    //        zip.Write(buffer, 0, buffer.Length);
    //    }
    //    return stream.ToArray().ToStr();
    //}

    //public static string UnzipStr(this string self)
    //{
    //    if (self == string.Empty)
    //        return self;
    //    try 
    //    { 
    //        byte[] buffer = self.ToByte();
    //        using MemoryStream stream = new(buffer);
    //        using GZipStream zip = new(stream, CompressionMode.Decompress);
    //        using StreamReader reader = new(zip);
    //        return reader.ReadToEnd();
    //    }
    //    catch 
    //    {
    //        Message.Log("Error UnzipStr");
    //        return self;
    //    }
    //}

    //public static byte[] ToByte(this string self)
    //{
    //    char[] chars = self.ToCharArray();
    //    byte[] buffer = new byte[chars.Length];
    //    for (int i = 0; i < chars.Length; i++)
    //        buffer[i] = (byte)chars[i];
    //    return buffer;
    //}

    //public static string ToStr(this byte[] self)
    //{
    //    char[] chars = new char[self.Length];
    //    for (int i = 0; i < self.Length; i++)
    //        chars[i] = (char)self[i];
    //    return new(chars);
    //}
}
