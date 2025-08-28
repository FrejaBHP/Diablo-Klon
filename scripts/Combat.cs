using System.Collections.Generic;
using System.Text;
using Godot;

public class DamageModifiers() {
    public DamageTypeStat Physical { get; protected set; } = new();
    public DamageTypeStat Fire { get; protected set; } = new();
    public DamageTypeStat Cold { get; protected set; } = new();
    public DamageTypeStat Lightning { get; protected set; } = new();
    public DamageTypeStat Chaos { get; protected set; } = new();

    public double IncreasedAttack = 0;
    public double IncreasedSpell = 0;
    public double IncreasedMelee = 0;
    public double IncreasedProjectile = 0;
    public double IncreasedArea = 0;
    public double IncreasedDoT = 0;
    public double IncreasedAll = 0;

    public double BleedMagnitude = 0;
    public double IgniteMagnitude = 0;
    public double PoisonMagnitude = 0;

    public double MoreAttack = 1;
    public double MoreSpell = 1;
    public double MoreMelee = 1;
    public double MoreProjectile = 1;
    public double MoreArea = 1;
    public double MoreDoT = 1;
    public double MoreAll = 1;

    public double ExtraPhysical = 0;
    public double ExtraFire = 0;
    public double ExtraCold = 0;
    public double ExtraLightning = 0;
    public double ExtraChaos = 0;

    protected DamageTypeStat GetDamageTypeStat(EDamageType damageType) {
        DamageTypeStat damageStat;

        switch (damageType) {
            case EDamageType.Physical:
                damageStat = Physical;
                break;

            case EDamageType.Fire:
                damageStat = Fire;
                break;
            
            case EDamageType.Cold:
                damageStat = Cold;
                break;
            
            case EDamageType.Lightning:
                damageStat = Lightning;
                break;
            
            case EDamageType.Chaos:
                damageStat = Chaos;
                break;

            // Failsafe
            default:
                damageStat = Physical;
                break;
        }

        return damageStat;
    }

    public static SkillDamageRangeInfo CalculateBaseAttackDamage(DamageModifiers damageMods, ActorWeaponStats weaponStats, double addedMultiplier) {
        double physMin = (weaponStats.PhysMinDamage + damageMods.Physical.SMinAdded + damageMods.Physical.SAttackMinAdded) * addedMultiplier;
        double physMax = (weaponStats.PhysMaxDamage + damageMods.Physical.SMaxAdded + damageMods.Physical.SAttackMaxAdded) * addedMultiplier;
        double fireMin = (weaponStats.FireMinDamage + damageMods.Fire.SMinAdded + damageMods.Fire.SAttackMinAdded) * addedMultiplier;
        double fireMax = (weaponStats.FireMaxDamage + damageMods.Fire.SMaxAdded + damageMods.Fire.SAttackMaxAdded) * addedMultiplier;
        double coldMin = (weaponStats.ColdMinDamage + damageMods.Cold.SMinAdded + damageMods.Cold.SAttackMinAdded) * addedMultiplier;
        double coldMax = (weaponStats.ColdMaxDamage + damageMods.Cold.SMaxAdded + damageMods.Cold.SAttackMaxAdded) * addedMultiplier;
        double lightningMin = (weaponStats.LightningMinDamage + damageMods.Lightning.SMinAdded + damageMods.Lightning.SAttackMinAdded) * addedMultiplier;
        double lightningMax = (weaponStats.LightningMaxDamage + damageMods.Lightning.SMaxAdded + damageMods.Lightning.SAttackMaxAdded) * addedMultiplier;
        double chaosMin = (weaponStats.ChaosMinDamage + damageMods.Chaos.SMinAdded + damageMods.Chaos.SAttackMinAdded) * addedMultiplier;
        double chaosMax = (weaponStats.ChaosMaxDamage + damageMods.Chaos.SMaxAdded + damageMods.Chaos.SAttackMaxAdded) * addedMultiplier;

        double combinedMinDamage = physMin + fireMin + coldMin + lightningMin + chaosMin;
        double combinedMaxDamage = physMax + fireMax + coldMax + lightningMax + chaosMax;

        if (damageMods.ExtraPhysical > 0) {
            physMin += combinedMinDamage * damageMods.ExtraPhysical;
            physMax += combinedMaxDamage * damageMods.ExtraPhysical;
        }
        if (damageMods.ExtraFire > 0) {
            fireMin += combinedMinDamage * damageMods.ExtraFire;
            fireMax += combinedMaxDamage * damageMods.ExtraFire;
        }
        if (damageMods.ExtraCold > 0) {
            coldMin += combinedMinDamage * damageMods.ExtraCold;
            coldMax += combinedMaxDamage * damageMods.ExtraCold;
        }
        if (damageMods.ExtraLightning > 0) {
            lightningMin += combinedMinDamage * damageMods.ExtraLightning;
            lightningMax += combinedMaxDamage * damageMods.ExtraLightning;
        }
        if (damageMods.ExtraChaos > 0) {
            chaosMin += combinedMinDamage * damageMods.ExtraChaos;
            chaosMax += combinedMaxDamage * damageMods.ExtraChaos;
        }

        return new SkillDamageRangeInfo(physMin, physMax, fireMin, fireMax, coldMin, coldMax, lightningMin, lightningMax, chaosMin, chaosMax);
    }

