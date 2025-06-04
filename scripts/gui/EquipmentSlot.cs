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

	public void GUIInput(InputEvent @event) {
		// On left-click
		if (@event.IsActionPressed("LeftClick")) {
			if (InventoryReference.IsAnItemSelected && InventoryReference.CanEquipItemInSlot(this, InventoryReference.SelectedItem) && CanEquipItem(InventoryReference.SelectedItem.ItemReference.ItemAllBaseType)) {
				if (itemInSlot == null) {
					SetItem(InventoryReference.SelectedItem);
					EmitSignal(SignalName.ItemEquipped, this, itemInSlot);
				}
				else {
					InventoryItem temp = InventoryReference.SelectedItem;
					EmitSignal(SignalName.ItemsSwapped, this, itemInSlot, InventoryReference.SelectedItem);
					SetItem(temp);
				}
			}
			else if (!InventoryReference.IsAnItemSelected && itemInSlot != null) {
				RemoveHighlight();
				InventoryItem ueItem = itemInSlot;
				SetItem(null);
				EmitSignal(SignalName.ItemUnequipped, this, ueItem);
			}
		}
	}

	public void OnMouseEntered() {
		IsHovered = true;
		
		if (itemInSlot != null) {
			HighlightSlot();
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateEquipmentTooltip(anchor, GetGlobalRect(), true);
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

		if (itemInSlot != null && IsHovered) {
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateEquipmentTooltip(anchor, GetGlobalRect(), true);
		}
	}

	public void SetItemSilent(InventoryItem item) {
		itemInSlot = item;
	}

	public void UnequipItem(InventoryItem item) {
		EmitSignal(SignalName.ItemUnequipped, this, item);
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

			case EItemAllBaseType.Quiver:
				if (Slot == EItemEquipmentSlot.WeaponRight) {
					return true;
				}
				return false;

			default:
				return false;
		}
	}
}
