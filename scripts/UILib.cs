using Godot;
using System;

public static class UILib {
	public static readonly Texture2D InventoryGridSlot = GD.Load<Texture2D>("res://textures/ui/inventoryGridSlot.png");
	public static readonly Texture2D TestItem = GD.Load<Texture2D>("res://textures/ui/test.png");
	public static readonly Texture2D TestItem2 = GD.Load<Texture2D>("res://textures/ui/test2.png");
	public static readonly Texture2D TestItem3 = GD.Load<Texture2D>("res://textures/ui/test3.png");

	public static readonly Texture2D TextureItemD2LeatherArmour = GD.Load<Texture2D>("res://textures/items/d2_LeatherArmour.png");
	public static readonly Texture2D TextureItemD2LeatherGloves = GD.Load<Texture2D>("res://textures/items/d2_LeatherGloves.png");
	public static readonly Texture2D TextureItemD2Ring0 = GD.Load<Texture2D>("res://textures/items/d2_Ring0.png");

	public static readonly Color ColorWhite = Color.Color8(255, 255, 255, 255);
	public static readonly Color ColorEquipmentSlotBackground = Color.Color8(55, 55, 55, 255);
	public static readonly Color ColorEquipmentSlotBorder = Color.Color8(35, 35, 35, 255);
	public static readonly Color ColorGreenBorderHighlight = Color.Color8(40, 240, 70, 255);
	public static readonly Color ColorTransparent = Color.Color8(255, 255, 255, 0);

	public static readonly Color ColorMagic = Color.Color8(25, 135, 255, 255);
	public static readonly Color ColorRare = Color.Color8(245, 245, 0, 255);
	public static readonly Color ColorUnique = Color.Color8(255, 175, 0, 255);

	public static readonly Color ColorItemBackground = Color.Color8(0, 70, 190, 55);
	public static readonly Color ColorItemBackgroundHovered = Color.Color8(0, 100, 255, 55);
}