    public static SkillDamageRangeInfo CalculateBaseSpellDamage(DamageModifiers damageMods, double addedMultiplier) {
        double physMin = damageMods.Physical.SMinBase + ((damageMods.Physical.SMinAdded + damageMods.Physical.SSpellMinAdded) * addedMultiplier);
        double physMax = damageMods.Physical.SMaxBase + ((damageMods.Physical.SMaxAdded + damageMods.Physical.SSpellMaxAdded) * addedMultiplier);
        double fireMin = damageMods.Fire.SMinBase + ((damageMods.Fire.SMinAdded + damageMods.Fire.SSpellMinAdded) * addedMultiplier);
        double fireMax = damageMods.Fire.SMaxBase + ((damageMods.Fire.SMaxAdded + damageMods.Fire.SSpellMaxAdded) * addedMultiplier);
        double coldMin = damageMods.Cold.SMinBase + ((damageMods.Cold.SMinAdded + damageMods.Cold.SSpellMinAdded) * addedMultiplier);
        double coldMax = damageMods.Cold.SMaxBase + ((damageMods.Cold.SMaxAdded + damageMods.Cold.SSpellMaxAdded) * addedMultiplier);
        double lightningMin = damageMods.Lightning.SMinBase + ((damageMods.Lightning.SMinAdded + damageMods.Lightning.SSpellMinAdded) * addedMultiplier);
        double lightningMax = damageMods.Lightning.SMaxBase + ((damageMods.Lightning.SMaxAdded + damageMods.Lightning.SSpellMaxAdded) * addedMultiplier);
        double chaosMin = damageMods.Chaos.SMinBase + ((damageMods.Chaos.SMinAdded + damageMods.Chaos.SSpellMinAdded) * addedMultiplier);
        double chaosMax = damageMods.Chaos.SMaxBase + ((damageMods.Chaos.SMaxAdded + damageMods.Chaos.SSpellMaxAdded) * addedMultiplier);

        double combinedMinDamage = physMin + fireMin + coldMin + lightningMin + chaosMin;
        double combinedMaxDamage = physMax + fireMax + coldMax + lightningMax + chaosMax;

        if (damageMods.ExtraPhysical > 0) {
            physMin += combinedMinDamage * damageMods.ExtraPhysical;
            physMax += combinedMaxDamage * damageMods.ExtraPhysical;
        }
        if (damageMods.ExtraFire > 0) {
            fireMin += combinedMinDamage * damageMods.ExtraFire;
            fireMax += combinedMaxDamage * damageMods.ExtraFire;
        }
        if (damageMods.ExtraCold > 0) {
            coldMin += combinedMinDamage * damageMods.ExtraCold;
            coldMax += combinedMaxDamage * damageMods.ExtraCold;
        }
        if (damageMods.ExtraLightning > 0) {
            lightningMin += combinedMinDamage * damageMods.ExtraLightning;
            lightningMax += combinedMaxDamage * damageMods.ExtraLightning;
        }
        if (damageMods.ExtraChaos > 0) {
            chaosMin += combinedMinDamage * damageMods.ExtraChaos;
            chaosMax += combinedMaxDamage * damageMods.ExtraChaos;
        }

        return new SkillDamageRangeInfo(physMin, physMax, fireMin, fireMax, coldMin, coldMax, lightningMin, lightningMax, chaosMin, chaosMax);
    }

