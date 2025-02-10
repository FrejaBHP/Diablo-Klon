using Godot;
using System;

public partial class Player : Actor {
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

	public HUD PlayerHUD;
	public Inventory PlayerInventory;

	public override void _Ready() {
		playerCamera = GetNode<PlayerCamera>("PlayerCamera");
		playerCamera.AssignPlayer(this);

		debugLabel = GetNode<Label>("DebugLabel");
		PlayerHUD = GetNode<HUD>("PlayerHUD");
		PlayerInventory = GetNode<Inventory>("PlayerHUD/Inventory");
		moveTo = GlobalPosition;
	}

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed()) {
			// Dette skal fikses, så der ikke forsøges at bevæges mod et punkt uanset hvad
			SetDestinationPosition(eventMouseButton.Position);
		}
		else if (@event.IsActionPressed("InventoryKey")) {
			PlayerInventory.ToggleInventory();
		}
		else if (@event.IsActionPressed("DebugSpawnTestItem")) {
			// logik for at spawne items skal flyttes til en mere generel klasse som fx Combat eller Game
			Game game = (Game)GetParent();

			TestItem testItem = new();
			WorldItem worldItem = testItem.ConvertToWorldItem();
			game.AddChild(worldItem);
			worldItem.Position = Position;
			worldItem.PostSpawn();
		}
    }

	public void SetDestinationPosition(Vector2 position) {
		MovingTowardsObject = false;
		targetedNode = null;
		lastMouseInputPos = position;
		newMouseInput = true;

		GD.Print("Moving towards position");
	}

	public void SetDestinationNode(Node3D node) {
		MovingTowardsObject = true;
		targetedNode = node;
		moveTo = node.GlobalPosition;
		newMouseInput = true;

		GD.Print("Moving towards object");
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

				LookAt(moveTo with { Y = GlobalPosition.Y }, null, true);
			}
		}
		else {
			Vector3 direction = GlobalPosition.DirectionTo(moveTo);
			Velocity = direction * Speed;

			LookAt(moveTo with { Y = GlobalPosition.Y }, null, true);
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

		debugLabel.Text = $"Velocity: {Velocity}\nVel Length: {Velocity.Length()}\nRem. Dist: {remainingDist}\nRotation: {RotationDegrees.Y}";
	}

	public bool PickupItem(ref WorldItem item) {
		targetedNode = null;
		GD.Print("Tried picking up item");

		item.ItemReference.ConvertToInventoryItem(this);

		return true;
	}
}
