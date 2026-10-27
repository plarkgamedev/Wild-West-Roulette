using System;
using System.Collections.Generic;
using System.Linq;

public static class PitOdds
{
    // How many reels landed on each rarity: [White, Blue, Purple, Legendary]
    public static int[] CountRarities(Rarity[] result)
    {
        var counts = new int[4];
        if (result != null)
            foreach (var r in result) counts[(int)r]++;
        return counts;
    }

    // Chance per pit bullet for each rarity, 0 to 100.
    // Example: 3 of 6 reels on White = 50.
    public static float[] Percentages(int[] counts)
    {
        int sum = counts.Sum();
        var p = new float[counts.Length];
        if (sum == 0) return p;
        for (int i = 0; i < counts.Length; i++)
            p[i] = 100f * counts[i] / sum;
        return p;
    }

    // One bullet: checks the rarest tier first, then works down to White.
    // The chance of each rarity equals its percentage exactly.
    public static Rarity RollOne(float[] percentages, Random rng)
    {
        double x = rng.NextDouble() * 100.0;
        for (int i = percentages.Length - 1; i >= 0; i--)   // Legendary first, White last
        {
            x -= percentages[i];
            if (x < 0) return (Rarity)i;
        }
        return Rarity.White;   // safety net for float rounding
    }

    // Rolls every bullet in the pit independently, so no rarity has a guaranteed count.
    public static List<Rarity> RollPool(float[] percentages, int pitSize, Random rng)
    {
        var pool = new List<Rarity>(pitSize);
        for (int i = 0; i < pitSize; i++)
            pool.Add(RollOne(percentages, rng));
        return pool;
    }
}