using Godot;
using System;
using System.Collections.Generic;

public class SCobraShot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public double BaseProjectileLifetime { get; set; } = 2f;
    public ESkillProjectileType ProjectileType { get; set; } = ESkillProjectileType.Default;
    
    public Stat Pierces { get; set; } = new(0, true, 0);
    public Stat NumberOfProjectiles { get; set; } = new (2, true, 0);
    public Stat ProjectileSpeed { get; set; } = new(25, false, 0);

    public bool AlwaysPierces { get; set; } = false;

    public bool CanFireSequentially { get; set; } = true;
    public bool FiresSequentially { get; set; } = false;

    public double MinimumSpreadAngle { get; set; } = 0;
    public double MaximumSpreadAngle { get; set; } = 0;
    public double MinimumSpreadAngleOverride { get; set; } = 0;
    public double MaximumSpreadAngleOverride { get; set; } = 0;

    private static readonly double[] addedDamageModArray = [
        1.30, 1.38, 1.47, 1.57, 1.67,
        1.78, 1.90, 2.02, 2.15, 2.29,
        2.44, 2.60, 2.77, 2.95, 3.14
    ];

    public SCobraShot() {
        BaseStatusEffectModifiers.Poison.SBaseChance = 0.5;
        BaseStatusEffectModifiers.Poison.SMoreDuration = 1.5;

        Name = "Cobra Shot";
        Description = "Fires twin arrows with a chance to Poison. Requires a bow.";
        Effects = [
            $"Fires {NumberOfProjectiles.SBase} projectiles",
            $"{BaseStatusEffectModifiers.Poison.SBaseChance:P0} chance to Poison on Hit",
            $"{BaseStatusEffectModifiers.Poison.SMoreDuration - 1:P0} more Poison duration"
        ];

        SkillName = ESkillName.CobraShot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged | ESkillTags.Chaos;
        SkillDamageTags = ESkillDamageTags.Attack | ESkillDamageTags.Projectile;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillCobraShot;

        ManaCost.SBase = 2;

        CastRange = 15f;
    }

    protected override void OnSkillLevelChanged() {
        AddedDamageModifier = addedDamageModArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);
            DeductManaFromActor();
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true, target);
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }
}
