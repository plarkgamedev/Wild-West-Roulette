using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public void Go(string scene) => SceneLoader.Instance.Load(scene);

    public void CompleteStepAndGo(string scene)
    {
        GameManager.Instance.Run.Advance();
        SceneLoader.Instance.Load(scene);
    }
}