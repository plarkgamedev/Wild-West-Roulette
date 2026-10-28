using System;
using System.Collections.Generic;

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

    // Chance per pit bullet for each rarity, in percent, adding up to exactly 100.
    //   baseChance[1..3] = starting chance of Blue, Purple, Legendary (index 0 is ignored)
    //   perReel[1..3]    = chance added per reel on that rarity
    //   perReel[0]       = points moved from the rarer tiers to White per White reel
    // White always gets whatever is left.
    public static float[] Chances(int[] counts, float[] baseChance, float[] perReel)
    {
        var p = new float[4];
        float rarer = 0f;
        for (int i = 1; i <= 3; i++)
        {
            p[i] = Math.Max(0f, baseChance[i] + counts[i] * perReel[i]);
            rarer += p[i];
        }

        // White reels shave the rarer tiers proportionally.
        float target = Math.Max(0f, rarer - counts[0] * perReel[0]);
        // The rarer tiers can never take more than the whole pit.
        target = Math.Min(target, 100f);

        if (rarer > 0f)
        {
            float scale = target / rarer;
            for (int i = 1; i <= 3; i++) p[i] *= scale;
        }

        p[0] = 100f - (p[1] + p[2] + p[3]);
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