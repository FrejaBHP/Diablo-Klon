using Godot;
using System;

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
		else if (@event.IsActionPressed("DebugSpawnTestItem")) {
			TestItem23 testItem = new();
			WorldItem worldItem = testItem.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnTestItem2")) {
			TestItem22 testItem = new();
			WorldItem worldItem = testItem.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnTestItem3")) {
			TestItem11 testItem = new();
			WorldItem worldItem = testItem.ConvertToWorldItem();
			DropItem(worldItem);
		}
		else if (@event.IsActionPressed("DebugSpawnTestItem4")) {
			TestItem21 testItem = new();
			WorldItem worldItem = testItem.ConvertToWorldItem();
			DropItem(worldItem);
		}
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

	public void DropItem(WorldItem worldItem) {
		Game game = (Game)GetParent();
		CanvasLayer gameObjectLayer = game.GetNode<CanvasLayer>("WorldObjects");

		gameObjectLayer.AddChild(worldItem);
		worldItem.Position = Position;
		worldItem.PostSpawn();
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
		//GD.Print("Tried picking up item");

		item.ItemReference.ConvertToInventoryItem(this);

		return true;
	}
}
