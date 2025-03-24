using Godot;
using System;
using System.Collections.Generic;

public class ItemData {
	public string BaseName;
	public EItemBaseSpecifierFlags ItemSpecifierFlags;
	public Texture2D Texture;
	public int MinimumLevel;
	public List<Type> ImplicitTypes;
}

public class WeaponItemData : ItemData {
	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
	public float BaseAttackSpeed;
	public float BaseCritChance;
}

public class ArmourItemData : ItemData {
	public EItemDefences ItemDefences;
	public int BaseArmour;
	public int BaseEvasion;
	public int BaseEnergyShield;
}

public static class ItemDataTables {
	public static readonly List<WeaponItemData> OHSwordWeaponData = [
		new WeaponItemData {
			BaseName = "Short Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W1HSword,
			Texture = UILib.TextureItemD2ShortSword,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 5,
			BasePhysicalMaximumDamage = 9,
			BaseAttackSpeed = 0.714f,
			BaseCritChance = 5f,
			ImplicitTypes = []
		}
	];

	public static readonly List<WeaponItemData> THSwordWeaponData = [
		new WeaponItemData {
			BaseName = "Long Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W2HSword,
			Texture = UILib.TextureItemD2LongSword,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 7,
			BasePhysicalMaximumDamage = 11,
			BaseAttackSpeed = 0.769f,
			BaseCritChance = 5f,
			ImplicitTypes = []
		}
	];

	public static readonly List<ArmourItemData> HeadArmourData = [
		new ArmourItemData {
			BaseName = "Cap",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion,
			Texture = UILib.TextureItemD2Cap,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Evasion,
			BaseArmour = 0,
			BaseEvasion = 15,
			BaseEnergyShield = 0,
			ImplicitTypes = []
		}
	];

	public static readonly List<ArmourItemData> ChestArmourData = [
		new ArmourItemData {
			BaseName = "Leather Armour",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion,
			Texture = UILib.TextureItemD2LeatherArmour,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Evasion,
			BaseArmour = 0,
			BaseEvasion = 25,
			BaseEnergyShield = 0,
			ImplicitTypes = []
		}
	];

	public static readonly List<ArmourItemData> HandsArmourData = [
		new ArmourItemData {
			BaseName = "Leather Gloves",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion,
			Texture = UILib.TextureItemD2LeatherGloves,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Evasion,
			BaseArmour = 0,
			BaseEvasion = 15,
			BaseEnergyShield = 0,
			ImplicitTypes = []
		}
	];

	public static readonly List<ArmourItemData> FeetArmourData = [
		new ArmourItemData {
			BaseName = "Boots",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion,
			Texture = UILib.TextureItemD2Boots,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Evasion,
			BaseArmour = 0,
			BaseEvasion = 15,
			BaseEnergyShield = 0,
			ImplicitTypes = []
		}
	];

	public static readonly List<ItemData> AmuletJewelleryData = [
		new ItemData {
			BaseName = "Amulet",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2Amulet0,
			MinimumLevel = 0,
			ImplicitTypes = [
				typeof(BasicAmuletPhysImplicit)
			]
		}
	];

	public static readonly List<ItemData> RingJewelleryData = [
		new ItemData {
			BaseName = "Ring",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2Ring0,
			MinimumLevel = 0,
			ImplicitTypes = [
				typeof(BasicRingLifeImplicit)
			]
		}
	];

	public static readonly List<ItemData> BeltJewelleryData = [
		new ItemData {
			BaseName = "Sash",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2Sash,
			MinimumLevel = 0,
			ImplicitTypes = []
		}
	];

	public static readonly List<ArmourItemData> SmallShieldArmourData = [
		new ArmourItemData {
			BaseName = "Small Shield",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AEvasion,
			Texture = UILib.TextureItemD2SmallShield,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Evasion,
			BaseArmour = 0,
			BaseEvasion = 20,
			BaseEnergyShield = 0,
			ImplicitTypes = []
		}
	];
}
