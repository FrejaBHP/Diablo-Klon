using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class InventoryGridCell : TextureRect {
	public bool IsEmpty = true;

	//private Item referencedItem = null;
	public Vector2I GridPosition = Vector2I.Zero;

	public InventoryGridCell(Vector2I pos) {
		GridPosition = pos;
	}

    public override void _Ready() {
		MouseFilter = MouseFilterEnum.Ignore;
		Texture = UILib.InventoryGridSlot;
    }
}
