using Godot;
using System;
using System.Collections.Generic;

public class SFireball : Skill, ISpell, IProjectileSkill, IAreaSkill {
    public double BaseCastTime { get; set; } = 0.75;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public double BaseProjectileLifetime { get; set; } = 2f;
    public ESkillProjectileType ProjectileType { get; set; } = ESkillProjectileType.Default;

    public Stat Pierces { get; set; } = new(0, true, 0);
    public Stat NumberOfProjectiles { get; set; } = new (1, true, 0);
    public Stat ProjectileSpeed { get; set; } = new(15, false, 0);

    public bool AlwaysPierces { get; set; } = false;

    public bool CanFireSequentially { get; set; } = true;
    public bool FiresSequentially { get; set; } = false;

    public double MinimumSpreadAngle { get; set; } = 0;
    public double MaximumSpreadAngle { get; set; } = 0;
    public double MinimumSpreadAngleOverride { get; set; } = 0;
    public double MaximumSpreadAngleOverride { get; set; } = 0;

    public float BaseAreaRadius { get; set; } = 1f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = false;

    private static readonly double[] minDamageArray = [
        10, 12, 14, 17, 20,
        24, 29, 35, 43, 51,
        61, 74, 89, 106, 128
    ];

    private static readonly double[] maxDamageArray = [
        14, 16, 20, 24, 29,
        34, 41, 50, 60, 72,
        86, 104, 124, 149, 179
    ];

    public SFireball() {
        Name = "Fireball";
        Description = "Fire a projectile that explodes into Fire damage in an area on hit.";

        SkillName = ESkillName.Fireball;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Projectile | ESkillTags.Area | ESkillTags.Fire;
        SkillDamageTags = ESkillDamageTags.Spell | ESkillDamageTags.Projectile | ESkillDamageTags.Area;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillFireball;

        ManaCost.SBase = 3;

        CastRange = 15f;

        CriticalStrikeChance.SBase = 0.06;
        AddedDamageModifier = 1.5;
        BaseStatusEffectModifiers.Ignite.SBaseChance = 0.25;

        TotalAreaRadius = BaseAreaRadius;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];

        UpdateEffectStrings();
    }

    protected override void UpdateEffectStrings() {
        Effects = [
            $"Base radius is {BaseAreaRadius} metre",
            $"{BaseStatusEffectModifiers.Ignite.SBaseChance:P0} chance to Ignite on Hit"
        ];
    }

    public override void RecalculateSkillValues() {
        base.RecalculateSkillValues();
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);
            DeductManaFromActor();
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        IAreaSkill aSkill = this;
        Vector3 position = target.GlobalPosition;
        position.Y += ActorOwner.OutgoingEffectAttachmentHeight;
        aSkill.BasicAreaSweepSkillBehaviour(this, position, "none", 0f); // No associated animation
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        foreach (Actor actor in targets) {
            SkillInfo info = CalculateOutgoingValuesIntoInfo(true, actor);
            actor.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
        }
    }
}
