using System.Collections.Generic;
using UnityEngine;

public class PitLoadout : MonoBehaviour
{
    [SerializeField] PitConfig config;
    [SerializeField] CylinderUI ui;

    readonly List<BulletInstance> loaded = new List<BulletInstance>();
    readonly List<PitBullet> views = new List<PitBullet>();

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

    // Puts the most recently picked bullet back where it was.
    public void RemoveLast()
    {
        if (loaded.Count == 0) return;
        int i = loaded.Count - 1;
        views[i].PutBack();
        loaded.RemoveAt(i);
        views.RemoveAt(i);
        Refresh();
    }

    public void Refresh() => ui.Refresh(loaded, Weight, Limit);
}