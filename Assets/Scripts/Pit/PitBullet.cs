using UnityEngine;

public class PitBullet : MonoBehaviour
{
    [SerializeField] GameObject defaultVisual;   // the child that holds the default mesh

    public BulletInstance Data { get; private set; }

    Rigidbody rb;
    Vector3 homePos;
    Quaternion homeRot;

    static readonly int BaseColor = Shader.PropertyToID("_BaseColor");   // URP Lit
    static readonly int LegacyColor = Shader.PropertyToID("_Color");

    void Awake() => rb = GetComponent<Rigidbody>();

    public void Setup(BulletInstance data, Color tint)
    {
        Data = data;
        var def = data.Definition;

        if (def != null && def.modelOverride != null)
        {
            // Unique model: replaces the default bullet and keeps its own colors.
            defaultVisual.SetActive(false);
            var m = Instantiate(def.modelOverride, transform);
            m.transform.localPosition = Vector3.zero;
            m.transform.localRotation = Quaternion.identity;
            m.transform.localScale = Vector3.one * def.modelScale;
            foreach (var c in m.GetComponentsInChildren<Collider>()) Destroy(c);    // only the root collides
            foreach (var r in m.GetComponentsInChildren<Rigidbody>()) Destroy(r);
        }
        else
        {
            // Default model: the rarity color is a tint on the material.
            var block = new MaterialPropertyBlock();
            foreach (var r in defaultVisual.GetComponentsInChildren<Renderer>())
            {
                r.GetPropertyBlock(block);
                block.SetColor(BaseColor, tint);
                block.SetColor(LegacyColor, tint);
                r.SetPropertyBlock(block);
            }
        }
    }

    public void Pick()
    {
        homePos = transform.position;
        homeRot = transform.rotation;
        gameObject.SetActive(false);
    }

    public void PutBack()
    {
        transform.SetPositionAndRotation(homePos + Vector3.up * 0.05f, homeRot);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(true);
    }
}