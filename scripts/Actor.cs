using System;
using System.Collections.Generic;
using System.Linq;
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

public class ActorResistances {
    public int ResPhysical;
    public int ResFire;
    public int ResCold;
    public int ResLightning;
    public int ResChaos;
}

public class ActorPenetrations {
    public int Physical;
    public int Fire;
    public int Cold;
    public int Lightning;
    public int Chaos;
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
    public delegate void HitTakenEventHandler(double damage, bool wasBlocked, bool isCritical, bool showDamageText);

    [Signal]
    public delegate void DamageTakenEventHandler();

    [Signal]
    public delegate void DamageEvadedEventHandler();

    [Signal]
    public delegate void DamageBlockedEventHandler();

    protected static readonly PackedScene floatingResourceBarsScene = GD.Load<PackedScene>("res://scenes/gui/actor_floating_resource_bars.tscn");

    public readonly float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public List<Skill> Skills = new List<Skill>();

    public Dictionary<EDamageType, double> PendingDamageOverTime = new() {
        { EDamageType.Untyped,      0 },
        { EDamageType.Physical,     0 },
        { EDamageType.Fire,         0 },
        { EDamageType.Cold,         0 },
        { EDamageType.Lightning,    0 },
        { EDamageType.Chaos,        0 },
    };

    public Dictionary<EEffectName, AttachedEffect> UniqueEffects = new();
    public Dictionary<EEffectName, List<AttachedEffect>> StackableEffects = new();

    public int ActorLevel = 1;
    protected int maxLevel = 40;

    public Stat Armour = new(0, true, 0);
    public Stat Evasion = new(0, true, 0);
    public Stat BlockChance = new(0, false, 0, 0.75);
    public Stat BlockEffectiveness = new(0.5, false, 0, 0.8);

    public ActorBasicStats BasicStats = new();
    public DamageModifiers DamageMods = new();
    public StatusEffectModifiers StatusMods = new();
    public ActorResistances Resistances = new();
    public ActorPenetrations Penetrations = new();

    public EActorState ActorState = EActorState.Actionable;

    public Stat AttackSpeedMod = new(1, false);
    public Stat CritChanceMod = new(1, false);
    public Stat CritMultiplier = new(1.5, false, 0);
    public Stat CastSpeedMod = new(1, false);
    public Stat ExperienceMod = new(1, false, 0);

    public Stat MovementSpeed = new(0, false, 0);

