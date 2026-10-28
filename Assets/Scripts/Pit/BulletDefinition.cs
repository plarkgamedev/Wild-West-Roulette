using UnityEngine;

[CreateAssetMenu(menuName = "Game/Bullet Definition")]
public class BulletDefinition : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "New Bullet";
    public Rarity rarity = Rarity.White;
    [Tooltip("Higher = appears more often among bullets of the same rarity.")]
    public float spawnWeight = 1f;

    [Header("Stats")]
    public float damageMultiplier = 1.1f;
    [Tooltip("Added on top of the rarity's weight. Usually 0.")]
    public int extraWeight = 0;

    [Header("Special effect (text for now)")]
    public string effectName;
    [TextArea(2, 4)] public string effectDescription;

    [Header("Visuals")]
    [Tooltip("Icon shown in the cylinder UI. Falls back to the Generic Icon in the Pit Config.")]
    public Sprite icon;
    [Tooltip("Optional unique 3D model. If set, it replaces the default pit bullet. Use it for special legendaries.")]
    public GameObject modelOverride;
    public float modelScale = 1f;

    // FUTURE: add new fields below. Existing assets keep working, and new fields take default values.
}