using Godot;
using System;
using System.Collections.Generic;

public static class UILib {
	public static readonly Texture2D InventoryGridSlot = GD.Load<Texture2D>("res://textures/ui/inventoryGridSlot.png");

	public static readonly Texture2D TextureItemD2Cap = GD.Load<Texture2D>("res://textures/items/d2_Cap.png");
	public static readonly Texture2D TextureItemD2SkullCap = GD.Load<Texture2D>("res://textures/items/d2_SkullCap.png");

	public static readonly Texture2D TextureItemD2LeatherArmour = GD.Load<Texture2D>("res://textures/items/d2_LeatherArmour.png");
	public static readonly Texture2D TextureItemD2PlateMail = GD.Load<Texture2D>("res://textures/items/d2_PlateMail.png");

	public static readonly Texture2D TextureItemD2LeatherGloves = GD.Load<Texture2D>("res://textures/items/d2_LeatherGloves.png");
	public static readonly Texture2D TextureItemD2ChainGloves = GD.Load<Texture2D>("res://textures/items/d2_ChainGloves.png");

	public static readonly Texture2D TextureItemD2Boots = GD.Load<Texture2D>("res://textures/items/d2_Boots.png");
	public static readonly Texture2D TextureItemD2LightPlatedBoots = GD.Load<Texture2D>("res://textures/items/d2_LightPlatedBoots.png");

	public static readonly Texture2D TextureItemD2SmallShield = GD.Load<Texture2D>("res://textures/items/d2_SmallShield.png");

	public static readonly Texture2D TextureItemD2Sash = GD.Load<Texture2D>("res://textures/items/d2_Sash.png");

	public static readonly Texture2D TextureItemD2Ring0 = GD.Load<Texture2D>("res://textures/items/d2_Ring0.png");

	public static readonly Texture2D TextureItemD2Amulet0 = GD.Load<Texture2D>("res://textures/items/d2_Amulet0.png");

	public static readonly Texture2D TextureItemD2Quiver = GD.Load<Texture2D>("res://textures/items/d2_ArrowQuiver.png");

	public static readonly Texture2D TextureItemD2ShortSword = GD.Load<Texture2D>("res://textures/items/d2_ShortSword.png");
	public static readonly Texture2D TextureItemD2LongSword = GD.Load<Texture2D>("res://textures/items/d2_LongSword.png");

	public static readonly Texture2D TextureItemD2LongStaff = GD.Load<Texture2D>("res://textures/items/d2_LongStaff.png");

	public static readonly Texture2D TextureItemD2ShortBow = GD.Load<Texture2D>("res://textures/items/d2_ShortBow.png");

	public static readonly Texture2D TextureItemD2JewelRed = GD.Load<Texture2D>("res://textures/items/d2_Jewel_red_flip.png");
	public static readonly Texture2D TextureItemD2JewelGreen = GD.Load<Texture2D>("res://textures/items/d2_Jewel_green_flip.png");
	public static readonly Texture2D TextureItemD2JewelBlue = GD.Load<Texture2D>("res://textures/items/d2_Jewel_blue.png");
	public static readonly Texture2D TextureItemD2JewelWhite = GD.Load<Texture2D>("res://textures/items/d2_Jewel_white_flip.png");

	public static readonly Texture2D TextureRemoveSkill = GD.Load<Texture2D>("res://textures/skills/remove_skill.png");
	public static readonly Texture2D TextureSkillNONE = GD.Load<Texture2D>("res://textures/skills/skill_none.png");
	public static readonly Texture2D TextureSkillThrust = GD.Load<Texture2D>("res://textures/skills/skill_thrust.png");
	public static readonly Texture2D TextureSkillShoot = GD.Load<Texture2D>("res://textures/skills/skill_shoot.png");
	public static readonly Texture2D TextureSkillPrismaticBolt = GD.Load<Texture2D>("res://textures/skills/skill_prismatic.png");

	public static readonly Texture2D GoldPileTiny = GD.Load<Texture2D>("res://textures/goldDrop0.png");
	public static readonly Texture2D GoldPileSmall = GD.Load<Texture2D>("res://textures/goldDrop1.png");
	public static readonly Texture2D GoldPileMedium = GD.Load<Texture2D>("res://textures/goldDrop2.png");
	public static readonly Texture2D GoldPileLarge = GD.Load<Texture2D>("res://textures/goldDrop3.png");

	public static readonly Color ColorWhite = Color.Color8(255, 255, 255, 255);
	public static readonly Color ColorGrey = Color.Color8(150, 150, 150, 255);
	public static readonly Color ColorBlurple = Color.Color8(100, 100, 190, 255);
	public static readonly Color ColorSkill = Color.Color8(27, 162, 155, 255);
	public static readonly Color ColorEquipmentSlotBackground = Color.Color8(55, 55, 55, 255);
	public static readonly Color ColorEquipmentSlotBorder = Color.Color8(35, 35, 35, 255);
	public static readonly Color ColorGreenBorderHighlight = Color.Color8(40, 240, 70, 255);
	public static readonly Color ColorTransparent = Color.Color8(255, 255, 255, 0);

	public static readonly Color ColorMagic = Color.Color8(25, 135, 255, 255);
	public static readonly Color ColorRare = Color.Color8(245, 245, 0, 255);
	public static readonly Color ColorUnique = Color.Color8(255, 175, 0, 255);

	public static readonly Color ColorFire = Color.Color8(185, 115, 35, 255);
	public static readonly Color ColorCold = Color.Color8(65, 110, 180, 255);
	public static readonly Color ColorLightning = Color.Color8(175, 170, 70, 255);
	public static readonly Color ColorChaos = Color.Color8(210, 30, 145, 255);

	public static readonly Color ColorItemBackground = Color.Color8(0, 65, 180, 55);
	public static readonly Color ColorItemBackgroundHovered = Color.Color8(0, 100, 255, 65);
}