    public Dictionary<EStatName, double> StatDictionary = new() {
		{ EStatName.FlatStrength, 					0 },
		{ EStatName.FlatDexterity, 					0 },
		{ EStatName.FlatIntelligence, 				0 },

		{ EStatName.FlatMaxLife, 					0 },
		{ EStatName.IncreasedMaxLife, 				0 },
		{ EStatName.AddedLifeRegen, 				0 },
		{ EStatName.PercentageLifeRegen, 			0 },

		{ EStatName.FlatMaxMana, 					0 },
		{ EStatName.IncreasedMaxMana, 				0 },
		{ EStatName.AddedManaRegen, 				0 },
		{ EStatName.IncreasedManaRegen, 			0 },

		{ EStatName.FlatMinPhysDamage, 				0 },
		{ EStatName.FlatMaxPhysDamage, 				0 },
        { EStatName.FlatAttackMinPhysDamage, 		0 },
		{ EStatName.FlatAttackMaxPhysDamage, 		0 },
        { EStatName.FlatSpellMinPhysDamage, 		0 },
		{ EStatName.FlatSpellMaxPhysDamage, 		0 },
		{ EStatName.IncreasedPhysDamage, 			0 },
        { EStatName.PhysicalPenetration, 			0 },

		{ EStatName.FlatMinFireDamage, 				0 },
		{ EStatName.FlatMaxFireDamage, 				0 },
        { EStatName.FlatAttackMinFireDamage, 		0 },
		{ EStatName.FlatAttackMaxFireDamage, 		0 },
        { EStatName.FlatSpellMinFireDamage, 		0 },
		{ EStatName.FlatSpellMaxFireDamage, 		0 },
		{ EStatName.IncreasedFireDamage, 			0 },
        { EStatName.FirePenetration, 			    0 },

		{ EStatName.FlatMinColdDamage, 				0 },
		{ EStatName.FlatMaxColdDamage, 				0 },
        { EStatName.FlatAttackMinColdDamage, 		0 },
		{ EStatName.FlatAttackMaxColdDamage, 		0 },
        { EStatName.FlatSpellMinColdDamage, 		0 },
		{ EStatName.FlatSpellMaxColdDamage, 		0 },
		{ EStatName.IncreasedColdDamage, 			0 },
        { EStatName.ColdPenetration, 			    0 },

		{ EStatName.FlatMinLightningDamage, 		0 },
		{ EStatName.FlatMaxLightningDamage, 		0 },
        { EStatName.FlatAttackMinLightningDamage, 	0 },
		{ EStatName.FlatAttackMaxLightningDamage, 	0 },
        { EStatName.FlatSpellMinLightningDamage, 	0 },
		{ EStatName.FlatSpellMaxLightningDamage, 	0 },
		{ EStatName.IncreasedLightningDamage, 		0 },
        { EStatName.LightningPenetration, 			0 },

		{ EStatName.FlatMinChaosDamage, 			0 },
		{ EStatName.FlatMaxChaosDamage, 			0 },
        { EStatName.FlatAttackMinChaosDamage, 		0 },
		{ EStatName.FlatAttackMaxChaosDamage, 		0 },
        { EStatName.FlatSpellMinChaosDamage, 		0 },
		{ EStatName.FlatSpellMaxChaosDamage, 		0 },
		{ EStatName.IncreasedChaosDamage, 			0 },
        { EStatName.ChaosPenetration, 			    0 },

        { EStatName.IncreasedAttackDamage, 			0 },
        { EStatName.IncreasedSpellDamage, 			0 },
        { EStatName.IncreasedMeleeDamage, 			0 },
		{ EStatName.IncreasedProjectileDamage, 		0 },
		{ EStatName.IncreasedAreaDamage, 			0 },
        { EStatName.IncreasedDamageOverTime, 		0 },

		{ EStatName.IncreasedAttackSpeed, 			0 },
		{ EStatName.IncreasedCastSpeed, 			0 },
		{ EStatName.IncreasedCritChance, 			0 },
		{ EStatName.AddedCritMulti, 				0 },

        { EStatName.AddedBleedChance, 			    0 },
        { EStatName.IncreasedBleedDamage, 		    0 },
        { EStatName.IncreasedBleedDuration,         0 },
        { EStatName.FasterBleed, 			        0 },
        { EStatName.AddedIgniteChance, 			    0 },
        { EStatName.IncreasedIgniteDamage, 		    0 },
        { EStatName.IncreasedIgniteDuration,        0 },
        { EStatName.FasterIgnite, 			        0 },
        { EStatName.AddedPoisonChance, 			    0 },
        { EStatName.IncreasedPoisonDamage, 		    0 },
        { EStatName.IncreasedPoisonDuration,        0 },
        { EStatName.FasterPoison, 			        0 },

		{ EStatName.IncreasedMovementSpeed, 		0 },
		{ EStatName.BlockChance, 					0 },

		{ EStatName.FlatArmour, 					0 },
		{ EStatName.IncreasedArmour, 				0 },
		{ EStatName.FlatEvasion, 					0 },
		{ EStatName.IncreasedEvasion, 				0 },
		{ EStatName.FlatEnergyShield, 				0 },
		{ EStatName.IncreasedEnergyShield, 			0 },

		{ EStatName.PhysicalResistance, 			0 },
		{ EStatName.FireResistance, 				0 },
		{ EStatName.ColdResistance, 				0 },
		{ EStatName.LightningResistance, 			0 },
		{ EStatName.ChaosResistance, 				0 },
	};

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
        HitTaken += OnHitTaken;
        DamageTaken += OnDamageTaken;
        DamageEvaded += OnDamageEvaded;
        DamageBlocked += OnDamageBlocked;
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
    /// <param name="isCritical"></param>
    /// <param name="createDamageText"></param>
    public void ReceiveHit(EDamageCategory dmgCategory, SkillInfo info, ActorPenetrations pens, bool createDamageText) {
        double physDamage = info.DamageInfo.Physical;
        double fireDamage = info.DamageInfo.Fire;
        double coldDamage = info.DamageInfo.Cold;
        double lightningDamage = info.DamageInfo.Lightning;
        double chaosDamage = info.DamageInfo.Chaos;
        double totalDamage;

        bool hitBlocked = false;

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

        if (RollForBlock(BlockChance.STotal)) {
            EmitSignal(SignalName.DamageBlocked);
            hitBlocked = true;

            double blockMitigation = 1 - BlockEffectiveness.STotal;

            physDamage *= blockMitigation;
            fireDamage *= blockMitigation;
            coldDamage *= blockMitigation;
            lightningDamage *= blockMitigation;
            chaosDamage *= blockMitigation;
        }

        physDamage *= 1 - ((double)(Resistances.ResPhysical - pens.Physical) / 100);
        fireDamage *= 1 - ((double)(Resistances.ResFire - pens.Fire) / 100);
        coldDamage *= 1 - ((double)(Resistances.ResCold - pens.Cold) / 100);
        lightningDamage *= 1 - ((double)(Resistances.ResLightning - pens.Lightning) / 100);
        chaosDamage *= 1 - ((double)(Resistances.ResChaos - pens.Chaos) / 100);

        totalDamage = physDamage + fireDamage + coldDamage + lightningDamage + chaosDamage;

        BasicStats.CurrentLife -= totalDamage;
        EmitSignal(SignalName.HitTaken, totalDamage, hitBlocked, info.InfoFlags.HasFlag(EDamageInfoFlags.Critical), createDamageText);
        EmitSignal(SignalName.DamageTaken);
        
        if (info.StatusInfo.StatusEffects.Count != 0) {
            foreach (AttachedEffect status in info.StatusInfo.StatusEffects) {
                status.AffectedActor = this;
                ReceiveEffect(status);
            }
        }
    }

