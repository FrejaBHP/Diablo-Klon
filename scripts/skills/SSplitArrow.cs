using Godot;
using System;
using System.Collections.Generic;

public class SSplitArrow : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;
    public int BasePierces { get; set; } = 0;
    public int AddedPierces { get; set; } = 0;
    public int TotalPierces { get; set; }
    public int BaseProjectiles { get; set; } = 5;
    public int AddedProjectiles { get; set; } = 0;
    public int TotalProjectiles { get; set; }

    public bool CanFireSequentially { get; set; } = true;
    public bool FiresSequentially { get; set; } = false;

    public double MinimumSpreadAngle { get; set; } = 0;
    public double MaximumSpreadAngle { get; set; } = 0;
    public double MinimumSpreadAngleOverride { get; set; } = 0;
    public double MaximumSpreadAngleOverride { get; set; } = 0;

    private static readonly double[] addedDamageModArray = [
        0.75, 0.80, 0.85, 0.90, 0.96,
        1.02, 1.09, 1.16, 1.24, 1.32,
        1.41, 1.50, 1.60, 1.70, 1.81
    ];

    private static readonly int[] baseProjArray = [
        5, 5, 5, 6, 6,
        6, 7, 7, 7, 8,
        8, 8, 9, 9, 9
    ];

    public SSplitArrow() {
        Name = "Split Arrow";
        Description = "Fires a fan of multiple arrows with a bow.";
        UpdateEffectStrings();

        SkillName = ESkillName.SplitArrow;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillSplitArrow;

        ManaCost = 1;

        CastRange = 15f;

        TotalPierces = BasePierces;
        TotalProjectiles = BaseProjectiles;
    }

    protected override void OnSkillLevelChanged() {
        AddedDamageModifier = addedDamageModArray[level];
        BaseProjectiles = baseProjArray[level];
        UpdateEffectStrings();
    }

    protected override void UpdateEffectStrings() {
        Effects = [
            $"Fires {BaseProjectiles} projectiles"
        ];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);
        }
    }
}
