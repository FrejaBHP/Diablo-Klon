using Godot;
using System;

public class SThrust : Skill, IAttack, IMeleeSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllMeleeWeapons;
    public bool CanDualWield { get; set; } = true;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseAttackRange { get; set; } = 3f;

    private const float speed = 25f;

    private static readonly double[] addedDamageModArray = [
        1.60, 1.70, 1.82, 1.93, 2.06,
        2.19, 2.34, 2.49, 2.65, 2.82,
        3.00, 3.20, 3.41, 3.63, 3.86
    ];

    public SThrust() {
        Name = "Thrust";
        Description = "Attacks in a straight line with a melee weapon.";
        Effects = [];

        SkillName = ESkillName.BasicThrust;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Melee;
        SkillDamageTags = ESkillDamageTags.Attack | ESkillDamageTags.Melee;
        DamageCategory = EDamageCategory.Melee;
        Texture = UILib.TextureSkillThrust;

        ManaCost.SBase = 2;

        CastRange = BaseAttackRange;

        UsesMouseAim = true;
    }

    protected override void OnSkillLevelChanged() {
        AddedDamageModifier = addedDamageModArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            SThrustScene thrustScene = thrustAttackScene.Instantiate() as SThrustScene;
            Run.Instance.AddChild(thrustScene);
            thrustScene.TargetHit += ApplyMeleeSkillBehaviourToTarget;
            SetSkillCollision(thrustScene.Hitbox);

            thrustScene.GlobalPosition = thrustScene.Position with { 
                X = ActorOwner.GlobalPosition.X, 
                Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight, 
                Z = ActorOwner.GlobalPosition.Z 
            };
            
            if (mouseAimPosition != Vector3.Zero) {
                thrustScene.LookAt(mouseAimPosition, Vector3.Up, true);
            }
            else {
                thrustScene.RotateY(ActorOwner.GlobalRotation.Y);
            }

            thrustScene.StartAttack(2f, BaseAttackRange, speed);

            DeductManaFromActor();
        }
    }

    public void ApplyMeleeSkillBehaviourToTarget(Actor target) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true); // Rerolls for every target atm
        target.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
    }
}
