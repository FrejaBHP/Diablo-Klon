using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class InventoryGridSquare : TextureRect {
	private Item referencedItem = null;
	private bool isEmpty = true;
	private Vector2 position = Vector2.Zero;

	public InventoryGridSquare(Vector2 pos) {
		position = pos;
	}

    public override void _Ready() {
		Texture = UITextureLib.InventoryGridSlot;
    }
}
