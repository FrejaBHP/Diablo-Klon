using Godot;
using System;
using System.Collections.Generic;

public class SSoulrend : Skill, ISpell, IProjectileSkill, IAreaSkill, IDurationSkill {
    public double BaseCastTime { get; set; } = 0.75;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public double BaseProjectileLifetime { get; set; } = 4f;
    public ESkillProjectileType ProjectileType { get; set; } = ESkillProjectileType.Soulrend;

    public Stat Pierces { get; set; } = new(0, true, 0);
    public Stat NumberOfProjectiles { get; set; } = new (1, true, 0);
    public Stat ProjectileSpeed { get; set; } = new(5, false, 0);

    public bool AlwaysPierces { get; set; } = true;

    public bool CanFireSequentially { get; set; } = true;
    public bool FiresSequentially { get; set; } = false;

    public double MinimumSpreadAngle { get; set; } = 0;
    public double MaximumSpreadAngle { get; set; } = 0;
    public double MinimumSpreadAngleOverride { get; set; } = 0;
    public double MaximumSpreadAngleOverride { get; set; } = 0;

    public float BaseAreaRadius { get; set; } = 0.3f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = false;

    public double BaseDuration { get; set; } = 0.6;
    public double IncreasedDuration { get; set; } = 0;
    public double MoreDuration { get; set; } = 1;
    public double TotalDuration { get; set; }

    protected ESkillDamageTags areaDamageTags = ESkillDamageTags.Spell | ESkillDamageTags.DoT;
    protected double totalDamageOverTimeDPS;

    private static readonly double[] minDamageArray = [
        6, 7, 8, 10, 12,
        14, 17, 21, 25, 30,
        37, 44, 53, 74, 77
    ];

    private static readonly double[] maxDamageArray = [
        9, 10, 12, 15, 18,
        22, 26, 32, 38, 46,
        55, 66, 80, 96, 115
    ];

    private static readonly double[] dotDamageArray = [
        15, 17, 20, 25, 30,
        36, 43, 53, 63, 76,
        92, 110, 133, 170, 192
    ];

    public SSoulrend() {
        Name = "Soulrend";
        Description = "Fire a projectile that turns towards enemies in front of it, damaging and piercing targets hit. Repeatedly applies a non-stackable damage over time Debuff to enemies around it as it travels.";

        SkillName = ESkillName.Soulrend;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Projectile | ESkillTags.Area | ESkillTags.Duration | ESkillTags.Chaos;
        SkillDamageTags = ESkillDamageTags.Spell | ESkillDamageTags.Projectile;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillSoulrend;

        ManaCost.SBase = 3;

        CastRange = 15f;

        CriticalStrikeChance.SBase = 0.05;
        AddedDamageModifier = 1.3;

        TotalAreaRadius = BaseAreaRadius;
        TotalDuration = BaseDuration;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Chaos.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Chaos.SMaxBase = maxDamageArray[level];

        UpdateEffectStrings();
    }

    protected override void UpdateEffectStrings() {
        Effects = [
            $"Deals {dotDamageArray[level]:F0} Chaos Damage per second",
            $"Base Duration is {BaseDuration:F2} seconds"
        ];
    }

    public override void RecalculateSkillValues() {
        base.RecalculateSkillValues();

        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Chaos, areaDamageTags, out double increasedMultiplier, out double moreMultiplier);
        totalDamageOverTimeDPS = dotDamageArray[level] * (increasedMultiplier + 1) * moreMultiplier;
    }

    public override string[] GetSecondaryEffectStrings() {
        string[] extraStrings = [
            $"Debuff deals {Math.Round(totalDamageOverTimeDPS, 0)} Chaos Damage per second"
        ];

        return extraStrings;
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            List<Projectile> projectiles = pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);

            foreach (Projectile proj in projectiles) {
                SoulrendProjectile srProj = proj as SoulrendProjectile;
                srProj.SetAoEProperties(TotalAreaRadius);
                SetSkillCollision(srProj.AoE);
                SetSkillCollision(srProj.SeekerRadius);
                srProj.AoE.TargetsAffected += ApplyAreaSkillBehaviourToTargets;
            }
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true);
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        foreach (Actor actor in targets) {
            actor.ReceiveEffect(new SoulrendEffect(actor, TotalDuration, totalDamageOverTimeDPS));
        }
    }
}

public class SoulrendEffect : AttachedEffect, IUniqueEffect, IDamageOverTimeEffect {
    public EDamageType DamageType { get; set; } = EDamageType.Chaos;

    public SoulrendEffect(Actor actor, double duration, double dps) {
        EffectIcon = UILib.AEffectSoulrend;
        CreatesPlayerStatusIcon = true;
        EffectRating = EEffectRating.Negative;
        EffectName = EEffectName.Soulrend;

        EffectString = "Soulrend";
        EffectDescription = "Taking Chaos Damage over Time";

        AffectedActor = actor;
        EffectLength = duration;
        RemainingTime = EffectLength;
        EffectValue = dps;
    }

    public bool ShouldReplaceCurrentEffect(double duration, double value) {
        return (duration * value) > (RemainingTime * EffectValue);
    }

    public override void Tick(double deltaTime) {
        double damage = EffectValue * deltaTime;
        AffectedActor.TallyDamageOverTimeForNextTick(DamageType, damage);
        RemainingTime -= deltaTime;
    }
}
