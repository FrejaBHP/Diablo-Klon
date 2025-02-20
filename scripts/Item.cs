using Godot;
using System;

public enum EItemRarity {
	Common,
	Magic,
	Rare,
	Unique,
	What
}

public enum EItemEquipmentSlot {
	None,
	Head,
	Chest,
	Hands,
	Feet,
	Ring,
	Amulet,
	Weapon,
	Weapon1H,
	Weapon2H,
	Shield
}

public partial class Item {
	PackedScene worldItemScene = GD.Load<PackedScene>("res://worlditem.tscn");
	PackedScene inventoryItemScene = GD.Load<PackedScene>("res://hud_item.tscn");

	public Player PlayerOwner = null;

	// Alle disse burde flyttes væk herfra og ind i en separat tabel
	public string ItemName;
	public EItemRarity ItemRarity;
	public EItemEquipmentSlot ItemEquipmentSlot = EItemEquipmentSlot.None;
	public Texture2D ItemTexture;
	public bool IsInWorld = false;
	public bool IsPickedUp = false;

	protected int gridSizeX = 1;
	protected int gridSizeY = 1;
	
	protected WorldItem worldItemRef;
	protected InventoryItem invItemRef;

	public Vector2I GetGridSize() {
		return new Vector2I(gridSizeX, gridSizeY);
	}

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

	public void ConvertToInventoryItem(Player player) {
		InventoryItem inventoryItem = inventoryItemScene.Instantiate<InventoryItem>();
		inventoryItem.SetItemReference(this);
		inventoryItem.SetGridSize(gridSizeX, gridSizeY);

		if (player.PlayerInventory.TryAddItemToInventory(ref inventoryItem)) {
			if (IsInWorld) {
				worldItemRef.QueueFree();
				worldItemRef = null;
			}

			IsInWorld = false;
			IsPickedUp = true;
			PlayerOwner = player;
			invItemRef = inventoryItem;
			inventoryItem.InventoryReference = player.PlayerInventory;
		}
		else {
			inventoryItem.QueueFree();
		}
	}
}

public partial class TestItem23 : Item {
	public TestItem23() {
		gridSizeX = 2;
		gridSizeY = 3;
		ItemTexture = UILib.TextureItemD2LeatherArmour;
		ItemRarity = EItemRarity.Common;
		ItemEquipmentSlot = EItemEquipmentSlot.Chest;
		ItemName = "Test Item 2x3";
	}
}

public partial class TestItem22 : Item {
	public TestItem22() {
		gridSizeX = 2;
		gridSizeY = 2;
		ItemTexture = UILib.TextureItemD2LeatherGloves;
		ItemRarity = EItemRarity.Rare;
		ItemEquipmentSlot = EItemEquipmentSlot.Hands;
		ItemName = "Test Item 2x2";
	}
}

public partial class TestItem11 : Item {
	public TestItem11() {
		gridSizeX = 1;
		gridSizeY = 1;
		ItemTexture = UILib.TextureItemD2Ring0;
		ItemRarity = EItemRarity.Unique;
		ItemEquipmentSlot = EItemEquipmentSlot.Ring;
		ItemName = "Test Item 1x1";
	}
}
