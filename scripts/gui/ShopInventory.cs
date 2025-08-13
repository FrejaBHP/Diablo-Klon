using Godot;
using System;
using System.Collections.Generic;

public partial class ShopInventory : Control {
    public Shop ShopOwner { get; set; }
    public InventoryGrid InventoryGrid { get; protected set; }
    public Button RerollButton { get; protected set; }
    protected int rerollCost = 100;

    public override void _Ready() {
        RerollButton = GetNode<Button>("Control/RerollButton");
    }

    public void GenerateShopGrid(int x, int y) {
        InventoryGrid = GetNode<InventoryGrid>("VBoxContainer/CenterContainer/InventoryGrid");
        InventoryGrid.GenerateGrid(x, y);
        InventoryGrid.ItemClicked += OnItemClicked;
    }

    public void PopulateShop() {
        if (ShopOwner.ShopType == EShopType.Equipment) {
            int random = Utilities.RNG.Next(10, 16);

            for (int i = 0; i < random; i++) {
                Item newItem = ItemGeneration.GenerateItemFromCategory(EItemCategory.None);
                newItem.ConvertToInventoryItem(InventoryGrid, null);
            }
        }
        else if (ShopOwner.ShopType == EShopType.Gems) {
            int random = Utilities.RNG.Next(6, 11);
            int amountSkillGems = 0;
            int amountSupportGems = 0;

            for (int i = 0; i < random; i++) {
                int roll = Utilities.RNG.Next(0, 3);
                if (roll == 0) {
                    amountSkillGems++;
                }
                else {
                    amountSupportGems++;
                }
            }

            List<SkillItem> skillGems = ItemGeneration.GenerateRandomSkillGems(amountSkillGems, false);
            List<SupportGem> supportGems = ItemGeneration.GenerateRandomSupportGems(amountSupportGems, false);

            foreach (SkillItem skillGem in skillGems) {
                skillGem.ConvertToInventoryItem(InventoryGrid, null);
            }

            foreach (SupportGem supportGem in supportGems) {
                supportGem.ConvertToInventoryItem(InventoryGrid, null);
            }
        }

        foreach (InventoryItem item in InventoryGrid.GetInventoryItems()) {
            item.IsForSale = true;
        }
    }

    public void OnRerollButtonPressed() {
        if (Run.Instance.PlayerActor.Gold >= rerollCost) {
            Run.Instance.PlayerActor.Gold -= rerollCost;
            InventoryGrid.ClearItems();
            PopulateShop();
        }
    }

    public void OnItemClicked(InventoryItem item) {
        if (Run.Instance.PlayerActor.Gold >= item.ItemReference.Price && Run.Instance.PlayerActor.PlayerHUD.Inventory.InventoryGrid.CanFitInInventory(ref item)) {
            item.IsForSale = false;
            Run.Instance.PlayerActor.Gold -= item.ItemReference.Price;
            InventoryGrid.RemoveItemFromInventory(item);
            Run.Instance.PlayerActor.PlayerHUD.Inventory.InventoryGrid.TryAddItemToInventory(ref item);
        }
    }
}
