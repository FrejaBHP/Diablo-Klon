using System.Collections.Generic;
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

    public delegate void CurrentLifeChangedEventHandler(object sender, double newCurrentLife);
    public event CurrentLifeChangedEventHandler CurrentLifeChanged;

    private double currentLife;
    public double CurrentLife {
        get => currentLife;
        set {
            if (value >= totalLife) {
                currentLife = totalLife;
            }
            else {
                currentLife = value;
            }

            CurrentLifeChanged?.Invoke(this, currentLife);
        }
    }

    private double addedLifeRegen;
    public double AddedLifeRegen { 
        get => addedLifeRegen; 
        set {
            addedLifeRegen = value;
            CalculateLifeRegen();
        }
    }

    private double percentageLifeRegen;
    public double PercentageLifeRegen {
        get => percentageLifeRegen;
        set {
            percentageLifeRegen = value;
            CalculateLifeRegen();
        }
    }

    private double increasedLifeRegen;
    public double IncreasedLifeRegen { 
        get => increasedLifeRegen; 
        set {
            increasedLifeRegen = value;
            CalculateLifeRegen();
        }
    }

    private double totalLifeRegen;
    public double TotalLifeRegen { get => totalLifeRegen; }


    // ======== MANA ========
    private int baseMana;
    public int BaseMana {
        get => baseMana;
        set {
            baseMana = value;
            CalculateMaxMana();
        }
    }

    private int addedMana;
    public int AddedMana {
        get => addedMana;
        set {
            addedMana = value;
            CalculateMaxMana();
        }
    }

    private double increasedMana;
    public double IncreasedMana {
        get => increasedMana;
        set {
            increasedMana = value;
            CalculateMaxMana();
        }
    }

    private double moreMana;
    public double MoreMana {
        get => moreMana;
        set {
            moreMana = value;
            CalculateMaxMana();
        }
    }

    private int totalMana;
    public int TotalMana { get => totalMana; }
    
    private double currentMana;
    public double CurrentMana {
        get => currentMana;
        set {
            if (value >= totalMana) {
                currentMana = totalMana;
            }
            else {
                currentMana = value;
            }
        }
    }

    private double addedManaRegen;
    public double AddedManaRegen { 
        get => addedManaRegen; 
        set {
            addedManaRegen = value;
            CalculateManaRegen();
        }
    }

    private double increasedManaRegen;
    public double IncreasedManaRegen { 
        get => increasedManaRegen; 
        set {
            increasedManaRegen = value;
            CalculateManaRegen();
        }
    }

    private double totalManaRegen;
    public double TotalManaRegen { get => totalManaRegen; }


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


    // ======== CALC ========

    public void CalculateMaxLife() {
        int oldTotalLife = totalLife;
        totalLife = (int)((baseLife + addedLife) * (1 + increasedLife) * (1 + moreLife));

        if (oldTotalLife != 0) {
            AdjustCurrentLife(oldTotalLife);
        }

        CalculateLifeRegen();
    }

    public void AdjustCurrentLife(int oldTotalLife) {
        double percentageCurrentLife = currentLife / oldTotalLife;
        int changeInLife = totalLife - oldTotalLife;

        //GD.Print($"%: {percentageCurrentLife}, Change: {changeInLife}");

        if (changeInLife > 0) {
            currentLife += changeInLife * percentageCurrentLife;
            //GD.Print($"Positiv Ændring, +{(double)(changeInLife * percentageCurrentLife)}");
        }
        else if (changeInLife < 0 && currentLife > totalLife) {
            currentLife = totalLife;
            //GD.Print("Life sat til fuld");
        }
    }

    public void AdjustCurrentMana(int oldTotalMana) {
        double percentageCurrentMana = currentMana / oldTotalMana;
        int changeInMana = totalMana - oldTotalMana;

        //GD.Print($"%: {percentageCurrentMana}, Change: {changeInMana}");

        if (changeInMana > 0) {
            currentMana += changeInMana * percentageCurrentMana;
            //GD.Print($"Positiv Ændring, +{(double)(changeInMana * percentageCurrentMana)}");
        }
        else if (changeInMana < 0 && currentMana > totalMana) {
            currentMana = totalMana;
            //GD.Print("Mana sat til fuld");
        }
    }

    public void CalculateLifeRegen() {
        totalLifeRegen = (totalLife * percentageLifeRegen) + addedLifeRegen * (1 + increasedLifeRegen);
    }

    public void CalculateMaxMana() {
        int oldTotalMana = totalMana;
        totalMana = (int)((baseMana + addedMana) * (1 + increasedMana) * (1 + moreMana));

        if (oldTotalMana != 0) {
            AdjustCurrentMana(oldTotalMana);
        }

        CalculateManaRegen();
    }

    public void CalculateManaRegen() {
        totalManaRegen = ((totalMana * 0.02) + addedManaRegen) * (1 + increasedManaRegen);
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

public class ActorMainHand {
    public WeaponItem Weapon = null;

    public int PhysMinDamage;
    public int PhysMaxDamage;
    public int FireMinDamage;
    public int FireMaxDamage;
    public int ColdMinDamage;
    public int ColdMaxDamage;
    public int LightningMinDamage;
    public int LightningMaxDamage;
    public int ChaosMinDamage;
    public int ChaosMaxDamage;

    public double Range;
    public double AttackSpeed;
    public double CritChance;
}

public partial class Actor : CharacterBody3D {
    protected PackedScene floatingResourceBarsScene = GD.Load<PackedScene>("res://scenes/gui/actor_floating_resource_bars.tscn");

    protected int ticksPerSecond = ProjectSettings.GetSetting("physics/common/physics_ticks_per_second").AsInt32();

    public List<Skill> Skills = new List<Skill>();

    public ActorBasicStats BasicStats = new();
    public ActorDamageModifiers DamageMods = new();
    public ActorResistances Resistances = new();
    public ActorPenetrations Penetrations = new();

    public EActorState ActorState = EActorState.Actionable;

    public Stat AttackSpeedMod = new(1, false);
    public Stat CritChanceMod = new(1, false);
    public Stat CastSpeedMod = new(1, false);

    public float OutgoingEffectAttachmentHeight { get; protected set; } = 1f;

    protected FloatingResourceBars fResBars;

    protected ActorMainHand MainHand = new();
    protected Item OffHandItem = null;
    protected bool IsOffHandAWeapon = false;

    public override void _Ready() {
        BasicStats.CurrentLifeChanged += OnCurrentLifeChanged;
    }

    protected void AddFloatingBars(Node3D anchor) {
        fResBars = floatingResourceBarsScene.Instantiate<FloatingResourceBars>();
        anchor.AddChild(fResBars);

        fResBars.SetLifePercentage(BasicStats.CurrentLife / BasicStats.TotalLife);

        if (BasicStats.TotalMana != 0) {
            fResBars.SetManaPercentage(BasicStats.CurrentMana / BasicStats.TotalMana);
        }
        else {
            fResBars.SetManaPercentage(0);
            fResBars.SetManaBarVisibility(false);
        }
        
    }

    protected void OnCurrentLifeChanged(object sender, double newCurrentLife) {
        if (fResBars != null) {
            fResBars.SetLifePercentage(newCurrentLife / BasicStats.TotalLife);
        }
    }

    public void CalculateHit() {
        HitDamageInstance damageInstance = new HitDamageInstance(0f, 0f, 0f, 0f, 0f);
    }

    protected void RefreshLifeMana() {
        BasicStats.CurrentLife = BasicStats.TotalLife;
        BasicStats.CurrentMana = BasicStats.TotalMana;
    }

    protected void ApplyRegen() {
        double prevLife = BasicStats.CurrentLife;
        BasicStats.CurrentLife += BasicStats.TotalLifeRegen / ticksPerSecond;
        BasicStats.CurrentMana += BasicStats.TotalManaRegen / ticksPerSecond;

        if (BasicStats.CurrentLife > prevLife) {
            //GD.Print($"+{BasicStats.CurrentLife - prevLife}");
        }
    }

    // Skal laves om senere til at bruge hit calc
    public void TakeDamage(double damage) {
        BasicStats.CurrentLife -= damage;
    }

}
