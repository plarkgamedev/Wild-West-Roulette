using System.Collections.Generic;

public enum HubStep { Match3Slot, BulletPit, Duel }

public class RunState
{
    public string LastStationId;
    public HubStep Next = HubStep.Match3Slot;
    public int Town = 1;
    public int DuelIndex = 0;
    public int PlayerLevel = 1;
    public int LoopsCompleted;

    // Slot output
    public Rarity[] LastSlotResult;
    public int PitSize = 100;
    public int[] PitReelCounts;        // reels per rarity: [White, Blue, Purple, Legendary]
    public float[] PitPercentages;     // chance per bullet, 0 to 100

    // Rolled once when the pit is first entered, then kept for this loop
    public List<Rarity> PitPool;

    public void Advance()
    {
        if (Next == HubStep.Duel)
        {
            Next = HubStep.Match3Slot;
            LoopsCompleted++;
        }
        else
        {
            Next = Next + 1;
        }
    }
}