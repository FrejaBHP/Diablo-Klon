using Godot;

public class ActorBasicStats {
    // ======== LIFE ========
    private int baseLife;
    public int BaseLife {
        get { return baseLife; }
        set { 
            baseLife = value;
            CalculateMaxLife();
        }
    }

    private int addedLife;
    public int AddedLife {
        get { return addedLife; }
        set { 
            addedLife = value;
            CalculateMaxLife();
        }
    }

    private double increasedLife;
    public double IncreasedLife {
        get { return increasedLife; }
        set { 
            increasedLife = value;
            CalculateMaxLife();
        }
    }

    private double moreLife;
    public double MoreLife {
        get { return moreLife; }
        set { 
            moreLife = value;
            CalculateMaxLife();
        }
    }
    
    private int totalLife;
    public int TotalLife { get => totalLife; }

    private double currentLife;
    public double CurrentLife { get => currentLife; }


    // ======== MANA ========
    public int BaseMana;
    public int AddedMana;
    public float IncreasedMana;
    public float MoreMana;
    public int TotalMana;
    
    private double currentMana;
    public double CurrentMana { get => currentMana; }


    // ======== ARMOUR ========
    private int addedArmour;
    public int AddedArmour {
        get { return addedArmour; }
        set { 
            addedArmour = value;
            CalculateArmour();
        }
    }

    private double increasedArmour;
    public double IncreasedArmour {
        get { return increasedArmour; }
        set { 
            increasedArmour = value;
            CalculateArmour();
        }
    }

    private double moreArmour;
    public double MoreArmour {
        get { return moreArmour; }
        set { 
            moreArmour = value;
            CalculateArmour();
        }
    }

    private int totalArmour;
    public int TotalArmour { get => totalArmour; }


    // ======== EVASION ========
    private int addedEvasion;
    public int AddedEvasion {
        get { return addedEvasion; }
        set { 
            addedEvasion = value;
            CalculateEvasion();
        }
    }

    private double increasedEvasion;
    public double IncreasedEvasion {
        get { return increasedEvasion; }
        set { 
            increasedEvasion = value;
            CalculateEvasion();
        }
    }

    private double moreEvasion;
    public double MoreEvasion {
        get { return moreEvasion; }
        set { 
            moreEvasion = value;
            CalculateEvasion();
        }
    }

    private int totalEvasion;
    public int TotalEvasion { get => totalEvasion; }

    public void CalculateMaxLife() {
        totalLife = (int)((baseLife + addedLife) * (1 + increasedLife) * (1 + moreLife));
    }

    public void CalculateMaxMana() {

    }

    public void CalculateArmour() {
        totalArmour = (int)(addedArmour * (1 + increasedArmour) * (1 + moreArmour));
    }

    public void CalculateEvasion() {
        totalEvasion = (int)(addedEvasion * (1 + increasedEvasion) * (1 + moreEvasion));
    }
}

public class ActorDamageModifiers {
    public int AddedPhysicalMin;
    public int AddedPhysicalMax;
    public int AddedFireMin;
    public int AddedFireMax;
    public int AddedColdMin;
    public int AddedColdMax;
    public int AddedLightningMin;
    public int AddedLightningMax;

    public double IncreasedMelee;
    public double IncreasedProjectile;
    public double IncreasedArea;
    public double IncreasedSpell;
    public double IncreasedPhysical;

    public double IncreasedFire;
    public double IncreasedCold;
    public double IncreasedLightning;

    public double MoreMelee;
    public double MoreProjectile;
    public double MoreArea;
    public double MoreSpell;

    public double MorePhysical;
    public double MoreFire;
    public double MoreCold;
    public double MoreLightning;
}

public class ActorResistances {
    public int ResPhysical;
    public int ResFire;
    public int ResCold;
    public int ResLightning;
}

public class ActorPenetrations {
    public int PenPhysical;
    public int PenFire;
    public int PenCold;
    public int PenLightning;
}

public partial class Actor : CharacterBody3D {
    protected ActorBasicStats BasicStats = new();
    protected ActorDamageModifiers DamageMods = new();
    protected ActorResistances Resistances = new();
    protected ActorPenetrations Penetrations = new();

    protected WeaponItem MainHandWeapon = null;
    protected Item OffHandItem = null;
    protected bool IsOffHandAWeapon = false;

    public void CalculateHit() {
        HitDamageInstance damageInstance = new HitDamageInstance(0f, 0f, 0f, 0f, 0f);
    }
}
