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
		new(typeof(HealthAffix), EAffixFamily.FlatMaxLife,
			EAffixItemFlags.Armour | EAffixItemFlags.Jewellery,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalFlatPhysDamageAffix), EAffixFamily.LocalFlatPhysDamage,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalPercentagePhysDamageAffix), EAffixFamily.LocalPercentagePhysDamage,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalFlatArmourAffix), EAffixFamily.LocalFlatArmour,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalPercentageArmourAffix), EAffixFamily.LocalPercentageArmour,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalFlatEvasionAffix), EAffixFamily.LocalFlatEvasion,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalPercentageEvasionAffix), EAffixFamily.LocalPercentageEvasion,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalFlatEnergyShieldAffix), EAffixFamily.LocalFlatEnergyShield,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
		new(typeof(LocalPercentageEnergyShieldAffix), EAffixFamily.LocalPercentageEnergyShield,
			EAffixItemFlags.Armour,
			EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour
		),
	];

	public static readonly List<AffixTableType> SuffixData = [
		new(typeof(LocalPercentageAttackSpeedAffix), EAffixFamily.LocalPercentageAttackSpeed,
			EAffixItemFlags.Weapon,
			EItemBaseSpecifierFlags.NoFlags
		),
		new(typeof(LocalPercentageCritChanceAffix), EAffixFamily.LocalPercentageCritChance,
			EAffixItemFlags.Weapon,
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
