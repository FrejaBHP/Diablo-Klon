using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class InventoryGridSquare : TextureRect {
	public bool IsEmpty = true;

	private Item referencedItem = null;
	private Vector2I position = Vector2I.Zero;

	//private Label label;

	public InventoryGridSquare(Vector2I pos) {
		position = pos;
	}

    public override void _Ready() {
		MouseFilter = MouseFilterEnum.Ignore;
		Texture = UITextureLib.InventoryGridSlot;

		//label = new Label();
		//label.Text = $"{position.X}, {position.Y}";
		//AddChild(label);
    }
}
