using Godot;
using System;

public partial class Shop : Area3D {
    public static readonly PackedScene shopInventoryScene = GD.Load<PackedScene>("res://scenes/gui/hud_shop.tscn");
    protected ShopInventory shopInventory;
    protected bool shopCreated = false;

    public override void _Ready() {
        AddToGroup("Shop");
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
        
    }

    public void OnMouseExited() {
        
    }

    public void OnBodyEntered(Node3D body) {
        Player player = (Player)body;
        if (player.TargetedNode == this) {
            if (!shopCreated) {
                shopCreated = true;
                shopInventory = shopInventoryScene.Instantiate<ShopInventory>();
                Game.Instance.PlayerActor.PlayerHUD.AddChild(shopInventory);

                shopInventory.ShopOwner = this;
                shopInventory.PopulateShop();
            }
            
            shopInventory.Visible = true;
        }
    }

    public void OnBodyExited(Node3D body) {
        Player player = (Player)body;
        if (shopInventory != null) {
            shopInventory.Visible = false;
        }
    }
}
