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
        25, 30, 35, 40, 45,
        50, 55, 60, 65, 70,
        75, 80, 85, 90, 95,
		100, 100, 100, 100, 100,
		100, 100, 100, 100, 100,
		100, 100, 100, 100, 100,
		100, 100, 100, 100, 100,
		100, 100, 100, 100, 100,
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

	protected Timer skillTimer;
	protected bool isLeftClickHeldForInteraction = false;
	protected bool isLeftClickHeld = false;
	protected bool isRightClickHeld = false;
	protected bool isSkillInput3Held = false;
	protected bool isSkillInput4Held = false;
	protected Skill skillInProgress = null;
	protected int skillInProgressNo = -1;
	protected const double skillMovementSpeedPenalty = 0.4;

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

		{ EStatName.FlatMinFireDamage, 				0 },
		{ EStatName.FlatMaxFireDamage, 				0 },
		{ EStatName.IncreasedFireDamage, 			0 },

		{ EStatName.FlatMinColdDamage, 				0 },
		{ EStatName.FlatMaxColdDamage, 				0 },
		{ EStatName.IncreasedColdDamage, 			0 },

		{ EStatName.FlatMinLightningDamage, 		0 },
		{ EStatName.FlatMaxLightningDamage, 		0 },
		{ EStatName.IncreasedLightningDamage, 		0 },

		{ EStatName.FlatMinChaosDamage, 			0 },
		{ EStatName.FlatMaxChaosDamage, 			0 },
		{ EStatName.IncreasedChaosDamage, 			0 },

		{ EStatName.IncreasedMeleeDamage, 			0 },
		{ EStatName.IncreasedRangedDamage, 			0 },
		{ EStatName.IncreasedSpellDamage, 			0 },

		{ EStatName.IncreasedAttackSpeed, 			0 },
		{ EStatName.IncreasedCastSpeed, 			0 },
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
		skillTimer = GetNode<Timer>("SkillTimer");
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
					CastAndInterpretMouseRaycast(mbe);
				}
			}
			else if (@event.IsActionReleased("LeftClick")) {
				isLeftClickHeld = false;
				isLeftClickHeldForInteraction = false;
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
		else if (@event.IsActionPressed("DebugSpawnRandomItem")) { // NUM0
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.None, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomWeapon")) { // NUM1
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Weapon, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomArmour")) { // NUM2
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Armour, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomJewellery")) { // NUM3
			Game.Instance.GenerateRandomItemFromCategory(EItemCategory.Jewellery, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnSkillItem")) { // NUM4
			Game.Instance.GenerateRandomSkillGem(GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugHalveLifeMana")) { // NUM5
			Gold += 1000;
			//BasicStats.CurrentLife /= 2;
			//BasicStats.CurrentMana /= 2;
		}
		else if (@event.IsActionPressed("DebugRemoveWorlditems")) { // NUM6
			Game.Instance.RemoveAllWorldItems();
		}
		else if (@event.IsActionPressed("DebugSpawnEnemy")) { // NUM7
			Game.Instance.GenerateRandomSupportGem(GlobalPosition);
			//Game.Instance.Test();
		}
		else if (@event.IsActionPressed("Pause")) { // ESC
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

	public Vector3 CreateCameraRaycastAndGetPosition() {
		Vector3 from = PlayerCamera.ProjectRayOrigin(GetViewport().GetMousePosition());
		Vector3 to = from + PlayerCamera.ProjectRayNormal(GetViewport().GetMousePosition()) * RayLength;
		PhysicsDirectSpaceState3D state = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, CollisionMask = 2); // 3 includes walls
		Godot.Collections.Dictionary result = state.IntersectRay(query);
			
		if (result.Count > 0) {
			return result["position"].AsVector3();
		}

		return Vector3.Zero;
	}

	private void CastAndInterpretMouseRaycast(InputEventMouseButton mbe) {
		Vector3 from = PlayerCamera.ProjectRayOrigin(mbe.GlobalPosition);
		Vector3 to = from + PlayerCamera.ProjectRayNormal(mbe.GlobalPosition) * RayLength;
		PhysicsDirectSpaceState3D state = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, CollisionMask = 0b00010010); // 2 + 16, floor + object
		query.CollideWithAreas = true;
		Godot.Collections.Dictionary result = state.IntersectRay(query);

		if (result.Count > 0) {
			Node3D collider = (Node3D)result["collider"];
			if (collider.IsInGroup("Interactable")) {
				isLeftClickHeldForInteraction = true;
				SetDestinationNode(collider);
				GetTree().Root.SetInputAsHandled();
			}
			else if (movementInputMethod == EMovementInputMethod.Mouse) {
				SetDestinationPosition(mbe.GlobalPosition);
				moveTo = result["position"].AsVector3();
				GetTree().Root.SetInputAsHandled();
			}
		}
	}

	// Logic for mouse movement
	private void HandleMouseMovementInput() {
		// Hits after calling SetDestinationPosition
		if (!MovingTowardsObject) {
			Vector3 direction = GlobalPosition.DirectionTo(moveTo);
			ApplyGroundedVelocity(direction.X, direction.Z);

			Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
			if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
				LookAt(lookAt, null, true);
			}
		}
		// Hits after calling SetDestinationNode
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
		if (ActorState != EActorState.Dying || ActorState != EActorState.Dead) {
			ApplyRegen(delta);

			if (movementInputMethod == EMovementInputMethod.Keyboard) {
				ProcessMovementKeyInput();
				FaceMouse();
			}

			if (newMouseButtonInput) {
				HandleMouseMovementInput();
			}

			if (isLeftClickHeld && !isLeftClickHeldForInteraction && movementInputMethod == EMovementInputMethod.Keyboard) {
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

				if (MovingTowardsObject) {
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
						}
					}
					else {
						ApplyGroundedVelocity(0f, 0f);
						ResetNodeTarget();
					}
				}
				else {
					if (remainingDist <= (float)MovementSpeed.STotal / 100f) {
						ApplyGroundedVelocity(0f, 0f);
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
			}

			if (!IsOnFloor()) {
				DoGravity(delta);
			}

			MoveAndSlide();
		}
		
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
		if (PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[skillNo].AssignedSkill != null && ActorState == EActorState.Actionable) {
			Skill skill = PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[skillNo].AssignedSkill;
			bool canUseSkill = skill.CanUseSkill();

			if (!canUseSkill) {
				return;
			}

			skillInProgress = skill;
			skillInProgressNo = skillNo;
			ActorState = EActorState.UsingSkill;
			MovementSpeed.SMore = skillMovementSpeedPenalty;

			if (skill != null && skill is IAttack attack) {
				if (IsDualWielding) {
					if (IsUsingMainHandDW) {
						skillTimer.Start(MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
						IsUsingMainHandDW = false;
					}
					else {
						skillTimer.Start(OffHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
						IsUsingMainHandDW = true;
					}
				}
				else {
					IsUsingMainHandDW = true;
					skillTimer.Start(MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal);
				}
			}
			else if (skill != null && skill is ISpell spell) {
				skillTimer.Start(spell.BaseCastTime / spell.ActiveCastSpeedModifiers.STotal);
			}

			//GD.Print($"Skill Time: {attackTimer.WaitTime:F2}");
		}
	}

	public void OnSkillTimerTimeout() {
		if (ActorState != EActorState.Dying || ActorState != EActorState.Dead || ActorState != EActorState.Stunned) { // Currently, stunned is not a reachable state and can be ignored
			PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[skillInProgressNo].TryUseSkill();

			// A "refire" check occurs here to smoothen out sustained action (and stops movement from stuttering)
			bool isSkillHeld = false;
			int skillNoHeld = -1;
			
			if (movementInputMethod == EMovementInputMethod.Keyboard) {
				if (isLeftClickHeld && PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[0].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 0;
				}
				else if (isRightClickHeld && PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[1].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 1;
				}
				else if (isSkillInput3Held && PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[2].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 2;
				}
				else if (isSkillInput4Held && PlayerHUD.PlayerLowerHUD.GetSkillHotbar().SkillHotbarSlots[3].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 3;
				}
			}

			ActorState = EActorState.Actionable;
			if (isSkillHeld && skillNoHeld != -1) {
				UseSkill(skillNoHeld);
			}
			else {
				skillInProgress = null;
				skillInProgressNo = -1;
				MovementSpeed.SMore = 1;
			}

			//GD.Print($"Held: {isSkillHeld}, index: {skillNoHeld}");
			//GD.Print($"SProg null: {skillInProgress == null}, index: {skillInProgressNo}");
		}
	}

	// For later
	public void OnStunned() {
		if (ActorState == EActorState.UsingSkill) {
			
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
		item.ItemReference.ConvertToInventoryItem(PlayerHUD.PlayerInventory.InventoryGrid, this);
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
		CastSpeedMod.SIncreased = ItemStatDictionary[EStatName.IncreasedCastSpeed];
		CritChanceMod.SIncreased = ItemStatDictionary[EStatName.IncreasedCritChance];
		CritMultiplier.SAdded = ItemStatDictionary[EStatName.AddedCritMulti];

		MovementSpeed.SIncreased = ItemStatDictionary[EStatName.IncreasedMovementSpeed];

		// Skal nok erstattes med et system, hvor jeg kan markere individuelle slags stats som "dirty"
		// Enten det, eller få disse ordentligt delt op
		DamageMods.Physical.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatMinPhysDamage], (int)ItemStatDictionary[EStatName.FlatMaxPhysDamage],
			ItemStatDictionary[EStatName.IncreasedPhysDamage]
		);

		DamageMods.Fire.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatMinFireDamage], (int)ItemStatDictionary[EStatName.FlatMaxFireDamage],
			ItemStatDictionary[EStatName.IncreasedFireDamage]
		);

		DamageMods.Cold.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatMinColdDamage], (int)ItemStatDictionary[EStatName.FlatMaxColdDamage],
			ItemStatDictionary[EStatName.IncreasedColdDamage]
		);

		DamageMods.Lightning.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatMinLightningDamage], (int)ItemStatDictionary[EStatName.FlatMaxLightningDamage],
			ItemStatDictionary[EStatName.IncreasedLightningDamage]
		);

		DamageMods.Chaos.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatMinChaosDamage], (int)ItemStatDictionary[EStatName.FlatMaxChaosDamage],
			ItemStatDictionary[EStatName.IncreasedChaosDamage]
		);

		DamageMods.IncreasedMelee = ItemStatDictionary[EStatName.IncreasedMeleeDamage] + StrMeleeBonus;
		DamageMods.IncreasedRanged = ItemStatDictionary[EStatName.IncreasedRangedDamage];
		DamageMods.IncreasedSpell = ItemStatDictionary[EStatName.IncreasedSpellDamage] + IntSpellBonus;

		Armour.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatArmour], 
			ItemStatDictionary[EStatName.IncreasedArmour]
		);

		Evasion.SetAddedIncreasedMore(
			(int)ItemStatDictionary[EStatName.FlatEvasion],
			ItemStatDictionary[EStatName.IncreasedEvasion] + DexEvasionBonus
		);

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
		// ===== OFFENCE =====
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedPhysDamage.SetValue($"{DamageMods.Physical.SMinAdded:F0} - {DamageMods.Physical.SMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedPhysDamage.SetValue($"{DamageMods.Physical.SIncreased * DamageMods.Physical.SMore:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedFireDamage.SetValue($"{DamageMods.Fire.SMinAdded:F0} - {DamageMods.Fire.SMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedFireDamage.SetValue($"{DamageMods.Fire.SIncreased * DamageMods.Fire.SMore:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedColdDamage.SetValue($"{DamageMods.Cold.SMinAdded:F0} - {DamageMods.Cold.SMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedColdDamage.SetValue($"{DamageMods.Cold.SIncreased * DamageMods.Cold.SMore:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedLightningDamage.SetValue($"{DamageMods.Lightning.SMinAdded:F0} - {DamageMods.Lightning.SMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedLightningDamage.SetValue($"{DamageMods.Lightning.SIncreased * DamageMods.Lightning.SMore:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedChaosDamage.SetValue($"{DamageMods.Chaos.SMinAdded:F0} - {DamageMods.Chaos.SMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedChaosDamage.SetValue($"{DamageMods.Chaos.SIncreased * DamageMods.Chaos.SMore:P0}");

		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedMeleeDamage.SetValue($"{DamageMods.IncreasedMelee * DamageMods.MoreMelee:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedRangedDamage.SetValue($"{DamageMods.IncreasedRanged * DamageMods.MoreRanged:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedSpellDamage.SetValue($"{DamageMods.IncreasedSpell * DamageMods.MoreSpell:P0}");

		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedAttackSpeed.SetValue($"{AttackSpeedMod.STotal - 1:P1}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedCastSpeed.SetValue($"{CastSpeedMod.STotal - 1:P1}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedCritChance.SetValue($"{CritChanceMod.STotal - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.CritMulti.SetValue($"{CritMultiplier.STotal:P0}");

		// ===== DEFENCE =====
		PlayerHUD.PlayerPanel.DefenceTabPanel.MovementSpeed.SetValue($"{MovementSpeed.SIncreased * MovementSpeed.SMore:P0}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {(1 - GetArmourMitigation(Armour.STotal, ActorLevel)):P1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel):P1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.PhysRes.SetValue($"{Resistances.ResPhysical}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.FireRes.SetValue($"{Resistances.ResFire}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ColdRes.SetValue($"{Resistances.ResCold}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LightningRes.SetValue($"{Resistances.ResLightning}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ChaosRes.SetValue($"{Resistances.ResChaos}%");
	}

	public void UpdateSkillValues() {
		for (int i = 0; i < Skills.Count; i++) {
			Skills[i].RecalculateSkillValues();
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
			PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {(1 - GetArmourMitigation(Armour.STotal, ActorLevel)):P1}");
			PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel):P1}");
		}
	}

	public override void OnNoLifeLeft() {
		ActorState = EActorState.Dying;
		Die();
    }

	// For lack of a better term
	public void Die() {
		ActorState = EActorState.Dead;
		ResetNodeTarget();
		Velocity = Vector3.Zero;
	}
}
