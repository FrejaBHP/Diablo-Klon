using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Affix {
	protected EAffixFamily affixFamily;
	public EAffixFamily AffixFamily { get => affixFamily; }

	protected AffixData tierData;
	public AffixData TierData { get => tierData; }

	protected double valueFirst;
	public double ValueFirst { get => valueFirst; } 

	protected double valueSecond;
	public double ValueSecond { get => valueSecond; } 

	protected EStatName statNameFirst = EStatName.None;
	public EStatName StatNameFirst { get => statNameFirst; }

	protected EStatName statNameSecond = EStatName.None;
	public EStatName StatNameSecond { get => statNameSecond; }

	protected bool isLocal;
	public bool IsLocal { get => isLocal; }
	
	protected bool isMultiplicative;
	public bool IsMultiplicative { get => isMultiplicative; }

	protected List<AffixData> affixDataTable;
	public List<AffixData> AffixDataTable  { get => affixDataTable; }

	public virtual void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = affixDataTable.Where(a => a.MinimumLevel <= itemLevel).ToList();
        tierData = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
	}

	public virtual void SetAffixTier(int tier) {
		tierData = affixDataTable[tier];
	}

	public virtual string GetAffixName() {
		return tierData.Name;
	}

	public abstract void RollAffixValue();

	public abstract string GetAffixTooltipText();
}

public class AffixData(int minLvl, string name, double affixMinF, double affixMaxF, double affixMinS, double affixMaxS) {
    public int MinimumLevel { get; } = minLvl;
    public string Name { get; } = name;
    public double AffixMinFirst { get; } = affixMinF;
    public double AffixMaxFirst { get; } = affixMaxF;
    public double AffixMinSecond { get; } = affixMinS;
    public double AffixMaxSecond { get; } = affixMaxS;
}


// ======== IMPLICITS ========

public class BasicRingLifeImplicit : Affix {
	private static readonly List<AffixData> basicRingLifeAffixData = [
		new(0, "",
			10, 15,
			0, 0
		)
	];

	public BasicRingLifeImplicit() {
		affixDataTable = basicRingLifeAffixData;
		affixFamily = EAffixFamily.None;
		statNameFirst = EStatName.FlatMaxLife;
		isLocal = false;
		isMultiplicative = false;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
        valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
    }

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Maximum Life";
	}
}

public class BasicAmuletPhysImplicit : Affix {
	private static readonly List<AffixData> basicAmuletPhysAffixData = [
		new(0, "",
			1, 1,
			3, 3
		)
	];

	public BasicAmuletPhysImplicit() {
		affixDataTable = basicAmuletPhysAffixData;
		affixFamily = EAffixFamily.None;
		statNameFirst = EStatName.FlatMinPhysDamage;
		statNameSecond = EStatName.FlatMaxPhysDamage;
		isLocal = false;
		isMultiplicative = false;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
        valueFirst = tierData.AffixMinFirst;
		valueSecond = tierData.AffixMinSecond;
    }

	public override string GetAffixTooltipText() {
		return $"Adds {(int)valueFirst} to {(int)valueSecond} Physical Damage to Attacks";
	}
}







public class LocalFlatPhysDamageAffix : Affix {
	private static readonly List<AffixData> localFlatPhysDamageAffixData = [
		new(0, "Pointy", 
			1, 2, 
			3, 4
		),
		new(0, "Sharp", 
			3, 4, 
			5, 7
		),
		new(0, "Jagged", 
			4, 6, 
			7, 9
		)
	];

	public LocalFlatPhysDamageAffix() {
		affixDataTable = localFlatPhysDamageAffixData;
		affixFamily = EAffixFamily.LocalFlatPhysDamage;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
			valueSecond = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinSecond, tierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)valueFirst} to {(int)valueSecond} Local Physical Damage";
	}
}

public class LocalIncreasedPhysDamageAffix : Affix {
	private static readonly List<AffixData> localIncreasedPhysDamageAffixData = [
		new(0, "Upset", 
			0.15, 0.24, 
			0, 0
		),
		new(0, "Angry", 
			0.25, 0.34, 
			0, 0
		),
		new(0, "Mad", 
			0.35, 0.44, 
			0, 0
		)
	];

