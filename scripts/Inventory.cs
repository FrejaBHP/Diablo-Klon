using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Control {
    public Player PlayerOwner;
	public GridContainer InventoryGridContainer;
    private Control equipmentPanelControl;

    public EquipmentSlot HelmetSlot;
    public EquipmentSlot ChestSlot;
    public EquipmentSlot HandsSlot;
    public EquipmentSlot FeetSlot;
    public EquipmentSlot RingSlotLeft;
    public EquipmentSlot RingSlotRight;
    public EquipmentSlot AmuletSlot;
    public EquipmentSlot WeaponSlotLeft;
    public EquipmentSlot WeaponSlotRight;

	private const int inventorySizeX = 13;
	private const int inventorySizeY = 5;

    public InventoryGridCell[,] invGridCells = new InventoryGridCell[inventorySizeX, inventorySizeY];

    public bool IsOpen = false;
    public bool IsAnItemSelected = false;

    private InventoryItem selectedItem = null;
    public InventoryItem SelectedItem {
        get { return selectedItem; }
    }

    private List<InventoryGridCell> tempUsedSlots = new List<InventoryGridCell>();
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    public override void _Ready() {
        AssignAndPrepareNodes();

        GenerateInventory();
    }

    private void AssignAndPrepareNodes() {
        InventoryGridContainer = GetNode<GridContainer>("InventoryContainer/InventoryGrid");
        equipmentPanelControl = GetNode<Control>("InventoryContainer/EquipmentContainer/Control/Control");

        HelmetSlot = equipmentPanelControl.GetNode<EquipmentSlot>("HelmetSlot");
        ChestSlot = equipmentPanelControl.GetNode<EquipmentSlot>("ChestSlot");
        HandsSlot = equipmentPanelControl.GetNode<EquipmentSlot>("HandsSlot");
        FeetSlot = equipmentPanelControl.GetNode<EquipmentSlot>("FeetSlot");
        RingSlotLeft = equipmentPanelControl.GetNode<EquipmentSlot>("RingSlotLeft");
        RingSlotRight = equipmentPanelControl.GetNode<EquipmentSlot>("RingSlotRight");
        AmuletSlot = equipmentPanelControl.GetNode<EquipmentSlot>("AmuletSlot");
        WeaponSlotLeft = equipmentPanelControl.GetNode<EquipmentSlot>("WeaponSlotLeft");
        WeaponSlotRight = equipmentPanelControl.GetNode<EquipmentSlot>("WeaponSlotRight");

        HelmetSlot.Slot = EItemEquipmentSlot.Head;
        ChestSlot.Slot = EItemEquipmentSlot.Chest;
        HandsSlot.Slot = EItemEquipmentSlot.Hands;
        FeetSlot.Slot = EItemEquipmentSlot.Feet;
        RingSlotLeft.Slot = EItemEquipmentSlot.Ring;
        RingSlotRight.Slot = EItemEquipmentSlot.Ring;
        AmuletSlot.Slot = EItemEquipmentSlot.Amulet;
        WeaponSlotLeft.Slot = EItemEquipmentSlot.Weapon;
        WeaponSlotRight.Slot = EItemEquipmentSlot.Weapon;

        equipmentSlots.Add(HelmetSlot);
        equipmentSlots.Add(ChestSlot);
        equipmentSlots.Add(HandsSlot);
        equipmentSlots.Add(FeetSlot);
        equipmentSlots.Add(RingSlotLeft);
        equipmentSlots.Add(RingSlotRight);
        equipmentSlots.Add(AmuletSlot);
        equipmentSlots.Add(WeaponSlotLeft);
        equipmentSlots.Add(WeaponSlotRight);

        foreach (EquipmentSlot slot in equipmentSlots) {
            slot.InventoryReference = this;
            slot.ItemEquipped += EquipItemInSlot;
            slot.ItemUnequipped += UnequipItemInSlot;
            slot.ItemsSwapped += SwapItemsInSlot;
        }
    }

    public void OnGUIInput(InputEvent @event) {
        // On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (IsAnItemSelected && selectedItem != null) {
                // NOTE: Items being clicked outside of the inventory panel is handled in the HUD class

                // If an item is selected and the click is inside the inventory panel, try and reparent it to the grid slot matching the top-left corner of the item's bounding box
                // A small adjustment is added to the offset to make the expected placement match the art better and prevent placing them in unintended slots
                if (InventoryGridContainer.GetGlobalRect().HasPoint(selectedItem.GlobalPosition)) {
                    Vector2 offset = selectedItem.GlobalPosition - InventoryGridContainer.GlobalPosition + new Vector2(12, 12);
                    Vector2 slotSize = invGridCells[0, 0].Size;

                    int slotX = (int)Math.Floor(offset.X / slotSize.X);
                    int slotY = (int)Math.Floor(offset.Y / slotSize.Y);

                    //GD.Print($"X: {slotX}, Y: {slotY}");

                    if (CanFitInSlot(slotX, slotY, selectedItem)) {
                        MoveSelectedItemToSlot(slotX, slotY);
                    }
                }
                else {
                    ItemClickDeselect(selectedItem);
                }
            }
		}
	}

    // Generates the player's inventory grid
    private void GenerateInventory() {
        InventoryGridContainer.Columns = inventorySizeX;

        for (int i = 0; i < inventorySizeY; i++) {
            for (int j = 0; j < inventorySizeX; j++) {
                InventoryGridCell newCell = new(new Vector2I(j, i));
                invGridCells[j, i] = newCell;
                InventoryGridContainer.AddChild(newCell);
            }
        }
    }

    // Goes through the inventory slots one by one (ignoring rows and columns that would be impossible to fit into due to size) to find an open and suitable slot to add it to
    // The order is determined as going through each column, top to bottom, left to right
    public bool TryAddItemToInventory(ref InventoryItem item) {
        for (int i = 0; i < inventorySizeX - (item.GetGridSize().X - 1); i++) {
            for (int j = 0; j < inventorySizeY - (item.GetGridSize().Y - 1); j++) {
                if (invGridCells[i, j].IsEmpty) {
                    if (CanFitInSlot(i, j, item)) {
                        AddItemToInventory(item, i, j);
                        //GD.Print("Slots not occupied, picking up");
                        return true;
                    }
                }
            }
        }
        
        //GD.Print("Slots occupied, can't pick up");
        return false;
    }

    public void AddItemToInventory(InventoryItem item, int slotX, int slotY) {
        invGridCells[slotX, slotY].AddChild(item);
        item.Position = Vector2.Zero;
        item.SetOccupiedSlots(tempUsedSlots);
        inventoryItems.Add(item);
    }

    // Reparents an item to a different inventory slot to anchor it there, then deselects the item
    public void MoveSelectedItemToSlot(int slotX, int slotY) {
        selectedItem.ClearOccupiedSlots();
        selectedItem.GetParent().RemoveChild(selectedItem);
        AddItemToInventory(selectedItem, slotX, slotY);
        ItemClickDeselect(selectedItem);
    }

    // Checks the relevant slots relative to the selected slot based on the item's size. Returns true if it can fit, false if it can't
    public bool CanFitInSlot(int slotX, int slotY, InventoryItem item) {
        tempUsedSlots.Clear();
        
        for (int i = 0; i < item.GetGridSize().X; i++) {
            for (int j = 0; j < item.GetGridSize().Y; j++) {
                //GD.Print("Testing slot " + (slotX + i) + ", " + (slotY + j));
                if (!invGridCells[slotX + i, slotY + j].IsEmpty) {
                    return false;
                }
                tempUsedSlots.Add(invGridCells[slotX + i, slotY + j]);
            }
        }

        return true;
    }

    public void ItemClickSelect(InventoryItem item) {
        if (!IsAnItemSelected && selectedItem == null) {
            //GD.Print("Item Selected!");

            selectedItem = item;
            selectedItem.IsClicked = true;
            IsAnItemSelected = true;
            
            selectedItem.ToggleBackground();
            selectedItem.OpenOccupiedSlots();
            
            item.ZIndex = 5;
            item.ToggleClickable();

            foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemEquipmentSlot)) {
                slot.HighlightSlot();
            }
        }
    }

    public void ItemClickDeselect(InventoryItem item) {
        if (IsAnItemSelected && selectedItem != null) {
            //GD.Print("Item Deselected!");

            item.CloseOccupiedSlots();
            item.Position = Vector2.Zero;

            item.IsClicked = false;
            item.ToggleBackground();
            IsAnItemSelected = false;
            selectedItem = null;

            item.ZIndex = 1;
            item.ToggleClickable();

            foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemEquipmentSlot)) {
                slot.RemoveHighlight();
            }
        }
    }

    public void ItemClickDrop(InventoryItem item) {
        //GD.Print("Item Dropped!");

        foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemEquipmentSlot)) {
            slot.RemoveHighlight();
        }

        inventoryItems.Remove(item);
        item.IsClicked = false;
        IsAnItemSelected = false;
        selectedItem = null;

        item.ClearOccupiedSlots();
        WorldItem worldItem = item.ItemReference.ConvertToWorldItem();
        PlayerOwner.DropItem(worldItem);
    }

    public void ToggleInventory() {
        if (!IsOpen) {
            Visible = true;
            IsOpen = true;
        }
        else {
            Visible = false;
            IsOpen = false;

            if (IsAnItemSelected && selectedItem != null) {
                ItemClickDeselect(selectedItem);
            }
        }
    }

    public void EquipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        //GD.Print("Item Equipped!");

        item.ClearOccupiedSlots();
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        item.Position = Vector2.Zero;
        item.IsClicked = false;
        IsAnItemSelected = false;
        selectedItem = null;

        item.ZIndex = 1;

        foreach (EquipmentSlot slotAll in equipmentSlots) {
            if (!slotAll.IsHovered) {
                slotAll.RemoveHighlight();
            }
        }
    }

    public void UnequipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        //GD.Print("Item Unequipped!");

        slot.RemoveChild(item);

        if (!TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItem(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }
    }

    public void SwapItemsInSlot(EquipmentSlot slot, InventoryItem oldItem, InventoryItem newItem) {
        //GD.Print("Items Swapped!");

        newItem.ClearOccupiedSlots();
        slot.RemoveChild(oldItem);
        newItem.GetParent().RemoveChild(newItem);
        slot.AddChild(newItem);

        if (!TryAddItemToInventory(ref oldItem)) {
            PlayerOwner.DropItem(oldItem.ItemReference.ConvertToWorldItem());
        }
        else {
            oldItem.ToggleClickable();
            oldItem.ToggleBackground();
        }
        
        newItem.Position = Vector2.Zero;
        newItem.IsClicked = false;
        IsAnItemSelected = false;
        selectedItem = null;

        newItem.ZIndex = 1;

        foreach (EquipmentSlot slotAll in equipmentSlots) {
            if (!slotAll.IsHovered) {
                slotAll.RemoveHighlight();
            }
        }
    }

    private List<EquipmentSlot> GetEquipmentSlots(EItemEquipmentSlot slot) {
        List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

        switch (slot) {
            case EItemEquipmentSlot.Head:
                equipmentSlots.Add(HelmetSlot);
                break;
            
            case EItemEquipmentSlot.Chest:
                equipmentSlots.Add(ChestSlot);
                break;
            
            case EItemEquipmentSlot.Hands:
                equipmentSlots.Add(HandsSlot);
                break;
            
            case EItemEquipmentSlot.Feet:
                equipmentSlots.Add(FeetSlot);
                break;

            case EItemEquipmentSlot.Ring:
                equipmentSlots.Add(RingSlotLeft);
                equipmentSlots.Add(RingSlotRight);
                break;
            
            case EItemEquipmentSlot.Amulet:
                equipmentSlots.Add(AmuletSlot);
                break;

            case EItemEquipmentSlot.Weapon1H:
                equipmentSlots.Add(WeaponSlotLeft);
                equipmentSlots.Add(WeaponSlotRight);
                break;
            
            case EItemEquipmentSlot.Weapon2H:
                equipmentSlots.Add(WeaponSlotLeft);
                break;
            
            case EItemEquipmentSlot.Shield:
                equipmentSlots.Add(WeaponSlotRight);
                break;

            default:
                break;
        }

        return equipmentSlots;
    }
}
