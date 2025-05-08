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
		new(typeof(FlatLifeAffix), EAffixFamily.FlatMaxLife,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatManaAffix), EAffixFamily.FlatMaxMana,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalFlatPhysDamageAffix), EAffixFamily.LocalFlatPhysDamage,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalIncreasedPhysDamageAffix), EAffixFamily.LocalIncreasedPhysDamage,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
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
		new(typeof(IncreasedMeleeDamageAffix), EAffixFamily.IncreasedMeleeDamage,
			EAffixItemFlags.Shield,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedRangedDamageAffix), EAffixFamily.IncreasedRangedDamage,
			EAffixItemFlags.None, // Cannot currently roll on anything. In the future, should roll on quivers or other ranged offhand items
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(IncreasedSpellDamageAffix), EAffixFamily.IncreasedSpellDamage,
			EAffixItemFlags.None, // Cannot currently roll on anything. In the future, should roll on catalysts or other spell weapons or offhand items
			EItemBaseSpecifierFlags.NoFlags
		),
	];

	public static readonly List<AffixTableType> SuffixData = [
		new(typeof(LocalIncreasedAttackSpeedAffix), EAffixFamily.LocalIncreasedAttackSpeed,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalIncreasedCritChanceAffix), EAffixFamily.LocalIncreasedCritChance,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(GlobalIncreasedAttackSpeedAffix), EAffixFamily.IncreasedAttackSpeed,
			EAffixItemFlags.Ring | EAffixItemFlags.Amulet,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(GlobalIncreasedCritChanceAffix), EAffixFamily.IncreasedCritChance,
			EAffixItemFlags.Ring | EAffixItemFlags.Amulet,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatStrengthAffix), EAffixFamily.AddedStrength,
			EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatDexterityAffix), EAffixFamily.AddedDexterity,
			EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FlatIntelligenceAffix), EAffixFamily.AddedIntelligence,
			EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(FireResistanceAffix), EAffixFamily.FireResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(ColdResistanceAffix), EAffixFamily.ColdResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LightningResistanceAffix), EAffixFamily.LightningResistance,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		)
	];
}
