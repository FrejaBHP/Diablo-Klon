using Godot;
using System;

public partial class Shop : Area3D {
    public static readonly PackedScene shopInventoryScene = GD.Load<PackedScene>("res://scenes/gui/hud_shop.tscn");

    [Export]
    public EShopType ShopType;

    protected ShopInventory shopInventory;
    protected bool shopCreated = false;

    public override void _Ready() {
        AddToGroup("Shop");
        AddToGroup("Interactable");
    }

    public void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeidx) {

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
    }

    public void OnBodyExited(Node3D body) {
        Player player = (Player)body;
        if (shopInventory != null) {
            shopInventory.Visible = false;
        }
    }
}
