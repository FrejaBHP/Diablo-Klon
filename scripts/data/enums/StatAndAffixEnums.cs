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
	FlatAttackMinPhysDamage,
	FlatAttackMaxPhysDamage,
	FlatSpellMinPhysDamage,
	FlatSpellMaxPhysDamage,
	FlatPhysDamage,
	IncreasedPhysDamage,

	FlatMinFireDamage,
	FlatMaxFireDamage,
	FlatAttackMinFireDamage,
	FlatAttackMaxFireDamage,
	FlatSpellMinFireDamage,
	FlatSpellMaxFireDamage,
	FlatFireDamage,
	IncreasedFireDamage,

	FlatMinColdDamage,
	FlatMaxColdDamage,
	FlatAttackMinColdDamage,
	FlatAttackMaxColdDamage,
	FlatSpellMinColdDamage,
	FlatSpellMaxColdDamage,
	FlatColdDamage,
	IncreasedColdDamage,

	FlatMinLightningDamage,
	FlatMaxLightningDamage,
	FlatAttackMinLightningDamage,
	FlatAttackMaxLightningDamage,
	FlatSpellMinLightningDamage,
	FlatSpellMaxLightningDamage,
	FlatLightningDamage,
	IncreasedLightningDamage,

	FlatMinChaosDamage,
	FlatMaxChaosDamage,
	FlatAttackMinChaosDamage,
	FlatAttackMaxChaosDamage,
	FlatSpellMinChaosDamage,
	FlatSpellMaxChaosDamage,
	FlatChaosDamage,
	IncreasedChaosDamage,

	IncreasedAttackDamage,
	IncreasedSpellDamage,
	IncreasedMeleeDamage,
	IncreasedProjectileDamage,
	IncreasedAreaDamage,
	IncreasedDamageOverTime,
	IncreasedAllDamage,

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

	IncreasedAttackDamage,
	IncreasedSpellDamage,
	IncreasedMeleeDamage,
	IncreasedProjectileDamage,
	IncreasedAreaDamage,
	IncreasedDamageOverTime,
	IncreasedAllDamage,

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
