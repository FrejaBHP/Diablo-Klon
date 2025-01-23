using Godot;
using System;

public partial class Player : Actor {
	private const float RayLength = 1000f;
	public const float Speed = 5.0f;

	private PlayerCamera playerCamera;
	private Label debugLabel;
	private bool newMouseInput = false;
	private bool controlsCamera = false;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector2 lastMouseInputPos = new(0, 0);
	private Vector3 moveTo = new(0, 0, 0);

	private float remainingDist = 0f;

	public void AssignCamera(PlayerCamera cam) {
		playerCamera = cam;
		controlsCamera = true;
	}

	public override void _Ready() {
		debugLabel = GetNode<Label>("DebugLabel");
		moveTo = GlobalPosition;
	}

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed()) {
			lastMouseInputPos = eventMouseButton.Position;
			newMouseInput = true;
		}
    }

    public override void _PhysicsProcess(double delta) {
		if (newMouseInput && controlsCamera) {
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

			newMouseInput = false;
		}

		remainingDist = (GlobalPosition with { Y = 0f }).DistanceTo(moveTo with { Y = 0f });

		if (remainingDist <= Speed / 100f) {
			Velocity = Vector3.Zero;
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
}
