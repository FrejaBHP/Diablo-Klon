using Godot;
using System;
using System.Collections.Generic;

public class SPiercingShot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;
    public ESkillProjectileType ProjectileType { get; set; } = ESkillProjectileType.Default;
    
    public int BasePierces { get; set; } = 2;
    public int AddedPierces { get; set; } = 0;
    public int TotalPierces { get; set; }
    public bool AlwaysPierces { get; set; } = false;
    public int BaseProjectiles { get; set; } = 1;
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

    public SPiercingShot() {
        Name = "Piercing Shot";
        Description = "Fires a penetrating arrow in a straight line with a bow.";
        Effects = [
            $"Pierces {BasePierces} targets"
        ];

        SkillName = ESkillName.PiercingShot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged;
        SkillDamageTags = ESkillDamageTags.Attack | ESkillDamageTags.Projectile;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillPiercingShot;

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