    public static SkillDamageRangeInfo ApplyMultipliersToDamageRangeInfo(SkillDamageRangeInfo baseDamageInfo, DamageModifiers damageMods, ESkillDamageTags damageTags) {
        double newPhysMin = baseDamageInfo.PhysicalMin;
        double newPhysMax = baseDamageInfo.PhysicalMax;
        double newFireMin = baseDamageInfo.FireMin;
        double newFireMax = baseDamageInfo.FireMax;
        double newColdMin = baseDamageInfo.ColdMin;
        double newColdMax = baseDamageInfo.ColdMax;
        double newLightningMin = baseDamageInfo.LightningMin;
        double newLightningMax = baseDamageInfo.LightningMax;
        double newChaosMin = baseDamageInfo.ChaosMin;
        double newChaosMax = baseDamageInfo.ChaosMax;

        double increasedMultiplier = 0;
        double moreMultiplier = 1;

        if (damageTags.HasFlag(ESkillDamageTags.Attack)) {
            increasedMultiplier += damageMods.IncreasedAttack;
            moreMultiplier *= damageMods.MoreAttack;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Spell)) {
            increasedMultiplier += damageMods.IncreasedSpell;
            moreMultiplier *= damageMods.MoreSpell;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Melee)) {
            increasedMultiplier += damageMods.IncreasedMelee;
            moreMultiplier *= damageMods.MoreMelee;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Projectile)) {
            increasedMultiplier += damageMods.IncreasedProjectile;
            moreMultiplier *= damageMods.MoreProjectile;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Area)) {
            increasedMultiplier += damageMods.IncreasedArea;
            moreMultiplier *= damageMods.MoreArea;
        }
        if (damageTags.HasFlag(ESkillDamageTags.DoT)) {
            increasedMultiplier += damageMods.IncreasedDoT;
            moreMultiplier *= damageMods.MoreDoT;
        }

        increasedMultiplier += damageMods.IncreasedAll;
        moreMultiplier *= damageMods.MoreAll;

        newPhysMin *= (1 + damageMods.Physical.SIncreased + increasedMultiplier) * damageMods.Physical.SMore * moreMultiplier;
        newPhysMax *= (1 + damageMods.Physical.SIncreased + increasedMultiplier) * damageMods.Physical.SMore * moreMultiplier;
        newFireMin *= (1 + damageMods.Fire.SIncreased + increasedMultiplier) * damageMods.Fire.SMore * moreMultiplier;
        newFireMax *= (1 + damageMods.Fire.SIncreased + increasedMultiplier) * damageMods.Fire.SMore * moreMultiplier;
        newColdMin *= (1 + damageMods.Cold.SIncreased + increasedMultiplier) * damageMods.Cold.SMore * moreMultiplier;
        newColdMax *= (1 + damageMods.Cold.SIncreased + increasedMultiplier) * damageMods.Cold.SMore * moreMultiplier;
        newLightningMin *= (1 + damageMods.Lightning.SIncreased + increasedMultiplier) * damageMods.Lightning.SMore * moreMultiplier;
        newLightningMax *= (1 + damageMods.Lightning.SIncreased + increasedMultiplier) * damageMods.Lightning.SMore * moreMultiplier;
        newChaosMin *= (1 + damageMods.Chaos.SIncreased + increasedMultiplier) * damageMods.Chaos.SMore * moreMultiplier;
        newChaosMax *= (1 + damageMods.Chaos.SIncreased + increasedMultiplier) * damageMods.Chaos.SMore * moreMultiplier;

        return new SkillDamageRangeInfo(newPhysMin, newPhysMax, newFireMin, newFireMax, newColdMin, newColdMax, newLightningMin, newLightningMax, newChaosMin, newChaosMax);
    }

    public static SkillDamageRangeInfo CalculateAttackSkillDamageRange(DamageModifiers damageMods, ActorWeaponStats weaponStats, double addedMultiplier, ESkillDamageTags damageTags) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseAttackDamage(damageMods, weaponStats, addedMultiplier), damageMods, damageTags);
    }

    public static SkillDamageRangeInfo CalculateSpellSkillDamageRange(DamageModifiers damageMods, double addedMultiplier, ESkillDamageTags damageTags) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseSpellDamage(damageMods, addedMultiplier), damageMods, damageTags);
    }

    public static double RollDamageRange(double min, double max) {
        return Utilities.RandomDouble(min, max);
    }

    // Holdover from old system. Still works for secondary damage, so keeping it for that purpose
    public void CalculateMultipliersWithType(DamageTypeStat damageStat, ESkillDamageTags damageTags, out double increasedMultiplier, out double moreMultiplier) {
        increasedMultiplier = damageStat.SIncreased;
        moreMultiplier = damageStat.SMore;

        if (damageTags.HasFlag(ESkillDamageTags.Attack)) {
            increasedMultiplier += IncreasedAttack;
            moreMultiplier *= MoreAttack;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Spell)) {
            increasedMultiplier += IncreasedSpell;
            moreMultiplier *= MoreSpell;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Melee)) {
            increasedMultiplier += IncreasedMelee;
            moreMultiplier *= MoreMelee;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Projectile)) {
            increasedMultiplier += IncreasedProjectile;
            moreMultiplier *= MoreProjectile;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Area)) {
            increasedMultiplier += IncreasedArea;
            moreMultiplier *= MoreArea;
        }
        if (damageTags.HasFlag(ESkillDamageTags.DoT)) {
            increasedMultiplier += IncreasedDoT;
            moreMultiplier *= MoreDoT;
        }

        increasedMultiplier += IncreasedAll;
        moreMultiplier *= MoreAll;
    }

    public DamageModifiers ShallowCopy() {
        DamageModifiers copy = (DamageModifiers)MemberwiseClone();
        copy.Physical = Physical.ShallowCopy();
        copy.Fire = Fire.ShallowCopy();
        copy.Cold = Cold.ShallowCopy();
        copy.Lightning = Lightning.ShallowCopy();
        copy.Chaos = Chaos.ShallowCopy();

        return copy;
    }

    public static DamageModifiers operator +(DamageModifiers a, DamageModifiers b) {
        DamageModifiers c = new DamageModifiers();

        c.Physical = a.Physical + b.Physical;
        c.Fire = a.Fire + b.Fire;
        c.Cold = a.Cold + b.Cold;
        c.Lightning = a.Lightning + b.Lightning;
        c.Chaos = a.Chaos + b.Chaos;

        c.IncreasedAttack = a.IncreasedAttack + b.IncreasedAttack;
        c.IncreasedSpell = a.IncreasedSpell + b.IncreasedSpell;
        c.IncreasedMelee = a.IncreasedMelee + b.IncreasedMelee;
        c.IncreasedProjectile = a.IncreasedProjectile + b.IncreasedProjectile;
        c.IncreasedArea = a.IncreasedArea + b.IncreasedArea;
        c.IncreasedDoT = a.IncreasedDoT + b.IncreasedDoT;
        c.IncreasedAll = a.IncreasedAll + b.IncreasedAll;

        c.BleedMagnitude = a.BleedMagnitude + b.BleedMagnitude;
        c.IgniteMagnitude = a.IgniteMagnitude + b.IgniteMagnitude;
        c.PoisonMagnitude = a.PoisonMagnitude + b.PoisonMagnitude;

        c.MoreAttack = a.MoreAttack * b.MoreAttack;
        c.MoreSpell = a.MoreSpell * b.MoreSpell;
        c.MoreMelee = a.MoreMelee * b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile * b.MoreProjectile;
        c.MoreArea = a.MoreArea * b.MoreArea;
        c.MoreDoT = a.MoreDoT * b.MoreDoT;
        c.MoreAll = a.MoreAll * b.MoreAll;

        c.ExtraPhysical = a.ExtraPhysical + b.ExtraPhysical;
        c.ExtraFire = a.ExtraFire + b.ExtraFire;
        c.ExtraCold = a.ExtraCold + b.ExtraCold;
        c.ExtraLightning = a.ExtraLightning + b.ExtraLightning;
        c.ExtraChaos = a.ExtraChaos + b.ExtraChaos;
        
        return c;
    }

    public static DamageModifiers operator -(DamageModifiers a, DamageModifiers b) {
        DamageModifiers c = new DamageModifiers();

        c.Physical = a.Physical - b.Physical;
        c.Fire = a.Fire - b.Fire;
        c.Cold = a.Cold - b.Cold;
        c.Lightning = a.Lightning - b.Lightning;
        c.Chaos = a.Chaos - b.Chaos;

        c.IncreasedAttack = a.IncreasedAttack - b.IncreasedAttack;
        c.IncreasedSpell = a.IncreasedSpell - b.IncreasedSpell;
        c.IncreasedMelee = a.IncreasedMelee - b.IncreasedMelee;
        c.IncreasedProjectile = a.IncreasedProjectile - b.IncreasedProjectile;
        c.IncreasedArea = a.IncreasedArea - b.IncreasedArea;
        c.IncreasedDoT = a.IncreasedDoT - b.IncreasedDoT;
        c.IncreasedAll = a.IncreasedAll - b.IncreasedAll;

        c.BleedMagnitude = a.BleedMagnitude - b.BleedMagnitude;
        c.IgniteMagnitude = a.IgniteMagnitude - b.IgniteMagnitude;
        c.PoisonMagnitude = a.PoisonMagnitude - b.PoisonMagnitude;

        c.MoreAttack = a.MoreAttack / b.MoreAttack;
        c.MoreSpell = a.MoreSpell / b.MoreSpell;
        c.MoreMelee = a.MoreMelee / b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile / b.MoreProjectile;
        c.MoreArea = a.MoreArea / b.MoreArea;
        c.MoreDoT = a.MoreDoT / b.MoreDoT;
        c.MoreAll = a.MoreAll / b.MoreAll;

        c.ExtraPhysical = a.ExtraPhysical - b.ExtraPhysical;
        c.ExtraFire = a.ExtraFire - b.ExtraFire;
        c.ExtraCold = a.ExtraCold - b.ExtraCold;
        c.ExtraLightning = a.ExtraLightning - b.ExtraLightning;
        c.ExtraChaos = a.ExtraChaos - b.ExtraChaos;
        
        return c;
    }
}

