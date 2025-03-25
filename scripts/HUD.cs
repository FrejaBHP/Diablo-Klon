using Godot;
using System;

public partial class HUD : Control {
	PackedScene itemTooltipPopupScene = GD.Load<PackedScene>("res://hud_item_tooltip_popup.tscn");
	
	public Player PlayerOwner;
	public CharacterPanel PlayerPanel;
	public Inventory PlayerInventory;
	public LowerHUD PlayerLowerHUD;

	private ItemTooltipPopup activeTooltipPopup = null;

	public override void _Ready() {
		PlayerPanel = GetNode<CharacterPanel>("PlayerPanel");
		PlayerInventory = GetNode<Inventory>("Inventory");
		PlayerLowerHUD = GetNode<LowerHUD>("LowerHUD");
	}

    public void CreateItemTooltip(Control tooltipContent, Vector2 anchor, Rect2 itemRect) {
		RemoveItemTooltip();
		
		ItemTooltipPopup tooltipPopup = itemTooltipPopupScene.Instantiate<ItemTooltipPopup>();
		tooltipPopup.ItemRect = itemRect;
		tooltipPopup.Unfocusable = true; // If false, consumes all input events while open
		AddChild(tooltipPopup);
		activeTooltipPopup = tooltipPopup;

		tooltipPopup.AddChild(tooltipContent);
		tooltipPopup.Position = (Vector2I)anchor;
    }

	public void RemoveItemTooltip() {
		if (activeTooltipPopup != null) {
			activeTooltipPopup.QueueFree();
			activeTooltipPopup = null;
		}
	}
}
