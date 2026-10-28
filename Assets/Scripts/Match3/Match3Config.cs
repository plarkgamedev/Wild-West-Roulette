using UnityEngine;

[CreateAssetMenu(menuName = "Game/Match3 Config")]
public class Match3Config : ScriptableObject
{
    [Header("Board")]
    public int columns = 6;
    public int rows = 4;
    public int moves = 10;

    [Header("Match-3 board: which bullets appear as tiles (any positive numbers)")]
    public float whiteWeight = 50f;
    public float blueWeight = 25f;
    public float purpleWeight = 15f;
    public float legendaryWeight = 10f;

    [Header("Bullet pit: base chance per bullet, in percent (white is the remainder)")]
    public float blueBase = 20f;
    public float purpleBase = 10f;
    public float legendaryBase = 5f;

    [Header("Bullet pit: added for EACH slot reel that lands on that rarity")]
    public float bluePerReel = 5f;
    public float purplePerReel = 5f;
    public float legendaryPerReel = 5f;
    [Tooltip("Percentage points moved from the rarer tiers back to White for each White reel. 0 = white reels do nothing.")]
    public float whiteReelShift = 0f;

    [Header("Rarity tints")]
    public Color whiteTint = Color.white;
    public Color blueTint = new Color(0.35f, 0.6f, 1f);
    public Color purpleTint = new Color(0.7f, 0.4f, 1f);
    public Color legendaryTint = new Color(1f, 0.65f, 0.1f);

    [Header("Timing (seconds)")]
    public float swapTime = 0.15f;
    public float clearTime = 0.15f;
    public float fallTime = 0.3f;
    public float spinStartDelay = 0.6f;
    public float spinStepFast = 0.04f;
    public float spinStepSlow = 0.22f;

    // Order everywhere: [White, Blue, Purple, Legendary]
    public float[] PitBase => new[] { WhiteBase, blueBase, purpleBase, legendaryBase };
    public float[] PitStep => new[] { whiteReelShift, bluePerReel, purplePerReel, legendaryPerReel };
    public float WhiteBase => Mathf.Max(0f, 100f - blueBase - purpleBase - legendaryBase);

    public float[] PitChances(int[] reelCounts) => PitOdds.Chances(reelCounts, PitBase, PitStep);

    public Rarity Roll(System.Random rng)
    {
        float total = whiteWeight + blueWeight + purpleWeight + legendaryWeight;
        float x = (float)rng.NextDouble() * total;
        if ((x -= whiteWeight) < 0) return Rarity.White;
        if ((x -= blueWeight) < 0) return Rarity.Blue;
        if ((x -= purpleWeight) < 0) return Rarity.Purple;
        return Rarity.Legendary;
    }

    public Color Tint(Rarity r) => r switch
    {
        Rarity.White => whiteTint,
        Rarity.Blue => blueTint,
        Rarity.Purple => purpleTint,
        _ => legendaryTint
    };
}