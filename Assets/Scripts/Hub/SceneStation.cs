using UnityEngine;

public class SceneStation : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneName;
    [SerializeField] string stationId;
    [SerializeField] bool isLoopStation;   // false for the upgrade hub
    [SerializeField] HubStep step;

    public HubStep Step => step;
    public bool IsLoopStation => isLoopStation;

    public void Interact()
    {
        var run = GameManager.Instance.Run;
        if (isLoopStation && run.Next != step)
        {
            Debug.Log("Not yet."); // replace with a visible bark later
            return;
        }
        run.LastStationId = stationId;
        SceneLoader.Instance.Load(sceneName);
    }
}