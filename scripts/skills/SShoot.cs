using Godot;
using System;
using System.Collections.Generic;

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

    public SShoot() {
        Name = "Piercing Shot";
        Description = "Fires a penetrating arrow in a straight line with a bow.";

        SkillName = ESkillName.BasicShoot;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Projectile | ESkillTags.Ranged;
        DamageCategory = EDamageCategory.Ranged;
        Texture = UILib.TextureSkillShoot;

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
            float coefficient = IProjectileSkill.GetSpreadCoefficient(ActorOwner.GlobalPosition, mouseAimPosition);
            Vector3 diffVector = mouseAimPosition - ActorOwner.GlobalPosition;

            IProjectileSkill pSkill = this;
            float[] angleOffsets = pSkill.GetMultipleProjectileAngles(TotalProjectiles, coefficient);

            for (int i = 0; i < TotalProjectiles; i++) {
                Projectile proj = genericProjectileScene.Instantiate() as Projectile;
                Game.Instance.AddChild(proj);
                SetSkillCollision(proj.Hitbox);

                proj.GlobalPosition = proj.Position with { 
                    X = ActorOwner.GlobalPosition.X, 
                    Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight, 
                    Z = ActorOwner.GlobalPosition.Z 
                };

                if (mouseAimPosition != Vector3.Zero) {
                    if (!FiresSequentially) {
                        Vector3 newPos = diffVector.Rotated(Vector3.Up, angleOffsets[i]) + ActorOwner.GlobalPosition;
                        proj.SetFacing(newPos);
                    }
                    else {
                        proj.SetFacing(mouseAimPosition);
                    }
                }
                else {
                    if (!FiresSequentially) {
                        Vector3 rotation = ActorOwner.GlobalRotation.Rotated(Vector3.Up, angleOffsets[i]);
                        proj.SetFacing(rotation.Y);
                    }
                    else {
                        proj.SetFacing(ActorOwner.GlobalRotation.Y);
                    }
                }
                
                proj.SetProperties(DamageCategory, RollForDamage(true), ActorOwner.Penetrations, BaseProjectileSpeed, -1, TotalPierces);
            }

            DeductManaFromActor();
        }
    }
}
