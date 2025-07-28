using Godot;
using System;

public enum EItemRarity {
	None, // Not an actual rarity, but signifies that a rarity should be randomly decided on generation
	Common,
	Magic,
	Rare,
	Unique,
	Skill
}

public enum EItemEquipmentSlot {
	None,
	Head,
	Chest,
	Hands,
	Feet,
	Belt,
	Ring,
	Amulet,
	WeaponLeft,
	WeaponRight,
	SkillActive,
	SkillSupport,
	COUNT
}

public enum EItemAllBaseType {
	None,
	Head,
	Chest,
	Hands,
	Feet,
	Belt,
	Ring,
	Amulet,
	Weapon1H,
	Weapon2H,
	Shield,
	Quiver,
	SkillActive,
	SkillSupport,
	COUNT
}

public enum EItemWeaponBaseType {
	WeaponMelee1H,
	WeaponMelee2H,
	WeaponRanged1H,
	WeaponRanged2H,
	COUNT, // Used for item generation, and there's no need to roll for unarmed weapons
	Unarmed
}

public enum EItemArmourBaseType {
	Helmet,
	Chestplate,
	Gloves,
	Boots,
    Shield,
	COUNT
}

public enum EItemJewelleryBaseType {
	Belt,
	Ring,
	Amulet,
	Quiver,
	COUNT
}

public enum EItemCategory {
	None,
	Armour,
	Weapon,
	Jewellery,
	COUNT
}

[Flags]
public enum EItemBaseSpecifierFlags {
	NONE = 0,
	NoFlags = 1 << 0,
	AArmour = 1 << 1,
	AEvasion = 1 << 2,
	AEnergyShield = 1 << 3,
	AArmourEvasion = 1 << 4,
	AEvasionEnergyShield = 1 << 5,
	AEnergyShieldArmour = 1 << 6,
	AAll = 1 << 7,

	W1HSword = 1 << 8,
	W2HSword = 1 << 9,
	WBow = 1 << 10,
	WStaff = 1 << 11,
}

[Flags]
public enum EItemDefences {
	None = 0,
	Armour = 1,
	Evasion = 2,
	EnergyShield = 4,

	ArmourEvasion = Armour | Evasion,
	EvasionEnergyShield = Evasion | EnergyShield,
	EnergyShieldArmour = EnergyShield | Armour,

	All = Armour | Evasion | EnergyShield
}
