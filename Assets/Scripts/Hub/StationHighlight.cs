using UnityEngine;

public class StationHighlight : MonoBehaviour
{
    [SerializeField] SceneStation station;
    [SerializeField] GameObject glow;
    [SerializeField] int glowForLoops = 2;   // glow during the first 2 loops, then never

    void Start() => Refresh();

    void Refresh()
    {
        var run = GameManager.Instance.Run;
        bool show = station.IsLoopStation
                    && run.Next == station.Step
                    && run.LoopsCompleted < glowForLoops;
        glow.SetActive(show);
    }
}