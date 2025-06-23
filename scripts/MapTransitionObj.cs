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
        AddToGroup("Interactable");
        portalSprites.Play();
    }

    public void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeidx) {

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
