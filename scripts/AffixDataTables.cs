using Godot;
using System;
using System.Collections.Generic;

[Flags]
public enum EAffixItemFlags {
	None = 0,
	Helmet = 1,
	Chest = 2,
	Gloves = 4,
	Boots = 8,
	Belt = 16,
	Ring = 32,
	Amulet = 64,
	OHWeapon = 128,
	THWeapon = 256,
	Shield = 512,

	Armour = 1024,
	Jewellery = 2048,
	Weapon = 4096,

	All = 8191
}

public class AffixData {
	public EAffixItemFlags AffixItemFlags { get; }
	public string Name { get; }
	public double AffixMin { get; }
	public double AffixMax { get; }
	public int MinimumLevel { get; }

	public AffixData(EAffixItemFlags itemFlags, string name, double affixMin, double affixMax, int minLvl) {
		AffixItemFlags = itemFlags;
		Name = name;
		AffixMin = affixMin;
		AffixMax = affixMax;
		MinimumLevel = minLvl;
	}
}

public class AffixTableType {
	public EAffixPosition AffixPosition { get; }
	public EAffixItemFlags AffixItemFlags { get; }
	public List<AffixData> AffixTable { get; }
	public Type AffixClassType { get; }

	public AffixTableType(EAffixPosition Pos, EAffixItemFlags itemFlags, List<AffixData> table, Type type) {
		AffixPosition = Pos;
		AffixItemFlags = itemFlags;
		AffixTable = table;
		AffixClassType = type;
	}
}

public static class AffixDataTables {
	public static readonly List<AffixData> HealthAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, "Healthy", 20, 29, 0),
		new(EAffixItemFlags.Armour, "Vital", 30, 39, 0),
		new(EAffixItemFlags.Armour, "Vibrant", 40, 49, 0)
	};

	public static readonly List<AffixData> FireResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, "of the Wick", 5, 9, 0),
		new(EAffixItemFlags.Armour, "of the Sun", 10, 14, 0),
		new(EAffixItemFlags.Armour, "of the Flame", 15, 19, 0)
	};

	public static readonly List<AffixData> ColdResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, "of Sleet", 5, 9, 0),
		new(EAffixItemFlags.Armour, "of the Tundra", 10, 14, 0),
		new(EAffixItemFlags.Armour, "of Ice", 15, 19, 0)
	};

	public static readonly List<AffixData> LightningResistanceAffixData = new List<AffixData> {
		new(EAffixItemFlags.Armour, "of the Spark", 5, 9, 0),
		new(EAffixItemFlags.Armour, "of Insulation", 10, 14, 0),
		new(EAffixItemFlags.Armour, "of Lightning", 15, 19, 0)
	};

	public static readonly List<AffixTableType> PrefixData = new List<AffixTableType> {
		new(EAffixPosition.Prefix, EAffixItemFlags.Armour, HealthAffixData, typeof(HealthAffix)),
	};

	public static readonly List<AffixTableType> SuffixData = new List<AffixTableType> {
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour, FireResistanceAffixData, typeof(FireResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour, ColdResistanceAffixData, typeof(ColdResistanceAffix)),
		new(EAffixPosition.Suffix, EAffixItemFlags.Armour, LightningResistanceAffixData, typeof(LightningResistanceAffix))
	};
}
