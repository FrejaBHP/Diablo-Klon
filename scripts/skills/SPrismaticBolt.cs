using Godot;
using System;

public class SPrismaticBolt : Skill, ISpell, IProjectileSkill {
    public double BaseCastTime { get; set; } = 0.70;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;
    public int BasePierces { get; set; } = 0;
    public int AddedPierces { get; set; } = 0;
    public int TotalPierces { get; set; }
    public int BaseProjectiles { get; set; } = 1;
    public int AddedProjectiles { get; set; } = 0;
    public int TotalProjectiles { get; set; }

    public bool CanFireSequentially { get; set; } = true;
    public bool FiresSequentially { get; set; } = false;

    public double MinimumSpreadAngle { get; set; } = 0;
    public double MaximumSpreadAngle { get; set; } = 0;
    public double MinimumSpreadAngleOverride { get; set; } = 0;
    public double MaximumSpreadAngleOverride { get; set; } = 0;

    private static readonly double[] minDamageArray = [
        1, 1, 2, 2, 3,
        3, 4, 5, 6, 7,
        9, 11, 14, 18, 22
    ];

    private static readonly double[] maxDamageArray = [
        3, 4, 4, 6, 7,
        9, 11, 14, 18, 22,
        28, 35, 44, 54, 68
    ];

    public SPrismaticBolt() {
        Name = "Prismatic Bolt";
        Description = "Conjures a bolt that travels in a straight line, dealing damage to the first enemy hit.";

        SkillName = ESkillName.PrismaticBolt;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Projectile | ESkillTags.Physical | ESkillTags.Fire | ESkillTags.Cold | ESkillTags.Lightning | ESkillTags.Chaos;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillPrismaticBolt;

        ManaCost = 1;

        CastRange = 15f;

        AddedDamageModifier = 2.50;

        TotalPierces = BasePierces;
        TotalProjectiles = BaseProjectiles;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Physical.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Physical.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Cold.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Cold.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Lightning.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Lightning.SMaxBase = maxDamageArray[level];

        BaseDamageModifiers.Chaos.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Chaos.SMaxBase = maxDamageArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            Projectile proj = genericProjectileScene.Instantiate() as Projectile;
            Game.Instance.AddChild(proj);
            SetSkillCollision(proj.Hitbox);

            proj.GlobalPosition = proj.Position with { 
                X = ActorOwner.GlobalPosition.X, 
                Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight, 
                Z = ActorOwner.GlobalPosition.Z 
            };

            if (mouseAimPosition != Vector3.Zero) {
                proj.SetFacing(mouseAimPosition);
            }
            else {
                proj.SetFacing(ActorOwner.GlobalRotation.Y);
            }
            proj.SetProperties(DamageCategory, RollForDamage(true), ActorOwner.Penetrations, BaseProjectileSpeed, -1, TotalPierces);

            DeductManaFromActor();
        }
    }
}
