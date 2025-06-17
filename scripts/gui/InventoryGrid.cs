using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryGrid : GridContainer {
    [Signal]
    public delegate void ItemClickedEventHandler(InventoryItem item);

    public int InventorySizeX { get; protected set; }
    public int InventorySizeY { get; protected set; }

    public InventoryGridCell[,] InvGridCells;

    protected List<InventoryGridCell> tempUsedSlots = new List<InventoryGridCell>();
    protected List<InventoryItem> inventoryItems = new List<InventoryItem>(); // Potentially move this away to parent inventory

    public override void _Ready() {

    }

    public void GenerateGrid(int sizeX, int sizeY) {
        Columns = sizeX;
        InventorySizeX = sizeX;
        InventorySizeY = sizeY;
        InvGridCells = new InventoryGridCell[InventorySizeX, InventorySizeY];

        for (int i = 0; i < sizeY; i++) {
            for (int j = 0; j < sizeX; j++) {
                InventoryGridCell newCell = new(new Vector2I(j, i));
                InvGridCells[j, i] = newCell;
                AddChild(newCell);
            }
        }
    }

    // Goes through the inventory slots one by one (ignoring rows and columns that would be impossible to fit into due to size) to find an open and suitable slot to add it to
    // The order is determined as going through each column, top to bottom, left to right
    // FIXME: Ultimately, however, this deserves a rewrite, because it is messy when you want to check for available space without adding the item,
    // and if you do want to add an item later, you have to scan the grid twice. Messy and bad.
    /// <summary>
    /// Finds the first available slot in the grid that can house the provided item. If one is found, it is added to the grid.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryAddItemToInventory(ref InventoryItem item) {
        for (int i = 0; i < InventorySizeX - (item.GetGridSize().X - 1); i++) {
            for (int j = 0; j < InventorySizeY - (item.GetGridSize().Y - 1); j++) {
                if (InvGridCells[i, j].IsEmpty) {
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
        InvGridCells[slotX, slotY].AddChild(item);
        item.Position = Vector2.Zero;
        item.SetOccupiedSlots(tempUsedSlots);
        inventoryItems.Add(item);
        item.InventoryReference = this;
    }

    public void RemoveItemFromInventory(InventoryItem item) {
        item.ClearOccupiedSlots();
        item.GetParent().RemoveChild(item);
        inventoryItems.Remove(item);
    }

    /// <summary>
    /// Works the same as TryAddItemToInventory, but does not set any internal values and will not add the item on success.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CanFitInInventory(ref InventoryItem item) {
        for (int i = 0; i < InventorySizeX - (item.GetGridSize().X - 1); i++) {
            for (int j = 0; j < InventorySizeY - (item.GetGridSize().Y - 1); j++) {
                if (InvGridCells[i, j].IsEmpty) {
                    if (CanFitInSlotNoSet(i, j, item)) {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    public bool CanFitInSlot(int slotX, int slotY, InventoryItem item) {
        tempUsedSlots.Clear();
        
        for (int i = 0; i < item.GetGridSize().X; i++) {
            for (int j = 0; j < item.GetGridSize().Y; j++) {
                if (slotX + i >= InventorySizeX || slotY + j >= InventorySizeY || !InvGridCells[slotX + i, slotY + j].IsEmpty) {
                    return false;
                }
                tempUsedSlots.Add(InvGridCells[slotX + i, slotY + j]);
            }
        }

        return true;
    }

    /// <summary>
    /// Same as CanFitInSlot, but does not set any internal values, and can not be used to place an item
    /// </summary>
    /// <param name="slotX"></param>
    /// <param name="slotY"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected bool CanFitInSlotNoSet(int slotX, int slotY, InventoryItem item) {
        for (int i = 0; i < item.GetGridSize().X; i++) {
            for (int j = 0; j < item.GetGridSize().Y; j++) {
                if (slotX + i >= InventorySizeX || slotY + j >= InventorySizeY || !InvGridCells[slotX + i, slotY + j].IsEmpty) {
                    return false;
                }
            }
        }

        return true;
    }

    public void ClearItems() {
        foreach (InventoryItem item in inventoryItems) {
            item.ClearOccupiedSlots();
            item.GetParent().RemoveChild(item);
        }

        inventoryItems.Clear();
    }

    public void OnItemClicked(InventoryItem item) {
        EmitSignal(SignalName.ItemClicked, item);
    }

    public void OnItemMouseEntered(InventoryItem item) {

    }

    public void OnItemMouseExited(InventoryItem item) {

    }

    public List<InventoryItem> GetInventoryItems() {
        return inventoryItems;
    }
}
