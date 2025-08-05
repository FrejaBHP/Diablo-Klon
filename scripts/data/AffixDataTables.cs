using Godot;
using System;
using System.Collections.Generic;

public class AffixTableType {
	public Type AffixClassType { get; }
	public EAffixFamily AffixFamily { get; }
	public EAffixItemFlags AffixItemFlags { get; }
	public EItemBaseSpecifierFlags AffixItemSpecifierFlags { get; }

	public AffixTableType(Type type, EAffixFamily affixFamily, EAffixItemFlags itemFlags, EItemBaseSpecifierFlags specFlags) {
		AffixClassType = type;
		AffixFamily = affixFamily;
		AffixItemFlags = itemFlags;
		AffixItemSpecifierFlags = specFlags;
	}
}

public static class AffixDataTables {
	public static readonly List<AffixTableType> PrefixData = [
		// ===== LIFE AND MANA =====
		new(typeof(FlatLifeAffix), EAffixFamily.FlatMaxLife,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatManaAffix), EAffixFamily.FlatMaxMana,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Staff | EAffixItemFlags.Wand,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== LOCAL WEAPON DAMAGE =====
		new(typeof(Local1HFlatPhysDamageAffix), EAffixFamily.LocalFlatPhysDamage,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HFlatPhysDamageAffix), EAffixFamily.LocalFlatPhysDamage,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalIncreasedPhysDamageAffix), EAffixFamily.LocalIncreasedPhysDamage,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local1HFlatFireDamageAffix), EAffixFamily.LocalFlatFireDamage,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HFlatFireDamageAffix), EAffixFamily.LocalFlatFireDamage,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local1HFlatColdDamageAffix), EAffixFamily.LocalFlatColdDamage,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HFlatColdDamageAffix), EAffixFamily.LocalFlatColdDamage,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local1HFlatLightningDamageAffix), EAffixFamily.LocalFlatLightningDamage,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HFlatLightningDamageAffix), EAffixFamily.LocalFlatLightningDamage,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local1HFlatChaosDamageAffix), EAffixFamily.LocalFlatChaosDamage,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HFlatChaosDamageAffix), EAffixFamily.LocalFlatChaosDamage,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== LOCAL DEFENCES =====
		new(typeof(LocalFlatArmourAffix), EAffixFamily.LocalFlatArmour,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalIncreasedArmourAffix), EAffixFamily.LocalIncreasedArmour,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalFlatEvasionAffix), EAffixFamily.LocalFlatEvasion,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalIncreasedEvasionAffix), EAffixFamily.LocalIncreasedEvasion,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalFlatEnergyShieldAffix), EAffixFamily.LocalFlatEnergyShield,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalIncreasedEnergyShieldAffix), EAffixFamily.LocalIncreasedEnergyShield,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),

		new(typeof(IncreasedMovementSpeedAffix), EAffixFamily.IncreasedMovementSpeed,
			EAffixItemFlags.Boots,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(AddedShieldBlockAffix), EAffixFamily.AddedBlockChance,
			EAffixItemFlags.Shield,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== GLOBAL DAMAGE INCREASES =====
		new(typeof(IncreasedMeleeDamageAffix), EAffixFamily.IncreasedMeleeDamage,
			EAffixItemFlags.Shield,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedRangedDamageAffix), EAffixFamily.IncreasedRangedDamage,
			EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedSpellDamageAffix), EAffixFamily.IncreasedSpellDamage,
			EAffixItemFlags.Staff | EAffixItemFlags.Wand, // In the future, should roll on catalysts or other spell-related offhand items
			EItemBaseSpecifierFlags.NoFlags
		),
	];

	public static readonly List<AffixTableType> SuffixData = [
		// ===== REGENERATION =====
		new(typeof(AddedLifeRegenAffix), EAffixFamily.AddedLifeRegen,
			EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedManaRegenAffix), EAffixFamily.IncreasedManaRegen,
			EAffixItemFlags.Jewellery | EAffixItemFlags.Staff | EAffixItemFlags.Wand,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== LOCAL WEAPON STATS =====
		new(typeof(Local1HIncreasedAttackSpeedAffix), EAffixFamily.LocalIncreasedAttackSpeed,
			EAffixItemFlags.OHWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(Local2HIncreasedAttackSpeedAffix), EAffixFamily.LocalIncreasedAttackSpeed,
			EAffixItemFlags.THWeapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalIncreasedCritChanceAffix), EAffixFamily.LocalIncreasedCritChance,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== OTHER DAMAGE RELATED STATS =====
		new(typeof(IncreasedAttackSpeedAffix), EAffixFamily.IncreasedAttackSpeed,
			EAffixItemFlags.Ring | EAffixItemFlags.Amulet | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedCastSpeedAffix), EAffixFamily.IncreasedCastSpeed,
			EAffixItemFlags.Ring | EAffixItemFlags.Amulet | EAffixItemFlags.Wand,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedCastSpeedStaffAffix), EAffixFamily.IncreasedCastSpeed,
			EAffixItemFlags.Staff,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedCritChanceAffix), EAffixFamily.IncreasedCritChance,
			EAffixItemFlags.Ring | EAffixItemFlags.Amulet | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		
		// ===== ATTRIBUTES =====
		new(typeof(FlatStrengthAffix), EAffixFamily.AddedStrength,
			EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatDexterityAffix), EAffixFamily.AddedDexterity,
			EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatIntelligenceAffix), EAffixFamily.AddedIntelligence,
			EAffixItemFlags.Jewellery | EAffixItemFlags.Staff | EAffixItemFlags.Wand,
			EItemBaseSpecifierFlags.NoFlags
		),

		// ===== RESISTANCES =====
		new(typeof(FireResistanceAffix), EAffixFamily.FireResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(ColdResistanceAffix), EAffixFamily.ColdResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LightningResistanceAffix), EAffixFamily.LightningResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(ChaosResistanceAffix), EAffixFamily.ChaosResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery | EAffixItemFlags.Quiver,
			EItemBaseSpecifierFlags.NoFlags
		)
	];
}
