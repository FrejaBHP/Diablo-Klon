using Godot;
using System;

public enum EMovementInputMethod {
	Mouse,
	Keyboard
}


public enum EMapType {
	Town,
	Intermission,
	Objective,
	Miniboss,
	Boss
}

public enum EMapObjective {
	None,
	Survival
}

public enum EEnemyType {
	TestEnemy,
	TestEnemy2,
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

	FlatMinFireDamage,
	FlatMaxFireDamage,
	FlatFireDamage,
	IncreasedFireDamage,

	FlatMinColdDamage,
	FlatMaxColdDamage,
	FlatColdDamage,
	IncreasedColdDamage,

	FlatMinLightningDamage,
	FlatMaxLightningDamage,
	FlatLightningDamage,
	IncreasedLightningDamage,

	FlatMinChaosDamage,
	FlatMaxChaosDamage,
	FlatChaosDamage,
	IncreasedChaosDamage,

	IncreasedMeleeDamage,
	IncreasedRangedDamage,
	IncreasedSpellDamage,

	IncreasedAttackSpeed,
	IncreasedCastSpeed,
	IncreasedCritChance,
	AddedCritMulti,

	IncreasedMovementSpeed,
	BlockChance,
	BlockEffectiveness,

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
	None = 		0,
	Helmet = 	1 << 0,
	Chest = 	1 << 1,
	Gloves = 	1 << 2,
	Boots = 	1 << 3,
	Belt = 		1 << 4,
	Ring = 		1 << 5,
	Amulet = 	1 << 6,
	OHWeapon = 	1 << 7,
	THWeapon = 	1 << 8,
	Bow = 		1 << 9,
	Staff = 	1 << 10,

	Shield = 	1 << 11,
	Quiver = 	1 << 12,

	Armour = 	1 << 13,
	Jewellery = 1 << 14,
	Weapon = 	1 << 15,
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

	FlatFireDamage,
	IncreasedFireDamage,

	FlatColdDamage,
	IncreasedColdDamage,

	FlatLightningDamage,
	IncreasedLightningDamage,

	FlatChaosDamage,
	IncreasedChaosDamage,

	IncreasedMeleeDamage,
	IncreasedRangedDamage,
	IncreasedSpellDamage,

	IncreasedAttackSpeed,
	IncreasedCastSpeed,
	IncreasedCritChance,
	AddedCritMulti,

	IncreasedMovementSpeed,
	AddedBlockChance,
	IncreasedBlockChance,

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

	LocalFlatFireDamage,
	LocalFlatColdDamage,
	LocalFlatLightningDamage,
	LocalFlatChaosDamage,

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



public enum EDamageCategory {
	Melee,
	Ranged,
	Spell,
	Untyped
}

public enum ESkillName {
	BasicThrust,
	PiercingShot,
	PrismaticBolt,
	SplitArrow,
	FireNova,
	COUNT
}

public enum ESkillType {
    Attack,
    Spell
}

[Flags]
public enum ESkillTags {
    None = 0,
	Attack = 1 << 0,
    Spell = 1 << 1,
	
    Melee = 1 << 2,
    Ranged = 1 << 3,

	Projectile = 1 << 4,
    Area = 1 << 5,
	Duration = 1 << 6,

	Physical = 1 << 7,
	Fire = 1 << 8,
	Cold = 1 << 9,
	Lightning = 1 << 10,
	Chaos = 1 << 11
}

[Flags]
public enum ESkillWeapons {
    None = 0,
    Unarmed = 1 << 0,
    Melee1H = 1 << 1,
    Melee2H = 1 << 2,
    MeleeDW = 1 << 3,
    Ranged1H = 1 << 4,
    Ranged2H = 1 << 5,

    AllMelee = Unarmed | Melee1H | Melee2H | MeleeDW,
	AllMeleeWeapons = Melee1H | Melee2H | MeleeDW,
    AllRangedWeapons = Ranged1H | Ranged2H
}


public enum EActorState {
	Actionable,
	UsingSkill,
	Stunned,
	Dying,
	Dead
}

public enum EEnemyRarity {
	Normal,
	Magic,
	Rare,
	Unique,
	Boss
}

public enum EShopType {
	Equipment,
	Gems
}
