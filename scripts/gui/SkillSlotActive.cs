using Godot;
using System;

public partial class SkillSlotActive : Control {
	[Signal]
    public delegate void SkillEquippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void SkillUnequippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void SkillsSwappedEventHandler(Control slot, InventoryItem oldSkill, InventoryItem newSkill);

	public PlayerInventory InventoryReference;
	
	public readonly EItemEquipmentSlot Slot = EItemEquipmentSlot.SkillActive;

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

	public void GUIInput(InputEvent @event) {
		// On left-click
		if (@event.IsActionPressed("LeftClick")) {
			if (InventoryReference.IsAnItemSelected && InventoryReference.SelectedItem.ItemReference.ItemAllBaseType == EItemAllBaseType.SkillActive) {
				if (itemInSlot == null) {
					SetSkill(InventoryReference.SelectedItem);
					EmitSignal(SignalName.SkillEquipped, this, itemInSlot);
				}
				else {
					InventoryItem temp = InventoryReference.SelectedItem;
					EmitSignal(SignalName.SkillsSwapped, this, itemInSlot, InventoryReference.SelectedItem);
					SetSkill(temp);
				}
			}
			else if (!InventoryReference.IsAnItemSelected && itemInSlot != null) {
				RemoveHighlight();
				EmitSignal(SignalName.SkillUnequipped, this, itemInSlot);
				SetSkill(null);
			}
		}
	}

	public void OnMouseEntered() {
		IsHovered = true;
		
		if (itemInSlot != null) {
			HighlightSlot();
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateSkillItemTooltip(anchor, GetGlobalRect(), false);
		}
	}

	public void OnMouseExited() {
		IsHovered = false;

		if (itemInSlot != null) {
			RemoveHighlight();
			InventoryReference.PlayerOwner.PlayerHUD.RemoveItemTooltip();
		}
	}

	public void SetSkill(InventoryItem item) {
		itemInSlot = item;

		if (itemInSlot != null && IsHovered) {
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateSkillItemTooltip(anchor, GetGlobalRect(), false);
		}
	}

	public void RemoveSkill(InventoryItem item) {
		//EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
		SetSkill(null);
	}

	public void HighlightSlot() {
		highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		highlight.Color = UILib.ColorTransparent;
	}
}
