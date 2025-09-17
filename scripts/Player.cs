using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Actor {
	public HUD PlayerHUD;
	public Control PauseMenu;
	public PlayerCamera PlayerCamera;

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

	public Stat Strength { get; protected set; } = new(0, true);
    public Stat Dexterity { get; protected set; } = new(0, true);
    public Stat Intelligence { get; protected set; } = new(0, true);

	protected int lifeGrowth = 5;
	protected int manaGrowth = 3;

	protected double StrLifeBonus;
	protected double StrMeleeBonus;
	protected double DexASBonus;
	protected double DexEvasionBonus;
	protected double IntManaBonus;
	protected double IntSpellBonus;

	public List<EPlayerClass> PlayerClasses = new();

	public Dictionary<EStatName, double> ItemStatDictionary = new();

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

		PlayerHUD.PlayerPanel.CharacterLevelLabel.Text = $"Level {ActorLevel} Creature";

		PlayerHUD.LowerHUD.SetGoldAmount(Gold);
		PlayerHUD.LowerHUD.UpdateLevelLabel(ActorLevel);
		PlayerHUD.LowerHUD.SetExperienceBarLimit(experienceRequirements[ActorLevel - 1]);
		PlayerHUD.LowerHUD.UpdateExperienceBar(Experience);
		
		PlayerHUD.PassiveTreePanel.PassiveTreeChanged += ResetAndMergeStatDictionaries;
		ResetAndMergeStatDictionaries();

		AddToGroup("Player");
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton mbe) {
			// On left click outside of UI elements
			if (@event.IsActionPressed("LeftClick")) {
				if (PlayerHUD.LowerHUD.GetSkillHotbar().IsSkillBeingSelected) {
					PlayerHUD.LowerHUD.GetSkillHotbar().DestroySkillAssignmentMenu();
				}
				// If an item is currently selected
				if (PlayerHUD.Inventory.IsAnItemSelected && PlayerHUD.Inventory.SelectedItem != null) {
					// If click is outside the inventory panel, drop it on the floor
					if (!PlayerHUD.Inventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !PlayerHUD.Inventory.IsOpen) {
						PlayerHUD.Inventory.ItemClickDrop(PlayerHUD.Inventory.SelectedItem);
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
		else if (@event.IsActionPressed("PassiveTreePanelKey")) {
			PlayerHUD.TogglePassiveTree();

			if (Velocity.Length() > 0 && !MovingTowardsObject) {
				ApplyGroundedVelocity(0f, 0f);
			}
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
			Run.Instance.GenerateRandomItemFromCategory(EItemCategory.None, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomWeapon")) { // NUM1
			Run.Instance.GenerateRandomItemFromCategory(EItemCategory.Weapon, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomArmour")) { // NUM2
			Run.Instance.GenerateRandomItemFromCategory(EItemCategory.Armour, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomJewellery")) { // NUM3
			Run.Instance.GenerateRandomItemFromCategory(EItemCategory.Jewellery, GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugSpawnSkillItem")) { // NUM4
			Run.Instance.GenerateRandomSkillGem(GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugHalveLifeMana")) { // NUM5
			Gold += 1000;
			//BasicStats.CurrentLife /= 2;
			//BasicStats.CurrentMana /= 2;
		}
		else if (@event.IsActionPressed("DebugRemoveWorlditems")) { // NUM6
			Run.Instance.RemoveAllWorldItems();
		}
		else if (@event.IsActionPressed("DebugSpawnEnemy")) { // NUM7
			Run.Instance.GenerateRandomSupportGem(GlobalPosition);
		}
		else if (@event.IsActionPressed("DebugIncGemLevel")) { // KP_ADD
			Run.Instance.GemLevel++;
		}
		else if (@event.IsActionPressed("DebugDecGemLevel")) { // KP_MIN
			Run.Instance.GemLevel--;
		}
		else if (@event.IsActionPressed("DebugOpenSkillSelectionPanel")) { // KP_MUL
			PlayerHUD.PassiveTreePanel.CreateClassSelectionPanel();
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
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, 0b00000010); // including 1st bit also flags walls
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
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, 0b00010010); // 2 + 16, floor + object
		query.CollideWithAreas = true;
		Godot.Collections.Dictionary result = state.IntersectRay(query);

		if (result.Count > 0) {
			Node3D collider = (Node3D)result["collider"];
			if (collider.IsInGroup("Interactable")) {
				isLeftClickHeldForInteraction = true;

				if (collider.IsInGroup("Shop")) {
					Shop shop = collider as Shop;
					shop.OnClickedByPlayer(this);
				}
				else {
					SetDestinationNode(collider);
				}

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
			TickEffects(delta);
			TakeDamageOverTime();

			if (movementInputMethod == EMovementInputMethod.Keyboard) {
				// It looks weird without when the passive tree is open and transparent, so I'm keeping it here for emotional support
				FaceMouse();
			}

			if (!PlayerHUD.PassiveTreePanel.Visible) {
				if (movementInputMethod == EMovementInputMethod.Keyboard) {
					ProcessMovementKeyInput();
					//FaceMouse();
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

			DoGravity(delta);
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
		PlayerHUD.LowerHUD.GetSkillHotbar().ClearInvalidSkills(skill.SkillName);
		Skills.Remove(skill);
	}

	public void UseSkill(int skillNo) {
		if (PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[skillNo].AssignedSkill != null && ActorState == EActorState.Actionable) {
			Skill skill = PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[skillNo].AssignedSkill;
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
			PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[skillInProgressNo].TryUseSkill();

			// A "refire" check occurs here to smoothen out sustained action (and stops movement from stuttering)
			bool isSkillHeld = false;
			int skillNoHeld = -1;
			
			if (movementInputMethod == EMovementInputMethod.Keyboard) {
				if (isLeftClickHeld && PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[0].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 0;
				}
				else if (isRightClickHeld && PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[1].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 1;
				}
				else if (isSkillInput3Held && PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[2].AssignedSkill != null) {
					isSkillHeld = true;
					skillNoHeld = 2;
				}
				else if (isSkillInput4Held && PlayerHUD.LowerHUD.GetSkillHotbar().SkillHotbarSlots[3].AssignedSkill != null) {
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
		item.ItemReference.ConvertToInventoryItem(PlayerHUD.Inventory.InventoryGrid, this);
		return true;
	}

	public void DropItemOnFloor(WorldItem worldItem) {
		Run.Instance.DropItem(worldItem, GlobalPosition);
	}

	public void ApplyItemStats(EquipmentSlot slot, Item item) {
		foreach (var stat in item.StatDictionary) {
			if (ItemStatDictionary.ContainsKey(stat.Key)) {
				if (Utilities.MultiplicativeStatNames.Contains(stat.Key)) {
					ItemStatDictionary[stat.Key] *= stat.Value;
				}
				else {
					ItemStatDictionary[stat.Key] += stat.Value;
				}
			}
			else {
				ItemStatDictionary.TryAdd(stat.Key, stat.Value);
			}
		}
		
		ResetAndMergeStatDictionaries();
	}

	public void RemoveItemStats(EquipmentSlot slot, Item item) {
		foreach (KeyValuePair<EStatName, double> stat in item.StatDictionary) {
			if (ItemStatDictionary.ContainsKey(stat.Key)) {
				if (Utilities.MultiplicativeStatNames.Contains(stat.Key)) {
					if (stat.Value != 0) {
						ItemStatDictionary[stat.Key] /= stat.Value;
					}
					else {
						GD.PrintErr($"Tried to divide by 0 from stat {stat.Key} ");
					}
				}
				else {
					ItemStatDictionary[stat.Key] -= stat.Value;
				}
			}
			else {
				GD.PrintErr($"Can't remove from stat {stat.Key} from Item Stat Dictionary, because it does not exist");
			}
		}

		ResetAndMergeStatDictionaries();
	}

	public void RecalculateAllItemStats() {
		ItemStatDictionary.Clear();
		
		foreach (EquipmentSlot slot in PlayerHUD.Inventory.GetEquipmentSlots()) {
			if (slot.ItemInSlot != null) {
				foreach (KeyValuePair<EStatName, double> stat in slot.ItemInSlot.ItemReference.StatDictionary) {
					ItemStatDictionary.TryAdd(stat.Key, stat.Value);
				}
			}
		}

		ResetAndMergeStatDictionaries();
	}

	public override void ResetAndMergeStatDictionaries() {
		foreach (EStatName key in StatDictionary.Keys.ToList()) {
			StatDictionary[key] = 0;
		}

		foreach (EStatName key in MultiplicativeStatDictionary.Keys.ToList()) {
			MultiplicativeStatDictionary[key] = 1;
		}

		foreach (KeyValuePair<EStatName, double> stat in ItemStatDictionary) {
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

		foreach (KeyValuePair<EStatName, double> stat in PlayerHUD.PassiveTreePanel.PassiveTreeStatDictionary) {
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

		// Entire thing isn't exactly efficient, so take another look later, please

		ActorFlags = PlayerHUD.PassiveTreePanel.PassiveTreeActorFlags;

		CalculateStats();
	}

	protected void StrTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.StrContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateStrBonuses() {
		StrLifeBonus = Strength.STotal * 2; 		// +2 Max Life per Strength
		StrMeleeBonus = Strength.STotal / 100; 		// +1% Melee Damage per Strength
	}

	protected void DexTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.DexContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateDexBonuses() {
		DexASBonus = Dexterity.STotal / 200;		// +0,5% Attack Speed per Dexterity
		DexEvasionBonus = Dexterity.STotal / 100;	// +1% Evasion Rating per Dexterity
	}

	protected void IntTotalChanged(double newStatTotal) {
		PlayerHUD.PlayerPanel.IntContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateIntBonuses() {
		IntManaBonus = Intelligence.STotal * 2;		// +2 Max Mana per Intelligence
		IntSpellBonus = Intelligence.STotal / 100;	// +1% Spell Damage per Intelligence
	}

	protected void CalculateMaxLifeAndMana() {
		BasicStats.AddedLife = (int)StatDictionary[EStatName.FlatMaxLife] + (int)StrLifeBonus + (lifeGrowth * (ActorLevel - 1));
		BasicStats.IncreasedLife = StatDictionary[EStatName.IncreasedMaxLife];
		BasicStats.MoreLife = MultiplicativeStatDictionary[EStatName.MoreMaxLife];
		BasicStats.AddedMana = (int)StatDictionary[EStatName.FlatMaxMana] + (int)IntManaBonus + (manaGrowth * (ActorLevel - 1));
		BasicStats.IncreasedMana = StatDictionary[EStatName.IncreasedMaxMana];
		BasicStats.MoreMana = MultiplicativeStatDictionary[EStatName.MoreMaxMana];

		BasicStats.AddedLifeRegen = StatDictionary[EStatName.AddedLifeRegen] + 1;
		BasicStats.PercentageLifeRegen = StatDictionary[EStatName.PercentageLifeRegen];
		BasicStats.AddedManaRegen = StatDictionary[EStatName.AddedManaRegen];
		BasicStats.IncreasedManaRegen = StatDictionary[EStatName.IncreasedManaRegen];

		PlayerHUD.PlayerPanel.LifeContainer.SetValue($"{BasicStats.TotalLife}");
		PlayerHUD.PlayerPanel.ManaContainer.SetValue($"{BasicStats.TotalMana}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LifeRegen.SetValue($"{BasicStats.TotalLifeRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ManaRegen.SetValue($"{BasicStats.TotalManaRegen:F1}");
	}

	// ======================================================================================================
	// Evt. ting til optimering af stat-relaterede opgaver:
	// Der kunne indføres en funktion til Actor, der tager et StatName fra en tilføjelse til StatDictionary
	// Denne funktion beder så en anden funktion om at omregne *kun* den statistik, der blev ændret
	// Det kunne reducere mængden af beregninger en hel del, højst sandsynligt
	// ======================================================================================================

	protected void CalculateStats() {
		Strength.SAdded = StatDictionary[EStatName.FlatStrength];
		UpdateStrBonuses();
		Dexterity.SAdded = StatDictionary[EStatName.FlatDexterity];
		UpdateDexBonuses();
		Intelligence.SAdded = StatDictionary[EStatName.FlatIntelligence];
		UpdateIntBonuses();

		CalculateMaxLifeAndMana();

		AttackSpeedMod.SIncreased = StatDictionary[EStatName.IncreasedAttackSpeed] + DexASBonus;
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

		// Skal nok erstattes med et system, hvor jeg kan markere individuelle slags stats som "dirty"
		// Enten det, eller få disse ordentligt delt op
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
		DamageMods.IncreasedSpell = StatDictionary[EStatName.IncreasedSpellDamage] + IntSpellBonus;
		DamageMods.MoreSpell = MultiplicativeStatDictionary[EStatName.MoreSpellDamage];
		DamageMods.IncreasedMelee = StatDictionary[EStatName.IncreasedMeleeDamage] + StrMeleeBonus;
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

		if (IsMainHandTwoHandedMelee) {
			DamageMods.IncreasedAll += StatDictionary[EStatName.IncreasedDamageTwoHanded];
			DamageMods.MoreAll *= MultiplicativeStatDictionary[EStatName.MoreDamageTwoHanded];
			AreaOfEffect.SIncreased += StatDictionary[EStatName.IncreasedAreaOfEffectTwoHanded];
			AreaOfEffect.SMore *= MultiplicativeStatDictionary[EStatName.MoreAreaOfEffectTwoHanded];
		}

		if (IsDualWielding) {
			DamageMods.IncreasedAll += StatDictionary[EStatName.IncreasedDamageDualWield];
			DamageMods.MoreAll *= MultiplicativeStatDictionary[EStatName.MoreDamageDualWield];
			AttackSpeedMod.SIncreased += StatDictionary[EStatName.IncreasedAttackSpeedDualWield];
			AttackSpeedMod.SMore *= MultiplicativeStatDictionary[EStatName.MoreAttackSpeedDualWield];
			CastSpeedMod.SIncreased += StatDictionary[EStatName.IncreasedCastSpeedDualWield];
			CastSpeedMod.SMore *= MultiplicativeStatDictionary[EStatName.MoreCastSpeedDualWield];
		}

		if (IsOffHandAShield) {
			DamageMods.IncreasedAll += StatDictionary[EStatName.IncreasedDamageWithShield];
			DamageMods.MoreAll *= MultiplicativeStatDictionary[EStatName.MoreDamageWithShield];
			AttackSpeedMod.SIncreased += StatDictionary[EStatName.IncreasedSkillSpeedShield];
			AttackSpeedMod.SMore *= MultiplicativeStatDictionary[EStatName.MoreSkillSpeedShield];
			CastSpeedMod.SIncreased += StatDictionary[EStatName.IncreasedSkillSpeedShield];
			CastSpeedMod.SMore *= MultiplicativeStatDictionary[EStatName.MoreSkillSpeedShield];
		}

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
			StatDictionary[EStatName.IncreasedEvasion] + DexEvasionBonus,
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

		// De fleste af disse eksisterer primært for debugging og burde fjernes senere for at reducere den enorme mængde tal
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedAttackPhysDamage.SetValue($"{DamageMods.Physical.SMinAdded + DamageMods.Physical.SAttackMinAdded:F0} - {DamageMods.Physical.SMaxAdded + DamageMods.Physical.SAttackMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedSpellPhysDamage.SetValue($"{DamageMods.Physical.SMinAdded + DamageMods.Physical.SSpellMinAdded:F0} - {DamageMods.Physical.SMaxAdded + DamageMods.Physical.SSpellMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedPhysDamage.SetValue($"{(1 + DamageMods.Physical.SIncreased) * DamageMods.Physical.SMore - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.PhysicalPenetration.SetValue($"{Penetrations.Physical}%");

		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedAttackFireDamage.SetValue($"{DamageMods.Fire.SMinAdded + DamageMods.Fire.SAttackMinAdded:F0} - {DamageMods.Fire.SMaxAdded + DamageMods.Fire.SAttackMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedSpellFireDamage.SetValue($"{DamageMods.Fire.SMinAdded + DamageMods.Fire.SSpellMinAdded:F0} - {DamageMods.Fire.SMaxAdded + DamageMods.Fire.SSpellMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedFireDamage.SetValue($"{(1 + DamageMods.Fire.SIncreased) * DamageMods.Fire.SMore - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.FirePenetration.SetValue($"{Penetrations.Fire}%");

		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedAttackColdDamage.SetValue($"{DamageMods.Cold.SMinAdded + DamageMods.Cold.SAttackMinAdded:F0} - {DamageMods.Cold.SMaxAdded + DamageMods.Cold.SAttackMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedSpellColdDamage.SetValue($"{DamageMods.Cold.SMinAdded + DamageMods.Cold.SSpellMinAdded:F0} - {DamageMods.Cold.SMaxAdded + DamageMods.Cold.SSpellMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedColdDamage.SetValue($"{(1 + DamageMods.Cold.SIncreased) * DamageMods.Cold.SMore - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.ColdPenetration.SetValue($"{Penetrations.Cold}%");

		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedAttackLightningDamage.SetValue($"{DamageMods.Lightning.SMinAdded + DamageMods.Lightning.SAttackMinAdded:F0} - {DamageMods.Lightning.SMaxAdded + DamageMods.Lightning.SAttackMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedSpellLightningDamage.SetValue($"{DamageMods.Lightning.SMinAdded + DamageMods.Lightning.SSpellMinAdded:F0} - {DamageMods.Lightning.SMaxAdded + DamageMods.Lightning.SSpellMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedLightningDamage.SetValue($"{(1 + DamageMods.Lightning.SIncreased) * DamageMods.Lightning.SMore - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.LightningPenetration.SetValue($"{Penetrations.Lightning}%");

		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedAttackChaosDamage.SetValue($"{DamageMods.Chaos.SMinAdded + DamageMods.Chaos.SAttackMinAdded:F0} - {DamageMods.Chaos.SMaxAdded + DamageMods.Chaos.SAttackMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.AddedSpellChaosDamage.SetValue($"{DamageMods.Chaos.SMinAdded + DamageMods.Chaos.SSpellMinAdded:F0} - {DamageMods.Chaos.SMaxAdded + DamageMods.Chaos.SSpellMaxAdded:F0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedChaosDamage.SetValue($"{(1 + DamageMods.Chaos.SIncreased) * DamageMods.Chaos.SMore - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.ChaosPenetration.SetValue($"{Penetrations.Chaos}%");

		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedAttackDamage.SetValue($"{(1 + DamageMods.IncreasedAttack) * DamageMods.MoreAttack - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedSpellDamage.SetValue($"{(1 + DamageMods.IncreasedSpell) * DamageMods.MoreSpell - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedMeleeDamage.SetValue($"{(1 + DamageMods.IncreasedMelee) * DamageMods.MoreMelee - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedProjectileDamage.SetValue($"{(1 + DamageMods.IncreasedProjectile) * DamageMods.MoreProjectile - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedAreaDamage.SetValue($"{(1 + DamageMods.IncreasedArea) * DamageMods.MoreArea - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedDamageOverTime.SetValue($"{(1 + DamageMods.IncreasedDoT) * DamageMods.MoreDoT - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedAllDamage.SetValue($"{(1 + DamageMods.IncreasedAll) * DamageMods.MoreAll - 1:P0}");

		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedBleedDamage.SetValue($"{(1 + DamageMods.IncreasedBleedMagnitude) * DamageMods.MoreBleedMagnitude * (1 + StatusMods.Bleed.SFasterTicking) - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedBleedDuration.SetValue($"{StatusMods.Bleed.CalculateDurationModifier() - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedIgniteDamage.SetValue($"{(1 + DamageMods.IncreasedIgniteMagnitude) * DamageMods.MoreIgniteMagnitude * (1 + StatusMods.Ignite.SFasterTicking) - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedIgniteDuration.SetValue($"{StatusMods.Ignite.CalculateDurationModifier() - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedPoisonDamage.SetValue($"{(1 + DamageMods.IncreasedPoisonMagnitude) * DamageMods.MorePoisonMagnitude * (1 + StatusMods.Poison.SFasterTicking) - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedPoisonDuration.SetValue($"{StatusMods.Poison.CalculateDurationModifier() - 1:P0}");

		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedAttackSpeed.SetValue($"{AttackSpeedMod.STotal - 1:P1}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedCastSpeed.SetValue($"{CastSpeedMod.STotal - 1:P1}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.IncreasedCritChance.SetValue($"{CritChanceMod.STotal - 1:P0}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.CritMulti.SetValue($"{CritMultiplier.STotal:P0}");

		// ===== DEFENCE =====
		PlayerHUD.PlayerPanel.DefenceTabPanel.MovementSpeed.SetValue($"{MovementSpeed.SIncreased:P0}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {1 - GetArmourMitigation(Armour.STotal, ActorLevel):P1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel):P1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.BlockChance.SetValue($"{BlockChance.STotal:P0}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.BlockEffectiveness.SetValue($"{BlockEffectiveness.STotal:P0}");
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
        PlayerHUD.LowerHUD.UpdateLifeOrb(newValue);
    }

	protected override void UpdateManaDisplay(double newCurrentMana) {
		double newValue = newCurrentMana / BasicStats.TotalMana * 100;
        PlayerHUD.LowerHUD.UpdateManaOrb(newValue);
    }

	public void AssignMainHand(WeaponItem item) {
		MainHand = item;

		if (MainHand != null) {
			if (MainHand.ItemWeaponBaseType != EItemWeaponBaseType.WeaponMelee1H) {
				IsDualWielding = false;
			}

			if (MainHand.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee2H) {
				IsMainHandTwoHandedMelee = true;
			}
			else {
				IsMainHandTwoHandedMelee = false;
			}
		}
		else {
			IsDualWielding = false;
			IsMainHandTwoHandedMelee = false;
		}
	}

	public void AssignOffHand(Item item) {
		if (item == null) {
			OffHandItem = null;
			IsOffHandAWeapon = false;
			IsOffHandAShield = false;
			IsDualWielding = false;
			//PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
		}
		else {
			OffHandItem = item;

			if (OffHandItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
				if (!IsOffHandAWeapon) {
					//PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(true);
				}
				IsOffHandAWeapon = true;
				IsOffHandAShield = false;

				if (MainHand != null) {
					IsDualWielding = true;
				}
			}
			else if (OffHandItem.ItemAllBaseType == EItemAllBaseType.Shield) {
				IsOffHandAShield = true;
				IsOffHandAWeapon = false;
				IsDualWielding = false;
			}
			else {
				IsOffHandAWeapon = false;
				IsOffHandAShield = false;
				IsDualWielding = false;
				//PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
			}
		}
	}

	protected void GoldCountChanged() {
		PlayerHUD.LowerHUD.SetGoldAmount(Gold);
	}

	protected void ExperienceChanged() {
		PlayerHUD.LowerHUD.UpdateExperienceBar(Experience);
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
			PlayerHUD.LowerHUD.SetExperienceBarLimit(experienceRequirements[ActorLevel - 1]);
			PlayerHUD.LowerHUD.UpdateLevelLabel(ActorLevel);
			PlayerHUD.PlayerPanel.CharacterLevelLabel.Text = $"Level {ActorLevel} Creature";

			CalculateMaxLifeAndMana();
			PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{Math.Round(Armour.STotal, 0)} / {(1 - GetArmourMitigation(Armour.STotal, ActorLevel)):P1}");
			PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{Math.Round(Evasion.STotal, 0)} / {GetEvasionChance(Evasion.STotal, ActorLevel):P1}");

			PlayerHUD.PassiveTreePanel.PassiveTreePoints++;
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
