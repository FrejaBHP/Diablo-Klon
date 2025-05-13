using System;
using Godot;

public abstract class Skill {
    protected static readonly PackedScene thrustAttackScene = GD.Load<PackedScene>("res://scenes/test_attack_tween.tscn");

    public Actor ActorOwner { get; set; }

    public DamageModifiers BaseDamageModifiers { get; protected set; } = new(); // Kan måske erstattes med nogle mere simple værdier
    public DamageModifiers ActiveDamageModifiers { get; protected set; } = new(); // Som fx bare have en double der hedder "IncreasedPhys" og lægge det til i det relevante sted i et skill
    public double CriticalStrikeChance;
    public double CriticalStrikeMulti;

    public string Name { get; protected set; }
    public string Description { get; protected set; }

    public ESkillName SkillName { get; protected set; }
    public ESkillType Type { get; protected set; }
    public ESkillTags Tags { get; protected set; }
    public EDamageCategory DamageCategory { get; protected set; }
    public Texture2D Texture { get; protected set; }

    public double ManaCost { get; protected set; } = 0;
    public float ManaCostMultiplier { get; protected set; } = 1f;
    public double AddedDamageModifier { get; protected set; } = 1;
    public double SpeedModifier { get; protected set; } = 1;
    public double Cooldown { get; protected set; } = 0;

    public float CastRange { get; set; } // Mainly intended to be used by the AI to help them walk into range first

    public virtual bool CanUseSkill() {
        if (!ActorOwner.IsIgnoringManaCosts) {
            if (ActorOwner.BasicStats.CurrentMana < ManaCost * ManaCostMultiplier) {
                return false;
            }
        }
        
        // Indsæt tjek for cooldowns når de bliver implementeret

        if (this is IAttack attack) {
            if (ActorOwner.IsIgnoringWeaponRestrictions) {
                return true;
            }
            return attack.AreActorWeaponsCompatible(ActorOwner.MainHand.Weapon, ActorOwner.OffHandItem);
        }

        return true;
    }

    public abstract void UseSkill();

    public string GetAttackSpeedModifier() {
        return Math.Round(SpeedModifier * 100, 0).ToString();
    }

    public string GetDamageModifier() {
        return Math.Round(AddedDamageModifier * 100, 0).ToString();
    }

    public void DeductManaFromActor() {
        if (!ActorOwner.IsIgnoringManaCosts) {
            ActorOwner.BasicStats.CurrentMana -= ManaCost * ManaCostMultiplier;
        }
    }

    public SkillDamage RollForDamage(bool canCrit) {
        bool isCritical = false;
        if (canCrit) {
            isCritical = RollForCritical(CriticalStrikeChance);
        }

        double physical = Utilities.RandomDouble(ActiveDamageModifiers.Physical.SMinTotal, ActiveDamageModifiers.Physical.SMaxTotal);
        double fire = Utilities.RandomDouble(ActiveDamageModifiers.Fire.SMinTotal, ActiveDamageModifiers.Fire.SMaxTotal);
        double cold = Utilities.RandomDouble(ActiveDamageModifiers.Cold.SMinTotal, ActiveDamageModifiers.Cold.SMaxTotal);
        double lightning = Utilities.RandomDouble(ActiveDamageModifiers.Lightning.SMinTotal, ActiveDamageModifiers.Lightning.SMaxTotal);
        double chaos = Utilities.RandomDouble(ActiveDamageModifiers.Chaos.SMinTotal, ActiveDamageModifiers.Chaos.SMaxTotal);

        if (isCritical) {
            physical *= CriticalStrikeMulti;
            fire *= CriticalStrikeMulti;
            cold *= CriticalStrikeMulti;
            lightning *= CriticalStrikeMulti;
            chaos *= CriticalStrikeMulti;
        }

        return new SkillDamage(physical, fire, cold, lightning, chaos, isCritical);
    }

    public static bool RollForCritical(double chance) {
        double critRoll = Utilities.RNG.NextDouble();

        if (chance >= critRoll) {
            //GD.Print($"Crit: {chance:F3} / {critRoll:F3}, True");
            return true;
        }
        //GD.Print($"Crit: {chance:F3} / {critRoll:F3}, False");
        return false;
    }