public class StatusEffectModifiers() {
    public StatusEffectStats Bleed = new();
    public StatusEffectStats Ignite = new();
    public StatusEffectStats Chill = new();
    public StatusEffectStats Shock = new();
    public StatusEffectStats Poison = new();
    public StatusEffectStats Slow = new();

    public static StatusEffectModifiers operator +(StatusEffectModifiers a, StatusEffectModifiers b) {
        StatusEffectModifiers c = new StatusEffectModifiers();

        c.Bleed = a.Bleed + b.Bleed;
        c.Ignite = a.Ignite + b.Ignite;
        c.Chill = a.Chill + b.Chill;
        c.Shock = a.Shock + b.Shock;
        c.Poison = a.Poison + b.Poison;
        c.Slow = a.Slow + b.Slow;
        
        return c;
    }

    public static StatusEffectModifiers operator -(StatusEffectModifiers a, StatusEffectModifiers b) {
        StatusEffectModifiers c = new StatusEffectModifiers();

        c.Bleed = a.Bleed - b.Bleed;
        c.Ignite = a.Ignite - b.Ignite;
        c.Chill = a.Chill - b.Chill;
        c.Shock = a.Shock - b.Shock;
        c.Poison = a.Poison - b.Poison;
        c.Slow = a.Slow - b.Slow;
        
        return c;
    }
}

