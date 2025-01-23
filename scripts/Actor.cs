using Godot;

public class ActorBasicStats {
    public int BaseLife;
    public int AddedLife;
    public float ModAddLife;
    public float ModMultLife;
    public int TotalLife;

    public int BaseMana;
    public int AddedMana;
    public float ModAddMana;
    public float ModMultMana;
    public int TotalMana;

    public int AddedArmour;
    public float ModAddArmour;
    public float ModMultArmour;
    public int TotalArmour;

    public int AddedEvasion;
    public float ModAddEvasion;
    public float ModMultEvasion;
    public int TotalEvasion;
}

public class ActorDamageModifiers {
    public float ModAddMelee;
    public float ModAddProjectile;
    public float ModAddArea;
    public float ModAddSpell;

    public float ModAddPhysical;
    public float ModAddFire;
    public float ModAddCold;
    public float ModAddLightning;
    public float ModAddCorrosive;
    public float ModAddHoly;
    public float ModAddShadow;

    public float ModMultMelee;
    public float ModMultProjectile;
    public float ModMultArea;
    public float ModMultSpell;

    public float ModMultPhysical;
    public float ModMultFire;
    public float ModMultCold;
    public float ModMultLightning;
    public float ModMultCorrosive;
    public float ModMultHoly;
    public float ModMultShadow;
}

public class ActorResistances {
    public short ResPhysical;
    public short ResFire;
    public short ResCold;
    public short ResLightning;
    public short ResCorrosive;
    public short ResHoly;
    public short ResShadow;
}

public class ActorPenetrations {
    public short PenPhysical;
    public short PenFire;
    public short PenCold;
    public short PenLightning;
    public short PenCorrosive;
    public short PenHoly;
    public short PenShadow;
}

public partial class Actor : CharacterBody3D {
    protected ActorBasicStats stats = new();
    protected ActorDamageModifiers dmgMods = new();
    protected ActorResistances resists = new();
    protected ActorPenetrations penetrations = new();

    

    public void Hey() {
        
    }

    public void CalculateHit() {
        HitDamageInstance damageInstance = new HitDamageInstance(0f, 0f, 0f, 0f, 0f, 0f, 0f);
    }
}