    public virtual void UpdateSkillValues() {
        if (ActorOwner != null) {
            // In case it's not enough to separate damage into Base = Skill base damage and Added = Player base damage
            /*
            DamageModifiers actorDamageMods = ActorOwner.DamageMods.ShallowCopy();

            if (AddedDamageModifier != 1) {
                actorDamageMods.Physical.SMinAdded *= AddedDamageModifier;
                actorDamageMods.Physical.SMaxAdded *= AddedDamageModifier;
                actorDamageMods.Fire.SMinAdded *= AddedDamageModifier;
                actorDamageMods.Fire.SMaxAdded *= AddedDamageModifier;
                actorDamageMods.Cold.SMinAdded *= AddedDamageModifier;
                actorDamageMods.Cold.SMaxAdded *= AddedDamageModifier;
                actorDamageMods.Lightning.SMinAdded *= AddedDamageModifier;
                actorDamageMods.Lightning.SMaxAdded *= AddedDamageModifier;
                actorDamageMods.Chaos.SMinAdded *= AddedDamageModifier;
                actorDamageMods.Chaos.SMaxAdded *= AddedDamageModifier;
            }

            ActiveDamageModifiers = actorDamageMods + BaseDamageModifiers;
            */
            
            ActiveDamageModifiers = ActorOwner.DamageMods + BaseDamageModifiers;

            if (AddedDamageModifier != 1) {
                ActiveDamageModifiers.Physical.SMinAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Physical.SMaxAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Fire.SMinAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Fire.SMaxAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Cold.SMinAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Cold.SMaxAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Lightning.SMinAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Lightning.SMaxAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Chaos.SMinAdded *= AddedDamageModifier;
                ActiveDamageModifiers.Chaos.SMaxAdded *= AddedDamageModifier;
            }

            CriticalStrikeChance = ActorOwner.MainHand.CritChance * ActorOwner.CritChanceMod.STotal;
            CriticalStrikeMulti = ActorOwner.CritMultiplier.STotal;

            double activeIncreasedMod;
            double activeMoreMod;

            if (DamageCategory == EDamageCategory.Melee) {
                activeIncreasedMod = ActiveDamageModifiers.IncreasedMelee;
                activeMoreMod = ActiveDamageModifiers.MoreMelee;
            }
            else if (DamageCategory == EDamageCategory.Ranged) {
                activeIncreasedMod = ActiveDamageModifiers.IncreasedRanged;
                activeMoreMod = ActiveDamageModifiers.MoreRanged;
            }
            else if (DamageCategory == EDamageCategory.Spell) {
                activeIncreasedMod = ActiveDamageModifiers.IncreasedSpell;
                activeMoreMod = ActiveDamageModifiers.MoreSpell;
            }
            else {
                activeIncreasedMod = 0;
                activeMoreMod = 1;
            }

            ActiveDamageModifiers.Physical.SIncreased += activeIncreasedMod;
            ActiveDamageModifiers.Fire.SIncreased += activeIncreasedMod;
            ActiveDamageModifiers.Cold.SIncreased += activeIncreasedMod;
            ActiveDamageModifiers.Lightning.SIncreased += activeIncreasedMod;
            ActiveDamageModifiers.Chaos.SIncreased += activeIncreasedMod;

            ActiveDamageModifiers.Physical.SMore *= activeMoreMod;
            ActiveDamageModifiers.Fire.SMore *= activeMoreMod;
            ActiveDamageModifiers.Cold.SMore *= activeMoreMod;
            ActiveDamageModifiers.Lightning.SMore *= activeMoreMod;
            ActiveDamageModifiers.Chaos.SMore *= activeMoreMod;

            // Does not contain all variables needed
            if (this is IAttack attack) {
                attack.UpdateAttackSpeedValues(ActorOwner.AttackSpeedMod);
                return;
            }

            // Ditto
            if (this is ISpell spell) {
                spell.UpdateCastSpeedValues(ActorOwner.CastSpeedMod);
                return;
            }
        }
    }

    public void SetSkillCollision(Area3D collisionArea) {
        if (ActorOwner.IsInGroup("Enemy")) {
            collisionArea.CollisionMask = 4;
        }
        else if (ActorOwner.IsInGroup("Player")) {
            collisionArea.CollisionMask = 32;
        }
    }

    public void DebugPrint() {
        GD.Print("Player Damage:");
        GD.Print($"P: {ActorOwner.DamageMods.Physical}");
        GD.Print($"F: {ActorOwner.DamageMods.Fire}");
        GD.Print($"C: {ActorOwner.DamageMods.Cold}");
        GD.Print($"L: {ActorOwner.DamageMods.Lightning}");
        GD.Print($"Ch: {ActorOwner.DamageMods.Chaos}");

        GD.Print("Base Damage:");
        GD.Print($"P: {BaseDamageModifiers.Physical}");
        GD.Print($"F: {BaseDamageModifiers.Fire}");
        GD.Print($"C: {BaseDamageModifiers.Cold}");
        GD.Print($"L: {BaseDamageModifiers.Lightning}");
        GD.Print($"Ch: {BaseDamageModifiers.Chaos}");

        GD.Print("Active Damage:");
        GD.Print($"P: {ActiveDamageModifiers.Physical}");
        GD.Print($"F: {ActiveDamageModifiers.Fire}");
        GD.Print($"C: {ActiveDamageModifiers.Cold}");
        GD.Print($"L: {ActiveDamageModifiers.Lightning}");
        GD.Print($"Ch: {ActiveDamageModifiers.Chaos}");
    }
}

