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

    [Header("Damage multiplier used when no bullet definition exists for a rarity")]
    public float[] defaultDamageMultiplier = { 1.1f, 1.3f, 1.5f, 2f };

    [Header("Interaction")]
    public float pickupRange = 4f;

    [Header("Spawning")]
    public int bulletsPerBatch = 6;
    public float batchInterval = 0.03f;
    public float settleTime = 1f;          // pause before the timer starts so the pile can land

    [Header("Bullet types")]
    public List<BulletDefinition> bullets = new List<BulletDefinition>();
    public Sprite genericIcon;             // your bullet PNG, tinted by rarity

    public int WeightOf(Rarity r) => rarityWeight[(int)r];
    public float DefaultMultiplier(Rarity r) => defaultDamageMultiplier[(int)r];

    // Picks a bullet type of this rarity, favoring higher spawnWeight. Null if none exist.
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

    void OnValidate()
    {
        if (rarityWeight == null || rarityWeight.Length != 4) System.Array.Resize(ref rarityWeight, 4);
        if (defaultDamageMultiplier == null || defaultDamageMultiplier.Length != 4) System.Array.Resize(ref defaultDamageMultiplier, 4);
    }
}