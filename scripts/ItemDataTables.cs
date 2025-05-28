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

public class SkillItemData : ItemData {
	public ESkillName SkillName;
	public Type SkillType;
	public Skill SkillReference;
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
			BaseCritChance = 0.05f,
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
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		}
	];

	public static readonly List<WeaponItemData> BowWeaponData = [
		new WeaponItemData {
			BaseName = "Short Bow",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.WBow,
			Texture = UILib.TextureItemD2ShortBow,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 5,
			BasePhysicalMaximumDamage = 10,
			BaseAttackSpeed = 0.667f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		}
	];

	public static readonly List<WeaponItemData> StaffWeaponData = [
		new WeaponItemData {
			BaseName = "Long Staff",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.WStaff,
			Texture = UILib.TextureItemD2LongStaff,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 6,
			BasePhysicalMaximumDamage = 11,
			BaseAttackSpeed = 0.769f,
			BaseCritChance = 0.07f,
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
		},

		new ArmourItemData {
			BaseName = "Skull Cap",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AArmour,
			Texture = UILib.TextureItemD2SkullCap,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Armour,
			BaseArmour = 15,
			BaseEvasion = 0,
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
		},

		new ArmourItemData {
			BaseName = "Plate Armour",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AArmour,
			Texture = UILib.TextureItemD2PlateMail,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Armour,
			BaseArmour = 25,
			BaseEvasion = 0,
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
		},

		new ArmourItemData {
			BaseName = "Chain Gloves",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AArmour,
			Texture = UILib.TextureItemD2ChainGloves,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Armour,
			BaseArmour = 15,
			BaseEvasion = 0,
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
		},

		new ArmourItemData {
			BaseName = "Plated Boots",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.AArmour,
			Texture = UILib.TextureItemD2LightPlatedBoots,
			MinimumLevel = 0,
			ItemDefences = EItemDefences.Armour,
			BaseArmour = 15,
			BaseEvasion = 0,
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

	public static readonly List<ItemData> QuiverJewelleryData = [
		new ItemData {
			BaseName = "Light Quiver",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2Quiver,
			MinimumLevel = 0,
			ImplicitTypes = [
				typeof(BasicAmuletPhysImplicit)
			]
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

	public static readonly List<SkillItemData> SkillData = [
		new SkillItemData {
			BaseName = "Skill Gem",
			SkillName = ESkillName.BasicThrust,
			SkillType = typeof(SThrust),
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2JewelWhite,
			MinimumLevel = 0,
			ImplicitTypes = []
		},

		new SkillItemData {
			BaseName = "Skill Gem",
			SkillName = ESkillName.BasicShoot,
			SkillType = typeof(SShoot),
			ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE,
			Texture = UILib.TextureItemD2JewelGreen,
			MinimumLevel = 0,
			ImplicitTypes = []
		}
	];
}
