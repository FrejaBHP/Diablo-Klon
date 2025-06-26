using System;
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

    private double moreLife = 1;
    public double MoreLife {
        get { return moreLife; }
        set { 
            moreLife = value;
            CalculateMaxLife();
        }
    }
    
    private int totalLife;
    public int TotalLife { get => totalLife; }

    public delegate void CurrentLifeChangedEventHandler(double newCurrentLife);
    public event CurrentLifeChangedEventHandler CurrentLifeChanged;

    private double currentLife;
    public double CurrentLife {
        get => currentLife;
        set {
            if (value >= totalLife) {
                currentLife = totalLife;
                CurrentLifeChanged?.Invoke(currentLife);
            }
            else {
                currentLife = value;
                CurrentLifeChanged?.Invoke(currentLife);
            }
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

    private double moreMana = 1;
    public double MoreMana {
        get => moreMana;
        set {
            moreMana = value;
            CalculateMaxMana();
        }
    }

    private int totalMana;
    public int TotalMana { get => totalMana; }

    public delegate void CurrentManaChangedEventHandler(double newCurrentMana);
    public event CurrentManaChangedEventHandler CurrentManaChanged;
    
    private double currentMana;
    public double CurrentMana {
        get => currentMana;
        set {
            if (value >= totalMana) {
                currentMana = totalMana;
                CurrentManaChanged?.Invoke(currentMana);
            }
            else {
                currentMana = value;
                CurrentManaChanged?.Invoke(currentMana);
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
        totalLife = (int)((baseLife + addedLife) * (1 + increasedLife) * moreLife);

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
            CurrentLife += changeInLife * percentageCurrentLife;
            //GD.Print($"Positiv Ændring, +{(double)(changeInLife * percentageCurrentLife)}");
        }
        else if (changeInLife < 0 && currentLife > totalLife) {
            CurrentLife = totalLife;
            //GD.Print("Life sat til fuld");
        }
    }

    public void AdjustCurrentMana(int oldTotalMana) {
        double percentageCurrentMana = currentMana / oldTotalMana;
        int changeInMana = totalMana - oldTotalMana;

        //GD.Print($"%: {percentageCurrentMana}, Change: {changeInMana}");

        if (changeInMana > 0) {
            CurrentMana += changeInMana * percentageCurrentMana;
            //GD.Print($"Positiv Ændring, +{(double)(changeInMana * percentageCurrentMana)}");
        }
        else if (changeInMana < 0 && currentMana > totalMana) {
            CurrentMana = totalMana;
            //GD.Print("Mana sat til fuld");
        }
    }

    public void CalculateLifeRegen() {
        totalLifeRegen = (totalLife * percentageLifeRegen) + addedLifeRegen * (1 + increasedLifeRegen);
    }

    public void CalculateMaxMana() {
        int oldTotalMana = totalMana;
        totalMana = (int)((baseMana + addedMana) * (1 + increasedMana) * moreMana);

        if (oldTotalMana != 0) {
            AdjustCurrentMana(oldTotalMana);
        }

        CalculateManaRegen();
    }

    public void CalculateManaRegen() {
        totalManaRegen = ((totalMana * 0.02) + addedManaRegen) * (1 + increasedManaRegen);
    }
}

public class DamageModifiers() {
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

public class ActorWeaponStats {
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

    protected static readonly PackedScene floatingResourceBarsScene = GD.Load<PackedScene>("res://scenes/gui/actor_floating_resource_bars.tscn");

    public readonly float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public List<Skill> Skills = new List<Skill>();

    public int ActorLevel = 1;
    protected int maxLevel = 40;

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
    public Stat ExperienceMod = new(1, false, 0);

    public Stat MovementSpeed = new(0, false, 0);

    public float OutgoingEffectAttachmentHeight { get; protected set; } = 1f;

    public WeaponItem MainHand { get; protected set; } = null;
    public ActorWeaponStats MainHandStats { get; protected set; } = new();
    public Item OffHandItem { get; protected set; } = null;
    public bool IsOffHandAWeapon { get; protected set; } = false;
    public ActorWeaponStats OffHandStats { get; protected set; } = new();
    public bool IsDualWielding { get; protected set; } = false;
    public bool IsUsingMainHandDW { get; protected set; } = true;

    public int UnarmedMinDamage { get; protected set; }
    public int UnarmedMaxDamage { get; protected set; }
    public double UnarmedAttackSpeed { get; protected set; }
    public double UnarmedCritChance { get; protected set; } = 0.05;

    public bool IsIgnoringWeaponRestrictions { get; protected set; } = false;
    public bool IsIgnoringManaCosts { get; protected set; } = false;

    protected FloatingResourceBars fResBars;

    public override void _Ready() {
        BasicStats.CurrentLifeChanged += OnCurrentLifeChanged;
        BasicStats.CurrentManaChanged += OnCurrentManaChanged;
        DamageTaken += OnDamageTaken;
        DamageEvaded += OnDamageEvaded;
    }

    protected void DoGravity(double delta) {
        if (!IsOnFloor()) {
			Vector3 velocity = Velocity;
			velocity.Y -= gravity * (float)delta;
			Velocity = velocity;
		}
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

    protected void OnCurrentLifeChanged(double newCurrentLife) {
        UpdateLifeDisplay(newCurrentLife);
    }

    protected void OnCurrentManaChanged(double newCurrentMana) {
        UpdateManaDisplay(newCurrentMana);
    }

    protected virtual void UpdateLifeDisplay(double newCurrentLife) {
        
    }

    protected virtual void UpdateManaDisplay(double newCurrentMana) {
        
    }

    protected void RefreshLifeMana() {
        BasicStats.CurrentLife = BasicStats.TotalLife;
        BasicStats.CurrentMana = BasicStats.TotalMana;
    }

    protected void ApplyRegen(double delta) {
        double prevLife = BasicStats.CurrentLife;
        BasicStats.CurrentLife += BasicStats.TotalLifeRegen * delta;
        BasicStats.CurrentMana += BasicStats.TotalManaRegen * delta;

        if (BasicStats.CurrentLife > prevLife) {
            //GD.Print($"+{BasicStats.CurrentLife - prevLife}");
        }
    }

    // Should really have a separate function first called something along the lines of "RegisterIncomingDamage". Name is confusing
    /// <summary>
    /// <para>Sets up an Actor for incoming damage, then processes and applies it.</para>
    /// <para>A non-spell hit can be Evaded, by which the hit will be ignored. If hit damage isn't Evaded, Armour is applied.</para>
    /// If createDamageText is true, creates floating text above the Actor showing the final damage taken.
    /// </summary>
    /// <param name="dmgCategory"></param>
    /// <param name="damage"></param>
    /// <param name="pens"></param>
    /// <param name="isAHit"></param>
    /// <param name="isCritical"></param>
    /// <param name="createDamageText"></param>
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

            double armourMitigation = GetArmourMitigation(Armour.STotal, ActorLevel);
            double halfMitigation = armourMitigation + ((1 - armourMitigation) * 0.5);
            
            physDamage *= armourMitigation;
            fireDamage *= halfMitigation;
            coldDamage *= halfMitigation;
            lightningDamage *= halfMitigation;
            chaosDamage *= halfMitigation;
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
        return Math.Clamp(200 / (200 + (armour * Math.Pow(0.96, level - 1))), 0, 1);
    }

    public static double GetEvasionChance(double evasion, int level) {
        return Math.Clamp(1 - (200 / (200 + (evasion * Math.Pow(0.96, level - 1)))), 0, 1);
    }

    public static bool RollForEvade(double chance) {
        double evasionRoll = Utilities.RNG.NextDouble();

        if (chance >= evasionRoll) {
            return true;
        }
        return false;
    }

    public virtual void OnNoLifeLeft() {

    }
}
