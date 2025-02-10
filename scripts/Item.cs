using Godot;
using System;

public enum EItemRarity {
	Common,
	Magic,
	Rare,
	Unique,
	What
}

public partial class Item {
	PackedScene worldItemScene = GD.Load<PackedScene>("res://worlditem.tscn");
	PackedScene inventoryItemScene = GD.Load<PackedScene>("res://hud_item.tscn");

	public Player PlayerOwner = null;

	// Alle disse burde flyttes væk herfra og ind i en separat tabel
	public string ItemName;
	public EItemRarity ItemRarity;
	public Texture2D ItemTexture;
	public bool IsInWorld = false;
	public bool IsPickedUp = false;

	protected int gridSizeX = 1;
	protected int gridSizeY = 1;
	
	protected WorldItem worldItemRef;
	protected InventoryItem invItemRef;

	public WorldItem ConvertToWorldItem() {
		PlayerOwner = null;
		WorldItem worldItem = worldItemScene.Instantiate<WorldItem>();
		worldItem.SetItemReference(this);

		worldItemRef = worldItem;

		if (IsPickedUp) {
			invItemRef.QueueFree();
			invItemRef = null;
		}

		IsInWorld = true;
		IsPickedUp = false;

		return worldItem;
	}

	public void ConvertToInventoryItem(Player newOwner) {
		PlayerOwner = newOwner;
		InventoryItem inventoryItem = inventoryItemScene.Instantiate<InventoryItem>();
		inventoryItem.SetItemReference(this);
		inventoryItem.SetGridSize(gridSizeX, gridSizeY);

		if (IsInWorld) {
			worldItemRef.QueueFree();
			worldItemRef = null;
		}

		IsInWorld = false;
		IsPickedUp = true;
		
		PlayerOwner.PlayerInventory.TryAddItemToInventory(ref inventoryItem);
		//PlayerOwner.PlayerInventory.AddChild(inventoryItem);
	}
}

public partial class TestItem : Item {
	public TestItem() {
		gridSizeX = 2;
		gridSizeY = 3;
		ItemTexture = UITextureLib.TestItem;
		ItemRarity = EItemRarity.Common;
		ItemName = "Test Item";
	}
}
