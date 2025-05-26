using Godot;
using System;

public class SShoot : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.Ranged2H;
    public bool CanDualWield { get; set; } = false;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public double BaseProjectileLifetime { get; set; } = 2f;

    public SShoot() {
        Name = "Shoot";
        Description = "Fires an arrow in a straight line with a bow.";

        SkillName = ESkillName.BasicShoot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Ranged;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillShoot;

        ManaCost = 1;

        CastRange = 15f;
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
            
            proj.SetProperties(DamageCategory, RollForDamage(true), ActorOwner.Penetrations, BaseProjectileSpeed, -1, 0);

            DeductManaFromActor();
        }
    }
}
