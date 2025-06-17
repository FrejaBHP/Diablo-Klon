using Godot;
using System;

public partial class ShopInventory : Control {
    public Shop ShopOwner { get; set; }
    public InventoryGrid InventoryGrid { get; protected set; }
    public Button RerollButton { get; protected set; }
    protected int rerollCost = 100;

    public override void _Ready() {
        InventoryGrid = GetNode<InventoryGrid>("VBoxContainer/CenterContainer/InventoryGrid");
        InventoryGrid.GenerateGrid(12, 12);
        InventoryGrid.ItemClicked += OnItemClicked;

        RerollButton = GetNode<Button>("Control/RerollButton");
    }

    public void PopulateShop() {
        int random = Utilities.RNG.Next(10, 16);

        for (int i = 0; i < random; i++) {
            Item newItem = ItemGeneration.GenerateItemFromCategory(EItemCategory.None);
            newItem.ConvertToInventoryItem(InventoryGrid, null);
        }

        foreach (InventoryItem item in InventoryGrid.GetInventoryItems()) {
            item.IsForSale = true;
        }
    }

    public void OnRerollButtonPressed() {
        if (Game.Instance.PlayerActor.Gold >= rerollCost) {
            Game.Instance.PlayerActor.Gold -= rerollCost;
            InventoryGrid.ClearItems();
            PopulateShop();
        }
    }

    public void OnItemClicked(InventoryItem item) {
        if (Game.Instance.PlayerActor.Gold >= item.ItemReference.Price && Game.Instance.PlayerActor.PlayerHUD.PlayerInventory.InventoryGrid.CanFitInInventory(ref item)) {
            item.IsForSale = false;
            Game.Instance.PlayerActor.Gold -= item.ItemReference.Price;
            InventoryGrid.RemoveItemFromInventory(item);
            Game.Instance.PlayerActor.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref item);
        }
    }
}
