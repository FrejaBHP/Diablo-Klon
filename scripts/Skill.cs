using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public abstract class Skill {
    protected static readonly PackedScene thrustAttackScene = GD.Load<PackedScene>("res://scenes/skills/scene_thrust.tscn");
    protected static readonly PackedScene genericProjectileScene = GD.Load<PackedScene>("res://scenes/skills/scene_projectile_generic.tscn");

    public Actor ActorOwner { get; set; }
    public SkillSlotCluster HousingSkillCluster = null;

    public DamageModifiers BaseDamageModifiers { get; protected set; } = new(); // Kan måske erstattes med nogle mere simple værdier
    public DamageModifiers ActiveDamageModifiers { get; protected set; } = new(); // Som fx bare have en double der hedder "IncreasedPhys" og lægge det til i det relevante sted i et skill
    public double BaseCriticalStrikeChance;
    public double CriticalStrikeMulti;

    public string Name { get; protected set; }
    public string Description { get; protected set; }

    protected int level = 0;
    public int Level { 
        get => level; 
        set {
            level = value;
            OnSkillLevelChanged();
        }
    }

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

    public bool UsesMouseAim { get; protected set; } = true;
    public bool AimsInStraightLine { get; protected set; } = true;
    protected Vector3 mouseAimPosition = Vector3.Zero;

    public abstract void UseSkill();
    protected abstract void OnSkillLevelChanged();

    public virtual bool CanUseSkill() {
        if (!ActorOwner.IsIgnoringManaCosts) {
            if (ActorOwner.BasicStats.CurrentMana < ManaCost * ManaCostMultiplier) {
                return false;
            }
        }
        
        // Indsæt tjek for cooldowns når de bliver implementeret

        if (ActorOwner.IsInGroup("Player") && UsesMouseAim) {
            Vector3 pos = Run.Instance.PlayerActor.CreateCameraRaycastAndGetPosition();

            if (pos == Vector3.Zero) {
                return false;
            }

            SetMouseAimPosition(Run.Instance.PlayerActor.CreateCameraRaycastAndGetPosition());
        }

        if (this is IAttack attack) {
            if (ActorOwner.IsIgnoringWeaponRestrictions) {
                return true;
            }
            return attack.AreActorWeaponsCompatible(ActorOwner.MainHand, ActorOwner.OffHandItem);
        }

        return true;
    }

    public void SetMouseAimPosition(Vector3 pos) {
        if (AimsInStraightLine) {
            pos.Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight;
        }

        mouseAimPosition = pos;
    }

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
        double physical = 0;
        double fire = 0;
        double cold = 0;
        double lightning = 0;
        double chaos = 0;
        bool isCritical = false;

        if (this is IAttack attack) {
            attack.GetUsedWeaponStats(ActorOwner, out ActorWeaponStats wStats);

            if (canCrit) {
                isCritical = RollForCritical(wStats.CritChance * ActorOwner.CritChanceMod.STotal);
            }

            ActiveDamageModifiers.Physical.CalculateTotalWithBase(wStats.PhysMinDamage, wStats.PhysMaxDamage, AddedDamageModifier, out double pMin, out double pMax);
            physical = Utilities.RandomDouble(pMin, pMax);
            ActiveDamageModifiers.Fire.CalculateTotalWithBase(wStats.FireMinDamage, wStats.FireMaxDamage, AddedDamageModifier, out double fMin, out double fMax);
            fire = Utilities.RandomDouble(fMin, fMax);
            ActiveDamageModifiers.Cold.CalculateTotalWithBase(wStats.ColdMinDamage, wStats.ColdMaxDamage, AddedDamageModifier, out double cMin, out double cMax);
            cold = Utilities.RandomDouble(cMin, cMax);
            ActiveDamageModifiers.Lightning.CalculateTotalWithBase(wStats.LightningMinDamage, wStats.LightningMaxDamage, AddedDamageModifier, out double lMin, out double lMax);
            lightning = Utilities.RandomDouble(lMin, lMax);
            ActiveDamageModifiers.Chaos.CalculateTotalWithBase(wStats.ChaosMinDamage, wStats.ChaosMaxDamage, AddedDamageModifier, out double chMin, out double chMax);
            chaos = Utilities.RandomDouble(chMin, chMax);
        }
        else if (this is ISpell spell) {
            if (canCrit) {
                isCritical = RollForCritical(BaseCriticalStrikeChance * ActorOwner.CritChanceMod.STotal);
            }

            ActiveDamageModifiers.Physical.CalculateTotal(out double pMin, out double pMax);
            physical = Utilities.RandomDouble(pMin, pMax);
            ActiveDamageModifiers.Fire.CalculateTotal(out double fMin, out double fMax);
            fire = Utilities.RandomDouble(fMin, fMax);
            ActiveDamageModifiers.Cold.CalculateTotal(out double cMin, out double cMax);
            cold = Utilities.RandomDouble(cMin, cMax);
            ActiveDamageModifiers.Lightning.CalculateTotal(out double lMin, out double lMax);
            lightning = Utilities.RandomDouble(lMin, lMax);
            ActiveDamageModifiers.Chaos.CalculateTotal(out double chMin, out double chMax);
            chaos = Utilities.RandomDouble(chMin, chMax);
        }

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
        return chance >= critRoll;
    }

    public virtual void RecalculateSkillValues() {
        if (ActorOwner != null) {
            ActiveDamageModifiers = ActorOwner.DamageMods + BaseDamageModifiers;

            List<SupportGem> supportGems;

            bool isPartOfCluster = HousingSkillCluster != null;
            if (isPartOfCluster) {
                supportGems = HousingSkillCluster.GetSupports();
            }
            else {
                supportGems = [];
            }

            if (this is IAttack atSkill) {
                // Does not contain all variables needed
                atSkill.UpdateAttackSpeedValues(ActorOwner.AttackSpeedMod);
                //attack.UpdateWeaponStats(ActorOwner.MainHandStats, ActorOwner.OffHandStats);

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Attack)) {
                            support.ModifyAttackSkill(atSkill);
                        }
                    } 
                }
            }

            if (this is ISpell spSkill) {
                // Ditto
                spSkill.UpdateCastSpeedValues(ActorOwner.CastSpeedMod);

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Spell)) {
                            support.ModifySpellSkill(spSkill);
                        }
                    }
                }
            }

            if (this is IMeleeSkill mSkill) {
                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Melee)) {
                            support.ModifyMeleeSkill(mSkill);
                        }
                    }  
                }
            }

            if (this is IProjectileSkill pSkill) {
                pSkill.AddedPierces = 0;
                pSkill.AddedProjectiles = 0;
                pSkill.MinimumSpreadAngleOverride = 0;
                pSkill.MaximumSpreadAngleOverride = 0;
                
                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Projectile)) {
                            support.ModifyProjectileSkill(pSkill);
                        }
                    }
                }

                pSkill.TotalPierces = pSkill.BasePierces + pSkill.AddedPierces;
                pSkill.TotalProjectiles = pSkill.BaseProjectiles + pSkill.AddedProjectiles;
            }

            if (this is IAreaSkill aSkill) {
                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Area)) {
                            support.ModifyAreaSkill(aSkill);
                        }
                    }
                }
            }

            if (this is IDurationSkill dSkill) {
                dSkill.IncreasedDuration = 1;
                dSkill.MoreDuration = 1;

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Duration)) {
                            support.ModifyDurationSkill(dSkill);
                        }
                    }
                }

                dSkill.TotalDuration = dSkill.BaseDuration * dSkill.IncreasedDuration * dSkill.MoreDuration;
            }

            if (isPartOfCluster) {
                foreach (SupportGem support in supportGems) {
                    if (support.AffectsDamageModifiers) {
                        support.ApplyToDamageModifiers(ActiveDamageModifiers);
                    }
                }
            }
            
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
        }
    }

    public void SetSkillCollision(Area3D collisionArea) {
        if (ActorOwner.IsInGroup("Enemy")) {
            collisionArea.CollisionMask += 4;
        }
        else if (ActorOwner.IsInGroup("Player")) {
            collisionArea.CollisionMask += 32;
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

    public void GetUsedWeaponStats(Actor owner, out ActorWeaponStats wStats) {
        if (owner.IsDualWielding) {
            if (owner.IsUsingMainHandDW) {
                wStats = owner.MainHandStats;
            }
            else {
                wStats = owner.OffHandStats;
            }
        }
        else {
            wStats = owner.MainHandStats;
        }
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
    double BaseCastTime { get; protected set; }
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
    double BaseProjectileLifetime { get; protected set; }
    int BasePierces { get; protected set; }
    int AddedPierces { get; set; }
    int TotalPierces { get; set; }
    int BaseProjectiles { get; protected set; }
    int AddedProjectiles { get; set; }
    int TotalProjectiles { get; set; }
    bool CanFireSequentially { get; protected set; }
    bool FiresSequentially { get; set; }

    double MinimumSpreadAngle { get; protected set; }
    double MaximumSpreadAngle { get; protected set; }
    double MinimumSpreadAngleOverride { get; set; }
    double MaximumSpreadAngleOverride { get; set; }

    static readonly double[] projectileMinAngles = [
        0,                  // 1 projectile, no spread
        Math.Tau / 12,      // 2 projectiles, 30 degrees
        Math.Tau / 9,       // 3 projectiles, 40 degrees
        Math.Tau / 7.5,     // 4 projectiles, 48 degrees
        Math.Tau / 6,       // 5 projectiles, 60 degrees
        Math.Tau / 4.5,     // 6+ projectiles, 80 degrees
    ];

    static readonly double[] projectileMaxAngles = [
        0,                  // 1 projectile, no spread
        Math.Tau / 8,       // 2 projectiles, 45 degrees
        Math.Tau / 6,       // 3 projectiles, 60 degrees
        Math.Tau / 5,       // 4 projectiles, 72 degrees
        Math.Tau / 4,       // 5 projectiles, 90 degrees
        Math.Tau / 3,       // 6+ projectiles, 120 degrees
    ];

    public static float GetSpreadCoefficient(Vector3 startPos, Vector3 endPos) {
        float dist = startPos.DistanceTo(endPos);
        float coefficient = Math.Clamp(dist / 8f, 0f, 1f);
        GD.Print($"Coefficient: {coefficient:F2}");
        return coefficient;
    }

    float[] GetMultipleProjectileAngles(int noOfProjectiles, float spreadCoefficient) {
        float[] angles = new float[noOfProjectiles];

        if (noOfProjectiles > 1) {
            double usedMinSpread;
            double usedMaxSpread;
            float finalSpread;

            if (MinimumSpreadAngleOverride != 0) {
                usedMinSpread = MinimumSpreadAngleOverride;
            }
            else if (MinimumSpreadAngle != 0) {
                usedMinSpread = MinimumSpreadAngle;
            }
            else {
                if (noOfProjectiles - 1 > projectileMinAngles.Length) {
                    usedMinSpread = projectileMinAngles.Last();
                }
                else {
                    usedMinSpread = projectileMinAngles[noOfProjectiles - 1];
                }
            }

            if (MaximumSpreadAngleOverride != 0) {
                usedMaxSpread = MaximumSpreadAngleOverride;
            }
            else if (MaximumSpreadAngle != 0) {
                usedMaxSpread = MaximumSpreadAngle;
            }
            else {
                if (noOfProjectiles - 1 > projectileMaxAngles.Length) {
                    usedMaxSpread = projectileMaxAngles.Last();
                }
                else {
                    usedMaxSpread = projectileMaxAngles[noOfProjectiles - 1];
                }
            }

            finalSpread = (float)(usedMaxSpread - ((usedMaxSpread - usedMinSpread) * spreadCoefficient));
            float spreadSegment = finalSpread / (noOfProjectiles - 1);
            
            // if number of projectiles is even
            if (noOfProjectiles % 2 == 0) {
                for (int i = 0; i < noOfProjectiles; i++) {
                    angles[i] = (-finalSpread / 2) + (spreadSegment * i);
                }
            }
            // if number of projectiles is odd
            else {
                for (int i = 0; i < noOfProjectiles; i++) {
                    angles[i] = (-finalSpread / 2) + (spreadSegment * i);
                }
            }
        }
        else {
            angles[0] = 0;
        }

        return angles;
    }
}

public interface IAreaSkill {
    double BaseAreaRadius { get; protected set; }
}

public interface IDurationSkill {
    double BaseDuration { get; protected set; }
    double IncreasedDuration { get; set; }
    double MoreDuration { get; set; }
    double TotalDuration { get; set; }
}
