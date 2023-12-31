using System;
using System.Collections.Generic;

static class ShuffleExtension
{
    private static Random rng = new Random();

    public static bool Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return true;
    }
}