	public LocalIncreasedPhysDamageAffix() {
		affixDataTable = localIncreasedPhysDamageAffixData;
		affixFamily = EAffixFamily.LocalIncreasedPhysDamage;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Local Physical Damage";
	}
}

public class FlatLifeAffix : Affix {
	private static readonly List<AffixData> flatLifeAffixData = [
		new(0, "Healthy", 
			10, 19, 
			0, 0
		),
		new(0, "Vital", 
			20, 29, 
			0, 0
		),
		new(0, "Vibrant", 
			30, 39, 
			0, 0
		)
	];

	public FlatLifeAffix() {
		affixDataTable = flatLifeAffixData;
		affixFamily = EAffixFamily.FlatMaxLife;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FlatMaxLife;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Maximum Life";
	}
}

public class FlatManaAffix : Affix {
	private static readonly List<AffixData> flatManaAffixData = [
		new(0, "Bubbling", 
			10, 16, 
			0, 0
		),
		new(0, "Sage", 
			17, 23, 
			0, 0
		),
		new(0, "Magical", 
			24, 30, 
			0, 0
		)
	];

	public FlatManaAffix() {
		affixDataTable = flatManaAffixData;
		affixFamily = EAffixFamily.FlatMaxMana;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FlatMaxMana;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Maximum Mana";
	}
}

public class LocalFlatArmourAffix : Affix {
	private static readonly List<AffixData> localFlatArmourAffixData = [
		new(0, "Rough", 
			15, 24, 
			0, 0
		),
		new(0, "Hard", 
			25, 34, 
			0, 0
		),
		new(0, "Sturdy", 
			35, 44, 
			0, 0
		)
	];

	public LocalFlatArmourAffix() {
		affixDataTable = localFlatArmourAffixData;
		affixFamily = EAffixFamily.LocalFlatArmour;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Armour";
	}
}

public class LocalIncreasedArmourAffix : Affix {
	private static readonly List<AffixData> localIncreasedArmourAffixData = [
		new(0, "Tough", 
			0.15, 0.24, 
			0, 0
		),
		new(0, "Stable", 
			0.25, 0.34, 
			0, 0
		),
		new(0, "Rocky", 
			0.35, 0.44, 
			0, 0
		)
	];

	public LocalIncreasedArmourAffix() {
		affixDataTable = localIncreasedArmourAffixData;
		affixFamily = EAffixFamily.LocalIncreasedArmour;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		
		return $"{Math.Round(valueFirst * 100, 0)}% increased Armour";
	}
}

public class LocalFlatEvasionAffix : Affix {
	private static readonly List<AffixData> localFlatEvasionAffixData = [
		new(0, "Light", 
			15, 24, 
			0, 0
		),
		new(0, "Nimble", 
			25, 34, 
			0, 0
		),
		new(0, "Flexible", 
			35, 44, 
			0, 0
		)
	];

	public LocalFlatEvasionAffix() {
		affixDataTable = localFlatEvasionAffixData;
		affixFamily = EAffixFamily.LocalFlatEvasion;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Evasion Rating";
	}
}

public class LocalIncreasedEvasionAffix : Affix {
	private static readonly List<AffixData> localIncreasedEvasionAffixData = [
		new(0, "Evasive", 
			0.15, 0.24, 
			0, 0
		),
		new(0, "Fast", 
			0.25, 0.34, 
			0, 0
		),
		new(0, "Dodging", 
			0.35, 0.44, 
			0, 0
		)
	];

	public LocalIncreasedEvasionAffix() {
		affixDataTable = localIncreasedEvasionAffixData;
		affixFamily = EAffixFamily.LocalIncreasedEvasion;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Evasion Rating";
	}
}

public class LocalFlatEnergyShieldAffix : Affix {
	private static readonly List<AffixData> localFlatEnergyShieldAffixData = [
		new(0, "Energetic", 
			10, 19, 
			0, 0
		),
		new(0, "Durable", 
			20, 29, 
			0, 0
		),
		new(0, "Projecting", 
			30, 39, 
			0, 0
		)
	];

