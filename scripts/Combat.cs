using System.Text;

public enum DamageType {
    Untyped,
    Physical,   // DoT: Bleed/Bleeding
    Fire,       // DoT: Ignite - Burn/Burning
    Cold,       // DoT: Frostburn/Frostburning
    Lightning,  // DoT: Electrocute/Electrocuted
    Chaos       // DoT: Poison
}

public class SkillDamage(double phys, double fire, double cold, double lightning, double chaos, bool isCritical) {
    public readonly double Physical = phys;
    public readonly double Fire = fire;
    public readonly double Cold = cold;
    public readonly double Lightning = lightning;
    public readonly double Chaos = chaos;
    public readonly bool IsCritical = isCritical;

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

        sb.Append($"Total: {Physical + Fire + Cold + Lightning + Chaos:F2}\nCritical: {IsCritical}");
        return sb.ToString();
    }
}

interface IDoT {
    static DamageType Type { get; }
    static float BaseLength { get; }
    float TotalLength { get; }
    float TotalDamage { get; }

    void ModifyInstance(float lengthRed);
}

public class PoisonInstance : IDoT {
    public static DamageType Type { get; } = DamageType.Chaos;
    public static float BaseLength { get; } = 2f;
    public float TotalLength { get; private set; }
    public float TotalDamage { get; private set; }
    
    public PoisonInstance(float damage, float lengthMod) {
        TotalLength = BaseLength * lengthMod;
        TotalDamage = damage;
    }

    public void ModifyInstance(float lengthRed) {
        TotalLength *= lengthRed;
    }
}

public class BleedInstance : IDoT {
    public static DamageType Type { get; } = DamageType.Physical;
    public static float BaseLength { get; } = 4f;
    public float TotalLength { get; private set; }
    public float TotalDamage { get; private set; }
    
    public BleedInstance(float damage, float lengthMod) {
        TotalLength = BaseLength * lengthMod;
        TotalDamage = damage;
    }

    public void ModifyInstance(float lengthRed) {
        TotalLength *= lengthRed;
    }
}
