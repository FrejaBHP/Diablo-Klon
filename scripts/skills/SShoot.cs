using Godot;
using System;

public class SShoot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;
    public int BasePierces { get; set; } = 2;
    public int AddedPierces { get; set; } = 0;
    public int TotalPierces { get; set; }
    public int BaseProjectiles { get; set; } = 1;
    public int AddedProjectiles { get; set; } = 0;
    public int TotalProjectiles { get; set; }
    public bool FiresSequentially { get; set; } = false;

    public SShoot() {
        Name = "Shoot";
        Description = "Fires an arrow in a straight line with a bow.";

        SkillName = ESkillName.BasicShoot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillShoot;

        ManaCost = 1;

        CastRange = 15f;

        AddedDamageModifier = 1.30;

        TotalPierces = BasePierces;
        TotalProjectiles = BaseProjectiles;
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
