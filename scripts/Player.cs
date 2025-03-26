using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Actor {
	public HUD PlayerHUD;

	private const float RayLength = 1000f;
	public const float Speed = 5.0f;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public bool MovingTowardsObject = false;

	private readonly Stat strength = new(10, true);
	public Stat Strength { get => strength; }

	private readonly Stat dexterity = new(10, true);
    public Stat Dexterity { get => dexterity; }

	private readonly Stat intelligence = new(10, true);
    public Stat Intelligence { get => intelligence; }

	private PlayerCamera playerCamera;
	private Label debugLabel;
	private bool newMouseInput = false;
	private bool controlsCamera = true;

	private Vector2 lastMouseInputPos = new(0, 0);
	private Vector3 moveTo = new(0, 0, 0);
	private float remainingDist = 0f;
	private Node3D targetedNode;

	public Dictionary<EStatName, double> ItemStatDictionary = new() {
		{ EStatName.FlatStrength, 					0 },
		{ EStatName.FlatDexterity, 					0 },
		{ EStatName.FlatIntelligence, 				0 },

		{ EStatName.FlatMaxLife, 					0 },
		{ EStatName.PercentageMaxLife, 				0 },
		{ EStatName.AddedLifeRegen, 				0 },
		{ EStatName.PercentageLifeRegen, 			0 },

		{ EStatName.FlatMaxMana, 					0 },
		{ EStatName.PercentageMaxMana, 				0 },
		{ EStatName.AddedManaRegen, 				0 },
		{ EStatName.IncreasedManaRegen, 			0 },

		{ EStatName.FlatMinPhysDamage, 				0 },
		{ EStatName.FlatMaxPhysDamage, 				0 },
		{ EStatName.PercentagePhysDamage, 			0 },

		{ EStatName.FlatArmour, 					0 },
		{ EStatName.PercentageArmour, 				0 },
		{ EStatName.FlatEvasion, 					0 },
		{ EStatName.PercentageEvasion, 				0 },
		{ EStatName.FlatEnergyShield, 				0 },
		{ EStatName.PercentageEnergyShield, 		0 },

		{ EStatName.FireResistance, 				0 },
		{ EStatName.ColdResistance, 				0 },
		{ EStatName.LightningResistance, 			0 },
	};

	// Skal flyttes et andet sted hen senere
	private double mainHandMinPhysDamage;
	private double mainHandMaxPhysDamage;
	private double offHandMinPhysDamage;
	private double offHandMaxPhysDamage;

	public Player() {
		BasicStats.BaseLife = 40;
		BasicStats.BaseMana = 30;
		BasicStats.AddedLifeRegen = 1;
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
		
		CalculateStats();
	}

    public override void _UnhandledInput(InputEvent @event) {
		// On left click outside of UI elements
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			// If an item is currently selected
			if (PlayerHUD.PlayerInventory.IsAnItemSelected && PlayerHUD.PlayerInventory.SelectedItem != null) {
                // If click is outside the inventory panel, drop it on the floor
                if (!PlayerHUD.PlayerInventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !PlayerHUD.PlayerInventory.IsOpen) {
                    PlayerHUD.PlayerInventory.ItemClickDrop(PlayerHUD.PlayerInventory.SelectedItem);
                }
            }
			else {
				SetDestinationPosition(mbe.GlobalPosition);
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
    }

    public void SetDestinationPosition(Vector2 position) {
		MovingTowardsObject = false;
		targetedNode = null;
		lastMouseInputPos = position;
		newMouseInput = true;
	}

	public void SetDestinationNode(Node3D node) {
		MovingTowardsObject = true;
		targetedNode = node;
		moveTo = node.GlobalPosition;
		newMouseInput = true;
	}

	private void HandleMouseInput() {
		if (!MovingTowardsObject) {
			Vector3 from = playerCamera.ProjectRayOrigin(lastMouseInputPos);
			Vector3 to = from + playerCamera.ProjectRayNormal(lastMouseInputPos) * RayLength;
			PhysicsDirectSpaceState3D state = GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to, CollisionMask = 1 << 1);
			Godot.Collections.Dictionary result = state.IntersectRay(query);
			
			if (result.Count > 0) {
				moveTo = result["position"].AsVector3();
				Vector3 direction = GlobalPosition.DirectionTo(moveTo);
				Velocity = direction * Speed;

				Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
				if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
					LookAt(lookAt, null, true);
				}
			}
		}
		else {
			Vector3 direction = GlobalPosition.DirectionTo(moveTo);
			Velocity = direction * Speed;

			Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
			if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
				LookAt(lookAt, null, true);
			}
		}

		newMouseInput = false;
	}

    public override void _PhysicsProcess(double delta) {
		ApplyRegen();
		PlayerHUD.PlayerLowerHUD.UpdateOrbs();

		if (newMouseInput) {
			HandleMouseInput();
		}

		remainingDist = (GlobalPosition with { Y = 0f }).DistanceTo(moveTo with { Y = 0f });

		if (remainingDist <= Speed / 100f) {
			Velocity = Vector3.Zero;

			if (MovingTowardsObject && targetedNode != null) {
				if (targetedNode.IsInGroup("WorldItem")) {
					WorldItem wi = (WorldItem)targetedNode;
					PickupItem(ref wi);
				}
				MovingTowardsObject = false;
			}
		}
		else {
			if (Velocity.Length() < Speed - 0.01f) {
				float diff = Speed / Velocity.Length();
				Velocity *= diff;
			}
		}

		MoveAndSlide();

		//debugLabel.Text = $"Velocity: {Velocity}\nVel Length: {Velocity.Length()}\nRem. Dist: {remainingDist}\nRotation: {RotationDegrees.Y}";
	}

	public bool PickupItem(ref WorldItem item) {
		targetedNode = null;
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
		BasicStats.IncreasedLife = ItemStatDictionary[EStatName.PercentageMaxLife];
		BasicStats.AddedMana = (int)ItemStatDictionary[EStatName.FlatMaxMana];
		BasicStats.IncreasedMana = ItemStatDictionary[EStatName.PercentageMaxMana];

		BasicStats.AddedLifeRegen = ItemStatDictionary[EStatName.AddedLifeRegen];
		BasicStats.PercentageLifeRegen = ItemStatDictionary[EStatName.PercentageLifeRegen];
		BasicStats.AddedManaRegen = ItemStatDictionary[EStatName.AddedManaRegen];
		BasicStats.IncreasedManaRegen = ItemStatDictionary[EStatName.IncreasedManaRegen];

		DamageMods.AddedPhysicalMin = (int)ItemStatDictionary[EStatName.FlatMinPhysDamage];
		DamageMods.AddedPhysicalMax = (int)ItemStatDictionary[EStatName.FlatMaxPhysDamage];
		DamageMods.IncreasedPhysical = ItemStatDictionary[EStatName.PercentagePhysDamage];

		BasicStats.AddedEvasion = (int)ItemStatDictionary[EStatName.FlatEvasion];
		BasicStats.IncreasedEvasion = ItemStatDictionary[EStatName.PercentageEvasion];

		Resistances.ResFire = (int)ItemStatDictionary[EStatName.FireResistance];
		Resistances.ResCold = (int)ItemStatDictionary[EStatName.ColdResistance];
		Resistances.ResLightning = (int)ItemStatDictionary[EStatName.LightningResistance];

		// TEST
		if (MainHandWeapon != null) {
			mainHandMinPhysDamage = Math.Round((MainHandWeapon.PhysicalMinimumDamage + DamageMods.AddedPhysicalMin) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			mainHandMaxPhysDamage = Math.Round((MainHandWeapon.PhysicalMaximumDamage + DamageMods.AddedPhysicalMax) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
		}
		else {
			mainHandMinPhysDamage = Math.Round(DamageMods.AddedPhysicalMin * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			mainHandMaxPhysDamage = Math.Round(DamageMods.AddedPhysicalMax * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
		}

		if (OffHandItem != null && IsOffHandAWeapon) {
			WeaponItem offHandWeapon = (WeaponItem)OffHandItem;
			offHandMinPhysDamage = Math.Round((offHandWeapon.PhysicalMinimumDamage + DamageMods.AddedPhysicalMin) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandMaxPhysDamage = Math.Round((offHandWeapon.PhysicalMaximumDamage + DamageMods.AddedPhysicalMax) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
		}
		else {
			offHandMinPhysDamage = Math.Round(DamageMods.AddedPhysicalMin * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandMaxPhysDamage = Math.Round(DamageMods.AddedPhysicalMax * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
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

		PlayerHUD.PlayerPanel.OffenceTabPanel.MainHandPhysDamage.SetValue($"{mainHandMinPhysDamage} - {mainHandMaxPhysDamage}");
		PlayerHUD.PlayerPanel.OffenceTabPanel.OffHandPhysDamage.SetValue($"{offHandMinPhysDamage} - {offHandMaxPhysDamage}");

		PlayerHUD.PlayerPanel.DefenceTabPanel.LifeRegen.SetValue($"{BasicStats.TotalLifeRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ManaRegen.SetValue($"{BasicStats.TotalManaRegen:F1}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Armour.SetValue($"{BasicStats.TotalArmour}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.Evasion.SetValue($"{BasicStats.TotalEvasion}");
		PlayerHUD.PlayerPanel.DefenceTabPanel.FireRes.SetValue($"{Resistances.ResFire}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.ColdRes.SetValue($"{Resistances.ResCold}%");
		PlayerHUD.PlayerPanel.DefenceTabPanel.LightningRes.SetValue($"{Resistances.ResLightning}%");
	}

	public void AssignMainHand(WeaponItem item) {
		MainHandWeapon = item;
	}

	public void AssignOffHand(Item item) {
		if (item == null) {
			OffHandItem = null;
			IsOffHandAWeapon = false;
		}
		else {
			OffHandItem = item;

			if (OffHandItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
				IsOffHandAWeapon = true;
			}
			else {
				IsOffHandAWeapon = false;
			}
		}
	}
}
