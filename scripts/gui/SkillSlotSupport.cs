using Godot;
using System;

public partial class SkillSlotSupport : Control {
	[Signal]
    public delegate void SupportEquippedEventHandler(SkillSlotSupport slot, InventoryItem skill);

	[Signal]
    public delegate void SupportUnequippedEventHandler(SkillSlotSupport slot, InventoryItem skill);

	[Signal]
    public delegate void SupportsSwappedEventHandler(SkillSlotSupport slot, InventoryItem oldSkill, InventoryItem newSkill);

	public PlayerInventory InventoryReference;
	
	public readonly EItemEquipmentSlot Slot = EItemEquipmentSlot.SkillSupport;

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
			if (InventoryReference.IsAnItemSelected && InventoryReference.SelectedItem.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillSupport) {
				if (itemInSlot == null) {
					SetSupport(InventoryReference.SelectedItem);
					EmitSignal(SignalName.SupportEquipped, this, itemInSlot);
				}
				else {
					InventoryItem temp = InventoryReference.SelectedItem;
					SetSupport(temp);
					EmitSignal(SignalName.SupportsSwapped, this, itemInSlot, InventoryReference.SelectedItem);
				}
			}
			else if (!InventoryReference.IsAnItemSelected && itemInSlot != null) {
				RemoveHighlight();
				EmitSignal(SignalName.SupportUnequipped, this, itemInSlot);
				SetSupport(null);
			}
		}
	}

	public void OnMouseEntered() {
		IsHovered = true;
		
		if (itemInSlot != null) {
			HighlightSlot();
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateSupportItemTooltip(anchor, GetGlobalRect(), false);
		}
	}

	public void OnMouseExited() {
		IsHovered = false;

		if (itemInSlot != null) {
			RemoveHighlight();
			InventoryReference.PlayerOwner.PlayerHUD.RemoveItemTooltip();
		}
	}

	public void SetSupport(InventoryItem item) {
		itemInSlot = item;

		if (itemInSlot != null && IsHovered) {
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateSupportItemTooltip(anchor, GetGlobalRect(), false);
		}
	}

	public void RemoveSupport(InventoryItem item) {
		//EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
		SetSupport(null);
	}

	public void HighlightSlot() {
		highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		highlight.Color = UILib.ColorTransparent;
	}
}
