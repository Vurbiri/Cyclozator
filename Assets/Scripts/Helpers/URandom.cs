using UnityEngine;

public static class URandom 
{
    public static void InitState(int seed) => Random.InitState(seed);
    public static float Range(float minInclusive, float maxInclusive) => Random.Range(minInclusive, maxInclusive);
    public static int Range(int minInclusive, int maxExclusive) => Random.Range(minInclusive, maxExclusive);
}
