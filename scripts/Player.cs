using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Actor {
	public HUD PlayerHUD;
	public Inventory PlayerInventory;

	private const float RayLength = 1000f;
	public const float Speed = 5.0f;

	public bool MovingTowardsObject = false;

	private PlayerCamera playerCamera;
	private Label debugLabel;
	private bool newMouseInput = false;
	private bool controlsCamera = true;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector2 lastMouseInputPos = new(0, 0);
	private Vector3 moveTo = new(0, 0, 0);
	private float remainingDist = 0f;
	private Node3D targetedNode;

	public Dictionary<EStatName, double> ItemStatDictionary = new() {
		{ EStatName.FlatMaxLife, 					0 },
		{ EStatName.PercentageMaxLife, 				0 },
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
		BasicStats.BaseLife = 50;
	}

	public override void _Ready() {
		playerCamera = GetNode<PlayerCamera>("PlayerCamera");
		playerCamera.AssignPlayer(this);

		debugLabel = GetNode<Label>("DebugLabel");
		PlayerHUD = GetNode<HUD>("SubViewportContainer/SubViewport/CanvasLayer/PlayerHUD");
		PlayerHUD.PlayerOwner = this;
		PlayerInventory = GetNode<Inventory>("SubViewportContainer/SubViewport/CanvasLayer/PlayerHUD/Inventory");
		PlayerInventory.PlayerOwner = this;
		moveTo = GlobalPosition;
	}

    public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("InventoryKey")) {
			PlayerInventory.ToggleInventory();
		}
		// logik for at spawne items skal flyttes til en mere generel klasse som fx Combat eller Game
		else if (@event.IsActionPressed("DebugSpawnRandomItem")) {
			Item item = ItemGeneration.GenerateItemFromCategory(EItemCategory.None);
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

				//GD.Print("Moving towards position");
			}
		}
		else {
			Vector3 direction = GlobalPosition.DirectionTo(moveTo);
			Velocity = direction * Speed;

			Vector3 lookAt = moveTo with { Y = GlobalPosition.Y };
			if (!Mathf.IsZeroApprox(GlobalPosition.DistanceTo(lookAt))) {
				LookAt(lookAt, null, true);
			}

			//GD.Print("Moving towards object");
		}

		newMouseInput = false;
	}

    public override void _PhysicsProcess(double delta) {
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
		
		/*
		if (slot.ItemInSlot != null) {
			
		}
		*/
		CalculateStats();
	}

	public void RemoveItemStats(EquipmentSlot slot, Item item) {
		foreach (var stat in item.StatDictionary) {
			ItemStatDictionary[stat.Key] -= stat.Value;
		}

		/*
		if (slot.ItemInSlot != null) {
			
		}
		*/
		CalculateStats();
	}

	public void RecalculateAllItemStats() {
		foreach (var key in ItemStatDictionary.Keys.ToList()) {
			ItemStatDictionary[key] = 0;
		}
		
		foreach (var slot in PlayerInventory.GetEquipmentSlots()) {
			if (slot.ItemInSlot != null) {
				foreach (var stat in slot.ItemInSlot.ItemReference.StatDictionary) {
					ItemStatDictionary[stat.Key] += stat.Value;
				}
			}
		}

		CalculateStats();
	}

	protected void CalculateStats() {
		BasicStats.AddedLife = (int)ItemStatDictionary[EStatName.FlatMaxLife];

		BasicStats.AddedEvasion = (int)ItemStatDictionary[EStatName.FlatEvasion];
		BasicStats.IncreasedEvasion = (float)ItemStatDictionary[EStatName.PercentageEvasion];

		DamageMods.AddedPhysicalMin = (int)ItemStatDictionary[EStatName.FlatMinPhysDamage];
		DamageMods.AddedPhysicalMax = (int)ItemStatDictionary[EStatName.FlatMaxPhysDamage];
		DamageMods.IncreasedPhysical = (float)ItemStatDictionary[EStatName.PercentagePhysDamage];

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
			offHandMinPhysDamage = Math.Round((float)((offHandWeapon.PhysicalMinimumDamage + DamageMods.AddedPhysicalMin) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical)), 0);
			offHandMaxPhysDamage = Math.Round((float)((offHandWeapon.PhysicalMaximumDamage + DamageMods.AddedPhysicalMax) * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical)), 0);
		}
		else {
			offHandMinPhysDamage = Math.Round(DamageMods.AddedPhysicalMin * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
			offHandMaxPhysDamage = Math.Round(DamageMods.AddedPhysicalMax * (1 + DamageMods.IncreasedPhysical) * (1 + DamageMods.MorePhysical), 0);
		}

		debugLabel.Text = $"Life: {BasicStats.TotalLife}\nEvasion Rating: {BasicStats.TotalEvasion}\n\n" + 
		$"MH Physical Damage: {mainHandMinPhysDamage} - {mainHandMaxPhysDamage}\n" +
		$"OH Physical Damage: {offHandMinPhysDamage} - {offHandMaxPhysDamage}\n\n" +
		$"Fire Res: {Resistances.ResFire}\nCold Res: {Resistances.ResCold}\nLightning Res: {Resistances.ResLightning}";
	}

	public void AssignMainHand(WeaponItem item) {
		MainHandWeapon = item;
	}

	public void AssignOffHand(Item item) {
		OffHandItem = item;

		if (OffHandItem.GetType() == typeof(WeaponItem)) {
			IsOffHandAWeapon = true;
		}
		else {
			IsOffHandAWeapon = false;
		}
	}
}
