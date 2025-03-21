using Godot;
using System;
using System.Collections.Generic;

public class AffixData {
	public EAffixItemFlags AffixItemFlags { get; }
	public EItemBaseSpecifierFlags BaseSpecifierFlags { get; }
	public EAffixFamily AffixFamily { get; }
	public string Name { get; }
	public double AffixMinFirst { get; }
	public double AffixMaxFirst { get; }
	public double AffixMinSecond { get; }
	public double AffixMaxSecond { get; }
	public int MinimumLevel { get; }

	public AffixData(EAffixItemFlags itemFlags, EItemBaseSpecifierFlags specifier, EAffixFamily family, string name, double affixMinF, double affixMaxF, double affixMinS, double affixMaxS, int minLvl) {
		AffixItemFlags = itemFlags;
		BaseSpecifierFlags = specifier;
		AffixFamily = family;
		Name = name;
		AffixMinFirst = affixMinF;
		AffixMaxFirst = affixMaxF;
		AffixMinSecond = affixMinS;
		AffixMaxSecond = affixMaxS;
		MinimumLevel = minLvl;
	}
}

public class AffixTableType {
	public EAffixPosition AffixPosition { get; }
	public EAffixItemFlags AffixItemFlags { get; }
	public EItemBaseSpecifierFlags AffixItemSpecifierFlags { get; }
	public List<AffixData> AffixTable { get; }
	public Type AffixClassType { get; }

	public AffixTableType(EAffixPosition Pos, EAffixItemFlags itemFlags, EItemBaseSpecifierFlags specFlags, List<AffixData> table, Type type) {
		AffixPosition = Pos;
		AffixItemFlags = itemFlags;
		AffixItemSpecifierFlags = specFlags;
		AffixTable = table;
		AffixClassType = type;
	}
}

public static class AffixDataTables {
	public static readonly List<AffixData> LocalFlatPhysDamageAffixData = new List<AffixData> {
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalFlatPhysDamage, "Pointy", 1, 2, 3, 5, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalFlatPhysDamage, "Sharp", 3, 4, 5, 7, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalFlatPhysDamage, "Jagged", 4, 6, 7, 10, 0)
	};

	public static readonly List<AffixData> LocalPercentagePhysDamageAffixData = new List<AffixData> {
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Upset", 0.20, 0.29, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Angry", 0.30, 0.39, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Mad", 0.40, 0.49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalPercentageAttackSpeedAffixData = new List<AffixData> {
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageAttackSpeed, "of AS1", 0.05, 0.08, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageAttackSpeed, "of AS2", 0.09, 0.12, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageAttackSpeed, "of AS3", 0.13, 0.16, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalPercentageCritChanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageCritChance, "of CC1", 0.10, 0.13, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageCritChance, "of CC2", 0.14, 0.18, 0, 0, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentageCritChance, "of CC3", 0.19, 0.23, 0, 0, 0)
	};


	public static readonly List<AffixData> FlatHealthAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Healthy", 20, 29, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Vital", 30, 39, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Vibrant", 40, 49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalFlatArmourAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatArmour, "Rough", 20, 29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatArmour, "Hard", 30, 39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatArmour, "Sturdy", 40, 49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalPercentageArmourAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageArmour, "Toughness", 0.20, 0.29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageArmour, "Stable", 0.30, 0.39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageArmour, "Rocky", 0.40, 0.49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalFlatEvasionAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEvasion, "Light", 20, 29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEvasion, "Nimble", 30, 39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEvasion, "Flexible", 40, 49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalPercentageEvasionAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEvasion, "Evasive", 0.20, 0.29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEvasion, "Fast", 0.30, 0.39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEvasion, "Dodging", 0.40, 0.49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalFlatEnergyShieldAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEnergyShield, "Energetic", 20, 29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEnergyShield, "Durable", 30, 39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalFlatEnergyShield, "Projecting", 40, 49, 0, 0, 0)
	};

	public static readonly List<AffixData> LocalPercentageEnergyShieldAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEnergyShield, "Shielding", 0.20, 0.29, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEnergyShield, "Inner", 0.30, 0.39, 0, 0, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.LocalPercentageEnergyShield, "Blue", 0.40, 0.49, 0, 0, 0)
	};


	public static readonly List<AffixData> FireResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Wick", 5, 9, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Sun", 10, 14, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Flame", 15, 19, 0, 0, 0)
	};

	public static readonly List<AffixData> ColdResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of Sleet", 5, 9, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of the Tundra", 10, 14, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of Ice", 15, 19, 0, 0, 0)
	};

	public static readonly List<AffixData> LightningResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of the Spark", 5, 9, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of Insulation", 10, 14, 0, 0, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of Lightning", 15, 19, 0, 0, 0)
	};

	public static readonly List<AffixTableType> PrefixData = new List<AffixTableType> {
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, FlatHealthAffixData, typeof(HealthAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalFlatPhysDamageAffixData, typeof(LocalFlatPhysDamageAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalPercentagePhysDamageAffixData, typeof(LocalPercentagePhysDamageAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalFlatArmourAffixData, typeof(LocalFlatArmourAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalPercentageArmourAffixData, typeof(LocalPercentageArmourAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalFlatEvasionAffixData, typeof(LocalFlatEvasionAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalPercentageEvasionAffixData, typeof(LocalPercentageEvasionAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalFlatEnergyShieldAffixData, typeof(LocalFlatEnergyShieldAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, LocalPercentageEnergyShieldAffixData, typeof(LocalPercentageEnergyShieldAffix)),
	};

	public static readonly List<AffixTableType> SuffixData = new List<AffixTableType> {
		new(EAffixPosition.Suffix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalPercentageAttackSpeedAffixData, typeof(LocalPercentageAttackSpeedAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalPercentageCritChanceAffixData, typeof(LocalPercentageCritChanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, FireResistanceAffixData, typeof(FireResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, ColdResistanceAffixData, typeof(ColdResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, LightningResistanceAffixData, typeof(LightningResistanceAffix))
	};
}
