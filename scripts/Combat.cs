using System.Collections.Generic;
using System.Text;
using Godot;

public class DamageModifiers() {
    public DamageTypeStat Physical { get; protected set; } = new();
    public DamageTypeStat Fire { get; protected set; } = new();
    public DamageTypeStat Cold { get; protected set; } = new();
    public DamageTypeStat Lightning { get; protected set; } = new();
    public DamageTypeStat Chaos { get; protected set; } = new();

    public DamageConversionTable Conversion { get; protected set; } = new();

    public double IncreasedAttack = 0;
    public double IncreasedSpell = 0;
    public double IncreasedMelee = 0;
    public double IncreasedProjectile = 0;
    public double IncreasedArea = 0;
    public double IncreasedDoT = 0;
    public double IncreasedAll = 0;

    public double IncreasedLowLife = 0;

    public double IncreasedBleedMagnitude = 0;
    public double IncreasedIgniteMagnitude = 0;
    public double IncreasedPoisonMagnitude = 0;
    public double IncreasedStatusDamageWithCrit = 0;

    public double MoreAttack = 1;
    public double MoreSpell = 1;
    public double MoreMelee = 1;
    public double MoreProjectile = 1;
    public double MoreArea = 1;
    public double MoreDoT = 1;
    public double MoreAll = 1;

    public double MoreLowLife = 1;

    public double MoreBleedMagnitude = 1;
    public double MoreIgniteMagnitude = 1;
    public double MorePoisonMagnitude = 1;

    public double HitDamageMultiplier = 1;

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

    public static SkillDamageRangeInfo ConvertSkillDamage(DamageModifiers damageMods, SkillDamageRangeInfo rangeInfo) {
        return DamageConversionStep(damageMods, DamageConversionStep(damageMods, rangeInfo, true), false);
    }