public readonly struct SkillHitDamageInfo(double phys, double fire, double cold, double lightning, double chaos) {
    public readonly double Physical = phys;
    public readonly double Fire = fire;
    public readonly double Cold = cold;
    public readonly double Lightning = lightning;
    public readonly double Chaos = chaos;

    public override string ToString() {
        StringBuilder sb = new();
        
        if (Physical > 0) {
            sb.Append($"Physical: {Physical:F2}\n");
        }
        if (Fire > 0) {
            sb.Append($"Fire: {Fire:F2}\n");
        }
        if (Cold > 0) {
            sb.Append($"Cold: {Cold:F2}\n");
        }
        if (Lightning > 0) {
            sb.Append($"Lightning: {Lightning:F2}\n");
        }
        if (Chaos > 0) {
            sb.Append($"Chaos: {Chaos:F2}\n");
        }

        sb.Append($"Total: {Physical + Fire + Cold + Lightning + Chaos:F2}");
        return sb.ToString();
    }
}

public readonly struct SkillStatusInfo(List<AttachedEffect> effects) {
    public readonly List<AttachedEffect> StatusEffects = effects;
}

public readonly struct SkillInfo(SkillHitDamageInfo damageInfo, SkillStatusInfo statusInfo, EDamageInfoFlags flags) {
    public readonly SkillHitDamageInfo DamageInfo = damageInfo;
    public readonly SkillStatusInfo StatusInfo = statusInfo;
    public readonly EDamageInfoFlags InfoFlags = flags;
}

