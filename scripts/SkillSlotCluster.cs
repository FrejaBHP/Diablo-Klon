using Godot;
using System;

public partial class SkillSlotCluster : Control {
	[Signal]
    public delegate void ClusterChangedEventHandler(SkillSlotCluster cluster);

	public SkillSlotActive ActiveSlot { get; protected set; }
	public SkillSlotSupport SupportSlotLeft { get; protected set; }
	public SkillSlotSupport SupportSlotMiddle { get; protected set; }
	public SkillSlotSupport SupportSlotRight { get; protected set; }









	[Signal]
    public delegate void ActiveSkillEquippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void ActiveSkillUnequippedEventHandler(Control slot, InventoryItem skill);

	public Inventory InventoryReference;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ActiveSlot = GetNode<SkillSlotActive>("ActiveSlot");
		SupportSlotLeft = GetNode<SkillSlotSupport>("SupportSlotLeft");
		SupportSlotMiddle = GetNode<SkillSlotSupport>("SupportSlotMiddle");
		SupportSlotRight = GetNode<SkillSlotSupport>("SupportSlotRight");

		ActiveSlot.SkillEquipped += OnActiveSkillEquipped;
		ActiveSlot.SkillUnequipped += OnActiveSkillUnequipped;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}

	public void SetInventoryReference(Inventory inventory) {
		InventoryReference = inventory;

		ActiveSlot.InventoryReference = InventoryReference;
		SupportSlotLeft.InventoryReference = InventoryReference;
		SupportSlotMiddle.InventoryReference = InventoryReference;
		SupportSlotRight.InventoryReference = InventoryReference;
	}

	public void GUIInput(InputEvent @event) {
		/*
		// On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
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
				EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
				SetItem(null);
			}
		}
		*/
	}

	public void OnMouseEntered() {
		/*
		IsHovered = true;
		
		if (itemInSlot != null) {
			HighlightSlot();
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			//itemInSlot.SignalCreateEquipmentTooltip(anchor, GetGlobalRect());
		}
		*/
	}

	public void OnMouseExited() {
		/*
		IsHovered = false;

		if (itemInSlot != null) {
			RemoveHighlight();

			//InventoryReference.PlayerOwner.PlayerHUD.RemoveItemTooltip();
		}
		*/
	}

	public void OnActiveSkillEquipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillEquipped, slot, item);
	}

	public void OnActiveSkillUnequipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillUnequipped, slot, item);
	}

	public void EquipSkill(InventoryItem item) {
		/*
		itemInSlot = item;

		if (itemInSlot != null && IsHovered) {
			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			itemInSlot.SignalCreateEquipmentTooltip(anchor, GetGlobalRect());
		}
		*/
	}

	public void UnequipItem(InventoryItem item) {
		//EmitSignal(SignalName.ItemUnequipped, this, itemInSlot);
		//SetItem(null);
	}

	public void HighlightSlot() {
		//highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		//highlight.Color = UILib.ColorTransparent;
	}
}
