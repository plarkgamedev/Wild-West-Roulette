using UnityEngine;

public class BulletInstance
{
    public BulletDefinition Definition;   // null for plain fallback bullets
    public Rarity Rarity;
    public string DisplayName;
    public float DamageMultiplier;
    public int Weight;
    public string EffectName;
    public string EffectDescription;
    public Sprite Icon;

    public static BulletInstance Create(PitConfig cfg, Rarity rarity, System.Random rng)
    {
        var def = cfg.Pick(rarity, rng);
        return def != null ? FromDefinition(cfg, def) : Plain(cfg, rarity);
    }

    public static BulletInstance FromDefinition(PitConfig cfg, BulletDefinition def) => new BulletInstance
    {
        Definition = def,
        Rarity = def.rarity,
        DisplayName = def.displayName,
        DamageMultiplier = def.damageMultiplier,
        Weight = Mathf.Max(0, cfg.WeightOf(def.rarity) + def.extraWeight),
        EffectName = def.effectName,
        EffectDescription = def.effectDescription,
        Icon = def.icon != null ? def.icon : cfg.genericIcon
    };

    public static BulletInstance Plain(PitConfig cfg, Rarity rarity) => new BulletInstance
    {
        Rarity = rarity,
        DisplayName = rarity + " Bullet",
        DamageMultiplier = cfg.DefaultMultiplier(rarity),
        Weight = cfg.WeightOf(rarity),
        EffectName = "",
        EffectDescription = "",
        Icon = cfg.genericIcon
    };
}