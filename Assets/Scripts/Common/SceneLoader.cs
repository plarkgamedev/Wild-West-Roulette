using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    [SerializeField] CanvasGroup fade;
    [SerializeField] float time = 0.25f;
    bool busy;

    void Awake() => Instance = this;

    public void Load(string scene) { if (!busy) StartCoroutine(Routine(scene)); }

    IEnumerator Routine(string scene)
    {
        busy = true;
        fade.blocksRaycasts = true;
        yield return Fade(1f);
        var op = SceneManager.LoadSceneAsync(scene);
        while (!op.isDone) yield return null;
        yield return Fade(0f);
        fade.blocksRaycasts = false;
        busy = false;
    }

    IEnumerator Fade(float to)
    {
        float from = fade.alpha;
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            fade.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        fade.alpha = to;
    }
}