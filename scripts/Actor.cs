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
}

public class DamageModifiers {
    public DamageStat Physical { get; protected set; } = new();
    public DamageStat Fire { get; protected set; } = new();
    public DamageStat Cold { get; protected set; } = new();
    public DamageStat Lightning { get; protected set; } = new();
    public DamageStat Chaos { get; protected set; } = new();

    public double IncreasedMelee = 0;
    public double IncreasedRanged = 0;
    public double IncreasedSpell = 0;

    public double MoreMelee = 1;
    public double MoreRanged = 1;
    public double MoreSpell = 1;

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

        c.MoreMelee = a.MoreMelee * b.MoreMelee;
        c.MoreRanged = a.MoreRanged * b.MoreRanged;
        c.MoreSpell = a.MoreSpell * b.MoreSpell;
        
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

        c.MoreMelee = a.MoreMelee / b.MoreMelee;
        c.MoreRanged = a.MoreRanged / b.MoreRanged;
        c.MoreSpell = a.MoreSpell / b.MoreSpell;
        
        return c;
    }
}

public class ActorResistances {
    public int ResPhysical;
    public int ResFire;
    public int ResCold;
    public int ResLightning;
    public int ResChaos;
}

public class ActorPenetrations {
    public int PenPhysical;
    public int PenFire;
    public int PenCold;
    public int PenLightning;
    public int PenChaos;
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
    [Signal]
    public delegate void DamageTakenEventHandler(double damage, bool isCritical, bool showDamageText);

    [Signal]
    public delegate void DamageEvadedEventHandler();

    protected PackedScene floatingResourceBarsScene = GD.Load<PackedScene>("res://scenes/gui/actor_floating_resource_bars.tscn");

    protected int ticksPerSecond = ProjectSettings.GetSetting("physics/common/physics_ticks_per_second").AsInt32();

    public List<Skill> Skills = new List<Skill>();

    public int ActorLevel = 1;
    public Stat Armour = new(0, true, 0);
    public Stat Evasion = new(0, true, 0);

    public ActorBasicStats BasicStats = new();
    public DamageModifiers DamageMods = new();
    public ActorResistances Resistances = new();
    public ActorPenetrations Penetrations = new();

    public EActorState ActorState = EActorState.Actionable;

    public Stat AttackSpeedMod = new(1, false);
    public Stat CritChanceMod = new(1, false);
    public Stat CritMultiplier = new(1.5, false, 0);
    public Stat CastSpeedMod = new(1, false);

    public Stat MovementSpeed = new(0, false, 0);

    public float OutgoingEffectAttachmentHeight { get; protected set; } = 1f;

    protected FloatingResourceBars fResBars;

    public ActorMainHand MainHand { get; protected set; } = new();
    public Item OffHandItem { get; protected set; } = null;
    public bool IsOffHandAWeapon { get; protected set; } = false;

    public int UnarmedMinDamage { get; protected set; }
    public int UnarmedMaxDamage { get; protected set; }
    public double UnarmedAttackSpeed { get; protected set; }
    public double UnarmedCritChance { get; protected set; } = 0.05;

    public override void _Ready() {
        BasicStats.CurrentLifeChanged += OnCurrentLifeChanged;
        DamageTaken += OnDamageTaken;
        DamageEvaded += OnDamageEvaded;
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

    public void TakeDamage(EDamageCategory dmgCategory, SkillDamage damage, ActorPenetrations pens, bool isAHit, bool isCritical, bool createDamageText) {
        double physDamage = damage.Physical;
        double fireDamage = damage.Fire;
        double coldDamage = damage.Cold;
        double lightningDamage = damage.Lightning;
        double chaosDamage = damage.Chaos;
        double totalDamage;

        if (isAHit) {
            if (dmgCategory != EDamageCategory.Spell) {
                if (RollForEvade(GetEvasionChance(Evasion.STotal, ActorLevel))) {
                    EmitSignal(SignalName.DamageEvaded);
                    return;
                }
            }

            physDamage *= GetArmourMitigation(Armour.STotal, ActorLevel);
            fireDamage *= GetArmourMitigation(Armour.STotal, ActorLevel) / 2;
            coldDamage *= GetArmourMitigation(Armour.STotal, ActorLevel) / 2;
            lightningDamage *= GetArmourMitigation(Armour.STotal, ActorLevel) / 2;
            chaosDamage *= GetArmourMitigation(Armour.STotal, ActorLevel) / 2;
        }

        physDamage *= 1 - ((Resistances.ResPhysical - Penetrations.PenPhysical) / 100);
        fireDamage *= 1 - ((Resistances.ResFire - Penetrations.PenFire) / 100);
        coldDamage *= 1 - ((Resistances.ResCold - Penetrations.PenCold) / 100);
        lightningDamage *= 1 - ((Resistances.ResLightning - Penetrations.PenLightning) / 100);
        chaosDamage *= 1 - ((Resistances.ResChaos - Penetrations.PenChaos) / 100);

        totalDamage = physDamage + fireDamage + coldDamage + lightningDamage + chaosDamage;

        BasicStats.CurrentLife -= totalDamage;
        EmitSignal(SignalName.DamageTaken, totalDamage, isCritical, createDamageText);
    }

    public virtual void OnDamageTaken(double damage, bool isCritical, bool createDamageText) {

    }

    public virtual void OnDamageEvaded() {

    }

    public static double GetArmourMitigation(double armour, int level) {
        return 200 / (200 + armour - (20 * (level - 1)));
    }

    public static double GetEvasionChance(double evasion, int level) {
        return 1 - (200 / (200 + evasion - (20 * (level - 1))));
    }

    public static bool RollForEvade(double chance) {
        double evasionRoll = Utilities.RNG.NextDouble();

        if (chance >= evasionRoll) {
            return true;
        }

        return false;
    }
}
