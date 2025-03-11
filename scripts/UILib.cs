using Godot;
using System;
using System.Collections.Generic;

public static class UILib {
	public static readonly Texture2D InventoryGridSlot = GD.Load<Texture2D>("res://textures/ui/inventoryGridSlot.png");

	public static readonly Texture2D TextureItemD2Cap = GD.Load<Texture2D>("res://textures/items/d2_Cap.png");
	public static readonly Texture2D TextureItemD2LeatherArmour = GD.Load<Texture2D>("res://textures/items/d2_LeatherArmour.png");
	public static readonly Texture2D TextureItemD2LeatherGloves = GD.Load<Texture2D>("res://textures/items/d2_LeatherGloves.png");
	public static readonly Texture2D TextureItemD2Boots = GD.Load<Texture2D>("res://textures/items/d2_Boots.png");
	public static readonly Texture2D TextureItemD2Sash = GD.Load<Texture2D>("res://textures/items/d2_Sash.png");
	public static readonly Texture2D TextureItemD2Ring0 = GD.Load<Texture2D>("res://textures/items/d2_Ring0.png");
	public static readonly Texture2D TextureItemD2Amulet0 = GD.Load<Texture2D>("res://textures/items/d2_Amulet0.png");
	public static readonly Texture2D TextureItemD2ShortSword = GD.Load<Texture2D>("res://textures/items/d2_ShortSword.png");
	public static readonly Texture2D TextureItemD2LongSword = GD.Load<Texture2D>("res://textures/items/d2_LongSword.png");

	public static readonly Color ColorWhite = Color.Color8(255, 255, 255, 255);
	public static readonly Color ColorGrey = Color.Color8(150, 150, 150, 255);
	public static readonly Color ColorBlurple = Color.Color8(100, 100, 190, 255);
	public static readonly Color ColorEquipmentSlotBackground = Color.Color8(55, 55, 55, 255);
	public static readonly Color ColorEquipmentSlotBorder = Color.Color8(35, 35, 35, 255);
	public static readonly Color ColorGreenBorderHighlight = Color.Color8(40, 240, 70, 255);
	public static readonly Color ColorTransparent = Color.Color8(255, 255, 255, 0);

	public static readonly Color ColorMagic = Color.Color8(25, 135, 255, 255);
	public static readonly Color ColorRare = Color.Color8(245, 245, 0, 255);
	public static readonly Color ColorUnique = Color.Color8(255, 175, 0, 255);

	public static readonly Color ColorItemBackground = Color.Color8(0, 65, 180, 55);
	public static readonly Color ColorItemBackgroundHovered = Color.Color8(0, 100, 255, 65);

	static UILib() {

	}
}