    private static SkillDamageRangeInfo DamageConversionStep(DamageModifiers damageMods, SkillDamageRangeInfo rangeInfo, bool useBase) {
        double[] dmgVal = new double[10];
        /* Equivalent to: 
        double stepPhysMin = 0;
        double stepPhysMax = 0;
        double stepFireMin = 0;
        double stepFireMax = 0;
        double stepColdMin = 0;
        double stepColdMax = 0;
        double stepLightningMin = 0;
        double stepLightningMax = 0;
        double stepChaosMin = 0;
        double stepChaosMax = 0;
        */

        if (rangeInfo.PhysicalMin != 0 && rangeInfo.PhysicalMax != 0) {
            if (damageMods.Conversion.Physical.GetTotalConversionPercentage(useBase) != 0) {
                double[] convertedValues = damageMods.Conversion.Physical.GetConvertedValues(rangeInfo.PhysicalMin, rangeInfo.PhysicalMax, useBase);
                for (int i = 0; i < dmgVal.Length; i++) {
                    dmgVal[i] += convertedValues[i];
                }
            }
            else {
                dmgVal[0] += rangeInfo.PhysicalMin;
                dmgVal[1] += rangeInfo.PhysicalMax;
            }
        }
        
        if (rangeInfo.FireMin != 0 && rangeInfo.FireMax != 0) {
            if (damageMods.Conversion.Fire.GetTotalConversionPercentage(useBase) != 0) {
                double[] convertedValues = damageMods.Conversion.Fire.GetConvertedValues(rangeInfo.FireMin, rangeInfo.FireMax, useBase);
                for (int i = 0; i < dmgVal.Length; i++) {
                    dmgVal[i] += convertedValues[i];
                }
            }
            else {
                dmgVal[2] += rangeInfo.FireMin;
                dmgVal[3] += rangeInfo.FireMax;
            }
        }

        if (rangeInfo.ColdMin != 0 && rangeInfo.ColdMax != 0) {
            if (damageMods.Conversion.Cold.GetTotalConversionPercentage(useBase) != 0) {
                double[] convertedValues = damageMods.Conversion.Cold.GetConvertedValues(rangeInfo.ColdMin, rangeInfo.ColdMax, useBase);
                for (int i = 0; i < dmgVal.Length; i++) {
                    dmgVal[i] += convertedValues[i];
                }
            }
            else {
                dmgVal[4] += rangeInfo.ColdMin;
                dmgVal[5] += rangeInfo.ColdMax;
            }
        }

        if (rangeInfo.LightningMin != 0 && rangeInfo.LightningMax != 0) {
            if (damageMods.Conversion.Lightning.GetTotalConversionPercentage(useBase) != 0) {
                double[] convertedValues = damageMods.Conversion.Lightning.GetConvertedValues(rangeInfo.LightningMin, rangeInfo.LightningMax, useBase);
                for (int i = 0; i < dmgVal.Length; i++) {
                    dmgVal[i] += convertedValues[i];
                }
            }
            else {
                dmgVal[6] += rangeInfo.LightningMin;
                dmgVal[7] += rangeInfo.LightningMax;
            }
        }

        if (rangeInfo.ChaosMin != 0 && rangeInfo.ChaosMax != 0) {
            if (damageMods.Conversion.Chaos.GetTotalConversionPercentage(useBase) != 0) {
                double[] convertedValues = damageMods.Conversion.Chaos.GetConvertedValues(rangeInfo.ChaosMin, rangeInfo.ChaosMax, useBase);
                for (int i = 0; i < dmgVal.Length; i++) {
                    dmgVal[i] += convertedValues[i];
                }
            }
            else {
                dmgVal[8] += rangeInfo.ChaosMin;
                dmgVal[9] += rangeInfo.ChaosMax;
            }
        }

        return new SkillDamageRangeInfo(dmgVal[0], dmgVal[1], dmgVal[2], dmgVal[3], dmgVal[4], dmgVal[5], dmgVal[6], dmgVal[7], dmgVal[8], dmgVal[9]);
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

        SkillDamageRangeInfo convDmgInfo = ConvertSkillDamage(damageMods, new SkillDamageRangeInfo(physMin, physMax, fireMin, fireMax, coldMin, coldMax, lightningMin, lightningMax, chaosMin, chaosMax));

        physMin = convDmgInfo.PhysicalMin;
        physMax = convDmgInfo.PhysicalMax;
        fireMin = convDmgInfo.FireMin;
        fireMax = convDmgInfo.FireMax;
        coldMin = convDmgInfo.ColdMin;
        coldMax = convDmgInfo.ColdMax;
        lightningMin = convDmgInfo.LightningMin;
        lightningMax = convDmgInfo.LightningMax;
        chaosMin = convDmgInfo.ChaosMin;
        chaosMax = convDmgInfo.ChaosMax;

        double combinedMinDamage = convDmgInfo.PhysicalMin + convDmgInfo.FireMin + convDmgInfo.ColdMin + convDmgInfo.LightningMin + convDmgInfo.ChaosMin;
        double combinedMaxDamage = convDmgInfo.PhysicalMax + convDmgInfo.FireMax + convDmgInfo.ColdMax + convDmgInfo.LightningMax + convDmgInfo.ChaosMax;

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

    public static SkillDamageRangeInfo ApplyMultipliersToDamageRangeInfo(SkillDamageRangeInfo baseDamageInfo, DamageModifiers damageMods, ESkillDamageTags damageTags, double extraIncreased) {
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

        increasedMultiplier += damageMods.IncreasedAll + extraIncreased;
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

    public static SkillDamageRangeInfo CalculateAttackSkillDamageRange(DamageModifiers damageMods, ActorWeaponStats weaponStats, double addedMultiplier, ESkillDamageTags damageTags, double extraIncreased) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseAttackDamage(damageMods, weaponStats, addedMultiplier), damageMods, damageTags, extraIncreased);
    }