	public LocalFlatEnergyShieldAffix() {
		affixDataTable = localFlatEnergyShieldAffixData;
		affixFamily = EAffixFamily.LocalFlatEnergyShield;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Energy Shield";
	}
}

public class LocalIncreasedEnergyShieldAffix : Affix {
	private static readonly List<AffixData> localIncreasedEnergyShieldAffixData = [
		new(0, "Shielding", 
			0.15, 0.24, 
			0, 0
		),
		new(0, "Inner", 
			0.25, 0.34, 
			0, 0
		),
		new(9, "Blue", 
			0.35, 0.44, 
			0, 0
		)
	];

	public LocalIncreasedEnergyShieldAffix() {
		affixDataTable = localIncreasedEnergyShieldAffixData;
		affixFamily = EAffixFamily.LocalIncreasedEnergyShield;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Energy Shield";
	}
}

public class IncreasedMeleeDamageAffix : Affix {
	private static readonly List<AffixData> increasedMeleeDamageAffixData = [
		new(0, "Melee1", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Melee2", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Melee3", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedMeleeDamageAffix() {
		affixDataTable = increasedMeleeDamageAffixData;
		affixFamily = EAffixFamily.IncreasedMeleeDamage;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.IncreasedMeleeDamage;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Melee Damage";
	}
}

public class IncreasedMovementSpeedAffix : Affix {
	private static readonly List<AffixData> increasedMovementSpeedAffixData = [
		new(0, "MS1", 
			0.05, 0.09, 
			0, 0
		),
		new(0, "MS2", 
			0.10, 0.14, 
			0, 0
		),
		new(0, "MS3", 
			0.15, 0.19, 
			0, 0
		)
	];

	public IncreasedMovementSpeedAffix() {
		affixDataTable = increasedMovementSpeedAffixData;
		affixFamily = EAffixFamily.IncreasedMovementSpeed;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.IncreasedMovementSpeed;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Movement Speed";
	}
}




public class LocalIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> localIncreasedAttackSpeedAffixData = [
		new(0, "of AS1", 
			0.05, 0.08, 
			0, 0
		),
		new(0, "of AS2", 
			0.09, 0.12, 
			0, 0
		),
		new(0, "of AS3", 
			0.13, 0.16, 
			0, 0
		)
	];

	public LocalIncreasedAttackSpeedAffix() {
		affixDataTable = localIncreasedAttackSpeedAffixData;
		affixFamily = EAffixFamily.LocalIncreasedAttackSpeed;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Local Attack Speed";
	}
}

public class LocalIncreasedCritChanceAffix : Affix {
	private static readonly List<AffixData> localIncreasedCritChanceAffixData = [
		new(0, "of CC1", 
			0.10, 0.13, 
			0, 0
		),
		new(0, "of CC2", 
			0.14, 0.18, 
			0, 0
		),
		new(0, "of CC3", 
			0.19, 0.23, 
			0, 0
		)
	];

	public LocalIncreasedCritChanceAffix() {
		affixDataTable = localIncreasedCritChanceAffixData;
		affixFamily = EAffixFamily.LocalIncreasedCritChance;
		isLocal = true;
		isMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Local Critical Strike Chance";
	}
}



public class GlobalIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> globalIncreasedAttackSpeedAffixData = [
		new(0, "of Global AS1", 
			0.05, 0.08, 
			0, 0
		),
		new(0, "of Global AS2", 
			0.09, 0.12, 
			0, 0
		),
		new(0, "of Global AS3", 
			0.13, 0.17, 
			0, 0
		)
	];

	public GlobalIncreasedAttackSpeedAffix() {
		affixDataTable = globalIncreasedAttackSpeedAffixData;
		affixFamily = EAffixFamily.IncreasedAttackSpeed;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.IncreasedAttackSpeed;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Attack Speed";
	}
}

