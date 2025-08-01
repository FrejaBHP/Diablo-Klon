using Godot;
using System;

public partial class Shop : Area3D {
    public static readonly PackedScene shopInventoryScene = GD.Load<PackedScene>("res://scenes/gui/hud_shop.tscn");

    [Export]
    public EShopType ShopType;

    protected ShopInventory shopInventory;
    protected CylinderShape3D ShopCollisionShape;
    protected bool shopCreated = false;

    public override void _Ready() {
        CollisionShape3D shape = GetNode<CollisionShape3D>("ShopCollisionShape");
        ShopCollisionShape = shape.Shape as CylinderShape3D;

        AddToGroup("Shop");
        AddToGroup("Interactable");
    }

    public void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeidx) {

    }

    public void OnMouseEntered() {
        
    }

    public void OnMouseExited() {
        
    }

    public void OnClickedByPlayer(Player player) {
        if (player.GlobalPosition.DistanceTo(GlobalPosition) < ShopCollisionShape.Radius) {
            player.ResetNodeTarget();
		    player.Velocity = Vector3.Zero;
            CreateOrShowShop();
        }
        else {
            player.SetDestinationNode(this);
        }
    }

    public void OnBodyEntered(Node3D body) {
        Player player = (Player)body;
        if (player.TargetedNode == this) {
            player.ResetNodeTarget();
		    player.Velocity = Vector3.Zero;
            CreateOrShowShop();
        }
    }

    protected void CreateOrShowShop() {
        if (!shopCreated) {
            shopCreated = true;
            shopInventory = shopInventoryScene.Instantiate<ShopInventory>();
            Run.Instance.PlayerActor.PlayerHUD.AddChild(shopInventory);

            int sizeX; int sizeY;
            if (ShopType == EShopType.Equipment) {
                sizeX = 12;
                sizeY = 12;
            }
            else {
                sizeX = 5;
                sizeY = 4;
            }

            shopInventory.ShopOwner = this;
            shopInventory.GenerateShopGrid(sizeX, sizeY);
            shopInventory.PopulateShop();
        }
        
        shopInventory.Visible = true;
    }

    public void OnBodyExited(Node3D body) {
        Player player = (Player)body;
        if (shopInventory != null) {
            shopInventory.Visible = false;
        }
    }
}
