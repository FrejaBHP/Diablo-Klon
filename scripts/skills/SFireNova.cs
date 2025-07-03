using Godot;
using System;
using System.Collections.Generic;

public class SFireNova : Skill, ISpell, IAreaSkill {
    public double BaseCastTime { get; set; } = 0.80;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public float BaseAreaRadius { get; set; } = 2.5f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = true;

    private const float pixelSize = 0.125f;

    private static readonly double[] minDamageArray = [
        10, 12, 15, 19, 24,
        30, 38, 47, 59, 74,
        93, 116, 145, 181, 227
    ];

    private static readonly double[] maxDamageArray = [
        13, 16, 20, 25, 31,
        39, 49, 61, 77, 96,
        121, 151, 189, 236, 295
    ];

    public SFireNova() {
        Name = "Fire Nova";
        Description = "Casts a nova of flame, dealing Fire damage to all surrounding enemies.";
        Effects = [];

        SkillName = ESkillName.FireNova;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Area | ESkillTags.Fire;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillFireNova;

        ManaCost = 1;

        AddedDamageModifier = 2.50;

        TotalAreaRadius = BaseAreaRadius;
        CastRange = TotalAreaRadius - 0.5f;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];
        UpdateEffectStrings();
    }

    protected override void UpdateEffectStrings() {
        Effects = [
            $"Base radius is {BaseAreaRadius} metres"
        ];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IAreaSkill aSkill = this;
            aSkill.BasicAreaSweepSkillBehaviour(this, "fireNova", pixelSize);
        }
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        SkillDamage damage = RollForDamage(true);

        foreach (Actor actor in targets) {
            actor.TakeDamage(DamageCategory, damage, ActorOwner.Penetrations, true, true);
        }
    }
}
