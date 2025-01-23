public enum DamageType {
    Untyped,
    Physical,   // DoT: Bleed/Bleeding
    Fire,       // DoT: Ignite - Burn/Burning
    Cold,       // DoT: Frostburn/Frostburning
    Lightning,  // DoT: Electrocute/Electrocuted
    Corrosive,  // DoT: Poison/Poisoned
    Holy,       // DoT: Soulfire/Soulfired
    Shadow      // DoT: Agony/Agonised
}

public class HitDamageInstance {
    public readonly float DamagePhysical;
    public readonly float DamageFire;
    public readonly float DamageCold;
    public readonly float DamageLightning;
    public readonly float DamageCorrosive;
    public readonly float DamageHoly;
    public readonly float DamageShadow;

    public HitDamageInstance(float phys, float fire, float cold, float lightning, float corrosive, float holy, float shadow) {
        DamagePhysical = phys;
        DamageFire = fire;
        DamageCold = cold;
        DamageLightning = lightning;
        DamageCorrosive = corrosive;
        DamageHoly = holy;
        DamageShadow = shadow;
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
    public static DamageType Type { get; } = DamageType.Corrosive;
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
