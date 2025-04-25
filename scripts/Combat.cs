public enum DamageType {
    Untyped,
    Physical,   // DoT: Bleed/Bleeding
    Fire,       // DoT: Ignite - Burn/Burning
    Cold,       // DoT: Frostburn/Frostburning
    Lightning,  // DoT: Electrocute/Electrocuted
    Chaos       // DoT: Poison
}

public class SkillDamage(double phys, double fire, double cold, double lightning, double chaos) {
    public readonly double Physical = phys;
    public readonly double Fire = fire;
    public readonly double Cold = cold;
    public readonly double Lightning = lightning;
    public readonly double Chaos = chaos;
}

public class HitDamageInstance(float phys, float fire, float cold, float lightning, float chaos) {
    public readonly float DamagePhysical = phys;
    public readonly float DamageFire = fire;
    public readonly float DamageCold = cold;
    public readonly float DamageLightning = lightning;
    public readonly float DamageChaos = chaos;
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
