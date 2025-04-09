using Godot;
using System;

public partial class MapTransitionObj : Area3D {
    private MeshInstance3D outlineMesh;

    public override void _Ready() {
        outlineMesh = GetNode<MeshInstance3D>("Mesh/MeshOutline");

        AddToGroup("MapTransition");
    }

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
            GD.Print("Slay");
        }
    }

}
