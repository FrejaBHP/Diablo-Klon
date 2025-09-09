using Godot;
using System;
using System.Collections.Generic;

public class SPiercingShot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public double BaseProjectileLifetime { get; set; } = 2f;
    public ESkillProjectileType ProjectileType { get; set; } = ESkillProjectileType.Default;
    
    public Stat Pierces { get; set; } = new(2, true, 0);
    public Stat NumberOfProjectiles { get; set; } = new (1, true, 0);
    public Stat ProjectileSpeed { get; set; } = new(15, false, 0);

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

    public SPiercingShot() {
        Name = "Piercing Shot";
        Description = "Fires a penetrating arrow in a straight line with a bow.";
        Effects = [
            $"Pierces {Pierces.SBase} targets"
        ];

        SkillName = ESkillName.PiercingShot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged;
        SkillDamageTags = ESkillDamageTags.Attack | ESkillDamageTags.Projectile;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillPiercingShot;

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
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true);
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }
}
