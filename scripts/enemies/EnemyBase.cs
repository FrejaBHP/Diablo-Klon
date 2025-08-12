using Godot;
using System;

public partial class EnemyBase : Actor {
    [Signal]
    public delegate void EnemyDiedEventHandler();

    protected Marker3D resBarAnchor;
    protected NavigationAgent3D navAgent;
    protected Timer navUpdateTimer;
    protected Timer skillTimer;
    protected Timer skillUsePointTimer;
    protected RayCast3D lineOfSightCast;

    protected Label3D debugLabel;

    protected Actor actorTarget;
    protected bool isChasingTarget = false;

    protected Skill currentlyUsedSkill = null;

    protected int goldBounty = 0;
    protected int experienceBounty = 0;

    protected EEnemyRarity enemyRarity = EEnemyRarity.Normal;
    protected const double normalDropChance = 0.1;
    protected const double magicDropChance = 0.33;
    protected const double rareDropChance = 1;
    protected bool canDropItems = true;

    public override void _Ready() {
        base._Ready();
        IsIgnoringWeaponRestrictions = true;
        IsIgnoringManaCosts = true;

        skillTimer = GetNode<Timer>("SkillTimer");
        skillUsePointTimer = GetNode<Timer>("SkillUsePointTimer");
        lineOfSightCast = GetNode<RayCast3D>("LoSCast");
        debugLabel = GetNode<Label3D>("Label3D");
        navUpdateTimer = GetNode<Timer>("NavigationUpdateTimer");
        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        navAgent.VelocityComputed += OnVelocityComputed;
        AddFloatingBars(resBarAnchor);

        if (enemyRarity != EEnemyRarity.Boss) {
            ApplyAreaLevelScaling();
        }

        AddToGroup("Enemy");

        CallDeferred(MethodName.NavSetup);
    }

    public override void _PhysicsProcess(double delta) {
        ApplyRegen(delta);
        TickEffects(delta);
        TakeDamageOverTime();
    }

    public void AddSkill(Skill skill) {
		Skills.Add(skill);
        skill.ActorOwner = this;
        skill.RecalculateSkillValues();
	}

    public void SetRarity(EEnemyRarity rarity) {
        enemyRarity = rarity;
    }

    public void ApplyAreaLevelScaling() {
        ActorLevel = Run.Instance.CurrentMap.LocalAreaLevel;

        double damageScaling = 1 * Math.Pow(Run.Instance.Rules.EnemyDamageScalingFactor, ActorLevel - 1);
        BasicStats.MoreLife *= 1 * Math.Pow(Run.Instance.Rules.EnemyLifeScalingFactor, ActorLevel - 1);
        DamageMods.MoreMelee *= damageScaling;
        DamageMods.MoreProjectile *= damageScaling;
        DamageMods.MoreSpell *= damageScaling;
    }

    // ===== Navigation =====
    #region Navigation
    public void NavSetup() {
        navAgent.SetNavigationMap(GetWorld3D().NavigationMap);
        
        // Temp
        SetActorTarget(Run.Instance.PlayerActor);
    }

    public void OnNavigationUpdateTimeout() {
        if (isChasingTarget && actorTarget != null) {
            SetNavigationTarget(actorTarget.GlobalPosition);
        }
    }

    public void SetActorTarget(Actor target) {
        actorTarget = target;
        SetNavigationTarget(target.GlobalPosition);
        isChasingTarget = true;
        navUpdateTimer.Start();
    }

    public void SetNavigationTarget(Vector3 targetPosition) {
        navAgent.TargetPosition = targetPosition;
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
        FacePathPosition();

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

    protected void FacePathPosition() {
        if (isChasingTarget && actorTarget != null) {
            Vector3 direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition() with { Y = GlobalPosition.Y });

            if (direction != Vector3.Zero) {
                Basis lookTarget = Basis.LookingAt(direction, null, true);
                Basis slerpTarget = Basis.Slerp(lookTarget, 0.08f);
                Basis = slerpTarget.Orthonormalized();
            }
        }
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
    protected virtual void UsePrimarySkill() {

    }

    protected void BasicChaseAIProcess(double delta) {
        if (isChasingTarget && actorTarget != null) {
            if (GlobalPosition.DistanceTo(actorTarget.GlobalPosition) < Skills[0].CastRange - 0.25f && ActorState != EActorState.UsingSkill) {
                lineOfSightCast.ForceRaycastUpdate();

                if (lineOfSightCast.IsColliding()) {
                    UsePrimarySkill();
                }
            }
        }

        if (ActorState == EActorState.Actionable) {
            ProcessNavigation();
        }

        DoGravity(delta);
        MoveAndSlide();
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

    protected bool RollToDropItems() {
        double chance = 0;
        bool haveItemsDropped = false;

        switch (enemyRarity) {
            case EEnemyRarity.Normal:
                chance = normalDropChance;
                break;
            
            case EEnemyRarity.Magic:
                chance = magicDropChance;
                break;

            case EEnemyRarity.Rare:
                chance = rareDropChance;
                break;
            
            default:
                break;
        }

        while (chance >= 1) {
            chance -= 1;
            Run.Instance.CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None));
            haveItemsDropped = true;
        }

        if (chance != 0 && chance >= Utilities.RNG.NextDouble()) {
            Run.Instance.CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None));
            haveItemsDropped = true;
        }
        
        return haveItemsDropped;
    }

    public override void OnNoLifeLeft() {
        ActorState = EActorState.Dying;
        Die();
    }

    public void Die() {
        ActorState = EActorState.Dead;

        if (goldBounty > 0) {
            Run.Instance.CurrentMap.ObjectiveController?.AddGoldToRewards(goldBounty, true);
        }

        if (experienceBounty > 0) {
            Run.Instance.AwardExperience(experienceBounty);
        }

        if (canDropItems) {
            RollToDropItems();
        }
        
        EmitSignal(SignalName.EnemyDied);
        QueueFree();
    }

    #endregion
}
