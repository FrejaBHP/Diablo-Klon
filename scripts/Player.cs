using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Actor {
	public HUD PlayerHUD;
	public Control PauseMenu;
	public PlayerCamera PlayerCamera;

	public Stat Strength { get; protected set; } = new(0, true);
    public Stat Dexterity { get; protected set; } = new(0, true);
    public Stat Intelligence { get; protected set; } = new(0, true);

	protected int lifeGrowth = 5;
	protected int manaGrowth = 3;

    public double Experience { get; protected set; } = 0;

    protected int[] experienceRequirements = [
        10, 12, 14, 17, 20,
        23, 28, 33, 39, 45,
        100, 100, 100, 100, 100
    ];

	private int gold = 0;
	public int Gold { 
		get => gold; 
		set {
			gold = value;
			GoldCountChanged();
		} 
	}

	public bool MovingTowardsObject = false;
	public Node3D TargetedNode { get; protected set; }

	protected const float RayLength = 1000f;

	protected EMovementInputMethod movementInputMethod = EMovementInputMethod.Keyboard;

	protected Timer attackTimer;
	protected bool isLeftClickHeld = false;
	protected bool isRightClickHeld = false;
	protected bool isSkillInput3Held = false;
	protected bool isSkillInput4Held = false;

	public Label debugLabel;
	protected bool newMouseButtonInput = false;
	protected bool controlsCamera = true;

	protected Vector2 lastMouseInputPos = new(0, 0);
	protected Vector3 moveTo = new(0, 0, 0);
	protected float remainingDist = 0f;

	public Dictionary<EStatName, double> ItemStatDictionary = new() {
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
		{ EStatName.IncreasedPhysDamage, 			0 },

		{ EStatName.IncreasedMeleeDamage, 			0 },
		{ EStatName.IncreasedRangedDamage, 			0 },
		{ EStatName.IncreasedSpellDamage, 			0 },

		{ EStatName.IncreasedAttackSpeed, 			0 },
		{ EStatName.IncreasedCritChance, 			0 },
		{ EStatName.AddedCritMulti, 				0 },

		{ EStatName.IncreasedMovementSpeed, 		0 },

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

	protected double StrLifeBonus;
	protected double StrMeleeBonus;
	protected double DexASBonus;
	protected double DexEvasionBonus;
	protected double IntManaBonus;
	protected double IntSpellBonus;

	public Player() {
		BasicStats.BaseLife = 50;
		BasicStats.BaseMana = 40;
		MovementSpeed.SBase = 5;

		UnarmedMinDamage = 3;
		UnarmedMaxDamage = 6;
		UnarmedAttackSpeed = 1;

		RefreshLifeMana();
	}

	public override void _Ready() {
		base._Ready();

		PlayerCamera = GetNode<PlayerCamera>("PlayerCamera");
		PlayerCamera.AssignPlayer(this);

		debugLabel = GetNode<Label>("DebugLabel");
		attackTimer = GetNode<Timer>("AttackTimer");
		PlayerHUD = GetNode<HUD>("HUDLayer/PlayerHUD");
		PlayerHUD.AssignPlayer(this);
		PauseMenu = GetNode<Control>("MenuLayer/PauseMenu");
		moveTo = GlobalPosition;

		Strength.StatTotalChanged += StrTotalChanged;
		Dexterity.StatTotalChanged += DexTotalChanged;
		Intelligence.StatTotalChanged += IntTotalChanged;

		PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
		PlayerHUD.PlayerPanel.CharacterLevelLabel.Text = $"Level {ActorLevel} Creature";

		PlayerHUD.PlayerLowerHUD.SetGoldAmount(Gold);
		PlayerHUD.PlayerLowerHUD.UpdateLevelLabel(ActorLevel);
		PlayerHUD.PlayerLowerHUD.SetExperienceBarLimit(experienceRequirements[ActorLevel - 1]);
		PlayerHUD.PlayerLowerHUD.UpdateExperienceBar(Experience);
		
		CalculateStats();

		AddToGroup("Player");
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton mbe) {
			// On left click outside of UI elements
			if (@event.IsActionPressed("LeftClick")) {
				if (PlayerHUD.PlayerLowerHUD.GetSkillHotbar().IsSkillBeingSelected) {
					PlayerHUD.PlayerLowerHUD.GetSkillHotbar().DestroySkillAssignmentMenu();
				}
				// If an item is currently selected
				if (PlayerHUD.PlayerInventory.IsAnItemSelected && PlayerHUD.PlayerInventory.SelectedItem != null) {
					// If click is outside the inventory panel, drop it on the floor
					if (!PlayerHUD.PlayerInventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !PlayerHUD.PlayerInventory.IsOpen) {
						PlayerHUD.PlayerInventory.ItemClickDrop(PlayerHUD.PlayerInventory.SelectedItem);
					}
				}
				else {
					isLeftClickHeld = true;

					if (movementInputMethod == EMovementInputMethod.Mouse) {
						SetDestinationPosition(mbe.GlobalPosition);
					}
				}
			}
			else if (@event.IsActionReleased("LeftClick")) {
				isLeftClickHeld = false;
			}
			else if (@event.IsActionPressed("RightClick")) {
				isRightClickHeld = true;
			}
			else if (@event.IsActionReleased("RightClick")) {
				isRightClickHeld = false;
			}
		}
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event.IsActionPressed("InventoryKey")) {
			PlayerHUD.ToggleInventory();
		}
		else if (@event.IsActionPressed("CharacterPanelKey")) {
			PlayerHUD.TogglePlayerPanel();
		}
		else if (@event.IsActionPressed("SkillPanelKey")) {
			PlayerHUD.ToggleSkillPanel();
		}
		else if (@event.IsActionPressed("SkillInput3")) {
			isSkillInput3Held = true;
		}
		else if (@event.IsActionReleased("SkillInput3")) {
			isSkillInput3Held = false;
		}
		else if (@event.IsActionPressed("SkillInput4")) {
			isSkillInput4Held = true;
		}
		else if (@event.IsActionReleased("SkillInput4")) {
			isSkillInput4Held = false;
		}
		else if (@event.IsActionPressed("DebugSpawnRandomItem")) {
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.None, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomWeapon")) {
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Weapon, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomArmour")) {
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Armour, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomJewellery")) {
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Jewellery, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnSkillItem")) {
			Game.Instance.GenerateRandomSkillItem(GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugHalveLifeMana")) {
			BasicStats.CurrentLife /= 2;
			BasicStats.CurrentMana /= 2;
		}
		else if (@event.IsActionPressed("DebugRemoveWorlditems")) {
			Game.Instance.RemoveAllWorldItems();
		}
		else if (@event.IsActionPressed("DebugSpawnEnemy")) {
			//Game.Instance.Test();
		}
		else if (@event.IsActionPressed("Pause")) {
			GetTree().Paused = true;
			PauseMenu.Show();
		}
    }

    public void SetDestinationPosition(Vector2 position) {
		MovingTowardsObject = false;
		TargetedNode = null;
		lastMouseInputPos = position;
		newMouseButtonInput = true;
	}

	public void SetDestinationNode(Node3D node) {
		MovingTowardsObject = true;
		TargetedNode = node;
		moveTo = node.GlobalPosition;
		newMouseButtonInput = true;
	}

	public Vector3 GetCameraRaycast() {
		Vector3 from = PlayerCamera.ProjectRayOrigin(GetViewport().GetMousePosition());
		Vector3 to = from + PlayerCamera.ProjectRayNormal(GetViewport().GetMousePosition()) * RayLength;
		PhysicsDirectSpaceState3D state = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, CollisionMask = 3);
		Godot.Collections.Dictionary result = state.IntersectRay(query);
			
		if (result.Count > 0) {
			return result["position"].AsVector3();
		}

		return Vector3.Zero;
	}

	// Logic for mouse movement
	private void HandleMouseMovementInput() {
		if (!MovingTowardsObject) {
			Vector3 from = PlayerCamera.ProjectRayOrigin(lastMouseInputPos);
			Vector3 to = from + PlayerCamera.ProjectRayNormal(lastMouseInputPos) * RayLength;
			PhysicsDirectSpaceState3D state = GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, CollisionMask = 1 << 1);
			Godot.Collections.Dictionary result = state.IntersectRay(query);
			
			if (result.Count > 0) {
				moveTo = result["position"].AsVector3();
				Vector3 direction = GlobalPosition.DirectionTo(moveTo);
				ApplyGroundedVelocity(direction.X, direction.Z);

				Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
				if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
					LookAt(lookAt, null, true);
				}
			}
		}
		else {
			Vector3 direction = GlobalPosition.DirectionTo(moveTo);
			ApplyGroundedVelocity(direction.X, direction.Z);

			Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
			if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
				LookAt(lookAt, null, true);
			}
		}

		newMouseButtonInput = false;
	}

	// Logic for WASD movement
	public void ProcessMovementKeyInput() {
		Vector2 inputDirection = Input.GetVector("MoveLeftKey", "MoveRightKey", "MoveUpKey", "MoveDownKey");
		Vector3 moveDirection = new(inputDirection.X, 0, inputDirection.Y);
		//debugLabel.Text = $"Move Input Vector\nX = {moveDirection.X}\nY = {moveDirection.Y}\nZ = {moveDirection.Z}\nLength: {moveDirection.Length()}";

		// If targeting and moving towards a node, but not pressing anything - otherwise the movement would be interrupted immediately
		if (MovingTowardsObject && moveDirection == Vector3.Zero) {
			return;
		}
		else {
			// Since the perspective is isometric, input is applied at a 45 degree angle to align with what you'd expect from the camera
			moveDirection = moveDirection.Rotated(Vector3.Up, (float)Math.PI / 4);

			ApplyGroundedVelocity(moveDirection.X, moveDirection.Z);
			ResetNodeTarget();
		}
    }

	// Makes the player character face the mouse pointer
	public void FaceMouse() {
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector2I windowSize = GetTree().Root.Size;

		float lookX = mousePosition.X - (windowSize.X / 2);
		float lookZ = mousePosition.Y - (windowSize.Y / 2);

		Vector3 diffVector = new(lookX, 0, lookZ);
		diffVector = diffVector.Normalized();

		if (diffVector != Vector3.Zero) {
			// Since the perspective is isometric, input is applied at a 45 degree angle to align with what you'd expect from the camera
			diffVector = diffVector.Rotated(Vector3.Up, (float)Math.PI / 4);

			// Projects a relative position from the player to rotate towards
			Vector3 lookVector = GlobalPosition + (diffVector * 10);
			LookAt(lookVector, null, true);
		}
	}

    public override void _PhysicsProcess(double delta) {
		ApplyRegen(delta);

		if (movementInputMethod == EMovementInputMethod.Keyboard) {
			ProcessMovementKeyInput();
			FaceMouse();
		}

		if (newMouseButtonInput) {
			HandleMouseMovementInput();
		}

		if (isLeftClickHeld && movementInputMethod == EMovementInputMethod.Keyboard) {
			UseSkill(0);
		}
		else if (isRightClickHeld) {
			UseSkill(1);
		}
		else if (isSkillInput3Held) {
			UseSkill(2);
		}
		else if (isSkillInput4Held) {
			UseSkill(3);
		}

		if (movementInputMethod == EMovementInputMethod.Mouse || MovingTowardsObject) {
			remainingDist = (GlobalPosition with { Y = 0f }).DistanceTo(moveTo with { Y = 0f });

			if (IsInstanceValid(TargetedNode) && TargetedNode != null) {
				if (remainingDist <= (float)MovementSpeed.STotal / 100f) {
					ApplyGroundedVelocity(0f, 0f);

					if (TargetedNode.IsInGroup("WorldItem")) {
						WorldItem wi = (WorldItem)TargetedNode;
						PickupItem(ref wi);
					}
					ResetNodeTarget();
				}
				else {
					Vector3 direction = GlobalPosition.DirectionTo(moveTo);
					ApplyGroundedVelocity(direction.X, direction.Z);
					
					Vector3 vecGrounded = Velocity with { Y = 0f };
					if (vecGrounded.Length() < (float)MovementSpeed.STotal - 0.01f) {
						if (Velocity.Length() != 0) {
							float diff = (float)MovementSpeed.STotal / Velocity.Length();
							Velocity *= diff;
						}
					}
				}
			}
			else {
				ApplyGroundedVelocity(0f, 0f);
				ResetNodeTarget();
			}
		}

		if (!IsOnFloor()) {
			DoGravity(delta);
		}

		MoveAndSlide();

		//debugLabel.Text = $"Velocity: {Velocity.ToString("F2")}\nVel Length: {Velocity.Length():F2}\nRem. Dist: {remainingDist:F2}\nRotation: {RotationDegrees.Y:F2}";
		//debugLabel.Text = $"Rotation: {GlobalRotationDegrees.Y:F2}";
		//GD.Print($"Velocity: {Velocity.ToString("F2")}\nVel Length: {Velocity.Length():F2}\nRem. Dist: {remainingDist:F2}\nRotation: {RotationDegrees.ToString("F2")}");
		//debugLabel.Text += $"\n\nVelocity: {Velocity.ToString("F2")}\nVel Length: {Velocity.Length():F2}\nRem. Dist: {remainingDist:F2}\nRotation: {RotationDegrees.Y:F2}";
	}

	public void AddSkill(Skill skill) {
		Skills.Add(skill);
	}

	public void RemoveSkill(Skill skill) {
		PlayerHUD.PlayerLowerHUD.GetSkillHotbar().ClearInvalidSkills(skill.SkillName);
		Skills.Remove(skill);
	}

	public void UseSkill(int skillNo) {
		if (ActorState == EActorState.Actionable) {
			bool skillUsed = false;
			Skill skill;

			switch (skillNo) {
				case 0:
					skillUsed = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot1.TryUseSkill();
					skill = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot1.AssignedSkill;
					break;
				case 1:
					skillUsed = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot2.TryUseSkill();
					skill = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot2.AssignedSkill;
					break;
				case 2:
					skillUsed = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot3.TryUseSkill();
					skill = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot3.AssignedSkill;
					break;
				case 3: 
					skillUsed = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot4.TryUseSkill();
					skill = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlot4.AssignedSkill;
					break;
				default:
					skillUsed = false;
					skill = null;
					break;
			}

			if (!skillUsed) {
				return;
			}

			ActorState = EActorState.Attacking;

			if (skill != null && skill is IAttack attack) {
				if (IsDualWielding) {
					if (IsUsingMainHandDW) {
						attackTimer.Start(MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
						IsUsingMainHandDW = false;
					}
					else {
						attackTimer.Start(OffHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
						IsUsingMainHandDW = true;
					}
				}
				else {
					IsUsingMainHandDW = true;
					attackTimer.Start(MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
				}
			}
			else if (skill != null && skill is ISpell spell) {
				attackTimer.Start(spell.BaseCastTime / spell.ActiveCastSpeedModifiers.STotal);
			}
		}
	}

	public void OnAttackTimerTimeout() {
		if (ActorState != EActorState.Stunned) {
			ActorState = EActorState.Actionable;
		}
	}

	// For later
	public void OnStunned() {
		if (ActorState == EActorState.Attacking) {
			
		}

		ActorState = EActorState.Stunned;
	}

	// Ditto
	public void OnStunnedTimeout() {
		ActorState = EActorState.Actionable;
	}

	public void ResetNodeTarget() {
		TargetedNode = null;
		MovingTowardsObject = false;
		remainingDist = 0f;
	}

	// Sets velocity for the X and Z axes (grounded) to avoid accidentally setting or changing Y (vertical) velocity
	public void ApplyGroundedVelocity(float velX, float velZ) {
		Vector3 velocity = Velocity;
		velocity.X = velX * (float)MovementSpeed.STotal;
		velocity.Z = velZ * (float)MovementSpeed.STotal;
		Velocity = velocity;
	}

	public bool PickupItem(ref WorldItem item) {
		item.ItemReference.ConvertToInventoryItem(this);
		return true;
	}

	public void DropItemOnFloor(WorldItem worldItem) {
		Game.Instance.DropItem(worldItem, GlobalPosition);
	}

	public void ApplyItemStats(EquipmentSlot slot, Item item) {
		foreach (var stat in item.StatDictionary) {
			ItemStatDictionary[stat.Key] += stat.Value;
		}
		
		CalculateStats();
	}

	public void RemoveItemStats(EquipmentSlot slot, Item item) {
		foreach (var stat in item.StatDictionary) {
			ItemStatDictionary[stat.Key] -= stat.Value;
		}

		CalculateStats();
	}

	public void RecalculateAllItemStats() {
		foreach (var key in ItemStatDictionary.Keys.ToList()) {
			ItemStatDictionary[key] = 0;
		}
		
		foreach (var slot in PlayerHUD.PlayerInventory.GetEquipmentSlots()) {
			if (slot.ItemInSlot != null) {
				foreach (var stat in slot.ItemInSlot.ItemReference.StatDictionary) {
					ItemStatDictionary[stat.Key] += stat.Value;
				}
			}
		}

		CalculateStats();
	}

	protected void StrTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.StrContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateStrBonuses() {
		StrLifeBonus = Strength.STotal * 3;
		StrMeleeBonus = Strength.STotal / 100;
	}

	protected void DexTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.DexContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateDexBonuses() {
		DexASBonus = Dexterity.STotal * 0.5 / 100;
		DexEvasionBonus = Dexterity.STotal / 100;
	}

	protected void IntTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.IntContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateIntBonuses() {
		IntManaBonus = Intelligence.STotal * 2;
		IntSpellBonus = Intelligence.STotal / 100;
	}

	protected void CalculateMaxLifeAndMana() {
		BasicStats.AddedLife = (int)ItemStatDictionary[EStatName.FlatMaxLife] + (int)StrLifeBonus + (lifeGrowth * (ActorLevel - 1));
		BasicStats.IncreasedLife = ItemStatDictionary[EStatName.IncreasedMaxLife];
		BasicStats.AddedMana = (int)ItemStatDictionary[EStatName.FlatMaxMana] + (int)IntManaBonus + (manaGrowth * (ActorLevel - 1));
		BasicStats.IncreasedMana = ItemStatDictionary[EStatName.IncreasedMaxMana];

		BasicStats.AddedLifeRegen = ItemStatDictionary[EStatName.AddedLifeRegen] + 1;
		BasicStats.PercentageLifeRegen = ItemStatDictionary[EStatName.PercentageLifeRegen];
		BasicStats.AddedManaRegen = ItemStatDictionary[EStatName.AddedManaRegen];
		BasicStats.IncreasedManaRegen = ItemStatDictionary[EStatName.IncreasedManaRegen];

		PlayerHUD.PlayerPanel.LifeContainer.SetValue($"{BasicStats.TotalLife}");
		PlayerHUD.PlayerPanel.ManaContainer.SetValue($"{BasicStats.TotalMana}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LifeRegen.SetValue($"{BasicStats.TotalLifeRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ManaRegen.SetValue($"{BasicStats.TotalManaRegen:F1}");
	}

	protected void CalculateStats() {
		Strength.SAdded = ItemStatDictionary[EStatName.FlatStrength];
		UpdateStrBonuses();
		Dexterity.SAdded = ItemStatDictionary[EStatName.FlatDexterity];
		UpdateDexBonuses();
		Intelligence.SAdded = ItemStatDictionary[EStatName.FlatIntelligence];
		UpdateIntBonuses();

		CalculateMaxLifeAndMana();

		AttackSpeedMod.SIncreased = ItemStatDictionary[EStatName.IncreasedAttackSpeed] + DexASBonus;
		CritChanceMod.SIncreased = ItemStatDictionary[EStatName.IncreasedCritChance];
		CritMultiplier.SAdded = ItemStatDictionary[EStatName.AddedCritMulti];

		MovementSpeed.SIncreased = ItemStatDictionary[EStatName.IncreasedMovementSpeed];
		
		DamageMods.Physical.SMinAdded = (int)ItemStatDictionary[EStatName.FlatMinPhysDamage];
		DamageMods.Physical.SMaxAdded = (int)ItemStatDictionary[EStatName.FlatMaxPhysDamage];
		DamageMods.Physical.SIncreased = ItemStatDictionary[EStatName.IncreasedPhysDamage];

		DamageMods.IncreasedMelee = ItemStatDictionary[EStatName.IncreasedMeleeDamage] + StrMeleeBonus;
		DamageMods.IncreasedRanged = ItemStatDictionary[EStatName.IncreasedRangedDamage];
		DamageMods.IncreasedSpell = ItemStatDictionary[EStatName.IncreasedSpellDamage] + IntSpellBonus;

		Armour.SAdded = (int)ItemStatDictionary[EStatName.FlatArmour];
		Armour.SIncreased = ItemStatDictionary[EStatName.IncreasedArmour];
		Evasion.SAdded = (int)ItemStatDictionary[EStatName.FlatEvasion];
		Evasion.SIncreased = ItemStatDictionary[EStatName.IncreasedEvasion] + DexEvasionBonus;

		Resistances.ResPhysical = (int)ItemStatDictionary[EStatName.PhysicalResistance];
		Resistances.ResFire = (int)ItemStatDictionary[EStatName.FireResistance];
		Resistances.ResCold = (int)ItemStatDictionary[EStatName.ColdResistance];
		Resistances.ResLightning = (int)ItemStatDictionary[EStatName.LightningResistance];
		Resistances.ResChaos = (int)ItemStatDictionary[EStatName.ChaosResistance];

		UpdateWeaponStats();
		UpdateStatsPanel();
		UpdateSkillValues();
	}

	public void UpdateWeaponStats() {
		if (MainHand != null) {
			MainHandStats.PhysMinDamage = MainHand.PhysicalMinimumDamage;
			MainHandStats.PhysMaxDamage = MainHand.PhysicalMaximumDamage;
			MainHandStats.FireMinDamage = MainHand.AddedFireMinimumDamage;
			MainHandStats.FireMaxDamage = MainHand.AddedFireMaximumDamage;
			MainHandStats.ColdMinDamage = MainHand.AddedColdMinimumDamage;
			MainHandStats.ColdMaxDamage = MainHand.AddedColdMaximumDamage;
			MainHandStats.LightningMinDamage = MainHand.AddedLightningMinimumDamage;
			MainHandStats.LightningMaxDamage = MainHand.AddedLightningMaximumDamage;
			MainHandStats.ChaosMinDamage = MainHand.AddedChaosMinimumDamage;
			MainHandStats.ChaosMaxDamage = MainHand.AddedChaosMaximumDamage;
			MainHandStats.AttackSpeed = MainHand.AttackSpeed;
			MainHandStats.CritChance = MainHand.CriticalStrikeChance;
		}
		else {
			MainHandStats.PhysMinDamage = UnarmedMinDamage;
			MainHandStats.PhysMaxDamage = UnarmedMaxDamage;
			MainHandStats.FireMinDamage = 0;
			MainHandStats.FireMaxDamage = 0;
			MainHandStats.ColdMinDamage = 0;
			MainHandStats.ColdMaxDamage = 0;
			MainHandStats.LightningMinDamage = 0;
			MainHandStats.LightningMaxDamage = 0;
			MainHandStats.ChaosMinDamage = 0;
			MainHandStats.ChaosMaxDamage = 0;
			MainHandStats.AttackSpeed = UnarmedAttackSpeed;
			MainHandStats.CritChance = UnarmedCritChance;
		}

		if (OffHandItem != null && IsOffHandAWeapon) {
			WeaponItem offHandWeapon = (WeaponItem)OffHandItem;
			OffHandStats.PhysMinDamage = offHandWeapon.PhysicalMinimumDamage;
			OffHandStats.PhysMaxDamage = offHandWeapon.PhysicalMaximumDamage;
			OffHandStats.FireMinDamage = offHandWeapon.AddedFireMinimumDamage;
			OffHandStats.FireMaxDamage = offHandWeapon.AddedFireMaximumDamage;
			OffHandStats.ColdMinDamage = offHandWeapon.AddedColdMinimumDamage;
			OffHandStats.ColdMaxDamage = offHandWeapon.AddedColdMaximumDamage;
			OffHandStats.LightningMinDamage = offHandWeapon.AddedLightningMinimumDamage;
			OffHandStats.LightningMaxDamage = offHandWeapon.AddedLightningMaximumDamage;
			OffHandStats.ChaosMinDamage = offHandWeapon.AddedChaosMinimumDamage;
			OffHandStats.ChaosMaxDamage = offHandWeapon.AddedChaosMaximumDamage;
			OffHandStats.AttackSpeed = offHandWeapon.AttackSpeed;
			OffHandStats.CritChance = offHandWeapon.CriticalStrikeChance;
		}
		else {
			OffHandStats.PhysMinDamage = UnarmedMinDamage;
			OffHandStats.PhysMaxDamage = UnarmedMaxDamage;
			OffHandStats.FireMinDamage = 0;
			OffHandStats.FireMaxDamage = 0;
			OffHandStats.ColdMinDamage = 0;
			OffHandStats.ColdMaxDamage = 0;
			OffHandStats.LightningMinDamage = 0;
			OffHandStats.LightningMaxDamage = 0;
			OffHandStats.ChaosMinDamage = 0;
			OffHandStats.ChaosMaxDamage = 0;
			OffHandStats.AttackSpeed = 0;
			OffHandStats.CritChance = 0;
		}
	}

	protected void UpdateStatsPanel() {
		//PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandPhysDamage.SetValue($"{MainHandStats.PhysMinDamage} - {MainHandStats.PhysMaxDamage}");
		//PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandAttackSpeed.SetValue($"{1 / MainHandStats.AttackSpeed:F2}");
		//PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandCritChance.SetValue($"{MainHandStats.CritChance * 100:F2}%");
		//PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandPhysDamage.SetValue($"{OffHandStats.PhysMinDamage} - {OffHandStats.PhysMaxDamage}");
		//PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandAttackSpeed.SetValue($"{1 / OffHandStats.AttackSpeed:F2}");
		//PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandCritChance.SetValue($"{OffHandStats.CritChance * 100:F2}%");
		PlayerHUD.PlayerPanel.OffenceTabPanel.CritMulti.SetValue($"{Math.Round(CritMultiplier.STotal, 2) * 100}%");

		PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {(1 - GetArmourMitigation(Armour.STotal, ActorLevel)) * 100:F0}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel) * 100:F0}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.PhysRes.SetValue($"{Resistances.ResPhysical}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.FireRes.SetValue($"{Resistances.ResFire}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ColdRes.SetValue($"{Resistances.ResCold}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LightningRes.SetValue($"{Resistances.ResLightning}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ChaosRes.SetValue($"{Resistances.ResChaos}%");
	}

	public void UpdateSkillValues() {
		for (int i = 0; i < Skills.Count; i++) {
			Skills[i].UpdateSkillValues();
		}
	}

	protected override void UpdateLifeDisplay(double newCurrentLife) {
		double newValue = newCurrentLife / BasicStats.TotalLife * 100;
        PlayerHUD.PlayerLowerHUD.UpdateLifeOrb(newValue);
    }

	protected override void UpdateManaDisplay(double newCurrentMana) {
		double newValue = newCurrentMana / BasicStats.TotalMana * 100;
        PlayerHUD.PlayerLowerHUD.UpdateManaOrb(newValue);
    }

	public void AssignMainHand(WeaponItem item) {
		MainHand = item;

		if (MainHand == null || MainHand.ItemWeaponBaseType != EItemWeaponBaseType.WeaponMelee1H) {
			IsDualWielding = false;
		}
	}

	public void AssignOffHand(Item item) {
		if (item == null) {
			OffHandItem = null;
			IsOffHandAWeapon = false;
			IsDualWielding = false;
			PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
		}
		else {
			OffHandItem = item;

			if (OffHandItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
				if (!IsOffHandAWeapon) {
					PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(true);
				}
				IsOffHandAWeapon = true;

				if (MainHand != null) {
					IsDualWielding = true;
				}
			}
			else {
				IsOffHandAWeapon = false;
				IsDualWielding = false;
				PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
			}
		}
	}

	protected void GoldCountChanged() {
		PlayerHUD.PlayerLowerHUD.SetGoldAmount(Gold);
	}

	protected void ExperienceChanged() {
		PlayerHUD.PlayerLowerHUD.UpdateExperienceBar(Experience);
	}

	public void GainExperience(double experienceGain) {
		if (Experience + experienceGain >= experienceRequirements[ActorLevel - 1]) {
			double overflowExp = Experience + experienceGain - experienceRequirements[ActorLevel - 1];
			LevelUp();
			Experience = overflowExp;
		}
		else if (Experience + experienceGain < 0) {
			Experience = 0;
		}
		else {
			Experience += experienceGain;
		}

		ExperienceChanged();
	}

	public void LevelUp() {
		if (ActorLevel < maxLevel) {
			ActorLevel++;
			PlayerHUD.PlayerLowerHUD.SetExperienceBarLimit(experienceRequirements[ActorLevel - 1]);
			PlayerHUD.PlayerLowerHUD.UpdateLevelLabel(ActorLevel);
			PlayerHUD.PlayerPanel.CharacterLevelLabel.Text = $"Level {ActorLevel} Creature";

			CalculateMaxLifeAndMana();
			PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {(1 - GetArmourMitigation(Armour.STotal, ActorLevel)) * 100:F0}%");
			PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel) * 100:F0}%");
		}
	}
}
