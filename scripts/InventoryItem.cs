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
	public Item ItemReference;

	protected List<InventoryGridSquare> occupiedInventorySlots = new List<InventoryGridSquare>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemTexture = GetNode<TextureRect>("ItemTexture");
		itemTexture.Texture = ItemReference.ItemTexture;
	}

	public void OnClicked(InputEvent @event) {
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (!IsClicked) {
				InventoryReference.ItemClickSelect(this);
			}
		}
	}

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

	public void SetOccupiedSlots(List<InventoryGridSquare> list) {
		occupiedInventorySlots = list;

		foreach (InventoryGridSquare slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	public void ClearOccupiedSlots() {
		foreach (InventoryGridSquare slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }

		occupiedInventorySlots.Clear();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (IsClicked) {
			float newX = GetGlobalMousePosition().X - Size.X / 2;
			float newY = GetGlobalMousePosition().Y - Size.Y / 2;
			Vector2 newPosition = new Vector2(newX, newY);
			GlobalPosition = newPosition;
		}
	}
}
