using UnityEngine;

static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (GameManager.Instance != null) return;
        var prefab = Resources.Load<GameObject>("Persistent");
        Object.DontDestroyOnLoad(Object.Instantiate(prefab));
    }
}