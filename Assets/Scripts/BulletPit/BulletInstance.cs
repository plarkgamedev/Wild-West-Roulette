using UnityEngine;

public class BulletInstance
{
    public BulletDefinition Definition;
    public Rarity Rarity;
    public string DisplayName;
    public float DamageMultiplier;
    public int Weight;
    public string EffectName;
    public string EffectDescription;
    public string BeforeEffect;
    public string DuringEffect;
    public string AfterEffect;
    public string FlavorText;
    public Sprite Icon;

    // Every bullet comes from a Bullet Definition you made.
    public static BulletInstance FromDefinition(PitConfig cfg, BulletDefinition def) => new BulletInstance
    {
        Definition = def,
        Rarity = def.rarity,
        DisplayName = def.displayName,
        DamageMultiplier = def.damageMultiplier,
        Weight = Mathf.Max(0, cfg.WeightOf(def.rarity) + def.extraWeight),
        EffectName = def.effectName,
        EffectDescription = def.effectDescription,
        BeforeEffect = def.beforeEffect,
        DuringEffect = def.duringEffect,
        AfterEffect = def.afterEffect,
        FlavorText = def.flavorText,
        Icon = def.icon != null ? def.icon : cfg.genericIcon
    };
}