using Godot;
using System;
using System.Collections.Generic;

public class SFireNova : Skill, ISpell, IAreaSkill {
    public double BaseCastTime { get; set; } = 0.80;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public float BaseAreaRadius { get; set; } = 3f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = true;

    private const float pixelSize = 0.15f;

    private static readonly double[] minDamageArray = [
        10, 12, 14, 17, 20,
        24, 29, 35, 43, 51,
        61, 74, 89, 106, 128
    ];

    private static readonly double[] maxDamageArray = [
        13, 15, 18, 22, 26,
        32, 38, 46, 55, 67,
        80, 96, 115, 139, 166
    ];

    public SFireNova() {
        BaseStatusEffectModifiers.Ignite.SBaseChance = 0.25;

        Name = "Fire Nova";
        Description = "Casts a nova of flame, dealing Fire damage to all surrounding enemies.";
        Effects = [
            $"Base radius is {BaseAreaRadius} metres",
            $"{BaseStatusEffectModifiers.Ignite.SBaseChance:P0} chance to Ignite on Hit"
        ];

        SkillName = ESkillName.FireNova;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Area | ESkillTags.Fire;
        SkillDamageTags = ESkillDamageTags.Spell | ESkillDamageTags.Area;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillFireNova;

        ManaCost = 1;

        AddedDamageModifier = 1.6;

        TotalAreaRadius = BaseAreaRadius;
        CastRange = TotalAreaRadius - 0.5f;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IAreaSkill aSkill = this;
            aSkill.BasicAreaSweepSkillBehaviour(this, "fireNova", pixelSize);
        }
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true);

        foreach (Actor actor in targets) {
            actor.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
        }
    }
}
