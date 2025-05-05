using Godot;
using System;

public partial class MapTransitionObj : Area3D {
    [Export]
    public PackedScene sceneToTransitionTo;

    private MeshInstance3D outlineMesh;

    public override void _Ready() {
        outlineMesh = GetNode<MeshInstance3D>("Mesh/MeshOutline");

        AddToGroup("MapTransition");
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

    public void OnMouseEntered() {
        outlineMesh.Visible = true;
    }

    public void OnMouseExited() {
        outlineMesh.Visible = false;
    }

    public void OnBodyEntered(Node3D body) {
        Player player = (Player)body;
        if (player.TargetedNode == this) {
            Game game = (Game)GetTree().Root.GetChild(0);
            if (sceneToTransitionTo != null) {
                game.ChangeMap(sceneToTransitionTo);
            }
            //GD.Print("Slay");
        }
    }
}
