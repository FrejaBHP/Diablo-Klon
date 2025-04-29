using Godot;
using System;

public enum EMovementInputMethod {
	Mouse,
	Keyboard
}

public enum EStatName {
	None,

	FlatStrength,
	FlatDexterity,
	FlatIntelligence,

	FlatMaxLife,
	IncreasedMaxLife,
	AddedLifeRegen,
	PercentageLifeRegen,
	IncreasedLifeRegen,

	FlatMaxMana,
	IncreasedMaxMana,
	AddedManaRegen,
	IncreasedManaRegen,

	FlatMinPhysDamage,
	FlatMaxPhysDamage,
	FlatPhysDamage,
	IncreasedPhysDamage,

	IncreasedAttackSpeed,
	IncreasedCritChance,
	AddedCritMulti,

	IncreasedMovementSpeed,

	FlatArmour,
	IncreasedArmour,
	FlatEvasion,
	IncreasedEvasion,
	FlatEnergyShield,
	IncreasedEnergyShield,

	PhysicalResistance,
	FireResistance,
	ColdResistance,
	LightningResistance,
	ChaosResistance,
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
	
	// Global
	AddedStrength,
	AddedDexterity,
	AddedIntelligence,

	FlatMaxLife,
	IncreasedMaxLife,
	AddedLifeRegen,
	PercentageLifeRegen,
	IncreasedLifeRegen,

	FlatMaxMana,
	IncreasedMaxMana,
	AddedManaRegen,
	IncreasedManaRegen,

	FlatPhysDamage,
	IncreasedPhysDamage,

	IncreasedAttackSpeed,
	IncreasedCritChance,
	AddedCritMulti,

	IncreasedMovementSpeed,

	FlatArmour,
	IncreasedArmour,
	FlatEvasion,
	IncreasedEvasion,
	FlatEnergyShield,
	IncreasedEnergyShield,

	PhysicalResistance,
	FireResistance,
	ColdResistance,
	LightningResistance,
	ChaosResistance,

	// Local
	LocalFlatPhysDamage,
	LocalIncreasedPhysDamage,

	LocalIncreasedAttackSpeed,
	LocalIncreasedCritChance,

	LocalFlatArmour,
	LocalIncreasedArmour,
	LocalFlatEvasion,
	LocalIncreasedEvasion,
	LocalFlatEnergyShield,
	LocalIncreasedEnergyShield,
}

// ======== LABELS =======

public enum ELabelColourSet {
	Default,
	Item
}

public enum ETextColour {
	Default,
	Common,
	Magic,
	Rare,
	Unique,
	Skill
}

// ======== ITEMS ========
public enum EItemRarity {
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
	SkillActive,
	SkillSupport,
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



public enum EDamageCategory {
	Melee,
	Ranged,
	Spell
}

public enum ESkillName {
	BasicThrust,
	COUNT
}

public enum ESkillType {
    Attack,
    Spell
}

[Flags]
public enum ESkillTags {
    None = 0,
    Melee = 1 << 0,
    Ranged = 1 << 1,
    Spell = 1 << 2,

	Projectile = 1 << 3,
    Area = 1 << 4,

	Physical = 1 << 5,
	Fire = 1 << 6,
	Cold = 1 << 7,
	Lightning = 1 << 8,
	Chaos = 1 << 9
}

[Flags]
public enum ESkillWeapons {
    None = 0,
    Sword = 1 << 0,
    Axe = 1 << 1,
    Mace = 1 << 2,
    Dagger = 1 << 3,
    Sword2H = 1 << 4,
    Axe2H = 1 << 5,
    Mace2H = 1 << 6,
    Staff = 1 << 7,
    Bow = 1 << 8,
    Wand = 1 << 9,

    AllMeleeOneHand = Sword | Axe | Mace | Dagger,
    AllMeleeTwoHand = Sword2H | Axe2H | Mace2H | Staff,
    AllMeleeWeapons = Sword | Axe | Mace | Dagger | Sword2H | Axe2H | Mace2H | Staff,
    AllRangedWeapons = Bow | Wand
}


public enum EActorState {
	Actionable,
	Attacking,
	Stunned
}
