using Godot;
using System;

public partial class PlayerCamera : Camera3D {
	private Player player;
	private bool followsPlayer = false;

	private RayCast3D occlusionCast;
	private StaticBody3D currentOccludingStaticMesh = null;

	private Vector3 offset = new(10, 10, 10);
	private float transValue = 0.6f;
	private float transFadeDuration = 0.2f;

	public override void _Ready() {
		occlusionCast = GetNode<RayCast3D>("OcclusionCast");	
	}

	public void AssignPlayer(Player p) {
		player = p;
		followsPlayer = true;
	}

	public override void _PhysicsProcess(double delta) {
		if (followsPlayer) {
			GlobalPosition = GlobalPosition.Lerp(player.GlobalPosition + offset, 0.5f);

			// Check if a wall is obstructing the raycast to the player
			if (occlusionCast.IsColliding() && occlusionCast.GetCollider().IsClass("StaticBody3D")) {
				StaticBody3D collider = occlusionCast.GetCollider() as StaticBody3D;

				// If it's a new wall and has a mesh instance
				if (collider != currentOccludingStaticMesh && collider.GetChildCount() > 0 && collider.GetChild(0).IsClass("MeshInstance3D")) {
					// If there's an immediate overlap between two walls, make the old one opaque before continuing (pending testing)
					if (currentOccludingStaticMesh != null) {
						MeshInstance3D oldColliderMesh = currentOccludingStaticMesh.GetChild(0) as MeshInstance3D;

						MakeColliderMeshOpaque(oldColliderMesh);
					}

					// Set new wall as current and make it transparent
					currentOccludingStaticMesh = collider;
					MeshInstance3D colliderMesh = collider.GetChild(0) as MeshInstance3D;

					MakeColliderMeshTransparent(colliderMesh);
				}
				
			}
			else {
				// If colliding with nothing and previously was colliding with a wall, make it opaque
				if (currentOccludingStaticMesh != null) {
					MeshInstance3D colliderMesh = currentOccludingStaticMesh.GetChild(0) as MeshInstance3D;

					MakeColliderMeshOpaque(colliderMesh);
				}

				currentOccludingStaticMesh = null;
			}
		}
	}

	protected void MakeColliderMeshTransparent(MeshInstance3D colliderMesh) {
		Tween tween = CreateTween();
		tween.TweenProperty(colliderMesh, "transparency", transValue, transFadeDuration);
		tween.TweenCallback(Callable.From(tween.Kill));
	}

	protected void MakeColliderMeshOpaque(MeshInstance3D colliderMesh) {
		Tween tween = CreateTween();
		tween.TweenProperty(colliderMesh, "transparency", 0f, transFadeDuration);
		tween.TweenCallback(Callable.From(tween.Kill));
	}
}
