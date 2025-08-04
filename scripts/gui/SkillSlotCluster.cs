using Godot;
using System;
using System.Collections.Generic;

public partial class SkillSlotCluster : VBoxContainer {
	[Signal]
    public delegate void ClusterChangedEventHandler(SkillSlotCluster cluster);

	[Signal]
    public delegate void ActiveSkillEquippedEventHandler(SkillSlotCluster cluster, Control slot, InventoryItem skill);

	[Signal]
    public delegate void ActiveSkillUnequippedEventHandler(SkillSlotCluster cluster, Control slot, InventoryItem skill);

	[Signal]
    public delegate void ActiveSkillsSwappedEventHandler(SkillSlotCluster cluster, Control slot, InventoryItem oldSkill, InventoryItem newSkill);

	[Signal]
    public delegate void SupportGemEquippedEventHandler(SkillSlotSupport slot, InventoryItem support);

	[Signal]
    public delegate void SupportGemUnequippedEventHandler(SkillSlotSupport slot, InventoryItem support);

	[Signal]
    public delegate void SupportGemsSwappedEventHandler(SkillSlotSupport slot, InventoryItem oldSupport, InventoryItem newSupport);

	public Label SkillNameLabel { get; protected set; }

	public SkillSlotActive ActiveSlot { get; protected set; }
	public SkillSlotSupport[] SupportSlots { get; protected set; } = new SkillSlotSupport[3];

	public PlayerInventory InventoryReference;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		SkillNameLabel = GetNode<Label>("InfoMargin/InfoContainer/SkillName");
		SkillNameLabel.Text = "Unassigned";

		ActiveSlot = GetNode<SkillSlotActive>("SlotsMargin/SlotsContainer/ActiveSlot");
		SupportSlots[0] = GetNode<SkillSlotSupport>("SlotsMargin/SlotsContainer/SupportSlotsContainer/SupportSlotLeft");
		SupportSlots[1] = GetNode<SkillSlotSupport>("SlotsMargin/SlotsContainer/SupportSlotsContainer/SupportSlotMiddle");
		SupportSlots[2] = GetNode<SkillSlotSupport>("SlotsMargin/SlotsContainer/SupportSlotsContainer/SupportSlotRight");

		ActiveSlot.SkillChanged += UpdateCluster;
		ActiveSlot.SkillEquipped += OnActiveSkillEquipped;
		ActiveSlot.SkillUnequipped += OnActiveSkillUnequipped;
		ActiveSlot.SkillsSwapped += OnActiveSkillsSwapped;

		for (int i = 0; i < SupportSlots.Length; i++) {
			SupportSlots[i].SupportChanged += UpdateCluster;
			SupportSlots[i].SupportEquipped += OnSupportEquipped;
			SupportSlots[i].SupportUnequipped += OnSupportUnequipped;
			SupportSlots[i].SupportsSwapped += OnSupportsSwapped;
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

	public void UpdateCluster() {
		if (ActiveSlot.ItemInSlot != null) {
			SkillItem skillItem = (SkillItem)ActiveSlot.ItemInSlot.ItemReference;
			SkillNameLabel.Text = skillItem.ItemName;
			skillItem.SkillReference.RecalculateSkillValues();
		}
		else {
			SkillNameLabel.Text = "Unassigned";
		}
	}

	public void OnActiveSkillEquipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillEquipped, this, slot, item);
	}

	public void OnActiveSkillUnequipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.ActiveSkillUnequipped, this, slot, item);
	}

	public void OnActiveSkillsSwapped(Control slot, InventoryItem oldSkill, InventoryItem newSkill) {
		EmitSignal(SignalName.ActiveSkillsSwapped, this, slot, oldSkill, newSkill);
	}

	public void OnSupportEquipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.SupportGemEquipped, slot, item);
	}

	public void OnSupportUnequipped(Control slot, InventoryItem item) {
		EmitSignal(SignalName.SupportGemUnequipped, slot, item);
	}

	public void OnSupportsSwapped(SkillSlotSupport slot, InventoryItem oldSupport, InventoryItem newSupport) {
		EmitSignal(SignalName.SupportGemsSwapped, slot, oldSupport, newSupport);
	}

	public List<SupportGem> GetSupports() {
		List<SupportGem> supports = new();

		for (int i = 0; i < SupportSlots.Length; i++) {
			if (SupportSlots[i].ItemInSlot != null) {
				SupportGem supportGem = (SupportGem)SupportSlots[i].ItemInSlot.ItemReference;
				supports.Add(supportGem);
			}
		}

		return supports;
	}
}
