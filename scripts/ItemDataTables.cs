using Godot;
using System;
using System.Collections.Generic;

public class ItemData {
	public string BaseName;
	public EItemBaseSpecifierFlags ItemSpecifierFlags;
	public Texture2D Texture;
	public int MinimumLevel;
}

public class ArmourItemData : ItemData {
	public EItemDefences ItemDefences;
	public int BaseArmour;
	public int BaseEvasion;
	public int BaseEnergyShield;
}

public class WeaponItemData : ItemData {
	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
}

public static class ItemDataTables {
	public static readonly List<WeaponItemData> OHSwordWeaponData = new List<WeaponItemData> {
		new WeaponItemData{ BaseName = "Short Sword", ItemSpecifierFlags = EItemBaseSpecifierFlags.W1HSword, Texture = UILib.TextureItemD2ShortSword, MinimumLevel = 0, BasePhysicalMinimumDamage = 5, BasePhysicalMaximumDamage = 9 } 
	};

	public static readonly List<WeaponItemData> THSwordWeaponData = new List<WeaponItemData> {
		new WeaponItemData{ BaseName = "Long Sword", ItemSpecifierFlags = EItemBaseSpecifierFlags.W2HSword, Texture = UILib.TextureItemD2LongSword, MinimumLevel = 0, BasePhysicalMinimumDamage = 7, BasePhysicalMaximumDamage = 11 } 
	};

	public static readonly List<ArmourItemData> HeadArmourData = new List<ArmourItemData> {
		new ArmourItemData{ BaseName = "Cap", ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion, Texture = UILib.TextureItemD2Cap, MinimumLevel = 0, ItemDefences = EItemDefences.Evasion, BaseArmour = 0, BaseEvasion = 15, BaseEnergyShield = 0 } 
	};

	public static readonly List<ArmourItemData> ChestArmourData = new List<ArmourItemData> {
		new ArmourItemData{ BaseName = "Leather Armour", ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion, Texture = UILib.TextureItemD2LeatherArmour, MinimumLevel = 0, ItemDefences = EItemDefences.Evasion, BaseArmour = 0, BaseEvasion = 25, BaseEnergyShield = 0 } 
	};

	public static readonly List<ArmourItemData> HandsArmourData = new List<ArmourItemData> {
		new ArmourItemData{ BaseName = "Leather Gloves", ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion, Texture = UILib.TextureItemD2LeatherGloves, MinimumLevel = 0, ItemDefences = EItemDefences.Evasion, BaseArmour = 0, BaseEvasion = 15, BaseEnergyShield = 0 } 
	};

	public static readonly List<ArmourItemData> FeetArmourData = new List<ArmourItemData> {
		new ArmourItemData{ BaseName = "Boots", ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion, Texture = UILib.TextureItemD2Boots, MinimumLevel = 0, ItemDefences = EItemDefences.Evasion, BaseArmour = 0, BaseEvasion = 15, BaseEnergyShield = 0 } 
	};
}
