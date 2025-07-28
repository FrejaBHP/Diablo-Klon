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
	public double BaseBlockChance;
}

public class SkillItemData : ItemData {
	public ESkillName SkillName;
	public Type SkillType;
	public Skill SkillReference;

	public SkillItemData() {
		BaseName = "Skill Gem";
		ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE;
		ImplicitTypes = [];
	}
}

public class SupportGemData : ItemData {
	public ESkillTags SkillTags;
	public Type SupportType;

	public SupportGemData() {
		BaseName = "Support Gem";
		ItemSpecifierFlags = EItemBaseSpecifierFlags.NONE;
		ImplicitTypes = [];
	}
}

public static class ItemDataTables {
	public static readonly List<WeaponItemData> OHSwordWeaponData = [
		new WeaponItemData {
			BaseName = "Short Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W1HSword,
			Texture = UILib.TextureItemD2ShortSword,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 5,
			BasePhysicalMaximumDamage = 8,
			BaseAttackSpeed = 0.714f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		},
		
		new WeaponItemData {
			BaseName = "Scimitar",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W1HSword,
			Texture = UILib.TextureItemD2Scimitar,
			MinimumLevel = 5,
			BasePhysicalMinimumDamage = 9,
			BasePhysicalMaximumDamage = 12,
			BaseAttackSpeed = 0.741f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		},

		new WeaponItemData {
			BaseName = "Long Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W1HSword,
			Texture = UILib.TextureItemD2LongSword,
			MinimumLevel = 11,
			BasePhysicalMinimumDamage = 13,
			BasePhysicalMaximumDamage = 17,
			BaseAttackSpeed = 0.741f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		},
	];

	public static readonly List<WeaponItemData> THSwordWeaponData = [
		new WeaponItemData {
			BaseName = "Two-Handed Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W2HSword,
			Texture = UILib.TextureItemD2TwoHandedSword,
			MinimumLevel = 0,
			BasePhysicalMinimumDamage = 7,
			BasePhysicalMaximumDamage = 11,
			BaseAttackSpeed = 0.769f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		},

		new WeaponItemData {
			BaseName = "Giant Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W2HSword,
			Texture = UILib.TextureItemD2GiantSword,
			MinimumLevel = 5,
			BasePhysicalMinimumDamage = 13,
			BasePhysicalMaximumDamage = 16,
			BaseAttackSpeed = 0.800f,
			BaseCritChance = 0.05f,
			ImplicitTypes = []
		},

		new WeaponItemData {
			BaseName = "Great Sword",
			ItemSpecifierFlags = EItemBaseSpecifierFlags.W2HSword,
			Texture = UILib.TextureItemD2GreatSword,
			MinimumLevel = 11,
			BasePhysicalMinimumDamage = 20,
			BasePhysicalMaximumDamage = 24,
			BaseAttackSpeed = 0.800f,
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
			ImplicitTypes = [
				typeof(BasicStaffBlockImplicit)
			]
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0,
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
			BaseBlockChance = 0.25,
			ImplicitTypes = []
		}
	];

	public static readonly List<SkillItemData> SkillData = [
		new SkillItemData {
			SkillName = ESkillName.BasicThrust,
			SkillType = typeof(SThrust),
			Texture = UILib.TextureItemD2JewelRed,
			MinimumLevel = 0,
		},

		new SkillItemData {
			SkillName = ESkillName.PiercingShot,
			SkillType = typeof(SShoot),
			Texture = UILib.TextureItemD2JewelGreen,
			MinimumLevel = 0,
		},

		new SkillItemData {
			SkillName = ESkillName.SplitArrow,
			SkillType = typeof(SSplitArrow),
			Texture = UILib.TextureItemD2JewelGreen,
			MinimumLevel = 0,
		},

		new SkillItemData {
			SkillName = ESkillName.PrismaticBolt,
			SkillType = typeof(SPrismaticBolt),
			Texture = UILib.TextureItemD2JewelBlue,
			MinimumLevel = 0,
		},

		new SkillItemData {
			SkillName = ESkillName.FireNova,
			SkillType = typeof(SFireNova),
			Texture = UILib.TextureItemD2JewelBlue,
			MinimumLevel = 0,
		}
	];

	public static readonly List<SupportGemData> SupportGemData = [
		new SupportGemData {
			SkillTags = ESkillTags.None,
			SupportType = typeof(SuppAddedFire),
			Texture = UILib.TextureItemD2JewelRedF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.None,
			SupportType = typeof(SuppAddedCold),
			Texture = UILib.TextureItemD2JewelGreenF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.None,
			SupportType = typeof(SuppAddedLightning),
			Texture = UILib.TextureItemD2JewelBlueF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.None,
			SupportType = typeof(SuppAddedChaos),
			Texture = UILib.TextureItemD2JewelWhiteF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Attack,
			SupportType = typeof(SuppAttackSpeed),
			Texture = UILib.TextureItemD2JewelGreenF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Spell,
			SupportType = typeof(SuppCastSpeed),
			Texture = UILib.TextureItemD2JewelBlueF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Projectile,
			SupportType = typeof(SuppPierce),
			Texture = UILib.TextureItemD2JewelGreenF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Duration,
			SupportType = typeof(SuppIncreasedDuration),
			Texture = UILib.TextureItemD2JewelWhiteF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Duration,
			SupportType = typeof(SuppLessDuration),
			Texture = UILib.TextureItemD2JewelWhiteF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Projectile,
			SupportType = typeof(SuppMultipleProjectiles),
			Texture = UILib.TextureItemD2JewelGreenF,
			MinimumLevel = 0,
		},

		new SupportGemData {
			SkillTags = ESkillTags.Area,
			SupportType = typeof(SuppIncreasedAoE),
			Texture = UILib.TextureItemD2JewelBlueF,
			MinimumLevel = 0,
		},
	];
}