    public static SkillDamageRangeInfo CalculateAttackSkillDamageRangeWithHitMulti(DamageModifiers damageMods, ActorWeaponStats weaponStats, double addedMultiplier, ESkillDamageTags damageTags, double extraIncreased) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseAttackDamage(damageMods, weaponStats, addedMultiplier), damageMods, damageTags, extraIncreased) * damageMods.HitDamageMultiplier;
    }

    public static SkillDamageRangeInfo CalculateSpellSkillDamageRange(DamageModifiers damageMods, double addedMultiplier, ESkillDamageTags damageTags, double extraIncreased) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseSpellDamage(damageMods, addedMultiplier), damageMods, damageTags, extraIncreased);
    }

    public static SkillDamageRangeInfo CalculateSpellSkillDamageRangeWithHitMulti(DamageModifiers damageMods, double addedMultiplier, ESkillDamageTags damageTags, double extraIncreased) {
        return ApplyMultipliersToDamageRangeInfo(CalculateBaseSpellDamage(damageMods, addedMultiplier), damageMods, damageTags, extraIncreased) * damageMods.HitDamageMultiplier;
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
        copy.Conversion = Conversion.ShallowCopy();
        copy.Physical = Physical.ShallowCopy();
        copy.Fire = Fire.ShallowCopy();
        copy.Cold = Cold.ShallowCopy();
        copy.Lightning = Lightning.ShallowCopy();
        copy.Chaos = Chaos.ShallowCopy();

        return copy;
    }

    public static DamageModifiers operator +(DamageModifiers a, DamageModifiers b) {
        DamageModifiers c = new DamageModifiers();
        c.Conversion = a.Conversion + b.Conversion;

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

        c.IncreasedLowLife = a.IncreasedLowLife + b.IncreasedLowLife;
        c.IncreasedStatusDamageWithCrit = a.IncreasedStatusDamageWithCrit + b.IncreasedStatusDamageWithCrit;

        c.IncreasedBleedMagnitude = a.IncreasedBleedMagnitude + b.IncreasedBleedMagnitude;
        c.IncreasedIgniteMagnitude = a.IncreasedIgniteMagnitude + b.IncreasedIgniteMagnitude;
        c.IncreasedPoisonMagnitude = a.IncreasedPoisonMagnitude + b.IncreasedPoisonMagnitude;

        c.MoreAttack = a.MoreAttack * b.MoreAttack;
        c.MoreSpell = a.MoreSpell * b.MoreSpell;
        c.MoreMelee = a.MoreMelee * b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile * b.MoreProjectile;
        c.MoreArea = a.MoreArea * b.MoreArea;
        c.MoreDoT = a.MoreDoT * b.MoreDoT;
        c.MoreAll = a.MoreAll * b.MoreAll;

        c.MoreLowLife = a.MoreLowLife * b.MoreLowLife;

        c.MoreBleedMagnitude = a.MoreBleedMagnitude * b.MoreBleedMagnitude;
        c.MoreIgniteMagnitude = a.MoreIgniteMagnitude * b.MoreIgniteMagnitude;
        c.MorePoisonMagnitude = a.MorePoisonMagnitude * b.MorePoisonMagnitude;

        c.HitDamageMultiplier = a.HitDamageMultiplier * b.HitDamageMultiplier;

        c.ExtraPhysical = a.ExtraPhysical + b.ExtraPhysical;
        c.ExtraFire = a.ExtraFire + b.ExtraFire;
        c.ExtraCold = a.ExtraCold + b.ExtraCold;
        c.ExtraLightning = a.ExtraLightning + b.ExtraLightning;
        c.ExtraChaos = a.ExtraChaos + b.ExtraChaos;
        
        return c;
    }

    public static DamageModifiers operator -(DamageModifiers a, DamageModifiers b) {
        DamageModifiers c = new DamageModifiers();
        c.Conversion = a.Conversion - b.Conversion;

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

        c.IncreasedLowLife = a.IncreasedLowLife - b.IncreasedLowLife;
        c.IncreasedStatusDamageWithCrit = a.IncreasedStatusDamageWithCrit - b.IncreasedStatusDamageWithCrit;

        c.IncreasedBleedMagnitude = a.IncreasedBleedMagnitude - b.IncreasedBleedMagnitude;
        c.IncreasedIgniteMagnitude = a.IncreasedIgniteMagnitude - b.IncreasedIgniteMagnitude;
        c.IncreasedPoisonMagnitude = a.IncreasedPoisonMagnitude - b.IncreasedPoisonMagnitude;

        c.MoreAttack = a.MoreAttack / b.MoreAttack;
        c.MoreSpell = a.MoreSpell / b.MoreSpell;
        c.MoreMelee = a.MoreMelee / b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile / b.MoreProjectile;
        c.MoreArea = a.MoreArea / b.MoreArea;
        c.MoreDoT = a.MoreDoT / b.MoreDoT;
        c.MoreAll = a.MoreAll / b.MoreAll;

        c.MoreLowLife = a.MoreLowLife / b.MoreLowLife;

        c.MoreBleedMagnitude = a.MoreBleedMagnitude / b.MoreBleedMagnitude;
        c.MoreIgniteMagnitude = a.MoreIgniteMagnitude / b.MoreIgniteMagnitude;
        c.MorePoisonMagnitude = a.MorePoisonMagnitude / b.MorePoisonMagnitude;

        c.HitDamageMultiplier = a.HitDamageMultiplier / b.HitDamageMultiplier;

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

public struct DamageRange {
    public double Minimum;
    public double Maximum;

    public DamageRange() {
        Minimum = 0;
        Maximum = 0;
    }

    public DamageRange(double min, double max) {
        Minimum = min;
        Maximum = max;
    }

    public static DamageRange operator *(DamageRange range, double mult) {
        return new DamageRange(range.Minimum * mult, range.Maximum * mult);
    }
}

public struct ConversionElement {
    public EDamageType DamageType { get; private set; }
    public ConversionMod ToPhysical = new(EDamageType.Physical, 0, 0);
    public ConversionMod ToFire = new(EDamageType.Fire, 0, 0);
    public ConversionMod ToCold = new(EDamageType.Cold, 0, 0);
    public ConversionMod ToLightning = new(EDamageType.Lightning, 0, 0);
    public ConversionMod ToChaos = new(EDamageType.Chaos, 0, 0);

    public ConversionElement(EDamageType dmgType) {
        DamageType = dmgType;
    }

    public readonly double GetTotalConversionPercentage(bool getBase) {
        double total = 0;

        int index = 0;
        if (!getBase) {
            index = 1;
        }

        ConversionMod[] elements = [
            ToPhysical,
            ToFire,
            ToCold,
            ToLightning,
            ToChaos
        ];

        foreach (ConversionMod element in elements) {
            if (DamageType != element.DamageType) {
                total += element.Values[index];
            }
        }
        
        return total;
    }

    public readonly double[] GetFinalConversionPercentages(bool useBase) {
        double[] percentages = new double[5];
        double conversionPercentage = 0;

        int index = 0;
        if (!useBase) {
            index = 1;
        }

        ConversionMod[] elements = [
            ToPhysical,
            ToFire,
            ToCold,
            ToLightning,
            ToChaos
        ];

        foreach (ConversionMod element in elements) {
            if (DamageType != element.DamageType) {
                conversionPercentage += element.Values[index];
            }
        }

        for (int i = 0; i < elements.Length; i++) {
            if (DamageType != elements[i].DamageType) {
                if (conversionPercentage > 1) {
                    percentages[i] = elements[i].Values[index] / conversionPercentage;
                }
                else {
                    percentages[i] = elements[i].Values[index];
                }
            }
            else {
                if (conversionPercentage < 1) {
                    percentages[i] = 1 - conversionPercentage;
                }
                else {
                    percentages[i] = 0;
                }
            }
        }

        //GD.Print($"Element: {DamageType}, Conversion: {percentages[0]:P0}/{percentages[1]:P0}/{percentages[2]:P0}/{percentages[3]:P0}/{percentages[4]:P0}");
        
        return percentages;
    }

    public readonly double[] GetConvertedValues(double minDamage, double maxDamage, bool useBase) {
        double[] conversionPercentages = GetFinalConversionPercentages(useBase);
        double[] damageValues = new double[10];

        int j = 0;
        for (int i = 0; i < conversionPercentages.Length; i++) {
            damageValues[j] = minDamage * conversionPercentages[i];
            damageValues[j + 1] = maxDamage * conversionPercentages[i];

            j += 2;
        }

        return damageValues;
    }

    public static ConversionElement operator +(ConversionElement a, ConversionElement b) {
        ConversionElement c = new ConversionElement();

        c.DamageType = a.DamageType;
        c.ToPhysical = a.ToPhysical + b.ToPhysical;
        c.ToFire = a.ToFire + b.ToFire;
        c.ToCold = a.ToCold + b.ToCold;
        c.ToLightning = a.ToLightning + b.ToLightning;
        c.ToChaos = a.ToChaos + b.ToChaos;

        return c;
    }

    public static ConversionElement operator -(ConversionElement a, ConversionElement b) {
        ConversionElement c = new ConversionElement();
        
        c.DamageType = a.DamageType;
        c.ToPhysical = a.ToPhysical - b.ToPhysical;
        c.ToFire = a.ToFire - b.ToFire;
        c.ToCold = a.ToCold - b.ToCold;
        c.ToLightning = a.ToLightning - b.ToLightning;
        c.ToChaos = a.ToChaos - b.ToChaos;

        return c;
    }
}

public struct ConversionMod {
    public EDamageType DamageType { get; private set; }
    public double[] Values { get; private set; } = new double[2];

    public ConversionMod(EDamageType dmgType) {
        DamageType = dmgType;
        Values[0] = 0;
        Values[1] = 0;
    }
    
    public ConversionMod(EDamageType dmgType, double basec, double addedc) {
       DamageType = dmgType;
       Values[0] = basec;
       Values[1] = addedc;
    }

    public static ConversionMod operator +(ConversionMod a, ConversionMod b) {
        return new ConversionMod(a.DamageType, a.Values[0] + b.Values[0], a.Values[1] + b.Values[1]);
    }

    public static ConversionMod operator -(ConversionMod a, ConversionMod b) {
        return new ConversionMod(a.DamageType, a.Values[0] - b.Values[0], a.Values[1] - b.Values[1]);
    }
}

public class DamageConversionTable() {
    public ConversionElement Physical = new(EDamageType.Physical);
    public ConversionElement Fire = new(EDamageType.Fire);
    public ConversionElement Cold = new(EDamageType.Cold);
    public ConversionElement Lightning = new(EDamageType.Lightning);
    public ConversionElement Chaos = new(EDamageType.Chaos);

    public DamageConversionTable ShallowCopy() {
        return (DamageConversionTable)MemberwiseClone();
    }

    public static DamageConversionTable operator +(DamageConversionTable a, DamageConversionTable b) {
        DamageConversionTable c = new DamageConversionTable();

        c.Physical = a.Physical + b.Physical;
        c.Fire = a.Fire + b.Fire;
        c.Cold = a.Cold + b.Cold;
        c.Lightning = a.Lightning + b.Lightning;
        c.Chaos = a.Chaos + b.Chaos;

        return c;
    }

    public static DamageConversionTable operator -(DamageConversionTable a, DamageConversionTable b) {
        DamageConversionTable c = new DamageConversionTable();

        c.Physical = a.Physical - b.Physical;
        c.Fire = a.Fire - b.Fire;
        c.Cold = a.Cold - b.Cold;
        c.Lightning = a.Lightning - b.Lightning;
        c.Chaos = a.Chaos - b.Chaos;

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

    public static SkillDamageRangeInfo operator *(SkillDamageRangeInfo info, double multiplier) {
        return new SkillDamageRangeInfo(
            info.PhysicalMin * multiplier,
            info.PhysicalMax * multiplier,
            info.FireMin * multiplier,
            info.FireMax * multiplier,
            info.ColdMin * multiplier,
            info.ColdMax * multiplier,
            info.LightningMin * multiplier,
            info.LightningMax * multiplier,
            info.ChaosMin * multiplier,
            info.ChaosMax * multiplier
        );
    }
}
