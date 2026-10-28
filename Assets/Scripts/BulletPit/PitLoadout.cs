using System;
using System.Collections.Generic;
using UnityEngine;

public class PitLoadout : MonoBehaviour
{
    [SerializeField] PitConfig config;

    readonly List<BulletInstance> loaded = new List<BulletInstance>();
    readonly List<PitBullet> views = new List<PitBullet>();

    // Every UI that shows the cylinder listens to this.
    public event Action Changed;

    public IReadOnlyList<BulletInstance> Loaded => loaded;
    public int Limit => config.cylinderWeightLimit;

    public int Weight
    {
        get { int w = 0; foreach (var b in loaded) w += b.Weight; return w; }
    }

    public bool CanAdd(BulletInstance b, out string reason)
    {
        if (loaded.Count >= config.chambers) { reason = "Cylinder full"; return false; }
        if (Weight + b.Weight > Limit) { reason = $"Too heavy  ({Weight + b.Weight}/{Limit})"; return false; }
        reason = null;
        return true;
    }

    public bool TryAdd(PitBullet view)
    {
        if (!CanAdd(view.Data, out _)) return false;
        loaded.Add(view.Data);
        views.Add(view);
        view.Pick();
        Refresh();
        return true;
    }

    // Cylinder screen only: drops the chosen bullet at `position`. Later bullets shift down one chamber.
    public void DropAt(int index, Vector3 position)
    {
        if (index < 0 || index >= loaded.Count) return;
        views[index].DropAt(position);
        loaded.RemoveAt(index);
        views.RemoveAt(index);
        Refresh();
    }

    public void Refresh() => Changed?.Invoke();
}