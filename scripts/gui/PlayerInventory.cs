using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInventory : Control {
    public Player PlayerOwner;
	public InventoryGrid InventoryGrid;
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

	//private const int inventorySizeX = 12;
	//private const int inventorySizeY = 5;

    //public InventoryGridCell[,] invGridCells = new InventoryGridCell[inventorySizeX, inventorySizeY];

    public bool IsOpen = false;
    public bool IsAnItemSelected = false;

    private InventoryItem selectedItem = null;
    public InventoryItem SelectedItem {
        get { return selectedItem; }
    }

    //private List<InventoryGridCell> tempUsedSlots = new List<InventoryGridCell>();
    //private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    public override void _Ready() {
        AssignAndPrepareNodes();
        InventoryGrid.GenerateGrid(12, 5);
    }

    private void AssignAndPrepareNodes() {
        equipmentPanelControl = GetNode<Control>("EquipmentContainer/Control/Control");
        InventoryGrid = GetNode<InventoryGrid>("CenterContainer/InventoryGrid");
        InventoryGrid.ItemClicked += ItemClickSelect;

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
		if (@event.IsActionPressed("LeftClick")) {
			if (IsAnItemSelected && selectedItem != null) {
                // If an item is selected and the click is inside the inventory panel, try and reparent it to the grid slot matching the top-left corner of the item's bounding box
                // A small adjustment is added to the offset to make the expected placement match the art better and prevent placing them in unintended slots
                if (InventoryGrid.GetGlobalRect().HasPoint(selectedItem.GlobalPosition)) {
                    Vector2 magicOffset = new(8, 8);
                    Vector2 offset = selectedItem.GlobalPosition - InventoryGrid.GlobalPosition + magicOffset;
                    Vector2 slotSize = InventoryGrid.InvGridCells[0, 0].Size;

                    int slotX = (int)Math.Floor(offset.X / slotSize.X);
                    int slotY = (int)Math.Floor(offset.Y / slotSize.Y);

                    if (InventoryGrid.CanFitInSlot(slotX, slotY, selectedItem)) {
                        MoveSelectedItemToSlot(slotX, slotY);
                    }
                }
                else {
                    ItemClickDeselect(selectedItem);
                }
            }
		}
    }

    // Reparents an item to a different inventory slot to anchor it there, then deselects the item
    public void MoveSelectedItemToSlot(int slotX, int slotY) {
        selectedItem.ClearOccupiedSlots();
        selectedItem.GetParent().RemoveChild(selectedItem);
        InventoryGrid.AddItemToInventory(selectedItem, slotX, slotY);
        ItemClickDeselect(selectedItem);
    }

    public void ClearSelectedItem() {
        if (selectedItem != null) {
            selectedItem.Position = Vector2.Zero;

            selectedItem.IsClicked = false;
            selectedItem.ToggleBackground();
            selectedItem.ZIndex = 1;
            selectedItem.ClearOccupiedSlots();

            IsAnItemSelected = false;
            selectedItem = null;
        }
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

            if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillActive) {
                PlayerOwner.PlayerHUD.SkillPanel.HighlightActiveSlots();
            }
            else if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillSupport) {
                PlayerOwner.PlayerHUD.SkillPanel.HighlightSupportSlots();
            }
            else {
                foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
                    slot.HighlightSlot();
                }
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

            if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillActive || item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillSupport) {
                PlayerOwner.PlayerHUD.SkillPanel.RemoveAllHighlights();
            }
            else {
                foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
                    slot.RemoveHighlight();
                }
            }
        }
    }

    public void ItemClickDrop(InventoryItem item) {
        if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillActive || item.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillSupport) {
            PlayerOwner.PlayerHUD.SkillPanel.RemoveAllHighlights();
        }
        foreach (EquipmentSlot slot in GetEquipmentSlots(item.ItemReference.ItemAllBaseType)) {
            slot.RemoveHighlight();
        }

        InventoryGrid.RemoveItemFromInventory(item);
        item.RemoveTooltip();
        item.IsClicked = false;
        IsAnItemSelected = false;
        selectedItem = null;

        //item.ClearOccupiedSlots();
        WorldItem worldItem = item.ItemReference.ConvertToWorldItem();
        PlayerOwner.DropItemOnFloor(worldItem);
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

    // Handles special equipment combination cases not covered by just an equipment slot's permissions
    public bool CanEquipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            // If main hand is not empty
            if (WeaponSlotLeft.ItemInSlot != null) {
                WeaponItem mhItem = WeaponSlotLeft.ItemInSlot.ItemReference as WeaponItem;
                
                if (mhItem.ItemAllBaseType == EItemAllBaseType.Weapon1H) {
                    // Disallow equipping a quiver with 1-handed weapons
                    if (item.ItemReference.ItemAllBaseType == EItemAllBaseType.Quiver) {
                        return false;
                    }
                    // Disallow dual wielding 1-handed weapons of different types (melee or ranged)
                    else if (item.ItemReference.GetType().IsSubclassOf(typeof(WeaponItem))) {
                        WeaponItem wItem = item.ItemReference as WeaponItem;

                        if (mhItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee1H && wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged1H) {
                            return false;
                        }
                        else if (mhItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged1H && wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee1H) {
                            return false;
                        }
                    }
                }
                else if (mhItem.ItemAllBaseType == EItemAllBaseType.Weapon2H) {
                    // Allow equipping a quiver with a bow
                    if (mhItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged2H && item.ItemReference.ItemAllBaseType == EItemAllBaseType.Quiver) {
                        return true;
                    }

                    // Disallow other combinations
                    return false;
                }
            }
        }

        return true;
    }

    public void EquipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        item.ClearOccupiedSlots();
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        ResetSelectedItem(item);
        HandleWeaponSlots(slot, item);

        PlayerOwner.ApplyItemStats(slot, item.ItemReference);
    }

    public void UnequipItemInSlot(EquipmentSlot slot, InventoryItem item) {
        item.RemoveTooltip();
        slot.RemoveChild(item);

        if (!InventoryGrid.TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItemOnFloor(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }

        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            // FIXME: Dirty solution to swapping remaining 1h weapon from the off hand into the main hand when main hand is unequipped
            // Please rewrite equipping and unequipping logic to be cleaner than this :pleading:
            if (WeaponSlotRight.ItemInSlot != null && WeaponSlotRight.ItemInSlot.ItemReference.GetType().IsSubclassOf(typeof(WeaponItem))) {
                WeaponItem wItem = WeaponSlotRight.ItemInSlot.ItemReference as WeaponItem;
                
                if (wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee1H || wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged1H) {
                    InventoryItem ohItem = WeaponSlotRight.ItemInSlot;
                    WeaponSlotRight.RemoveChild(ohItem);
                    WeaponSlotRight.SetItem(null);
                    WeaponSlotLeft.SetItemSilent(ohItem);
                    WeaponSlotLeft.AddChild(ohItem);
                    PlayerOwner.AssignOffHand(null);
                    PlayerOwner.AssignMainHand(wItem);
                }
            }
            else {
                PlayerOwner.AssignMainHand(null);
            }
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            PlayerOwner.AssignOffHand(null);
        }

        PlayerOwner.RemoveItemStats(slot, item.ItemReference);
    }

    public void SwapItemsInSlot(EquipmentSlot slot, InventoryItem oldItem, InventoryItem newItem) {
        newItem.ClearOccupiedSlots();
        slot.RemoveChild(oldItem);
        newItem.GetParent().RemoveChild(newItem);
        slot.AddChild(newItem);

        if (!InventoryGrid.TryAddItemToInventory(ref oldItem)) {
            PlayerOwner.DropItemOnFloor(oldItem.ItemReference.ConvertToWorldItem());
        }
        else {
            oldItem.ToggleClickable();
            oldItem.ToggleBackground();
        }
        
        ResetSelectedItem(newItem);
        HandleWeaponSlots(slot, newItem);

        PlayerOwner.RemoveItemStats(slot, oldItem.ItemReference);
        PlayerOwner.ApplyItemStats(slot, newItem.ItemReference);
    }

    protected void HandleWeaponSlots(EquipmentSlot slot, InventoryItem item) {
        if (slot.Slot == EItemEquipmentSlot.WeaponLeft) {
            if (item.ItemReference.GetType().IsSubclassOf(typeof(WeaponItem))) {
                WeaponItem wItem = item.ItemReference as WeaponItem;

                if ((wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee1H || wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged1H) &&
                WeaponSlotRight.ItemInSlot != null && WeaponSlotRight.ItemInSlot.ItemReference.ItemAllBaseType == EItemAllBaseType.Quiver) {
                    WeaponSlotRight.UnequipItem(WeaponSlotRight.ItemInSlot);
                    PlayerOwner.AssignOffHand(null);
                }
                else if (wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponMelee2H && WeaponSlotRight.ItemInSlot != null) {
                    WeaponSlotRight.UnequipItem(WeaponSlotRight.ItemInSlot);
                    PlayerOwner.AssignOffHand(null);
                }
                else if (wItem.ItemWeaponBaseType == EItemWeaponBaseType.WeaponRanged2H && 
                WeaponSlotRight.ItemInSlot != null && WeaponSlotRight.ItemInSlot.ItemReference.ItemAllBaseType != EItemAllBaseType.Quiver) {
                    WeaponSlotRight.UnequipItem(WeaponSlotRight.ItemInSlot);
                    PlayerOwner.AssignOffHand(null);
                }

                PlayerOwner.AssignMainHand(wItem);
            }
        }
        else if (slot.Slot == EItemEquipmentSlot.WeaponRight) {
            if (WeaponSlotLeft.ItemInSlot == null && item.ItemReference.GetType().IsSubclassOf(typeof(WeaponItem))) {
                WeaponItem wItem = item.ItemReference as WeaponItem;

                // Extremely dirty solution to placing equipped 1h weapon from off hand to main hand when main hand is empty
                // Please rewrite equipping and unequipping logic to be cleaner than this :pleading:
                PlayerOwner.PlayerHUD.RemoveItemTooltip();
                slot.RemoveChild(item);
                slot.RemoveHighlight();
                slot.SetItem(null);
                WeaponSlotLeft.SetItem(item);
                WeaponSlotLeft.AddChild(item);
                PlayerOwner.AssignMainHand(wItem);
            }
            else {
                PlayerOwner.AssignOffHand(item.ItemReference);
            }
        }
    }

    protected void ResetSelectedItem(InventoryItem item) {
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

            case EItemAllBaseType.Quiver:
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
