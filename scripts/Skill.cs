using System;
using Godot;

public abstract class Skill {
    protected static readonly PackedScene thrustAttackScene = GD.Load<PackedScene>("res://scenes/test_attack_tween.tscn");


    public Actor ActorOwner { get; set; }

    public string Name { get; protected set; }
    public string Description { get; protected set; }

    public ESkillType Type { get; protected set; }
    public ESkillTags Tags { get; protected set; }

    public int ManaCost { get; protected set; }
    public double AddedDamageModifier { get; protected set; }
    public double SpeedModifier { get; protected set; }

    public abstract void UseSkill();

    public string GetAttackSpeedModifier() {
        return Math.Round(SpeedModifier * 100, 0).ToString();
    }

    public string GetDamageModifier() {
        return Math.Round(AddedDamageModifier * 100, 0).ToString();
    }
}

public interface IAttack {
    ESkillWeapons Weapons { get; protected set; }
    bool CanDualWield { get; protected set; }
}

public interface ISpell {
    float BaseCastTime { get; protected set; }
}

public interface IMeleeSkill {
    float BaseAttackRange { get; protected set; }
}

public interface IProjectileSkill {
    float BaseProjectileSpeed { get; protected set; }
    float BaseProjectileLifetime { get; protected set; }
}

public interface IAreaSkill {
    float BaseAreaRadius { get; protected set; }
}

public class SkillThrust : Skill, IAttack, IMeleeSkill {
    public SkillThrust() {
        Name = "Thrust";
        Description = "Attacks in a straight line with a melee weapon.";

        Type = ESkillType.Attack;
        Tags = ESkillTags.Melee;

        ManaCost = 0;
        AddedDamageModifier = 1;
        SpeedModifier = 1;
    }

    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllMeleeWeapons;
    public bool CanDualWield { get; set; } = true;

    public float BaseAttackRange { get; set; } = 3f;

    public override void UseSkill() {
        if (ActorOwner != null) {
            TestAttack testAttack = thrustAttackScene.Instantiate() as TestAttack;
            ActorOwner.GetTree().Root.GetChild(0).AddChild(testAttack);

            testAttack.GlobalPosition = testAttack.Position with { 
                X = ActorOwner.GlobalPosition.X, 
                Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight, 
                Z = ActorOwner.GlobalPosition.Z 
            };
            testAttack.GlobalRotation = ActorOwner.GlobalRotation;

            testAttack.StartAttack(2f, 2.5f, 25f);
        }
    }
}

public class SkillDefaultProjectileAttack : Skill, IAttack, IProjectileSkill {
    public SkillDefaultProjectileAttack() {
        Name = "Default Ranged Attack";
        Description = "A default Attack";

        Type = ESkillType.Attack;
        Tags = ESkillTags.Projectile;
        
        ManaCost = 0;
        AddedDamageModifier = 1;
        SpeedModifier = 1;
    }

    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllRangedWeapons;
    public bool CanDualWield { get; set; } = true;

    public float BaseProjectileSpeed { get; set; } = 6f;
    public float BaseProjectileLifetime { get; set; } = 2f;

    public override void UseSkill() {
        
    }
}
