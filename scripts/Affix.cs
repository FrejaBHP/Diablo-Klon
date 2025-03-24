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
			20, 29,
			0, 0
		)
	];

	public BasicRingLifeImplicit() {
		affixDataTable = basicRingLifeAffixData;
		affixFamily = EAffixFamily.None;
		statNameFirst = EStatName.FlatMaxLife;
		isLocal = false;

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
			4, 4
		)
	];

	public BasicAmuletPhysImplicit() {
		affixDataTable = basicAmuletPhysAffixData;
		affixFamily = EAffixFamily.None;
		statNameFirst = EStatName.FlatMinPhysDamage;
		statNameSecond = EStatName.FlatMaxPhysDamage;
		isLocal = false;

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
			3, 5
		),
		new(0, "Sharp", 
			3, 4, 
			5, 7
		),
		new(0, "Jagged", 
			4, 6, 
			7, 10
		)
	];

	public LocalFlatPhysDamageAffix() {
		affixDataTable = localFlatPhysDamageAffixData;
		affixFamily = EAffixFamily.LocalFlatPhysDamage;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinFirst, tierData.AffixMaxFirst);
			valueSecond = Utilities.RandomDoubleInclusiveToInt(tierData.AffixMinSecond, tierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)valueFirst} to {(int)valueSecond} Physical Damage";
	}
}

public class LocalPercentagePhysDamageAffix : Affix {
	private static readonly List<AffixData> localPercentagePhysDamageAffixData = [
		new(0, "Upset", 
			0.20, 0.29, 
			0, 0
		),
		new(0, "Angry", 
			0.30, 0.39, 
			0, 0
		),
		new(0, "Mad", 
			0.40, 0.49, 
			0, 0
		)
	];

	public LocalPercentagePhysDamageAffix() {
		affixDataTable = localPercentagePhysDamageAffixData;
		affixFamily = EAffixFamily.LocalPercentagePhysDamage;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Physical Damage";
	}
}

public class HealthAffix : Affix {
	private static readonly List<AffixData> flatHealthAffixData = [
		new(0, "Healthy", 
			20, 29, 
			0, 0
		),
		new(0, "Vital", 
			30, 39, 
			0, 0
		),
		new(0, "Vibrant", 
			40, 49, 
			0, 0
		)
	];

	public HealthAffix() {
		affixDataTable = flatHealthAffixData;
		affixFamily = EAffixFamily.FlatMaxLife;
		isLocal = false;
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

public class LocalFlatArmourAffix : Affix {
	private static readonly List<AffixData> localFlatArmourAffixData = [
		new(0, "Rough", 
			20, 29, 
			0, 0
		),
		new(0, "Hard", 
			30, 39, 
			0, 0
		),
		new(0, "Sturdy", 
			40, 49, 
			0, 0
		)
	];

	public LocalFlatArmourAffix() {
		affixDataTable = localFlatArmourAffixData;
		affixFamily = EAffixFamily.LocalFlatArmour;
		isLocal = true;
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

public class LocalPercentageArmourAffix : Affix {
	private static readonly List<AffixData> localPercentageArmourAffixData = [
		new(0, "Toughness", 
			0.20, 0.29, 
			0, 0
		),
		new(0, "Stable", 
			0.30, 0.39, 
			0, 0
		),
		new(0, "Rocky", 
			0.40, 0.49, 
			0, 0
		)
	];

	public LocalPercentageArmourAffix() {
		affixDataTable = localPercentageArmourAffixData;
		affixFamily = EAffixFamily.LocalPercentageArmour;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		
		return $"{Math.Round(valueFirst * 100, 0)}% increased Armour";
	}
}

public class LocalFlatEvasionAffix : Affix {
	private static readonly List<AffixData> localFlatEvasionAffixData = [
		new(0, "Light", 
			20, 29, 
			0, 0
		),
		new(0, "Nimble", 
			30, 39, 
			0, 0
		),
		new(0, "Flexible", 
			40, 49, 
			0, 0
		)
	];

	public LocalFlatEvasionAffix() {
		affixDataTable = localFlatEvasionAffixData;
		affixFamily = EAffixFamily.LocalFlatEvasion;
		isLocal = true;
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

public class LocalPercentageEvasionAffix : Affix {
	private static readonly List<AffixData> localPercentageEvasionAffixData = [
		new(0, "Evasive", 
			0.20, 0.29, 
			0, 0
		),
		new(0, "Fast", 
			0.30, 0.39, 
			0, 0
		),
		new(0, "Dodging", 
			0.40, 0.49, 
			0, 0
		)
	];

	public LocalPercentageEvasionAffix() {
		affixDataTable = localPercentageEvasionAffixData;
		affixFamily = EAffixFamily.LocalPercentageEvasion;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Evasion Rating";
	}
}

public class LocalFlatEnergyShieldAffix : Affix {
	private static readonly List<AffixData> localFlatEnergyShieldAffixData = [
		new(0, "Energetic", 
			20, 29, 
			0, 0
		),
		new(0, "Durable", 
			30, 39, 
			0, 0
		),
		new(0, "Projecting", 
			40, 49, 
			0, 0
		)
	];

	public LocalFlatEnergyShieldAffix() {
		affixDataTable = localFlatEnergyShieldAffixData;
		affixFamily = EAffixFamily.LocalFlatEnergyShield;
		isLocal = true;
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

public class LocalPercentageEnergyShieldAffix : Affix {
	private static readonly List<AffixData> localPercentageEnergyShieldAffixData = [
		new(0, "Shielding", 
			0.20, 0.29, 
			0, 0
		),
		new(0, "Inner", 
			0.30, 0.39, 
			0, 0
		),
		new(9, "Blue", 
			0.40, 0.49, 
			0, 0
		)
	];

	public LocalPercentageEnergyShieldAffix() {
		affixDataTable = localPercentageEnergyShieldAffixData;
		affixFamily = EAffixFamily.LocalPercentageEnergyShield;
		isLocal = true;
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

public class LocalPercentageAttackSpeedAffix : Affix {
	private static readonly List<AffixData> localPercentageAttackSpeedAffixData = [
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

	public LocalPercentageAttackSpeedAffix() {
		affixDataTable = localPercentageAttackSpeedAffixData;
		affixFamily = EAffixFamily.LocalPercentageAttackSpeed;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Attack Speed";
	}
}

public class LocalPercentageCritChanceAffix : Affix {
	private static readonly List<AffixData> localPercentageCritChanceAffixData = [
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

	public LocalPercentageCritChanceAffix() {
		affixDataTable = localPercentageCritChanceAffixData;
		affixFamily = EAffixFamily.LocalPercentageCritChance;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (tierData != null) {
			valueFirst = Utilities.RandomDouble(tierData.AffixMinFirst, tierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Critical Strike Chance";
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
