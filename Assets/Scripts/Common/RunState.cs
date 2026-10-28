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
    public int PitSize = 1000;
    public int[] PitReelCounts;          // [White, Blue, Purple, Legendary]
    public float[] PitPercentages;       // chance per bullet, 0 to 100

    // Bullet pit (rolled once per loop; ResultsPanel clears PitPool to force a reroll)
    public List<Rarity> PitPool;
    public List<BulletInstance> PitBullets;
    public List<BulletInstance> Loadout; // what the player took, in cylinder order, read by the duel

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
