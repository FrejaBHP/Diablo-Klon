using Godot;
using System;

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
