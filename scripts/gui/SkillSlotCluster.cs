using Godot;
using System;
using System.Collections.Generic;

public partial class SkillSlotCluster : Control {
	[Signal]
    public delegate void ClusterChangedEventHandler(SkillSlotCluster cluster);

	[Signal]
    public delegate void ActiveSkillEquippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void ActiveSkillUnequippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void SupportEquippedEventHandler(Control slot, InventoryItem support);

	[Signal]
    public delegate void SupportUnequippedEventHandler(Control slot, InventoryItem support);

	public SkillSlotActive ActiveSlot { get; protected set; }
	public SkillSlotSupport[] SupportSlots { get; protected set; } = new SkillSlotSupport[3];

	public PlayerInventory InventoryReference;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ActiveSlot = GetNode<SkillSlotActive>("ActiveSlot");
		SupportSlots[0] = GetNode<SkillSlotSupport>("SupportSlotLeft");
		SupportSlots[1] = GetNode<SkillSlotSupport>("SupportSlotMiddle");
		SupportSlots[2] = GetNode<SkillSlotSupport>("SupportSlotRight");

		ActiveSlot.SkillEquipped += OnActiveSkillEquipped;
		ActiveSlot.SkillUnequipped += OnActiveSkillUnequipped;

		for (int i = 0; i < SupportSlots.Length; i++) {
			SupportSlots[i].SupportEquipped += OnSupportEquipped;
			SupportSlots[i].SupportUnequipped += OnSupportUnequipped;
		}
	}

	public void SetInventoryReference(PlayerInventory inventory) {
		InventoryReference = inventory;

		ActiveSlot.InventoryReference = InventoryReference;
		for (int i = 0; i < SupportSlots.Length; i++) {
			SupportSlots[i].InventoryReference = InventoryReference;
		}
	}

	public void GUIInput(InputEvent @event) {

	}

	public void OnMouseEntered() {

	}

	public void OnMouseExited() {

	}

	public void OnActiveSkillEquipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillEquipped, slot, item);
	}

	public void OnActiveSkillUnequipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillUnequipped, slot, item);
	}

	public void OnSupportEquipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.SupportEquipped, slot, item);

		if (ActiveSlot.ItemInSlot != null) {
			List<SkillSupportItem> supports = new();

			for (int i = 0; i < SupportSlots.Length; i++) {
				if (SupportSlots[i].ItemInSlot != null) {
					SkillSupportItem supportItem = (SkillSupportItem)SupportSlots[i].ItemInSlot.ItemReference;
					supports.Add(supportItem);
				}
			}
			SkillItem skillItem = (SkillItem)ActiveSlot.ItemInSlot.ItemReference;
			skillItem.SkillReference.UpdateSupportStatDictionary(supports);
		}
	}

	public void OnSupportUnequipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.SupportUnequipped, slot, item);

		if (ActiveSlot.ItemInSlot != null) {
			List<SkillSupportItem> supports = new();

			for (int i = 0; i < SupportSlots.Length; i++) {
				if (SupportSlots[i].ItemInSlot != null && SupportSlots[i].ItemInSlot != item) {
					SkillSupportItem supportItem = (SkillSupportItem)SupportSlots[i].ItemInSlot.ItemReference;
					supports.Add(supportItem);
				}
			}
			SkillItem skillItem = (SkillItem)ActiveSlot.ItemInSlot.ItemReference;
			skillItem.SkillReference.UpdateSupportStatDictionary(supports);
		}
	}

	public void HighlightSlot() {
		//highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		//highlight.Color = UILib.ColorTransparent;
	}
}