    public void TallyDamageOverTimeForNextTick(EDamageType type, double damage) {
        PendingDamageOverTime[type] += damage;
    }

    protected void TakeDamageOverTime() {
        if (ActorState == EActorState.Dying || ActorState == EActorState.Dead) {
            return;
        }
        
        double untypedDamage = PendingDamageOverTime[EDamageType.Untyped];
        double physDamage = PendingDamageOverTime[EDamageType.Physical];
        double fireDamage = PendingDamageOverTime[EDamageType.Fire];
        double coldDamage = PendingDamageOverTime[EDamageType.Cold];
        double lightningDamage = PendingDamageOverTime[EDamageType.Lightning];
        double chaosDamage = PendingDamageOverTime[EDamageType.Chaos];

        double unprocessedDamage = untypedDamage + physDamage + fireDamage + coldDamage + lightningDamage + chaosDamage;

        if (unprocessedDamage == 0) {
            return;
        }

        physDamage *= 1 - Resistances.ResPhysical;
        fireDamage *= 1 - Resistances.ResFire;
        coldDamage *= 1 - Resistances.ResCold;
        lightningDamage *= 1 - Resistances.ResLightning;
        chaosDamage *= 1 - Resistances.ResChaos;

        double processedDamage = untypedDamage + physDamage + fireDamage + coldDamage + lightningDamage + chaosDamage;

        BasicStats.CurrentLife -= processedDamage;
        EmitSignal(SignalName.DamageTaken);

        foreach (EDamageType damageType in PendingDamageOverTime.Keys) {
            PendingDamageOverTime[damageType] = 0;
        }
        
    }

    public virtual void OnHitTaken(double damage, bool wasBlocked, bool isCritical, bool createDamageText) {

    }

    public virtual void OnDamageTaken() {

    }

    public virtual void OnDamageEvaded() {

    }

