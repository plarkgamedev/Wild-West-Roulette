using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField, Range(0f, 0.5f)] float minAlpha = 0.4f;
    SpriteRenderer sr;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
        var c = sr.color;
        c.a = Mathf.Lerp(minAlpha, 0.5f, t);
        sr.color = c;
    }
}