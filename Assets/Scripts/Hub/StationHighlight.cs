using UnityEngine;

public class StationHighlight : MonoBehaviour
{
    [SerializeField] SceneStation station;
    [SerializeField] GameObject glow;

    void Start() => Refresh();

    void Refresh()
    {
        if (!station.IsLoopStation) { glow.SetActive(false); return; }
        glow.SetActive(GameManager.Instance.Run.Next == station.Step);
    }
}