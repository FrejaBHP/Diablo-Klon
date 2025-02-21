using Godot;
using System;

public partial class HUD : Control {
	public Player PlayerOwner;
	private Inventory playerInventory;

	public override void _Ready() {
		playerInventory = GetNode<Inventory>("Inventory");
	}

	public override void _Process(double delta) {
		
	}

	public void OnGUIInput(InputEvent @event) {
        // On left-click
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			// If an item is selected
			if (playerInventory.IsAnItemSelected && playerInventory.SelectedItem != null) {
                // If click is outside the inventory panel, drop it on the floor
                if (!playerInventory.GetGlobalRect().HasPoint(mbe.GlobalPosition) || !playerInventory.IsOpen) {
                    playerInventory.ItemClickDrop(playerInventory.SelectedItem);
                }
            }
			// If no items are held
			else {
				// If HUD elements are not obstructing the player
				
				// Since the HUD is set to always treat itself internally as 1280x720, scaling into the game world
				// will need to happen, because the player's viewport will follow the window size.
				// Stretched 1280x720 coordinates will not work in a 1920x1080 resolution or similar,
				// and therefore the HUD's input is scaled into the viewport's global space

				Vector2 playerViewportSize = PlayerOwner.GetViewport().GetVisibleRect().Size;
				Vector2 subViewportSize = GetGlobalRect().Size;
				Vector2 translationRatio = playerViewportSize / subViewportSize;

				Vector2 translatedPosition = mbe.GlobalPosition * translationRatio;

				PlayerOwner.SetDestinationPosition(translatedPosition);
			}
		}
	}
}
