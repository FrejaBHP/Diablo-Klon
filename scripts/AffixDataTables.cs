using Godot;
using System;
using System.Collections.Generic;

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

public class AffixData {
	public EAffixItemFlags AffixItemFlags { get; }
	public EItemBaseSpecifierFlags BaseSpecifierFlags { get; }
	public EAffixFamily AffixFamily { get; }
	public string Name { get; }
	public double AffixMinFirst { get; }
	public double AffixMaxFirst { get; }
	public double? AffixMinSecond { get; }
	public double? AffixMaxSecond { get; }
	public int MinimumLevel { get; }

	public AffixData(EAffixItemFlags itemFlags, EItemBaseSpecifierFlags specifier, EAffixFamily family, string name, double affixMinF, double affixMaxF, double? affixMinS, double? affixMaxS, int minLvl) {
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
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Upset", 20, 29, null, null, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Angry", 30, 39, null, null, 0),
		new(EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LocalPercentagePhysDamage, "Mad", 40, 49, null, null, 0)
	};


	public static readonly List<AffixData> FlatHealthAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Healthy", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Vital", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FlatMaxLife, "Vibrant", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> FlatArmourAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatArmour, "Rough", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatArmour, "Hard", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatArmour, "Sturdy", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> PercentageArmourAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageArmour, "Toughness", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageArmour, "Stable", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageArmour, "Rocky", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> FlatEvasionAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEvasion, "Light", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEvasion, "Nimble", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEvasion, "Flexible", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> PercentageEvasionAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEvasion, "Evasive", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEvasion, "Fast", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEvasion, "Dodging", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> FlatEnergyShieldAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEnergyShield, "Energetic", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEnergyShield, "Durable", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.FlatEnergyShield, "Projecting", 40, 49, null, null, 0)
	};

	public static readonly List<AffixData> PercentageEnergyShieldAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEnergyShield, "Shielding", 20, 29, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEnergyShield, "Inner", 30, 39, null, null, 0),
		new(EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, EAffixFamily.PercentageEnergyShield, "Blue", 40, 49, null, null, 0)
	};


	public static readonly List<AffixData> FireResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Wick", 5, 9, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Sun", 10, 14, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.FireResistance, "of the Flame", 15, 19, null, null, 0)
	};

	public static readonly List<AffixData> ColdResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of Sleet", 5, 9, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of the Tundra", 10, 14, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.ColdResistance, "of Ice", 15, 19, null, null, 0)
	};

	public static readonly List<AffixData> LightningResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of the Spark", 5, 9, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of Insulation", 10, 14, null, null, 0),
		new(EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, EAffixFamily.LightningResistance, "of Lightning", 15, 19, null, null, 0)
	};

	public static readonly List<AffixTableType> PrefixData = new List<AffixTableType> {
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, FlatHealthAffixData, typeof(HealthAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalFlatPhysDamageAffixData, typeof(LocalFlatPhysDamageAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Weapon, EItemBaseSpecifierFlags.NoFlags, LocalPercentagePhysDamageAffixData, typeof(LocalPercentagePhysDamageAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, FlatArmourAffixData, typeof(FlatArmourAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AArmour | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, PercentageArmourAffixData, typeof(PercentageArmourAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, FlatEvasionAffixData, typeof(FlatEvasionAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEvasion | EItemBaseSpecifierFlags.AArmourEvasion | EItemBaseSpecifierFlags.AEnergyShieldArmour, PercentageEvasionAffixData, typeof(PercentageEvasionAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, FlatEnergyShieldAffixData, typeof(FlatEnergyShieldAffix)),
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, EItemBaseSpecifierFlags.AEnergyShield | EItemBaseSpecifierFlags.AEvasionEnergyShield | EItemBaseSpecifierFlags.AEnergyShieldArmour, PercentageEnergyShieldAffixData, typeof(PercentageEnergyShieldAffix)),
	};

	public static readonly List<AffixTableType> SuffixData = new List<AffixTableType> {
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, FireResistanceAffixData, typeof(FireResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, ColdResistanceAffixData, typeof(ColdResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour | EAffixItemFlags.Jewellery, EItemBaseSpecifierFlags.NoFlags, LightningResistanceAffixData, typeof(LightningResistanceAffix))
	};
}
