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
	IncreasedPhysDamage,
	PhysicalPenetration,

	FlatMinFireDamage,
	FlatMaxFireDamage,
	FlatAttackMinFireDamage,
	FlatAttackMaxFireDamage,
	FlatSpellMinFireDamage,
	FlatSpellMaxFireDamage,
	IncreasedFireDamage,
	FirePenetration,

	FlatMinColdDamage,
	FlatMaxColdDamage,
	FlatAttackMinColdDamage,
	FlatAttackMaxColdDamage,
	FlatSpellMinColdDamage,
	FlatSpellMaxColdDamage,
	IncreasedColdDamage,
	ColdPenetration,

	FlatMinLightningDamage,
	FlatMaxLightningDamage,
	FlatAttackMinLightningDamage,
	FlatAttackMaxLightningDamage,
	FlatSpellMinLightningDamage,
	FlatSpellMaxLightningDamage,
	IncreasedLightningDamage,
	LightningPenetration,

	FlatMinChaosDamage,
	FlatMaxChaosDamage,
	FlatAttackMinChaosDamage,
	FlatAttackMaxChaosDamage,
	FlatSpellMinChaosDamage,
	FlatSpellMaxChaosDamage,
	IncreasedChaosDamage,
	ChaosPenetration,

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

	AddedBleedChance,
	IncreasedBleedDamage,
	FasterBleed,
	AddedIgniteChance,
	IncreasedIgniteDamage,
	FasterIgnite,
	AddedPoisonChance,
	IncreasedPoisonDamage,
	FasterPoison,

	IncreasedBleedDuration,
	IncreasedIgniteDuration,
	IncreasedPoisonDuration,

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
