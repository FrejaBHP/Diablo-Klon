using Godot;
using System;

public partial class Inventory : Control {
	private GridContainer inventoryGridContainer;

	private const int inventorySizeX = 13;
	private const int inventorySizeY = 5;

    private InventoryGridSquare[,] invGridSquares = new InventoryGridSquare[inventorySizeX, inventorySizeY];

    private bool isOpen = false;

    public override void _Ready() {
        inventoryGridContainer = GetNode<GridContainer>("InventoryContainer/InventoryGrid");

        GenerateInventory();
    }

    private void GenerateInventory() {
        inventoryGridContainer.Columns = inventorySizeX;

        for (int i = 0; i < inventorySizeY; i++) {
            for (int j = 0; j < inventorySizeX; j++) {
                InventoryGridSquare newSquare = new(new Vector2(i, j));
                invGridSquares[i, j] = newSquare;
                inventoryGridContainer.AddChild(newSquare);
            }
        }
    }

    public bool TryAddItemToInventory(ref InventoryItem item) {
        

        AddChild(item);
        return true;
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
