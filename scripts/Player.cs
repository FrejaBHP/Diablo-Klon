using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Actor {
	PackedScene testAttackScene = GD.Load<PackedScene>("res://scenes/test_attack_tween.tscn");

	public HUD PlayerHUD;

	private const float RayLength = 1000f;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private EMovementInputMethod movementInputMethod = EMovementInputMethod.Keyboard;

	public bool MovingTowardsObject = false;

	private readonly Stat strength = new(10, true);
	public Stat Strength { get => strength; }

	private readonly Stat dexterity = new(10, true);
    public Stat Dexterity { get => dexterity; }

	private readonly Stat intelligence = new(10, true);
    public Stat Intelligence { get => intelligence; }

	private readonly Stat movementSpeed = new(5, false);
    public Stat MovementSpeed { get => movementSpeed; }

	private Node3D targetedNode;
	public Node3D TargetedNode { get => targetedNode; }

	private Timer attackTimer;
	private bool isAttacking = false;
	private bool isAttackHeld = false;
	private float outgoingEffectAttachmentHeight = 1f;

	//private Node3D testSwordNode;
	//private AnimationPlayer animPlayer;

	private PlayerCamera playerCamera;
	private Label debugLabel;
	private bool newMouseButtonInput = false;
	private bool controlsCamera = true;

	private Vector2 lastMouseInputPos = new(0, 0);
	private Vector3 moveTo = new(0, 0, 0);
	private float remainingDist = 0f;

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

		{ EStatName.IncreasedAttackSpeed, 			0 },
		{ EStatName.IncreasedCritChance, 			0 },

		{ EStatName.IncreasedMovementSpeed, 		0 },

		{ EStatName.FlatArmour, 					0 },
		{ EStatName.IncreasedArmour, 				0 },
		{ EStatName.FlatEvasion, 					0 },
		{ EStatName.IncreasedEvasion, 				0 },
		{ EStatName.FlatEnergyShield, 				0 },
		{ EStatName.IncreasedEnergyShield, 			0 },

		{ EStatName.FireResistance, 				0 },
		{ EStatName.ColdResistance, 				0 },
		{ EStatName.LightningResistance, 			0 },
	};

	// Skal flyttes et andet sted hen senere
	private double offHandMinPhysDamage;
	private double offHandMaxPhysDamage;
	private double offHandAS;
	private double offHandCSC;

	public Player() {
		BasicStats.BaseLife = 40;
		BasicStats.BaseMana = 30;
		RefreshLifeMana();
	}

	public override void _Ready() {
		playerCamera = GetNode<PlayerCamera>("PlayerCamera");
		playerCamera.AssignPlayer(this);

		debugLabel = GetNode<Label>("DebugLabel");
		PlayerHUD = GetNode<HUD>("CanvasLayer/PlayerHUD");
		PlayerHUD.PlayerOwner = this;
		PlayerHUD.PlayerPanel.PlayerOwner = this;
		PlayerHUD.PlayerInventory.PlayerOwner = this;
		PlayerHUD.PlayerLowerHUD.PlayerOwner = this;
		moveTo = GlobalPosition;

		strength.StatTotalChanged += StrTotalChanged;
		dexterity.StatTotalChanged += DexTotalChanged;
		intelligence.StatTotalChanged += IntTotalChanged;

		PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
		
		CalculateStats();

		attackTimer = GetNode<Timer>("AttackTimer");
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton mbe) {
			// On left click outside of UI elements
			if (mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
				// If an item is currently selected
				if (PlayerHUD.PlayerInventory.IsAnItemSelected && PlayerHUD.PlayerInventory.SelectedItem != null) {
					// If click is outside the inventory panel, drop it on the floor
					if (!PlayerHUD.PlayerInventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !PlayerHUD.PlayerInventory.IsOpen) {
						PlayerHUD.PlayerInventory.ItemClickDrop(PlayerHUD.PlayerInventory.SelectedItem);
					}
				}
				else {
					if (movementInputMethod == EMovementInputMethod.Mouse) {
						SetDestinationPosition(mbe.GlobalPosition);
					}
				}
			}
			else if (mbe.ButtonIndex == MouseButton.Right && mbe.IsPressed()) {
				isAttackHeld = true;
			}
			else if (mbe.ButtonIndex == MouseButton.Right && mbe.IsReleased()) {
				isAttackHeld = false;
			}
		}
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event.IsActionPressed("InventoryKey")) {
			PlayerHUD.PlayerInventory.ToggleInventory();
		}
		else if (@event.IsActionPressed("CharacterPanelKey")) {
			PlayerHUD.PlayerPanel.TogglePanel();
		}
		// logik for at spawne items skal flyttes til en mere generel klasse som fx Combat eller Game
		else if (@event.IsActionPressed("DebugSpawnRandomItem")) {
			Item item = ItemGeneration.GenerateItemFromCategory(EItemCategory.None);
			WorldItem worldItem = item.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomWeapon")) {
			Item item = ItemGeneration.GenerateItemFromCategory(EItemCategory.Weapon);
			WorldItem worldItem = item.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomArmour")) {
			Item item = ItemGeneration.GenerateItemFromCategory(EItemCategory.Armour);
			WorldItem worldItem = item.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnRandomJewellery")) {
			Item item = ItemGeneration.GenerateItemFromCategory(EItemCategory.Jewellery);
			WorldItem worldItem = item.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugHalveLifeMana")) {
			BasicStats.CurrentLife /= 2;
			BasicStats.CurrentMana /= 2;
		}
    }

    public void SetDestinationPosition(Vector2 position) {
		MovingTowardsObject = false;
		targetedNode = null;
		lastMouseInputPos = position;
		newMouseButtonInput = true;
	}

	public void SetDestinationNode(Node3D node) {
		MovingTowardsObject = true;
		targetedNode = node;
		moveTo = node.GlobalPosition;
		newMouseButtonInput = true;
	}

	// Logic for mouse movement
	private void HandleMouseMovementInput() {
		if (!MovingTowardsObject) {
			Vector3 from = playerCamera.ProjectRayOrigin(lastMouseInputPos);
			Vector3 to = from + playerCamera.ProjectRayNormal(lastMouseInputPos) * RayLength;
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
		// Since the perspective is isometric, input is applied at a 45 degree angle to align with what you'd expect from the camera
		moveDirection = moveDirection.Rotated(Vector3.Up, (float)Math.PI / 4);

		//debugLabel.Text = $"Move Input Vector\nX = {moveDirection.X}\nY = {moveDirection.Y}\nZ = {moveDirection.Z}\nLength: {moveDirection.Length()}";

		// If targeting and moving towards a node, but not pressing anything - otherwise the movement would be interrupted immediately
		if (MovingTowardsObject && moveDirection == Vector3.Zero) {
			return;
		}
		else {
			ApplyGroundedVelocity(moveDirection.X, moveDirection.Z);
			ResetNodeTarget();
		}
    }

	// Makes the player character face the mouse pointer
	public void FaceMouse() {
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector2I windowSize = GetTree().Root.GetViewport().GetWindow().Size;

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
		ApplyRegen();
		PlayerHUD.PlayerLowerHUD.UpdateOrbs();

		if (movementInputMethod == EMovementInputMethod.Keyboard) {
			ProcessMovementKeyInput();
			FaceMouse();
		}

		if (newMouseButtonInput) {
			HandleMouseMovementInput();
		}

		if (isAttackHeld) {
			TestAttack();
		}

		if (movementInputMethod == EMovementInputMethod.Mouse || MovingTowardsObject) {
			remainingDist = (GlobalPosition with { Y = 0f }).DistanceTo(moveTo with { Y = 0f });

			if (remainingDist <= (float)movementSpeed.STotal / 100f) {
				ApplyGroundedVelocity(0f, 0f);

				if (targetedNode != null) {
					if (targetedNode.IsInGroup("WorldItem")) {
						WorldItem wi = (WorldItem)targetedNode;
						PickupItem(ref wi);
					}
					
					ResetNodeTarget();
				}
			}
			else {
				Vector3 vecGrounded = Velocity with { Y = 0f };
				if (vecGrounded.Length() < (float)movementSpeed.STotal - 0.01f) {
					float diff = (float)movementSpeed.STotal / Velocity.Length();
					Velocity *= diff;
				}
			}
		}

		MoveAndSlide();

		//debugLabel.Text = $"\n\nVelocity: {Velocity}\nVel Length: {Velocity.Length()}\nRem. Dist: {remainingDist}\nRotation: {RotationDegrees.Y}";
	}

	public void TestAttack() {
		if (!isAttacking) {
			isAttacking = true;
			attackTimer.Start(MainHand.AttackSpeed);
			GD.Print($"Started attack timer with duration {MainHand.AttackSpeed:F2} seconds");

			TestAttack testAttack = testAttackScene.Instantiate() as TestAttack;
			GetTree().Root.GetChild(0).AddChild(testAttack);

			testAttack.GlobalPosition = testAttack.Position with { 
				X = GlobalPosition.X, 
				Y = GlobalPosition.Y + outgoingEffectAttachmentHeight, 
				Z = GlobalPosition.Z 
			};
			testAttack.GlobalRotation = GlobalRotation;

			testAttack.StartAttack(2f, 2.5f, 25f);
		}
	}

	public void OnAttackTimerTimeout() {
		isAttacking = false;
	}

	public void ResetNodeTarget() {
		targetedNode = null;
		MovingTowardsObject = false;
		remainingDist = 0f;
	}

	// Sets velocity for the X and Z axes (grounded) to avoid accidentally setting or changing Y (vertical) velocity
	public void ApplyGroundedVelocity(float velX, float velZ) {
		Vector3 velocity = Velocity;
		velocity.X = velX * (float)movementSpeed.STotal;
		velocity.Z = velZ * (float)movementSpeed.STotal;
		Velocity = velocity;
	}

	public bool PickupItem(ref WorldItem item) {
		item.ItemReference.ConvertToInventoryItem(this);

		return true;
	}

	public void DropItem(WorldItem worldItem) {
		Game game = (Game)GetParent();
		CanvasLayer gameObjectLayer = game.GetNode<CanvasLayer>("WorldObjects");

		gameObjectLayer.AddChild(worldItem);
		worldItem.Position = Position;
		worldItem.PostSpawn();
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

	protected void CalculateStats() {
		strength.SAdded = ItemStatDictionary[EStatName.FlatStrength];
		dexterity.SAdded = ItemStatDictionary[EStatName.FlatDexterity];
		intelligence.SAdded = ItemStatDictionary[EStatName.FlatIntelligence];

		BasicStats.AddedLife = (int)ItemStatDictionary[EStatName.FlatMaxLife];
		BasicStats.IncreasedLife = ItemStatDictionary[EStatName.IncreasedMaxLife];
		BasicStats.AddedMana = (int)ItemStatDictionary[EStatName.FlatMaxMana];
		BasicStats.IncreasedMana = ItemStatDictionary[EStatName.IncreasedMaxMana];

		BasicStats.AddedLifeRegen = ItemStatDictionary[EStatName.AddedLifeRegen] + 1;
		BasicStats.PercentageLifeRegen = ItemStatDictionary[EStatName.PercentageLifeRegen];
		BasicStats.AddedManaRegen = ItemStatDictionary[EStatName.AddedManaRegen];
		BasicStats.IncreasedManaRegen = ItemStatDictionary[EStatName.IncreasedManaRegen];

		AttackSpeedMod.SIncreased = ItemStatDictionary[EStatName.IncreasedAttackSpeed];
		CritChanceMod.SIncreased = ItemStatDictionary[EStatName.IncreasedCritChance];

		MovementSpeed.SIncreased = ItemStatDictionary[EStatName.IncreasedMovementSpeed];

		DamageMods.AddedPhysicalMin = (int)ItemStatDictionary[EStatName.FlatMinPhysDamage];
		DamageMods.AddedPhysicalMax = (int)ItemStatDictionary[EStatName.FlatMaxPhysDamage];
		DamageMods.IncreasedPhysical = ItemStatDictionary[EStatName.IncreasedPhysDamage];

		BasicStats.AddedEvasion = (int)ItemStatDictionary[EStatName.FlatEvasion];
		BasicStats.IncreasedEvasion = ItemStatDictionary[EStatName.IncreasedEvasion];

		Resistances.ResFire = (int)ItemStatDictionary[EStatName.FireResistance];
		Resistances.ResCold = (int)ItemStatDictionary[EStatName.ColdResistance];
		Resistances.ResLightning = (int)ItemStatDictionary[EStatName.LightningResistance];

		// TEST
		if (MainHand.Weapon != null) {
			MainHand.PhysMinDamage = (int)Math.Round((MainHand.Weapon.PhysicalMinimumDamage + DamageMods.AddedPhysicalMin) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			MainHand.PhysMaxDamage = (int)Math.Round((MainHand.Weapon.PhysicalMaximumDamage + DamageMods.AddedPhysicalMax) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			MainHand.AttackSpeed = MainHand.Weapon.AttackSpeed / AttackSpeedMod.STotal; // Dårlig løsning. Gør det umuligt at tilføje modifiers senere hen. Split hellere Attack Speed ting op i to
			MainHand.CritChance = MainHand.Weapon.CriticalStrikeChance * CritChanceMod.STotal; // Dårlig løsning. Gør det umuligt at tilføje modifiers senere hen. Split hellere Crit Chance ting op i to
		}
		else {
			MainHand.PhysMinDamage = (int)Math.Round(DamageMods.AddedPhysicalMin * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			MainHand.PhysMaxDamage = (int)Math.Round(DamageMods.AddedPhysicalMax * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			MainHand.AttackSpeed = 1 / AttackSpeedMod.STotal;
			MainHand.CritChance = 5 * CritChanceMod.STotal;
		}

		if (OffHandItem != null && IsOffHandAWeapon) {
			WeaponItem offHandWeapon = (WeaponItem)OffHandItem;
			offHandMinPhysDamage = Math.Round((offHandWeapon.PhysicalMinimumDamage + DamageMods.AddedPhysicalMin) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandMaxPhysDamage = Math.Round((offHandWeapon.PhysicalMaximumDamage + DamageMods.AddedPhysicalMax) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandAS = offHandWeapon.AttackSpeed;
			offHandCSC = offHandWeapon.CriticalStrikeChance;
		}
		else {
			offHandMinPhysDamage = Math.Round(DamageMods.AddedPhysicalMin * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandMaxPhysDamage = Math.Round(DamageMods.AddedPhysicalMax * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandAS = 0;
			offHandCSC = 0;
		}

		UpdateStatsPanel();
	}

	protected void StrTotalChanged(object sender, double newStatTotal) {
		PlayerHUD.PlayerPanel.StrContainer.SetValue($"{newStatTotal}");
	}

	protected void DexTotalChanged(object sender, double newStatTotal) {
		PlayerHUD.PlayerPanel.DexContainer.SetValue($"{newStatTotal}");
	}

	protected void IntTotalChanged(object sender, double newStatTotal) {
		PlayerHUD.PlayerPanel.IntContainer.SetValue($"{newStatTotal}");
	}

	protected void UpdateStatsPanel() {
		PlayerHUD.PlayerPanel.LifeContainer.SetValue($"{BasicStats.TotalLife}");
		PlayerHUD.PlayerPanel.ManaContainer.SetValue($"{BasicStats.TotalMana}");

		PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandPhysDamage.SetValue($"{MainHand.PhysMinDamage} - {MainHand.PhysMaxDamage}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandAttackSpeed.SetValue($"{1 / MainHand.AttackSpeed:F2}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandCritChance.SetValue($"{MainHand.CritChance:F2}%");
		PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandPhysDamage.SetValue($"{offHandMinPhysDamage} - {offHandMaxPhysDamage}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandAttackSpeed.SetValue($"{1 / offHandAS:F2}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandCritChance.SetValue($"{offHandCSC:F2}%");

		PlayerHUD.PlayerPanel.DefenceTabPanel.LifeRegen.SetValue($"{BasicStats.TotalLifeRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ManaRegen.SetValue($"{BasicStats.TotalManaRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{BasicStats.TotalArmour}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{BasicStats.TotalEvasion}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.FireRes.SetValue($"{Resistances.ResFire}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ColdRes.SetValue($"{Resistances.ResCold}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LightningRes.SetValue($"{Resistances.ResLightning}%");
	}

	public void AssignMainHand(WeaponItem item) {
		MainHand.Weapon = item;
	}

	public void AssignOffHand(Item item) {
		if (item == null) {
			OffHandItem = null;
			IsOffHandAWeapon = false;
			PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
		}
		else {
			OffHandItem = item;

			if (OffHandItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
				if (!IsOffHandAWeapon) {
					PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(true);
				}
				IsOffHandAWeapon = true;
			}
			else {
				IsOffHandAWeapon = false;
				PlayerHUD.PlayerPanel.OffenceTabPanel.SetOffhandVisibility(false);
			}
		}
	}
}
