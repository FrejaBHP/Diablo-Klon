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
    public bool FiresSequentially { get; set; } = false;

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

        TotalPierces = BasePierces;
        TotalProjectiles = BaseProjectiles;

        BaseDamageModifiers.Physical.SMinBase = 1;
        BaseDamageModifiers.Physical.SMaxBase = 3;

        BaseDamageModifiers.Fire.SMinBase = 1;
        BaseDamageModifiers.Fire.SMaxBase = 3;

        BaseDamageModifiers.Cold.SMinBase = 1;
        BaseDamageModifiers.Cold.SMaxBase = 3;

        BaseDamageModifiers.Lightning.SMinBase = 1;
        BaseDamageModifiers.Lightning.SMaxBase = 3;

        BaseDamageModifiers.Chaos.SMinBase = 1;
        BaseDamageModifiers.Chaos.SMaxBase = 3;
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
