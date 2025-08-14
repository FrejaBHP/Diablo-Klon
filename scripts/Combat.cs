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
    public double IncreasedBleed = 0;
    public double IncreasedIgnite = 0;
    public double IncreasedPoison = 0;
    public double IncreasedAll = 0;

    public double MoreAttack = 1;
    public double MoreSpell = 1;
    public double MoreMelee = 1;
    public double MoreProjectile = 1;
    public double MoreArea = 1;
    public double MoreDoT = 1;
    public double MoreBleed = 1;
    public double MoreIgnite = 1;
    public double MorePoison = 1;
    public double MoreAll = 1;

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

    public static void CalculatePreAttackDamageWithType(DamageTypeStat damageStat, double baseMin, double baseMax, double addedMultiplier, out double minDamage, out double maxDamage) {
        minDamage = (baseMin + damageStat.SMinAdded + damageStat.SAttackMinAdded) * addedMultiplier;
        maxDamage = (baseMax + damageStat.SMaxAdded + damageStat.SAttackMaxAdded) * addedMultiplier;
    }

    public static void CalculatePreSpellDamageWithType(DamageTypeStat damageStat, double addedMultiplier, out double minDamage, out double maxDamage) {
        minDamage = damageStat.SMinBase + ((damageStat.SMinAdded + damageStat.SSpellMinAdded) * addedMultiplier);
        maxDamage = damageStat.SMaxBase + ((damageStat.SMaxAdded + damageStat.SSpellMaxAdded) * addedMultiplier);
    }

    public static double RollPreDamage(double preMin, double preMax) {
        return Utilities.RandomDouble(preMin, preMax);
    }

    public void CalculateMultipliers(ESkillDamageTags damageTags, out double increasedMultiplier, out double moreMultiplier) {
        increasedMultiplier = 0;
        moreMultiplier = 1;

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
        if (damageTags.HasFlag(ESkillDamageTags.Bleed)) {
            increasedMultiplier += IncreasedBleed;
            moreMultiplier *= MoreBleed;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Burn)) {
            increasedMultiplier += IncreasedIgnite;
            moreMultiplier *= MoreIgnite;
        }
        if (damageTags.HasFlag(ESkillDamageTags.Poison)) {
            increasedMultiplier += IncreasedPoison;
            moreMultiplier *= MorePoison;
        }

        increasedMultiplier += IncreasedAll;
        moreMultiplier *= MoreAll;
    }

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

    public double GetTotalDamageWithType(DamageTypeStat damageStat, ESkillDamageTags damageTags, double preDamage) {
        CalculateMultipliersWithType(damageStat, damageTags, out double increasedMultiplier, out double moreMultiplier);

        double totalDamage = preDamage * (1 + increasedMultiplier) * moreMultiplier;

        if (totalDamage < 0) {
            totalDamage = 0;
        }

        return totalDamage;
    }

    public void CalculateTotalAttackDamageWithType(DamageTypeStat damageStat, ESkillDamageTags damageTags, double baseMin, double baseMax, double addedMultiplier, out double totalMin, out double totalMax) {
        CalculatePreAttackDamageWithType(damageStat, baseMin, baseMax, addedMultiplier, out double preMin, out double preMax);
        CalculateMultipliersWithType(damageStat, damageTags, out double increasedMultiplier, out double moreMultiplier);

        totalMin = preMin * (1 + increasedMultiplier) * moreMultiplier;
        totalMax = preMax * (1 + increasedMultiplier) * moreMultiplier;
    }

    public void CalculateTotalSpellDamageWithType(DamageTypeStat damageStat, ESkillDamageTags damageTags, double addedMultiplier, out double totalMin, out double totalMax) {
        CalculatePreSpellDamageWithType(damageStat, addedMultiplier, out double preMin, out double preMax);
        CalculateMultipliersWithType(damageStat, damageTags, out double increasedMultiplier, out double moreMultiplier);

        totalMin = preMin * (1 + increasedMultiplier) * moreMultiplier;
        totalMax = preMax * (1 + increasedMultiplier) * moreMultiplier;
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

        c.MoreAttack = a.MoreAttack * b.MoreAttack;
        c.MoreSpell = a.MoreSpell * b.MoreSpell;
        c.MoreMelee = a.MoreMelee * b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile * b.MoreProjectile;
        c.MoreArea = a.MoreArea * b.MoreArea;
        c.MoreDoT = a.MoreDoT * b.MoreDoT;
        c.MoreAll = a.MoreAll * b.MoreAll;
        
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

        c.MoreAttack = a.MoreAttack / b.MoreAttack;
        c.MoreSpell = a.MoreSpell / b.MoreSpell;
        c.MoreMelee = a.MoreMelee / b.MoreMelee;
        c.MoreProjectile = a.MoreProjectile / b.MoreProjectile;
        c.MoreArea = a.MoreArea / b.MoreArea;
        c.MoreDoT = a.MoreDoT / b.MoreDoT;
        c.MoreAll = a.MoreAll / b.MoreAll;
        
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
