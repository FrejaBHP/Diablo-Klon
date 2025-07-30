using Godot;
using System;
using System.Collections.Generic;

public class SCobraShot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;
    public int BasePierces { get; set; } = 0;
    public int AddedPierces { get; set; } = 0;
    public int TotalPierces { get; set; }
    public int BaseProjectiles { get; set; } = 2;
    public int AddedProjectiles { get; set; } = 0;
    public int TotalProjectiles { get; set; }

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
            $"Fires {BaseProjectiles} projectiles",
            $"{BaseStatusEffectModifiers.Poison.SBaseChance:P0} chance to Poison on Hit",
            $"{BaseStatusEffectModifiers.Poison.SMoreDuration - 1:P0} more Poison duration"
        ];

        SkillName = ESkillName.CobraShot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged | ESkillTags.Chaos;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillCobraShot;

        ManaCost = 1;

        CastRange = 15f;

        TotalPierces = BasePierces;
        TotalProjectiles = BaseProjectiles;
    }

    protected override void OnSkillLevelChanged() {
        AddedDamageModifier = addedDamageModArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true);
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }
}