    public virtual void OnDamageBlocked() {

    }

    public static double GetArmourMitigation(double armour, int level) {
        return Math.Clamp(200 / (200 + (armour * Math.Pow(0.96, level - 1))), 0, 1);
    }

    public static double GetEvasionChance(double evasion, int level) {
        return Math.Clamp(1 - (200 / (200 + (evasion * Math.Pow(0.96, level - 1)))), 0, 1);
    }

    public static bool RollForEvade(double chance) {
        double evasionRoll = Utilities.RNG.NextDouble();

        if (chance != 0 && chance >= evasionRoll) {
            return true;
        }
        return false;
    }

    public static bool RollForBlock(double chance) {
        double blockRoll = Utilities.RNG.NextDouble();

        if (chance != 0 && chance >= blockRoll) {
            return true;
        }
        return false;
    }

    public void ReceiveEffect(AttachedEffect incEffect) {
        if (incEffect is IUniqueEffect ue) {
            if (UniqueEffects.TryGetValue(incEffect.EffectName, out AttachedEffect value)) {
                if (value == null) {
                    UniqueEffects[incEffect.EffectName] = incEffect;
                    incEffect.OnGained();
                }
                else if (ue.ShouldReplaceCurrentEffect(incEffect.RemainingTime, incEffect.EffectValue)) {
                    UniqueEffects[incEffect.EffectName] = incEffect;
                    //incEffect.OnGained(); // Should probably make another type of function here, since effect is never lost, but will definitely be updated
                }
            }
            else if (UniqueEffects.TryAdd(incEffect.EffectName, incEffect)) {
                incEffect.OnGained();
            }
        }
        else if (incEffect is IUniqueStackableEffect use) {
            if (UniqueEffects.TryGetValue(incEffect.EffectName, out AttachedEffect value)) {
                if (value == null) {
                    UniqueEffects[incEffect.EffectName] = incEffect;
                    incEffect.OnGained();
                }
                else if (UniqueEffects[incEffect.EffectName] is IUniqueStackableEffect cEffect) {
                    UniqueEffects[incEffect.EffectName].OverrideTimer(incEffect.RemainingTime);
                    cEffect.AddStack(use.StacksPerApplication);
                    //incEffect.OnGained(); // Should probably make another type of function here, since effect is never lost, but will definitely be updated
                }
            }
            else if (UniqueEffects.TryAdd(incEffect.EffectName, incEffect)) {
                incEffect.OnGained();
            }
        }
        else if (incEffect is IRepeatableEffect re) {
            if (StackableEffects.TryGetValue(incEffect.EffectName, out List<AttachedEffect> value)) {
                value.Add(incEffect);
                incEffect.OnGained();
            }
            else if (StackableEffects.TryAdd(incEffect.EffectName, new List<AttachedEffect>())) {
                StackableEffects[incEffect.EffectName].Add(incEffect);
                incEffect.OnGained();
            }
        }
    }

    protected void TickEffects(double delta) {
        foreach (KeyValuePair<EEffectName, AttachedEffect> kvp in UniqueEffects) {
            if (UniqueEffects[kvp.Key] != null) {
                if (kvp.Value.RemainingTime > 0) {
                    kvp.Value.Tick(delta);
                }
                else {
                    kvp.Value.OnExpired();
                    UniqueEffects[kvp.Key] = null;
                }
            }
        }

        foreach (KeyValuePair<EEffectName, List<AttachedEffect>> kvp in StackableEffects) {
            bool effectHasExpired = false;

            foreach (AttachedEffect effect in kvp.Value) {
                if (effect.RemainingTime > 0) {
                    effect.Tick(delta);
                }
                else {
                    effect.OnExpired();
                    effectHasExpired = true;
                }
            }

            if (effectHasExpired) {
                //StackableEffects[kvp.Key] = kvp.Value.Where(effect => !effect.HasExpired).ToList();
                StackableEffects[kvp.Key].RemoveAll(effect => effect.HasExpired);
            }
        }
    }

    public virtual void OnNoLifeLeft() {

    }
}
