using Godot;
using System;

public partial class SkillSlotCluster : Control {
	[Signal]
    public delegate void ClusterChangedEventHandler(SkillSlotCluster cluster);

	[Signal]
    public delegate void ActiveSkillEquippedEventHandler(Control slot, InventoryItem skill);

	[Signal]
    public delegate void ActiveSkillUnequippedEventHandler(Control slot, InventoryItem skill);

	public SkillSlotActive ActiveSlot { get; protected set; }
	public SkillSlotSupport SupportSlotLeft { get; protected set; }
	public SkillSlotSupport SupportSlotMiddle { get; protected set; }
	public SkillSlotSupport SupportSlotRight { get; protected set; }

	public PlayerInventory InventoryReference;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ActiveSlot = GetNode<SkillSlotActive>("ActiveSlot");
		SupportSlotLeft = GetNode<SkillSlotSupport>("SupportSlotLeft");
		SupportSlotMiddle = GetNode<SkillSlotSupport>("SupportSlotMiddle");
		SupportSlotRight = GetNode<SkillSlotSupport>("SupportSlotRight");

		ActiveSlot.SkillEquipped += OnActiveSkillEquipped;
		ActiveSlot.SkillUnequipped += OnActiveSkillUnequipped;
	}

	public void SetInventoryReference(PlayerInventory inventory) {
		InventoryReference = inventory;

		ActiveSlot.InventoryReference = InventoryReference;
		SupportSlotLeft.InventoryReference = InventoryReference;
		SupportSlotMiddle.InventoryReference = InventoryReference;
		SupportSlotRight.InventoryReference = InventoryReference;
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

	public void HighlightSlot() {
		//highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		//highlight.Color = UILib.ColorTransparent;
	}
}
