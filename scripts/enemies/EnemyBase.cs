using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyBase : Actor {
    // Shaders copied and modified from https://godotshaders.com/shader/pixel-perfect-outline-shader/
    protected static readonly Shader magicShader = GD.Load<Shader>("res://shaders/enemy_rarity_magic.gdshader");
    protected static readonly Shader rareShader = GD.Load<Shader>("res://shaders/enemy_rarity_rare.gdshader");

    [Signal]
    public delegate void EnemyDiedEventHandler();

    public AIController CAIController { get; protected set; }

    protected Marker3D resBarAnchor;
    protected NavigationAgent3D navAgent;
    protected Timer navUpdateTimer;
    protected Timer skillTimer;
    protected Timer skillUsePointTimer;
    public RayCast3D LineOfSightCast { get; protected set; }
    public RayCast3D WallCast { get; protected set; }

    protected Label3D debugLabel;

    protected Skill currentlyUsedSkill = null;

    public Stat GoldBounty = new(0, false, 0);
    public Stat ExperienceBounty = new(0, false, 0);

    public EEnemyRarity EnemyRarity { get; protected set; } = EEnemyRarity.Normal;
    public bool CanDropItems { get; protected set; } = true;

    public Dictionary<EStatName, double> RarityDictionary { get; protected set; } = new();
    public Dictionary<EStatName, double> AreaScalingDictionary { get; protected set; } = new() {
        { EStatName.MoreMaxLife, 1 },
        { EStatName.MoreAllDamage, 1 }
    };

    protected MeshInstance3D capsuleMesh;

    public override void _Ready() {
        base._Ready();
        PreSetup();
        Setup();
        PostSetup();
    }

    public virtual void PreSetup() {
        IsIgnoringWeaponRestrictions = true;
        IsIgnoringManaCosts = true;

        skillTimer = GetNode<Timer>("SkillTimer");
        skillUsePointTimer = GetNode<Timer>("SkillUsePointTimer");
        WallCast = GetNode<RayCast3D>("WallCast");
        LineOfSightCast = GetNode<RayCast3D>("LoSCast");
        debugLabel = GetNode<Label3D>("Label3D");
        navUpdateTimer = GetNode<Timer>("NavigationUpdateTimer");
        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        capsuleMesh = GetNode<MeshInstance3D>("CapsuleMesh");

        navAgent.VelocityComputed += OnVelocityComputed;
        AddFloatingBars(resBarAnchor);

        if (EnemyRarity != EEnemyRarity.Boss) {
            ApplyAreaLevelScaling();
        }

        AddToGroup("Enemy");
    }

    public virtual void Setup() {

    }

    public virtual void PostSetup() {
        CallDeferred(MethodName.NavSetup);
    }

    public override void _PhysicsProcess(double delta) {
        ApplyRegen(delta);
        TickEffects(delta);
        TakeDamageOverTime();
        CAIController?.Update(delta);
        ApplyMovement(delta);

        if (CAIController != null) {
            debugLabel.Text = CAIController.CurrentAIState.GetStateName();
        }
    }

    public void OnNavigationUpdateTimeout() {
        if (CAIController.IsAttackingTarget && CAIController.ActorTarget != null) {
            if (CAIController.IsChasingTarget) {
                SetNavigationTarget(CAIController.ActorTarget.GlobalPosition);
            }
        }
    }

    public void ApplyMovement(double delta) {
        DoGravity(delta);
        MoveAndSlide();
    }

    public void ResetPlaneVelocity() {
        Vector3 velocity = Velocity;
        velocity.X = 0f;
        velocity.Z = 0f;
        Velocity = velocity;
    }

    public void AddSkill(Skill skill) {
		Skills.Add(skill);
        skill.ActorOwner = this;
        skill.RecalculateSkillValues();
	}

    public void SetRarity(EEnemyRarity rarity) {
        EnemyRarity = rarity;

        if (rarity == EEnemyRarity.Magic) {
            RarityDictionary = EnemyRarityData.MagicStatDictionary;
            GoldBounty.SIncreased += 1.5;
            ExperienceBounty.SIncreased += 2;

            Material mat = capsuleMesh.GetSurfaceOverrideMaterial(0);
            if (mat.NextPass is ShaderMaterial smat) {
                smat.Shader = magicShader;
            }
        }
        else if (rarity == EEnemyRarity.Rare) {
            RarityDictionary = EnemyRarityData.RareStatDictionary;
            GoldBounty.SIncreased += 2.5;
            ExperienceBounty.SIncreased += 4;

            Material mat = capsuleMesh.GetSurfaceOverrideMaterial(0);
            if (mat.NextPass is ShaderMaterial smat) {
                smat.Shader = rareShader;
            }
        }

        ResetAndMergeStatDictionaries();
    }

    public void ApplyAreaLevelScaling() {
        ActorLevel = Run.Instance.CurrentMap.LocalAreaLevel;
        AreaScalingDictionary[EStatName.MoreMaxLife] *= 1 * Math.Pow(Run.Instance.Rules.EnemyDamageScalingFactor, ActorLevel - 1);
        AreaScalingDictionary[EStatName.MoreAllDamage] *= 1 * Math.Pow(Run.Instance.Rules.EnemyLifeScalingFactor, ActorLevel - 1);
    }

    public override void ResetAndMergeStatDictionaries() {
        foreach (EStatName key in StatDictionary.Keys.ToList()) {
			StatDictionary[key] = 0;
		}

		foreach (EStatName key in MultiplicativeStatDictionary.Keys.ToList()) {
			MultiplicativeStatDictionary[key] = 1;
		}

        foreach (KeyValuePair<EStatName, double> stat in RarityDictionary) {
			if (StatDictionary.ContainsKey(stat.Key)) {
				StatDictionary[stat.Key] += stat.Value;
			}
			else if (MultiplicativeStatDictionary.ContainsKey(stat.Key)) {
				MultiplicativeStatDictionary[stat.Key] *= stat.Value;
			}
			else {
				GD.PrintErr($"Key {stat.Key} not found, skipping");
			}
		}

        foreach (KeyValuePair<EStatName, double> stat in AreaScalingDictionary) {
			if (StatDictionary.ContainsKey(stat.Key)) {
				StatDictionary[stat.Key] += stat.Value;
			}
			else if (MultiplicativeStatDictionary.ContainsKey(stat.Key)) {
				MultiplicativeStatDictionary[stat.Key] *= stat.Value;
			}
			else {
				GD.PrintErr($"Key {stat.Key} not found, skipping");
			}
		}

		foreach (KeyValuePair<EStatName, double> stat in CombinedEffectStatDictionary) {
			if (StatDictionary.ContainsKey(stat.Key)) {
				StatDictionary[stat.Key] += stat.Value;
			}
			else if (MultiplicativeStatDictionary.ContainsKey(stat.Key)) {
				MultiplicativeStatDictionary[stat.Key] *= stat.Value;
			}
			else {
				GD.PrintErr($"Key {stat.Key} not found, skipping");
			}
		}

        CalculateStats();
    }

    protected void CalculateStats() {
        BasicStats.AddedLife = (int)StatDictionary[EStatName.FlatMaxLife];
		BasicStats.IncreasedLife = StatDictionary[EStatName.IncreasedMaxLife];
		BasicStats.MoreLife = MultiplicativeStatDictionary[EStatName.MoreMaxLife];

		BasicStats.AddedLifeRegen = StatDictionary[EStatName.AddedLifeRegen];
		BasicStats.PercentageLifeRegen = StatDictionary[EStatName.PercentageLifeRegen];

        AttackSpeedMod.SIncreased = StatDictionary[EStatName.IncreasedAttackSpeed];
		AttackSpeedMod.SMore = MultiplicativeStatDictionary[EStatName.MoreAttackSpeed];
		CastSpeedMod.SIncreased = StatDictionary[EStatName.IncreasedCastSpeed];
		CastSpeedMod.SMore = MultiplicativeStatDictionary[EStatName.MoreCastSpeed];
		CritChanceMod.SIncreased = StatDictionary[EStatName.IncreasedCritChance];
		CritChanceMod.SMore = MultiplicativeStatDictionary[EStatName.MoreCritChance];
		CritChanceAgainstLowLife.SIncreased = StatDictionary[EStatName.IncreasedCritChanceToLowLife];
		CritChanceAgainstLowLife.SMore = MultiplicativeStatDictionary[EStatName.MoreCritChanceToLowLife];
		CritMultiplier.SAdded = StatDictionary[EStatName.AddedCritMulti];
		CritMultiplierAgainstLowLife = StatDictionary[EStatName.AddedCritMultiplierToLowLife];

		MovementSpeed.SIncreased = StatDictionary[EStatName.IncreasedMovementSpeed];

        DamageMods.Physical.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatMinPhysDamage], (int)StatDictionary[EStatName.FlatMaxPhysDamage],
			(int)StatDictionary[EStatName.FlatAttackMinPhysDamage], (int)StatDictionary[EStatName.FlatAttackMaxPhysDamage],
			(int)StatDictionary[EStatName.FlatSpellMinPhysDamage], (int)StatDictionary[EStatName.FlatSpellMaxPhysDamage],
			StatDictionary[EStatName.IncreasedPhysDamage],
			MultiplicativeStatDictionary[EStatName.MorePhysDamage]
		);

		DamageMods.Fire.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatMinFireDamage], (int)StatDictionary[EStatName.FlatMaxFireDamage],
			(int)StatDictionary[EStatName.FlatAttackMinFireDamage], (int)StatDictionary[EStatName.FlatAttackMaxFireDamage],
			(int)StatDictionary[EStatName.FlatSpellMinFireDamage], (int)StatDictionary[EStatName.FlatSpellMaxFireDamage],
			StatDictionary[EStatName.IncreasedFireDamage],
			MultiplicativeStatDictionary[EStatName.MoreFireDamage]
		);

		DamageMods.Cold.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatMinColdDamage], (int)StatDictionary[EStatName.FlatMaxColdDamage],
			(int)StatDictionary[EStatName.FlatAttackMinColdDamage], (int)StatDictionary[EStatName.FlatAttackMaxColdDamage],
			(int)StatDictionary[EStatName.FlatSpellMinColdDamage], (int)StatDictionary[EStatName.FlatSpellMaxColdDamage],
			StatDictionary[EStatName.IncreasedColdDamage],
			MultiplicativeStatDictionary[EStatName.MoreColdDamage]
		);

		DamageMods.Lightning.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatMinLightningDamage], (int)StatDictionary[EStatName.FlatMaxLightningDamage],
			(int)StatDictionary[EStatName.FlatAttackMinLightningDamage], (int)StatDictionary[EStatName.FlatAttackMaxLightningDamage],
			(int)StatDictionary[EStatName.FlatSpellMinLightningDamage], (int)StatDictionary[EStatName.FlatSpellMaxLightningDamage],
			StatDictionary[EStatName.IncreasedLightningDamage],
			MultiplicativeStatDictionary[EStatName.MoreLightningDamage]
		);

		DamageMods.Chaos.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatMinChaosDamage], (int)StatDictionary[EStatName.FlatMaxChaosDamage],
			(int)StatDictionary[EStatName.FlatAttackMinChaosDamage], (int)StatDictionary[EStatName.FlatAttackMaxChaosDamage],
			(int)StatDictionary[EStatName.FlatSpellMinChaosDamage], (int)StatDictionary[EStatName.FlatSpellMaxChaosDamage],
			StatDictionary[EStatName.IncreasedChaosDamage],
			MultiplicativeStatDictionary[EStatName.MoreChaosDamage]
		);

		Penetrations.Physical = (int)StatDictionary[EStatName.PhysicalPenetration];
		Penetrations.Fire = (int)StatDictionary[EStatName.FirePenetration];
		Penetrations.Cold = (int)StatDictionary[EStatName.ColdPenetration];
		Penetrations.Lightning = (int)StatDictionary[EStatName.LightningPenetration];
		Penetrations.Chaos = (int)StatDictionary[EStatName.ChaosPenetration];

		DamageMods.IncreasedAttack = StatDictionary[EStatName.IncreasedAttackDamage];
		DamageMods.MoreAttack = MultiplicativeStatDictionary[EStatName.MoreAttackDamage];
		DamageMods.IncreasedSpell = StatDictionary[EStatName.IncreasedSpellDamage];
		DamageMods.MoreSpell = MultiplicativeStatDictionary[EStatName.MoreSpellDamage];
		DamageMods.IncreasedMelee = StatDictionary[EStatName.IncreasedMeleeDamage];
		DamageMods.MoreMelee = MultiplicativeStatDictionary[EStatName.MoreMeleeDamage];
		DamageMods.IncreasedProjectile = StatDictionary[EStatName.IncreasedProjectileDamage];
		DamageMods.MoreProjectile = MultiplicativeStatDictionary[EStatName.MoreProjectileDamage];
		DamageMods.IncreasedArea = StatDictionary[EStatName.IncreasedAreaDamage];
		DamageMods.MoreArea = MultiplicativeStatDictionary[EStatName.MoreAreaDamage];
		DamageMods.IncreasedDoT = StatDictionary[EStatName.IncreasedDamageOverTime];
		DamageMods.MoreDoT = MultiplicativeStatDictionary[EStatName.MoreDamageOverTime];
		DamageMods.IncreasedAll = StatDictionary[EStatName.IncreasedAllDamage];
		DamageMods.MoreAll = MultiplicativeStatDictionary[EStatName.MoreAllDamage];
		DamageMods.IncreasedLowLife = StatDictionary[EStatName.IncreasedDamageToLowLife];
		DamageMods.MoreLowLife = MultiplicativeStatDictionary[EStatName.MoreDamageToLowLife];

		DamageMods.IncreasedBleedMagnitude = StatDictionary[EStatName.IncreasedBleedDamageMult];
		DamageMods.MoreBleedMagnitude = MultiplicativeStatDictionary[EStatName.MoreBleedDamageMult];
		DamageMods.IncreasedIgniteMagnitude = StatDictionary[EStatName.IncreasedIgniteDamageMult];
		DamageMods.MoreIgniteMagnitude = MultiplicativeStatDictionary[EStatName.MoreIgniteDamageMult];
		DamageMods.IncreasedPoisonMagnitude = StatDictionary[EStatName.IncreasedPoisonDamageMult];
		DamageMods.MorePoisonMagnitude = MultiplicativeStatDictionary[EStatName.MorePoisonDamageMult];

		AreaOfEffect.SIncreased = StatDictionary[EStatName.IncreasedAreaOfEffect];
		AreaOfEffect.SMore = MultiplicativeStatDictionary[EStatName.MoreAreaOfEffect];

		ProjectileSpeed.SIncreased = StatDictionary[EStatName.IncreasedProjectileSpeed];
		ProjectileSpeed.SMore = MultiplicativeStatDictionary[EStatName.MoreProjectileSpeed];

		DamageMods.Conversion.Physical.ToFire.Values[1] = StatDictionary[EStatName.PhysToFireConversion];
		DamageMods.Conversion.Physical.ToCold.Values[1] = StatDictionary[EStatName.PhysToColdConversion];
		DamageMods.Conversion.Physical.ToLightning.Values[1] = StatDictionary[EStatName.PhysToLightningConversion];
		DamageMods.Conversion.Physical.ToChaos.Values[1] = StatDictionary[EStatName.PhysToChaosConversion];

		DamageMods.Conversion.Fire.ToPhysical.Values[1] = StatDictionary[EStatName.FireToPhysConversion];
		DamageMods.Conversion.Fire.ToCold.Values[1] = StatDictionary[EStatName.FireToColdConversion];
		DamageMods.Conversion.Fire.ToLightning.Values[1] = StatDictionary[EStatName.FireToLightningConversion];
		DamageMods.Conversion.Fire.ToChaos.Values[1] = StatDictionary[EStatName.FireToChaosConversion];

		DamageMods.Conversion.Cold.ToPhysical.Values[1] = StatDictionary[EStatName.ColdToPhysConversion];
		DamageMods.Conversion.Cold.ToFire.Values[1] = StatDictionary[EStatName.ColdToFireConversion];
		DamageMods.Conversion.Cold.ToLightning.Values[1] = StatDictionary[EStatName.ColdToLightningConversion];
		DamageMods.Conversion.Cold.ToChaos.Values[1] = StatDictionary[EStatName.ColdToChaosConversion];

		DamageMods.Conversion.Lightning.ToPhysical.Values[1] = StatDictionary[EStatName.LightningToPhysConversion];
		DamageMods.Conversion.Lightning.ToFire.Values[1] = StatDictionary[EStatName.LightningToFireConversion];
		DamageMods.Conversion.Lightning.ToCold.Values[1] = StatDictionary[EStatName.LightningToColdConversion];
		DamageMods.Conversion.Lightning.ToChaos.Values[1] = StatDictionary[EStatName.LightningToChaosConversion];

		DamageMods.Conversion.Chaos.ToPhysical.Values[1] = StatDictionary[EStatName.ChaosToPhysConversion];
		DamageMods.Conversion.Chaos.ToFire.Values[1] = StatDictionary[EStatName.ChaosToFireConversion];
		DamageMods.Conversion.Chaos.ToCold.Values[1] = StatDictionary[EStatName.ChaosToColdConversion];
		DamageMods.Conversion.Chaos.ToLightning.Values[1] = StatDictionary[EStatName.ChaosToLightningConversion];

		DamageMods.ExtraPhysical = StatDictionary[EStatName.DamageAsExtraPhysical];
		DamageMods.ExtraFire = StatDictionary[EStatName.DamageAsExtraFire];
		DamageMods.ExtraCold = StatDictionary[EStatName.DamageAsExtraCold];
		DamageMods.ExtraLightning = StatDictionary[EStatName.DamageAsExtraLightning];
		DamageMods.ExtraChaos = StatDictionary[EStatName.DamageAsExtraChaos];

		StatusMods.Bleed.SAddedChance = StatDictionary[EStatName.AddedBleedChance];
		StatusMods.Bleed.SIncreasedDuration = StatDictionary[EStatName.IncreasedBleedDuration];
		StatusMods.Bleed.SMoreDuration = MultiplicativeStatDictionary[EStatName.MoreBleedDuration];
		StatusMods.Bleed.SFasterTicking = StatDictionary[EStatName.FasterBleed];
		StatusMods.Ignite.SAddedChance = StatDictionary[EStatName.AddedIgniteChance];
		StatusMods.Ignite.SIncreasedDuration = StatDictionary[EStatName.IncreasedIgniteDuration];
		StatusMods.Ignite.SMoreDuration = MultiplicativeStatDictionary[EStatName.MoreIgniteDuration];
		StatusMods.Ignite.SFasterTicking = StatDictionary[EStatName.FasterIgnite];
		StatusMods.Poison.SAddedChance = StatDictionary[EStatName.AddedPoisonChance];
		StatusMods.Poison.SIncreasedDuration = StatDictionary[EStatName.IncreasedPoisonDuration];
		StatusMods.Poison.SMoreDuration = MultiplicativeStatDictionary[EStatName.MorePoisonDuration];
		StatusMods.Poison.SFasterTicking = StatDictionary[EStatName.FasterPoison];

		Armour.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatArmour], 
			StatDictionary[EStatName.IncreasedArmour],
			MultiplicativeStatDictionary[EStatName.MoreArmour]
		);

		Evasion.SetAddedIncreasedMore(
			(int)StatDictionary[EStatName.FlatEvasion],
			StatDictionary[EStatName.IncreasedEvasion],
			MultiplicativeStatDictionary[EStatName.MoreEvasion]
		);

		BlockChance.SAdded = StatDictionary[EStatName.BlockChance];
		BlockEffectiveness.SAdded = StatDictionary[EStatName.BlockEffectiveness];

		if (ActorFlags.HasFlag(EActorFlags.DamageScalesWithBlockChance)) {
			DamageMods.IncreasedAll += BlockChance.STotal * 0.75;
		}

		if (ActorFlags.HasFlag(EActorFlags.DamageScalesWithMaxMana)) {
			DamageMods.IncreasedAll += Math.Round(BasicStats.TotalMana * 0.0005, 2);
		}

		Resistances.ResPhysical = (int)StatDictionary[EStatName.PhysicalResistance];
		Resistances.ResFire = (int)StatDictionary[EStatName.FireResistance];
		Resistances.ResCold = (int)StatDictionary[EStatName.ColdResistance];
		Resistances.ResLightning = (int)StatDictionary[EStatName.LightningResistance];
		Resistances.ResChaos = (int)StatDictionary[EStatName.ChaosResistance];

		DamageTakenFromMana.SAdded = StatDictionary[EStatName.DamageTakenFromMana];

		BlockChance.SetMaxCap(0.75 + StatDictionary[EStatName.AddedBlockCap]);

		UpdateSkillValues();
    }

    public void UpdateSkillValues() {
		for (int i = 0; i < Skills.Count; i++) {
			Skills[i].RecalculateSkillValues();
		}
	}


    // ===== Navigation =====
    #region Navigation
    public void NavSetup() {
        navAgent.SetNavigationMap(GetWorld3D().NavigationMap);
        
        // Temp
        CAIController.SetTarget(Run.Instance.PlayerActor);
        //GD.Print("AIC Target set");
    }

    public void SetActorTarget(Actor target) {
        navUpdateTimer.Start();
    }

    public void SetNavigationTarget(Vector3 targetPosition) {
        navAgent.TargetPosition = targetPosition;
        ProcessNavigation();
    }

    public void ProcessNavigation() {
        // Do not query when the map has never synchronized and is empty.
        if (NavigationServer3D.MapGetIterationId(navAgent.GetNavigationMap()) == 0) {
            return;
        }

        if (navAgent.IsNavigationFinished()) {
            return;
        }

        Vector3 nextPathPosition = navAgent.GetNextPathPosition();
        Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition with { Y = GlobalPosition.Y }) * (float)MovementSpeed.STotal;

        if (navAgent.AvoidanceEnabled) {
            navAgent.Velocity = newVelocity;
        }
        else {
            OnVelocityComputed(newVelocity);
        }
    }

    public void OnVelocityComputed(Vector3 newVelocity) {
        Velocity = newVelocity;
    }

    public bool CanSeeTarget() {
        LineOfSightCast.TargetPosition = CAIController.ActorTarget.GlobalPosition - GlobalPosition;
        LineOfSightCast.ForceRaycastUpdate();

        if (LineOfSightCast.IsColliding()) {
            Node3D collider = (Node3D)LineOfSightCast.GetCollider();

            if (collider.IsClass("StaticBody3D")) {
                return false;
            }
        }
        else {
            return true;
        }

        return false;
    }

    public bool IsFacingTarget() {
        Vector3 facing = GlobalTransform.Basis.Z;
        Vector3 target = GlobalPosition.DirectionTo(CAIController.ActorTarget.GlobalPosition);
        float dot = facing.Dot(target);

        if (dot >= 0.99f) {
            return true;
        }

        return false;
    }

    public Vector3 GetDirectionAwayFromTarget() {
        Vector3 target = GlobalPosition.DirectionTo(CAIController.ActorTarget.GlobalPosition);
        target.X = -target.X;
        target.Z = -target.Z;

        return target;
    }

    public bool IsThereAWall(Vector3 direction, float distance) {
        WallCast.TargetPosition = direction * 10;
        WallCast.ForceRaycastUpdate();

        if (WallCast.IsColliding()) {
            Vector3 collisionPoint = WallCast.GetCollisionPoint();

            if (GlobalPosition.DistanceTo(collisionPoint) > distance) {
                return false;
            }
            else {
                return true;
            }
        }

        return false;
    }

    public void TurnTowardsPathPosition() {
        if (CAIController.IsAttackingTarget && CAIController.ActorTarget != null) {
            Vector3 direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition() with { Y = GlobalPosition.Y });

            if (direction != Vector3.Zero) {
                Basis lookTarget = Basis.LookingAt(direction, null, true);
                Basis slerpTarget = Basis.Slerp(lookTarget, 0.1f);
                Basis = slerpTarget.Orthonormalized();
            }
        }
    }

    public void TurnTowardsTargetPosition() {
        if (CAIController.IsAttackingTarget && CAIController.ActorTarget != null) {
            Vector3 direction = GlobalPosition.DirectionTo(CAIController.ActorTarget.GlobalPosition with { Y = GlobalPosition.Y });

            if (direction != Vector3.Zero) {
                Basis lookTarget = Basis.LookingAt(direction, null, true);
                Basis slerpTarget = Basis.Slerp(lookTarget, 0.1f);
                Basis = slerpTarget.Orthonormalized();
            }
        }
    }
    #endregion

    // ===== AI =====
    #region AI
    public void SetAIController(AIController controller) {
        CAIController = controller;
    }

    #endregion

    protected override void UpdateLifeDisplay(double newCurrentLife) {
        if (fResBars != null) {
            fResBars.SetLifePercentage(newCurrentLife / BasicStats.TotalLife);
        }
    }

    protected override void UpdateManaDisplay(double newCurrentMana) {
        if (fResBars != null && BasicStats.TotalMana != 0) {
            fResBars.SetManaPercentage(newCurrentMana / BasicStats.TotalMana);
        }
    }

    public virtual void OnSkillTimerTimeout() {
        if (ActorState == EActorState.UsingSkill) {
            ActorState = EActorState.Actionable;
            currentlyUsedSkill = null;
        }
    }

    public virtual void OnSkillUsePointTimerTimeout() {

    }

    // ===== Combat =====
    #region Combat
    public virtual void UsePrimarySkill() {

    }

    public float GetDistanceToTarget() {
        return GlobalPosition.DistanceTo(CAIController.ActorTarget.GlobalPosition);
    }

    public float GetSkillUseRange(int skillIndex) {
        if (skillIndex >= 0 && Skills.Count > skillIndex) {
            return Skills[skillIndex].CastRange - 0.25f;
        }
        else {
            GD.PrintErr("Trying to get skill indexed outside of array bounds");
            return 0f;
        }
    }

    public bool IsWithinSkillRangeOfTarget(int skillIndex) {
        if (skillIndex >= 0 && Skills.Count > skillIndex) {
            return GetDistanceToTarget() < GetSkillUseRange(skillIndex);
        }
        else {
            GD.PrintErr("Trying to get skill indexed outside of array bounds");
            return false;
        }
    }

    public override void OnHitTaken(double damage, bool wasBlocked, bool isCritical, bool createDamageText) {
        if (createDamageText) {
            ShowDamageText(damage, wasBlocked, isCritical);
        }
    }

    public override void OnDamageTaken() {
        if (BasicStats.CurrentLife <= 0) {
            OnNoLifeLeft();
        }
    }

    public override void OnDamageEvaded() {
        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber("Evaded!", false);
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }

    protected void ShowDamageText(double damage, bool wasBlocked, bool isCritical) {
        string labelText;
        if (isCritical) {
            labelText = $"{Math.Round(damage, 0)}!";
        }
        else {
            labelText = $"{Math.Round(damage, 0)}";
        }

        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber(labelText, wasBlocked);
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }

    public override void OnNoLifeLeft() {
        ActorState = EActorState.Dying;
        Die();
    }

    public void Die() {
        ActorState = EActorState.Dead;

        if (GoldBounty.STotal > 0) {
            Run.Instance.CurrentMap.ObjectiveController?.AddGoldToRewards((int)GoldBounty.STotal, true);
        }

        if (ExperienceBounty.STotal > 0) {
            Run.Instance.AwardExperience(ExperienceBounty.STotal);
        }

        if (CanDropItems) {
            Run.Instance.RollForEnemyItems(this);
        }
        
        EmitSignal(SignalName.EnemyDied);
        QueueFree();
    }

    #endregion
}
