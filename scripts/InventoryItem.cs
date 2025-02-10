using Godot;
using System;

public partial class InventoryItem : PanelContainer {
	protected int gridSizeX;
	protected int gridSizeY;

	protected TextureRect itemTexture;
	protected Item itemReference;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemTexture = GetNode<TextureRect>("ItemTexture");

		itemTexture.Texture = itemReference.ItemTexture;
	}

	public void SetItemReference(Item item) {
		itemReference = item;
	}

	public void SetGridSize(int x, int y) {
		gridSizeX = x;
		gridSizeY = y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}
}
