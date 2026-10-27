public enum HubStep { Match3Slot, BulletPit, Duel }

public class RunState
{
    public string LastStationId;
    public HubStep Next = HubStep.Match3Slot;
    public int Town = 1;
    public int DuelIndex = 0;   // 0..2 inside a town
    public int PlayerLevel = 1;
    public int LoopsCompleted;

    public void Advance()
    {
        if (Next == HubStep.Duel)
        {
            Next = HubStep.Match3Slot;
            LoopsCompleted++;   // a full loop = slot, pit, duel
        }
        else
        {
            Next = Next + 1;
        }
    }
}