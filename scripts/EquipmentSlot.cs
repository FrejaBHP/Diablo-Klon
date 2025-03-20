using Godot;
using System;

public partial class EquipmentSlot : PanelContainer {
	[Signal]
    public delegate void ItemEquippedEventHandler(EquipmentSlot slot, InventoryItem item);

	[Signal]
    public delegate void ItemUnequippedEventHandler(EquipmentSlot slot, InventoryItem item);

	[Signal]
    public delegate void ItemsSwappedEventHandler(EquipmentSlot slot, InventoryItem oldItem, InventoryItem newItem);

	public Inventory InventoryReference;
	
	public EItemEquipmentSlot Slot;

	private InventoryItem itemInSlot = null;
	public InventoryItem ItemInSlot {
		get { return itemInSlot; }
	}

	public bool IsHovered = false;

	private ColorRect highlight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		highlight = GetNode<ColorRect>("Highlight");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}

	public void OnGUIInput(InputEvent @event) {
        // On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			// Implement proper check for different weapon types and slots later, since this will now always fail for weapon slots
			if (InventoryReference.IsAnItemSelected && InventoryReference.CanEquipItemInSlot(this, InventoryReference.SelectedItem) && CanEquipItem(InventoryReference.SelectedItem.ItemReference.ItemAllBaseType)) {
				if (itemInSlot == null) {
					//GD.Print("Equip - slot empty");
					SetItem(InventoryReference.SelectedItem);
					EmitSignal(SignalName.ItemEquipped, this, itemInSlot);
				}
				else {
					//GD.Print("Equip - slot not empty");
					InventoryItem temp = InventoryReference.SelectedItem;
					EmitSignal(SignalName.ItemsSwapped, this, itemInSlot, InventoryReference.SelectedItem);
					SetItem(temp);
				}
			}
			else if (!InventoryReference.IsAnItemSelected && itemInSlot != null) {
				//GD.Print("Unequip - slot now empty");
				RemoveHighlight();
				EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
				SetItem(null);
			}
		}
	}

	public void OnMouseEntered() {
		IsHovered = true;
		
		if (itemInSlot != null) {
			HighlightSlot();
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateTooltip(anchor, GetGlobalRect());
		}
	}

	public void OnMouseExited() {
		IsHovered = false;

		if (itemInSlot != null) {
			RemoveHighlight();

			InventoryReference.PlayerOwner.PlayerHUD.RemoveItemTooltip();
		}
	}

	public void SetItem(InventoryItem item) {
		itemInSlot = item;
	}

	public void UnequipItem(InventoryItem item) {
		//GD.Print("Unequip - slot now empty");
		EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
		SetItem(null);
	}

	public void HighlightSlot() {
		highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		highlight.Color = UILib.ColorTransparent;
	}

	private bool CanEquipItem(EItemAllBaseType itemType) {
		switch (itemType) {
			case EItemAllBaseType.Head:
				if (Slot == EItemEquipmentSlot.Head) {
					return true;
				}
				return false;

			case EItemAllBaseType.Chest:
				if (Slot == EItemEquipmentSlot.Chest) {
					return true;
				}
				return false;

			case EItemAllBaseType.Hands:
				if (Slot == EItemEquipmentSlot.Hands) {
					return true;
				}
				return false;

			case EItemAllBaseType.Feet:
				if (Slot == EItemEquipmentSlot.Feet) {
					return true;
				}
				return false;

			case EItemAllBaseType.Belt:
				if (Slot == EItemEquipmentSlot.Belt) {
					return true;
				}
				return false;

			case EItemAllBaseType.Ring:
				if (Slot == EItemEquipmentSlot.Ring) {
					return true;
				}
				return false;

			case EItemAllBaseType.Amulet:
				if (Slot == EItemEquipmentSlot.Amulet) {
					return true;
				}
				return false;

			case EItemAllBaseType.Weapon1H:
				if (Slot == EItemEquipmentSlot.WeaponLeft || Slot == EItemEquipmentSlot.WeaponRight) {
					return true;
				}
				return false;

			case EItemAllBaseType.Weapon2H:
				if (Slot == EItemEquipmentSlot.WeaponLeft) {
					return true;
				}
				return false;

			case EItemAllBaseType.Shield:
				if (Slot == EItemEquipmentSlot.WeaponRight) {
					return true;
				}
				return false;

			default:
				return false;
		}
	}
}
