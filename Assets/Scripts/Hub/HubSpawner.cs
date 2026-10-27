using UnityEngine;

public class HubSpawner : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] SpawnPoint[] points;

    void Start()
    {
        var id = GameManager.Instance.Run.LastStationId;
        if (string.IsNullOrEmpty(id)) return;
        foreach (var p in points)
        {
            if (p.stationId != id) continue;
            player.position = p.transform.position;
            return;
        }
    }
}