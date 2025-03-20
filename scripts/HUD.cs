using Godot;
using System;

public partial class HUD : Control {
	PackedScene itemTooltipPopupScene = GD.Load<PackedScene>("res://hud_item_tooltip_popup.tscn");
	
	public Player PlayerOwner;
	public CharacterPanel PlayerPanel;
	public Inventory PlayerInventory;

	private ItemTooltipPopup activeTooltipPopup;

	public override void _Ready() {
		PlayerPanel = GetNode<CharacterPanel>("PlayerPanel");
		PlayerInventory = GetNode<Inventory>("Inventory");
	}

	public override void _Process(double delta) {
		
	}

	/* UNUSED
	public void OnGUIInput(InputEvent @event) {
        // On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			// If an item is selected
			if (PlayerInventory.IsAnItemSelected && PlayerInventory.SelectedItem != null) {
                // If click is outside the inventory panel, drop it on the floor
                if (!PlayerInventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !PlayerInventory.IsOpen) {
                    PlayerInventory.ItemClickDrop(PlayerInventory.SelectedItem);
                }
            }
		}
	}
	*/

    public void CreateItemTooltip(Control tooltipContent, Vector2 anchor, Rect2 itemRect) {
		ItemTooltipPopup tooltipPopup = itemTooltipPopupScene.Instantiate<ItemTooltipPopup>();
		tooltipPopup.ItemRect = itemRect;
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

	/* UNUSED
	public Vector2 SubViewportToViewport(Vector2 globalPosition) {
		Vector2 playerViewportSize = PlayerOwner.GetViewport().GetVisibleRect().Size;
		Vector2 subViewportSize = GetGlobalRect().Size;
		Vector2 translationRatio = playerViewportSize / subViewportSize;

		Vector2 translatedPosition = globalPosition * translationRatio;

		return translatedPosition;
	}
	*/
}
