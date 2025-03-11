using Godot;
using System;

public partial class HUD : Control {
	PackedScene itemTooltipPopupScene = GD.Load<PackedScene>("res://hud_item_tooltip_popup.tscn");
	
	public Player PlayerOwner;
	private Inventory playerInventory;

	private ItemTooltipPopup activeTooltipPopup;

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

				PlayerOwner.SetDestinationPosition(SubViewportToViewport(mbe.GlobalPosition));
			}
		}
	}

    public override void _UnhandledInput(InputEvent @event) {
		
    }

    public void CreateItemTooltip(Control tooltipContent, Vector2 anchor) {
		/* 
		=========================================================
		Skriv dette om, så den tager et origin + en størrelse og regner ankeret derfra
		Det er for at gøre det muligt sende nogle koordinater til selve vinduet, hvorfra den selv kan undgå at placere sig selv oven på items,
		men i stedet forsøge at være ved siden af, da det både ser træls ud, men også giver nogle problemer med inputprioritering
		=========================================================
		*/
		ItemTooltipPopup tooltipPopup = itemTooltipPopupScene.Instantiate<ItemTooltipPopup>();
		AddChild(tooltipPopup);
		activeTooltipPopup = tooltipPopup;

		tooltipPopup.AddChild(tooltipContent);
		tooltipPopup.Position = (Vector2I)SubViewportToViewport(anchor);
    }

	public void RemoveItemTooltip() {
		if (activeTooltipPopup != null) {
			activeTooltipPopup.QueueFree();
			activeTooltipPopup = null;
		}
	}

	public Vector2 SubViewportToViewport(Vector2 globalPosition) {
		Vector2 playerViewportSize = PlayerOwner.GetViewport().GetVisibleRect().Size;
		Vector2 subViewportSize = GetGlobalRect().Size;
		Vector2 translationRatio = playerViewportSize / subViewportSize;

		Vector2 translatedPosition = globalPosition * translationRatio;

		return translatedPosition;
	}
}
