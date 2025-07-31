using System.Collections.Generic;
using System.Text;

public class DamageModifiers() {
    public DamageStat Physical { get; protected set; } = new();
    public DamageStat Fire { get; protected set; } = new();
    public DamageStat Cold { get; protected set; } = new();
    public DamageStat Lightning { get; protected set; } = new();
    public DamageStat Chaos { get; protected set; } = new();

    public double IncreasedMelee = 0;
    public double IncreasedRanged = 0;
    public double IncreasedSpell = 0;
    public double IncreasedAll = 0;

    public double MoreMelee = 1;
    public double MoreRanged = 1;
    public double MoreSpell = 1;
    public double MoreAll = 1;

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

        c.IncreasedMelee = a.IncreasedMelee + b.IncreasedMelee;
        c.IncreasedRanged = a.IncreasedRanged + b.IncreasedRanged;
        c.IncreasedSpell = a.IncreasedSpell + b.IncreasedSpell;
        c.IncreasedAll = a.IncreasedAll + b.IncreasedAll;

        c.MoreMelee = a.MoreMelee * b.MoreMelee;
        c.MoreRanged = a.MoreRanged * b.MoreRanged;
        c.MoreSpell = a.MoreSpell * b.MoreSpell;
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

        c.IncreasedMelee = a.IncreasedMelee - b.IncreasedMelee;
        c.IncreasedRanged = a.IncreasedRanged - b.IncreasedRanged;
        c.IncreasedSpell = a.IncreasedSpell - b.IncreasedSpell;
        c.IncreasedAll = a.IncreasedAll - b.IncreasedAll;

        c.MoreMelee = a.MoreMelee / b.MoreMelee;
        c.MoreRanged = a.MoreRanged / b.MoreRanged;
        c.MoreSpell = a.MoreSpell / b.MoreSpell;
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
