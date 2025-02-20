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
				PlayerOwner.SetDestinationPosition(mbe.Position);
			}
		}
	}
}
