using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Control {
    public Player PlayerOwner;
	private GridContainer inventoryGridContainer;

	private const int inventorySizeX = 13;
	private const int inventorySizeY = 5;

    private InventoryGridSquare[,] invGridSquares = new InventoryGridSquare[inventorySizeX, inventorySizeY];

    private bool isOpen = false;
    private bool isAnItemSelected = false;
    private InventoryItem selectedItem = null;

    public override void _Ready() {
        inventoryGridContainer = GetNode<GridContainer>("InventoryContainer/InventoryGrid");

        GenerateInventory();
    }

    public void OnGUIInput(InputEvent @event) {
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (isAnItemSelected && selectedItem != null) {
                if (!GetGlobalRect().HasPoint(mbe.GlobalPosition)) {
                    ItemClickDrop(selectedItem);
                }
                else if (inventoryGridContainer.GetGlobalRect().HasPoint(selectedItem.GlobalPosition)) {
                    Vector2 offset = selectedItem.GlobalPosition - inventoryGridContainer.GlobalPosition;
                    Vector2 slotSize = invGridSquares[0, 0].Size;

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

    private void GenerateInventory() {
        inventoryGridContainer.Columns = inventorySizeX;

        for (int i = 0; i < inventorySizeY; i++) {
            for (int j = 0; j < inventorySizeX; j++) {
                InventoryGridSquare newSquare = new(new Vector2I(j, i));
                invGridSquares[j, i] = newSquare;
                inventoryGridContainer.AddChild(newSquare);
            }
        }
    }

    public bool TryAddItemToInventory(ref InventoryItem item) {
        for (int i = 0; i < inventorySizeX - (item.GetGridSize().X - 1); i++) {
            for (int j = 0; j < inventorySizeY - (item.GetGridSize().Y - 1); j++) {
                if (invGridSquares[i, j].IsEmpty) {
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
        invGridSquares[slotX, slotY].AddChild(item);
    }

    public void MoveSelectedItemToSlot(int slotX, int slotY) {
        selectedItem.GetParent().RemoveChild(selectedItem);
        AddItemToInventory(selectedItem, slotX, slotY);
        ItemClickDeselect(selectedItem);
    }

    public bool CanFitInSlot(int slotX, int slotY, InventoryItem item) {
        List<InventoryGridSquare> tempUsedSlots = new List<InventoryGridSquare>();

        for (int i = 0; i < item.GetGridSize().X; i++) {
            for (int j = 0; j < item.GetGridSize().Y; j++) {
                //GD.Print("Testing slot " + (slotX + i) + ", " + (slotY + j));
                if (!invGridSquares[slotX + i, slotY + j].IsEmpty) {
                    return false;
                }
                tempUsedSlots.Add(invGridSquares[slotX + i, slotY + j]);
            }
        }

        item.SetOccupiedSlots(tempUsedSlots);
        return true;
    }

    public void ItemClickSelect(InventoryItem item) {
        GD.Print("Item Selected!");
        if (!isAnItemSelected && selectedItem == null) {
            item.IsClicked = true;
            isAnItemSelected = true;
            selectedItem = item;
            item.ToggleClickable();
            selectedItem.ClearOccupiedSlots();
        }
    }

    public void ItemClickDeselect(InventoryItem item) {
        if (isAnItemSelected && selectedItem != null) {
            GD.Print("Item Deselected!");
            item.Position = Vector2.Zero;
            item.IsClicked = false;
            isAnItemSelected = false;
            selectedItem = null;
            item.ToggleClickable();
        }
    }

    public void ItemClickDrop(InventoryItem item) {
        GD.Print("Item Dropped!");
        item.IsClicked = false;
        isAnItemSelected = false;
        selectedItem = null;

        item.ClearOccupiedSlots();
        WorldItem worldItem = item.ItemReference.ConvertToWorldItem();
        PlayerOwner.DropItem(worldItem);
    }

    public void ToggleInventory() {
        if (!isOpen) {
            Visible = true;
            isOpen = true;
        }
        else {
            Visible = false;
            isOpen = false;
        }
    }
}
