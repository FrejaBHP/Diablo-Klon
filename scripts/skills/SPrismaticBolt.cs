using Godot;
using System;

public class SPrismaticBolt : Skill, ISpell, IProjectileSkill {
    public double BaseCastTime { get; set; } = 0.70;
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

    private static readonly double[] minDamageArray = [
        3, 3, 4, 5, 6,
        7, 8, 10, 12, 15,
        18, 22, 26, 32, 38
    ];

    private static readonly double[] maxDamageArray = [
        5, 6, 7, 8, 10,
        12, 14, 17, 21, 25,
        30, 37, 44, 53, 64
    ];

    public SPrismaticBolt() {
        Name = "Prismatic Bolt";
        Description = "Conjures an elemental bolt that travels in a straight line, dealing damage to the first enemy hit.";
        Effects = [];

        SkillName = ESkillName.PrismaticBolt;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Projectile | ESkillTags.Fire | ESkillTags.Cold | ESkillTags.Lightning;
        SkillDamageTags = ESkillDamageTags.Spell | ESkillDamageTags.Projectile;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillPrismaticBolt;

        ManaCost.SBase = 3;

        CastRange = 15f;

        CriticalStrikeChance.SBase = 0.06;
        AddedDamageModifier = 2;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Cold.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Cold.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Lightning.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Lightning.SMaxBase = maxDamageArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IProjectileSkill pSkill = this;
            pSkill.BasicProjectileSkillBehaviour(this, mouseAimPosition);
            DeductManaFromActor();

            // Effect GUI test
            //ActorOwner.ReceiveEffect(new SpeedBurstTestEffect(ActorOwner));
            //ActorOwner.ReceiveEffect(new TestStackEffect(ActorOwner));
            //ActorOwner.ReceiveEffect(new TestRepeatableEffect(ActorOwner));
        }
    }

    public void ApplyProjectileSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true, target);
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }
}
