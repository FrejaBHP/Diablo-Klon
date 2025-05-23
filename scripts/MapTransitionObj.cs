using Godot;
using System;

public partial class MapTransitionObj : Area3D {
    [Export]
    public bool GoesToTown = false;

    [Export]
    public bool UseRedPortal = false;

    public PackedScene SceneToTransitionTo;
    
    private AnimatedSprite3D portalSprites;
    private MeshInstance3D outlineMesh;

    public override void _Ready() {
        portalSprites = GetNode<AnimatedSprite3D>("APortalSprite");
        outlineMesh = GetNode<MeshInstance3D>("Mesh/MeshOutline");

        AddToGroup("MapTransition");
        portalSprites.Play();
    }

    // This executes in the last Input step, namely the Physics Picking step, and therefore will not prevent propagating any input to the Unhandled steps
    // A workaround to avoid left clicking on this also causing an attack would be to not detect this Area3D through its own input event, but rather through a raycast from the camera
    // https://docs.godotengine.org/en/4.3/tutorials/inputs/inputevent.html
    // https://stackoverflow.com/questions/78464266/how-to-implement-move-to-click-with-interactable-objects-event-order-is-breakin
    public void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeidx) {
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			Player player = camera.GetParent<Player>();
			if (!player.PlayerHUD.PlayerInventory.IsAnItemSelected) {
				player.SetDestinationNode(this);
			}
		}
    }

    public void UpdatePortalAnimationAndVisibility() {
        if (!UseRedPortal) {
            portalSprites.Animation = "portal_blue";
        }
        else {
            portalSprites.Animation = "portal_red";
        }

        portalSprites.Visible = true;
    }

    public void OnMouseEntered() {
        outlineMesh.Visible = true;
    }

    public void OnMouseExited() {
        outlineMesh.Visible = false;
    }

    public void OnBodyEntered(Node3D body) {
        Player player = (Player)body;
        if (player.TargetedNode == this) {
            if (GoesToTown) {
                Game.Instance.LoadAndSetMapToTown();
            }
            else if (SceneToTransitionTo != null) {
                Game.Instance.ChangeMap(SceneToTransitionTo);
            }
            else {
                GD.PrintErr("Cannot transition to new map: Map Scene field is null");
            }
        }
    }
}
