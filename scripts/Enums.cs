using Godot;
using System;

public enum EStatName {
	None,
	FlatMaxLife,
	PercentageMaxLife,
	FlatMinPhysDamage,
	FlatMaxPhysDamage,
	FlatPhysDamage,
	PercentagePhysDamage,
	FlatArmour,
	PercentageArmour,
	FlatEvasion,
	PercentageEvasion,
	FlatEnergyShield,
	PercentageEnergyShield,
	FireResistance,
	ColdResistance,
	LightningResistance,
}


// ======== AFFIXES ========
[Flags]
public enum EAffixItemFlags {
	None = 0,
	Helmet = 1 << 0,
	Chest = 1 << 1,
	Gloves = 1 << 2,
	Boots = 1 << 3,
	Belt = 1 << 4,
	Ring = 1 << 5,
	Amulet = 1 << 6,
	OHWeapon = 1 << 7,
	THWeapon = 1 << 8,
	Shield = 1 << 9,

	Armour = 1 << 10,
	Jewellery = 1 << 11,
	Weapon = 1 << 12,

	All = (1 << 13) - 1 // Unused
}

public enum EAffixPosition {
	Prefix,
	Suffix
}

public enum EAffixFamily {
	None,
	FlatMaxLife,
	PercentageMaxLife,
	FlatPhysDamage,
	PercentagePhysDamage,
	PercentageAttackSpeed,
	PercentageCritChance,
	FlatArmour,
	PercentageArmour,
	FlatEvasion,
	PercentageEvasion,
	FlatEnergyShield,
	PercentageEnergyShield,
	FireResistance,
	ColdResistance,
	LightningResistance,

	LocalFlatPhysDamage,
	LocalPercentagePhysDamage,
	LocalPercentageAttackSpeed,
	LocalPercentageCritChance,
	LocalFlatArmour,
	LocalPercentageArmour,
	LocalFlatEvasion,
	LocalPercentageEvasion,
	LocalFlatEnergyShield,
	LocalPercentageEnergyShield,
}


// ======== ITEMS ========
public enum EItemRarity {
	Common,
	Magic,
	Rare,
	Unique,
	What
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
	COUNT
}

public enum EItemArmourBaseType {
	Helmet,
	Chestplate,
	Gloves,
	Boots,
    Shield,
	COUNT
}

public enum EItemWeaponBaseType {
	Weapon1H,
	Weapon2H,
	COUNT
}

public enum EItemJewelleryBaseType {
	Belt,
	Ring,
	Amulet,
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
	W2HSword = 1 << 9
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
