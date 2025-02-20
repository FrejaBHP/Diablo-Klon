using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class InventoryItem : PanelContainer {
	public Inventory InventoryReference;
	public bool IsClicked = false;

	protected int gridSizeX;
	protected int gridSizeY;

	protected TextureRect itemTexture;
	protected ColorRect itemBackground;
	public Item ItemReference;

	protected const int margin = 6; // Don't touch

	protected List<InventoryGridCell> occupiedInventorySlots = new List<InventoryGridCell>();

	private bool isHovered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemTexture = GetNode<TextureRect>("ItemTexture");
		itemTexture.Texture = ItemReference.ItemTexture;

		itemBackground = GetNode<ColorRect>("ItemBackground");
		itemBackground.Color = UILib.ColorItemBackground;

		ApplyBorder();

		// Each grid slot is 32x32 units
		Vector2 newMinSize = GetGridSize() * 32;
		newMinSize.X -= margin;
		newMinSize.Y -= margin;
		itemTexture.CustomMinimumSize = newMinSize;
	}

	public void OnClicked(InputEvent @event) {
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (!IsClicked) {
				InventoryReference.ItemClickSelect(this);
			}
		}
	}

	public void OnMouseEntered() {
		if (!InventoryReference.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackgroundHovered;
			isHovered = true;
		}
	}

	public void OnMouseExited() {
		if (!InventoryReference.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackground;
			isHovered = false;
		}
	}

	public void ToggleBackground() {
		if (IsClicked) {
			itemBackground.Color = UILib.ColorTransparent;
		}
		else {
			itemBackground.Color = UILib.ColorItemBackground;
		}
	}

	// For adjusting click functionality
	// A physics frame delay is inserted to prevent things from overlapping in unintended ways
	public async void ToggleClickable() {
		if (MouseFilter == MouseFilterEnum.Stop) {
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			MouseFilter = MouseFilterEnum.Ignore;
		}
		else if (MouseFilter == MouseFilterEnum.Ignore) {
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			MouseFilter = MouseFilterEnum.Stop;
		}
	}

	public void SetItemReference(Item item) {
		ItemReference = item;
	}

	public Vector2I GetGridSize() {
		return new Vector2I(gridSizeX, gridSizeY);
	}

	public void SetGridSize(int x, int y) {
		gridSizeX = x;
		gridSizeY = y;
	}

	// Sets the list of slots that this item will occupy
	public void SetOccupiedSlots(List<InventoryGridCell> list) {
		//GD.Print("Slots set");
		occupiedInventorySlots = list.GetRange(0, list.Count);

		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Sets the list of occupied slots as empty without taking a new list. Used for when moving items around
	public void OpenOccupiedSlots() {
		//GD.Print("Slots opened");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }
	}

	// Sets the list of occupied slots as used without clearing the list. Used for when moving items around
	public void CloseOccupiedSlots() {
		//GD.Print("Slots closed");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Empties the list of occupied slots
	public void ClearOccupiedSlots() {
		//GD.Print("Slots cleared");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }

		occupiedInventorySlots.Clear();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (IsClicked) {
			float newX = GetGlobalMousePosition().X - (Size.X / 2) * Scale.X;
			float newY = GetGlobalMousePosition().Y - (Size.Y / 2) * Scale.Y;
			Vector2 newPosition = new Vector2(newX, newY);
			GlobalPosition = newPosition;
		}
	}

	public void ApplyBorder() {
		StyleBoxFlat styleBoxFlat = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
		const int marginFactor = margin / 2;
		styleBoxFlat.BorderWidthLeft = marginFactor;
		styleBoxFlat.BorderWidthTop = marginFactor;
		styleBoxFlat.BorderWidthRight = marginFactor;
		styleBoxFlat.BorderWidthBottom = marginFactor;

		switch (ItemReference.ItemRarity) {
			case EItemRarity.Magic:
				styleBoxFlat.BorderColor = UILib.ColorMagic;
				break;

			case EItemRarity.Rare:
				styleBoxFlat.BorderColor = UILib.ColorRare;
				break;

			case EItemRarity.Unique:
				styleBoxFlat.BorderColor = UILib.ColorUnique;
				break;
			
			default:
				styleBoxFlat.BorderColor = UILib.ColorTransparent;
				break;
		}

		AddThemeStyleboxOverride("panel", styleBoxFlat);
	}
}