public interface IAttack {
    ESkillWeapons Weapons { get; protected set; }
    Stat BaseAttackSpeedModifiers { get; protected set; }
    Stat ActiveAttackSpeedModifiers { get; protected set; }
    bool CanDualWield { get; protected set; }

    public void UpdateAttackSpeedValues(Stat actorAS) {
        ActiveAttackSpeedModifiers = actorAS + BaseAttackSpeedModifiers;
    }

    public string GetAttackSpeedModifier() {
        return Math.Round(ActiveAttackSpeedModifiers.STotal * 100, 0).ToString();
    }

    public bool AreActorWeaponsCompatible(WeaponItem mhWeapon, Item ohItem) {
        EItemWeaponBaseType mhWeaponType = mhWeapon != null ? mhWeapon.ItemWeaponBaseType : EItemWeaponBaseType.Unarmed;
        EItemWeaponBaseType ohWeaponType;

        if (ohItem != null && ohItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
            WeaponItem ohWeapon = ohItem as WeaponItem;
            ohWeaponType = ohWeapon.ItemWeaponBaseType;
        }
        else {
            ohWeaponType = EItemWeaponBaseType.Unarmed;
        }

        switch (mhWeaponType) {
            case EItemWeaponBaseType.Unarmed: 
                return Weapons.HasFlag(ESkillWeapons.Unarmed);
            
            case EItemWeaponBaseType.WeaponMelee1H:
                if (Weapons == ESkillWeapons.MeleeDW && ohWeaponType == EItemWeaponBaseType.Unarmed) {
                    return false;
                }
                return Weapons.HasFlag(ESkillWeapons.Melee1H);

            case EItemWeaponBaseType.WeaponMelee2H:
                return Weapons.HasFlag(ESkillWeapons.Melee2H);
            
            case EItemWeaponBaseType.WeaponRanged1H:
                return Weapons.HasFlag(ESkillWeapons.Ranged1H);
                
            case EItemWeaponBaseType.WeaponRanged2H:
                return Weapons.HasFlag(ESkillWeapons.Ranged2H);

            default:
                return false;
        }
    }
}

public interface ISpell {
    float BaseCastTime { get; protected set; }
    Stat BaseCastSpeedModifiers { get; protected set; }
    Stat ActiveCastSpeedModifiers { get; protected set; }

    public void UpdateCastSpeedValues(Stat actorCS) {
        ActiveCastSpeedModifiers = actorCS + BaseCastSpeedModifiers;
    }

    public string GetCastSpeedModifier() {
        return Math.Round(ActiveCastSpeedModifiers.STotal * 100, 0).ToString();
    }
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
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllMeleeWeapons;
    public bool CanDualWield { get; set; } = true;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseAttackRange { get; set; } = 3f;

    public SkillThrust() {
        Name = "Thrust";
        Description = "Attacks in a straight line with a melee weapon.";

        SkillName = ESkillName.BasicThrust;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Melee;
        DamageCategory = EDamageCategory.Melee;
        Texture = UILib.TextureSkillThrust;

        ManaCost = 1;

        CastRange = BaseAttackRange;

        //BaseDamageModifiers.IncreasedMelee = 0.35;
        //BaseDamageModifiers.MoreMelee = 1.25;
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            TestAttack testAttack = thrustAttackScene.Instantiate() as TestAttack;
            ActorOwner.GetTree().Root.GetChild(0).AddChild(testAttack);
            SetSkillCollision(testAttack.Hitbox);

            testAttack.GlobalPosition = testAttack.Position with { 
                X = ActorOwner.GlobalPosition.X, 
                Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight, 
                Z = ActorOwner.GlobalPosition.Z 
            };
            testAttack.GlobalRotation = ActorOwner.GlobalRotation;

            testAttack.StartAttack(DamageCategory, RollForDamage(true), ActorOwner.Penetrations, 2f, BaseAttackRange, 25f);

            DeductManaFromActor();
        }
    }
}

public class SkillDefaultProjectileAttack : Skill, IAttack, IProjectileSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllRangedWeapons;
    public bool CanDualWield { get; set; } = true;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseProjectileSpeed { get; set; } = 15f;
    public float BaseProjectileLifetime { get; set; } = 2f;

    public SkillDefaultProjectileAttack() {
        Name = "Default Ranged Attack";
        Description = "A default Attack";

        Type = ESkillType.Attack;
        Tags = ESkillTags.Ranged | ESkillTags.Projectile;
        DamageCategory = EDamageCategory.Ranged;
        
        ManaCost = 0;

        CastRange = 10f;
    }

    public override void UseSkill() {
        
    }
}
