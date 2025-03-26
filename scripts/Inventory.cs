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
    public EquipmentSlot BeltSlot;
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
        equipmentPanelControl = GetNode<Control>("EquipmentContainer/Control/Control");
        InventoryGridContainer = GetNode<GridContainer>("CenterContainer/InventoryGrid");

        HelmetSlot = equipmentPanelControl.GetNode<EquipmentSlot>("HelmetSlot");
        ChestSlot = equipmentPanelControl.GetNode<EquipmentSlot>("ChestSlot");
        HandsSlot = equipmentPanelControl.GetNode<EquipmentSlot>("HandsSlot");
        FeetSlot = equipmentPanelControl.GetNode<EquipmentSlot>("FeetSlot");
        BeltSlot = equipmentPanelControl.GetNode<EquipmentSlot>("BeltSlot");
        RingSlotLeft = equipmentPanelControl.GetNode<EquipmentSlot>("RingSlotLeft");
        RingSlotRight = equipmentPanelControl.GetNode<EquipmentSlot>("RingSlotRight");
        AmuletSlot = equipmentPanelControl.GetNode<EquipmentSlot>("AmuletSlot");
        WeaponSlotLeft = equipmentPanelControl.GetNode<EquipmentSlot>("WeaponSlotLeft");
        WeaponSlotRight = equipmentPanelControl.GetNode<EquipmentSlot>("WeaponSlotRight");

        HelmetSlot.Slot = EItemEquipmentSlot.Head;
        ChestSlot.Slot = EItemEquipmentSlot.Chest;
        HandsSlot.Slot = EItemEquipmentSlot.Hands;
        FeetSlot.Slot = EItemEquipmentSlot.Feet;
        BeltSlot.Slot = EItemEquipmentSlot.Belt;
        RingSlotLeft.Slot = EItemEquipmentSlot.Ring;
        RingSlotRight.Slot = EItemEquipmentSlot.Ring;
        AmuletSlot.Slot = EItemEquipmentSlot.Amulet;
        WeaponSlotLeft.Slot = EItemEquipmentSlot.WeaponLeft;
        WeaponSlotRight.Slot = EItemEquipmentSlot.WeaponRight;

        equipmentSlots.Add(HelmetSlot);
        equipmentSlots.Add(ChestSlot);
        equipmentSlots.Add(HandsSlot);
        equipmentSlots.Add(FeetSlot);
        equipmentSlots.Add(BeltSlot);
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

    public void GUIInput(InputEvent @event) {
        // On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (IsAnItemSelected && selectedItem != null) {
                // If an item is selected and the click is inside the inventory panel, try and reparent it to the grid slot matching the top-left corner of the item's bounding box
                // A small adjustment is added to the offset to make the expected placement match the art better and prevent placing them in unintended slots
                if (InventoryGridContainer.GetGlobalRect().HasPoint(selectedItem.GlobalPosition)) {
                    Vector2 offset = selectedItem.GlobalPosition - InventoryGridContainer.GlobalPosition + new Vector2(12, 12);
                    Vector2 slotSize = invGridCells[0, 0].Size;

                    int slotX = (int)Math.Floor(offset.X / slotSize.X);
                    int slotY = (int)Math.Floor(offset.Y / slotSize.Y);

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
                        return true;
                    }
                }
            }
        }
        
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
            selectedItem = item;
            selectedItem.IsClicked = true;
            IsAnItemSelected = true;
            
            selectedItem.ToggleBackground();
            selectedItem.OpenOccupiedSlots();
            
            item.ZIndex = 5;
            item.ToggleClickable();

            foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
                slot.HighlightSlot();
            }
        }
    }

    public void ItemClickDeselect(InventoryItem item) {
        if (IsAnItemSelected && selectedItem != null) {
            item.CloseOccupiedSlots();
            item.Position = Vector2.Zero;

            item.IsClicked = false;
            item.ToggleBackground();
            IsAnItemSelected = false;
            selectedItem = null;

            item.ZIndex = 1;
            item.ToggleClickable();

            foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
                slot.RemoveHighlight();
            }
        }
    }

    public void ItemClickDrop(InventoryItem item) {
        foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
            slot.RemoveHighlight();
        }

        inventoryItems.Remove(item);
        item.RemoveTooltip();
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

    public bool CanEquipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.Shield) {
                return false;
            }
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.Weapon2H) {
                return false;
            }
            else if (WeaponSlotLeft.ItemInSlot != null && WeaponSlotLeft.ItemInSlot.ItemReference.ItemAllBaseType == EItemAllBaseType.Weapon2H) {
                return false;
            }
        }

        return true;
    }

    public void EquipItemInSlot(EquipmentSlot slot, InventoryItem item) {
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

        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            PlayerOwner.AssignMainHand((WeaponItem)item.ItemReference);
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            PlayerOwner.AssignOffHand(item.ItemReference);
        }

        if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.Weapon2H && WeaponSlotRight.ItemInSlot != null) {
            WeaponSlotRight.UnequipItem(WeaponSlotRight.ItemInSlot);
            PlayerOwner.AssignOffHand(null);
        }

        PlayerOwner.ApplyItemStats(slot, item.ItemReference);
    }

    public void UnequipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        item.RemoveTooltip();
        
        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            PlayerOwner.AssignMainHand(null);
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            PlayerOwner.AssignOffHand(null);
        }

        slot.RemoveChild(item);

        if (!TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItem(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }

        PlayerOwner.RemoveItemStats(slot, item.ItemReference);
    }

    public void SwapItemsInSlot(EquipmentSlot slot, InventoryItem oldItem, InventoryItem newItem) {
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

        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            PlayerOwner.AssignMainHand((WeaponItem)newItem.ItemReference);
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            PlayerOwner.AssignOffHand(newItem.ItemReference);
        }

        if (newItem.ItemReference.ItemAllBaseType == EItemAllBaseType.Weapon2H && WeaponSlotRight.ItemInSlot != null) {
            WeaponSlotRight.UnequipItem(WeaponSlotRight.ItemInSlot);
            PlayerOwner.AssignOffHand(null);
        }

        PlayerOwner.RemoveItemStats(slot, oldItem.ItemReference);
        PlayerOwner.ApplyItemStats(slot, newItem.ItemReference);
    }

    private List<EquipmentSlot> GetEquipmentSlots(EItemAllBaseType type) {
        List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

        switch (type) {
            case EItemAllBaseType.Head:
                equipmentSlots.Add(HelmetSlot);
                break;
            
            case EItemAllBaseType.Chest:
                equipmentSlots.Add(ChestSlot);
                break;
            
            case EItemAllBaseType.Hands:
                equipmentSlots.Add(HandsSlot);
                break;
            
            case EItemAllBaseType.Feet:
                equipmentSlots.Add(FeetSlot);
                break;

            case EItemAllBaseType.Belt:
                equipmentSlots.Add(BeltSlot);
                break;

            case EItemAllBaseType.Ring:
                equipmentSlots.Add(RingSlotLeft);
                equipmentSlots.Add(RingSlotRight);
                break;
            
            case EItemAllBaseType.Amulet:
                equipmentSlots.Add(AmuletSlot);
                break;
            
            case EItemAllBaseType.Weapon1H:
                equipmentSlots.Add(WeaponSlotLeft);
                equipmentSlots.Add(WeaponSlotRight);
                break;
            
            case EItemAllBaseType.Weapon2H:
                equipmentSlots.Add(WeaponSlotLeft);
                break;
            
            case EItemAllBaseType.Shield:
                equipmentSlots.Add(WeaponSlotRight);
                break;

            default:
                break;
        }

        return equipmentSlots;
    }

    public List<EquipmentSlot> GetEquipmentSlots() {
        return equipmentSlots;
    }
}