public class GlobalIncreasedCritChanceAffix : Affix {
	private static readonly List<AffixData> globalIncreasedCritChanceAffixData = [
		new(0, "of Global CC1", 
			0.15, 0.19, 
			0, 0
		),
		new(0, "of Global CC2", 
			0.20, 0.24, 
			0, 0
		),
		new(0, "of Global CC3", 
			0.25, 0.29, 
			0, 0
		)
	];

	public GlobalIncreasedCritChanceAffix() {
		affixDataTable = globalIncreasedCritChanceAffixData;
		affixFamily = EAffixFamily.IncreasedCritChance;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.IncreasedCritChance;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Math.Round(Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Critical Strike Chance";
	}
}

public class FlatStrengthAffix : Affix {
	private static readonly List<AffixData> flatStrAffixData = [
		new(0, "of Beef", 
			2, 4, 
			0, 0
		),
		new(0, "of Stronk", 
			5, 8, 
			0, 0
		),
		new(0, "of Muscle", 
			9, 12, 
			0, 0
		)
	];

	public FlatStrengthAffix() {
		affixDataTable = flatStrAffixData;
		affixFamily = EAffixFamily.AddedStrength;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FlatStrength;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Strength";
	}
}

public class FlatDexterityAffix : Affix {
	private static readonly List<AffixData> flatDexAffixData = [
		new(0, "of Pace", 
			2, 4, 
			0, 0
		),
		new(0, "of Flexibility", 
			5, 8, 
			0, 0
		),
		new(0, "of Green", 
			9, 12, 
			0, 0
		)
	];

	public FlatDexterityAffix() {
		affixDataTable = flatDexAffixData;
		affixFamily = EAffixFamily.AddedDexterity;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FlatDexterity;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Dexterity";
	}
}

public class FlatIntelligenceAffix : Affix {
	private static readonly List<AffixData> flatIntAffixData = [
		new(0, "of Mind", 
			2, 4, 
			0, 0
		),
		new(0, "of Insight", 
			5, 8, 
			0, 0
		),
		new(0, "of the Book", 
			9, 12, 
			0, 0
		)
	];

	public FlatIntelligenceAffix() {
		affixDataTable = flatIntAffixData;
		affixFamily = EAffixFamily.AddedIntelligence;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FlatIntelligence;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Intelligence";
	}
}


public class FireResistanceAffix : Affix {
	private static readonly List<AffixData> fireResistanceAffixData = [
		new(0, "of the Wick", 
			5, 9, 
			0, 0
		),
		new(0, "of the Sun", 
			10, 14, 
			0, 0
		),
		new(0, "of the Flame", 
			15, 19, 
			0, 0
		)
	];

	public FireResistanceAffix() {
		affixDataTable = fireResistanceAffixData;
		affixFamily = EAffixFamily.FireResistance;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.FireResistance;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Fire Resistance";
	}
}

public class ColdResistanceAffix : Affix {
	private static readonly List<AffixData> coldResistanceAffixData = [
		new(0, "of Sleet", 
			5, 9, 
			0, 0
		),
		new(0, "of the Tundra", 
			10, 14, 
			0, 0
		),
		new(0, "of Ice", 
			15, 19, 
			0, 0
		)
	];

	public ColdResistanceAffix() {
		affixDataTable = coldResistanceAffixData;
		affixFamily = EAffixFamily.ColdResistance;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.ColdResistance;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Cold Resistance";
	}
}

public class LightningResistanceAffix : Affix {
	private static readonly List<AffixData> lightningResistanceAffixData = [
		new(0, "of the Spark", 
			5, 9, 
			0, 0
		),
		new(0, "of Insulation", 
			10, 14, 
			0, 0
		),
		new(0, "of Lightning", 
			15, 19, 
			0, 0
		)
	];

	public LightningResistanceAffix() {
		affixDataTable = lightningResistanceAffixData;
		affixFamily = EAffixFamily.LightningResistance;
		isLocal = false;
		isMultiplicative = false;
		statNameFirst = EStatName.LightningResistance;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Lightning Resistance";
	}
}
