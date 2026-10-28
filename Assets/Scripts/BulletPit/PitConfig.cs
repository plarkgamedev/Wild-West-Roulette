using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Pit Config")]
public class PitConfig : ScriptableObject
{
    [Header("Timer and size")]
    public float timeLimit = 30f;
    public int fallbackPitSize = 100;      // only used when you press Play directly in the pit scene

    [Header("Cylinder (later this comes from the cylinder part)")]
    public int chambers = 6;
    public int cylinderWeightLimit = 10;

    [Header("Weight per rarity: [White, Blue, Purple, Legendary]")]
    public int[] rarityWeight = { 1, 2, 3, 5 };

    [Header("Interaction")]
    public float pickupRange = 4f;

    [Header("Spawning")]
    public int bulletsPerBatch = 6;
    public float batchInterval = 0.03f;
    public float settleTime = 1f;

    [Header("Bullet types: ONLY these can ever spawn")]
    public List<BulletDefinition> bullets = new List<BulletDefinition>();
    [Tooltip("Shown in the cylinder UI for definitions that have no icon of their own.")]
    public Sprite genericIcon;

    public int WeightOf(Rarity r) => rarityWeight[(int)r];

    public bool HasAny
    {
        get { foreach (var b in bullets) if (b != null) return true; return false; }
    }

    // A definition of exactly this rarity, favoring higher spawnWeight. Null if none.
    public BulletDefinition Pick(Rarity r, System.Random rng)
    {
        float total = 0f;
        foreach (var b in bullets)
            if (b != null && b.rarity == r) total += Mathf.Max(0f, b.spawnWeight);
        if (total <= 0f) return null;

        float x = (float)rng.NextDouble() * total;
        BulletDefinition last = null;
        foreach (var b in bullets)
        {
            if (b == null || b.rarity != r) continue;
            last = b;
            x -= Mathf.Max(0f, b.spawnWeight);
            if (x < 0f) return b;
        }
        return last;
    }

    // Same, but if that rarity has no definitions it uses the nearest lower rarity, then the nearest higher.
    // Returns null only when the list holds no usable definition at all.
    public BulletDefinition PickWithFallback(Rarity wanted, System.Random rng, out Rarity used)
    {
        used = wanted;
        var def = Pick(wanted, rng);
        if (def != null) return def;

        for (int i = (int)wanted - 1; i >= 0; i--)
        {
            def = Pick((Rarity)i, rng);
            if (def != null) { used = (Rarity)i; return def; }
        }
        for (int i = (int)wanted + 1; i <= 3; i++)
        {
            def = Pick((Rarity)i, rng);
            if (def != null) { used = (Rarity)i; return def; }
        }
        return null;
    }

    // The lightest bullet you have defined (used if the player picks nothing).
    public BulletDefinition PickLightest(System.Random rng)
    {
        for (int i = 0; i <= 3; i++)
        {
            var def = Pick((Rarity)i, rng);
            if (def != null) return def;
        }
        return null;
    }

    void OnValidate()
    {
        if (rarityWeight == null || rarityWeight.Length != 4) System.Array.Resize(ref rarityWeight, 4);
    }
}