public readonly struct SkillFeedbackInfo(SkillHitDamageInfo damageInfo, EDamageFeedbackInfoFlags flags) {
    public readonly SkillHitDamageInfo DamageInfo = damageInfo;
    public readonly EDamageFeedbackInfoFlags InfoFlags = flags;
}

public readonly struct SkillDamageRangeInfo(double physMin, double physMax, double fireMin, double fireMax, double coldMin, double coldMax, double lightningMin, double lightningMax, double chaosMin, double chaosMax) {
    public readonly double PhysicalMin = physMin;
    public readonly double PhysicalMax = physMax;
    public readonly double FireMin = fireMin;
    public readonly double FireMax = fireMax;
    public readonly double ColdMin = coldMin;
    public readonly double ColdMax = coldMax;
    public readonly double LightningMin = lightningMin;
    public readonly double LightningMax = lightningMax;
    public readonly double ChaosMin = chaosMin;
    public readonly double ChaosMax = chaosMax;

    /*
    public override string ToString() {
        StringBuilder sb = new();
        
        if (Physical > 0) {
            sb.Append($"Physical: {Physical:F2}\n");
        }
        if (Fire > 0) {
            sb.Append($"Fire: {Fire:F2}\n");
        }
        if (Cold > 0) {
            sb.Append($"Cold: {Cold:F2}\n");
        }
        if (Lightning > 0) {
            sb.Append($"Lightning: {Lightning:F2}\n");
        }
        if (Chaos > 0) {
            sb.Append($"Chaos: {Chaos:F2}\n");
        }

        sb.Append($"Total: {Physical + Fire + Cold + Lightning + Chaos:F2}");
        return sb.ToString();
    }
    */
}
