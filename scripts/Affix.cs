using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Affix {
	public List<AffixData> AffixDataTable  { get; protected set; }
	public EAffixFamily Family { get; protected set; }
	public bool IsLocal { get; protected set; }
	public bool IsMultiplicative { get; protected set; }

	public EStatName StatNameFirst { get; protected set; } = EStatName.None;
	public EStatName StatNameSecond { get; protected set; } = EStatName.None;
	public double ValueFirst { get; protected set; } 
	public double ValueSecond { get; protected set; } 
	public AffixData TierData { get; protected set; }

	public virtual void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTable.Where(a => a.MinimumLevel <= itemLevel).ToList();
        TierData = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
	}

	public virtual void SetAffixTier(int tier) {
		TierData = AffixDataTable[tier];
	}

	public virtual string GetAffixName() {
		return TierData.Name;
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


// =========== IMPLICITS ===========

public class BasicRingLifeImplicit : Affix {
	private static readonly List<AffixData> basicRingLifeAffixData = [
		new(0, "",
			10, 15,
			0, 0
		)
	];

	public BasicRingLifeImplicit() {
		AffixDataTable = basicRingLifeAffixData;
		Family = EAffixFamily.None;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatMaxLife;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
        ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
    }

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Maximum Life";
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
		AffixDataTable = basicAmuletPhysAffixData;
		Family = EAffixFamily.None;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatMinPhysDamage;
		StatNameSecond = EStatName.FlatMaxPhysDamage;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
        ValueFirst = TierData.AffixMinFirst;
		ValueSecond = TierData.AffixMinSecond;
    }

	public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Physical Damage to Attacks";
	}
}


// =========== PREFIXES ===========

public class Local1HFlatPhysDamageAffix : Affix {
	private static readonly List<AffixData> local1HFlatPhysDamageAffixData = [
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

	public Local1HFlatPhysDamageAffix() {
		AffixDataTable = local1HFlatPhysDamageAffixData;
		Family = EAffixFamily.LocalFlatPhysDamage;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Physical Damage";
	}
}

public class Local2HFlatPhysDamageAffix : Affix {
	private static readonly List<AffixData> local2HFlatPhysDamageAffixData = [
		new(0, "Pointy", 
			2, 3, 
			4, 5
		),
		new(0, "Sharp", 
			4, 6, 
			7, 9
		),
		new(0, "Jagged", 
			8, 10, 
			11, 13
		)
	];

	public Local2HFlatPhysDamageAffix() {
		AffixDataTable = local2HFlatPhysDamageAffixData;
		Family = EAffixFamily.LocalFlatPhysDamage;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Physical Damage";
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
		AffixDataTable = localIncreasedPhysDamageAffixData;
		Family = EAffixFamily.LocalIncreasedPhysDamage;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Physical Damage";
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
		AffixDataTable = flatLifeAffixData;
		Family = EAffixFamily.FlatMaxLife;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatMaxLife;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Maximum Life";
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
		AffixDataTable = flatManaAffixData;
		Family = EAffixFamily.FlatMaxMana;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatMaxMana;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Maximum Mana";
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
		AffixDataTable = localFlatArmourAffixData;
		Family = EAffixFamily.LocalFlatArmour;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Local Armour";
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
		AffixDataTable = localIncreasedArmourAffixData;
		Family = EAffixFamily.LocalIncreasedArmour;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Armour";
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
		AffixDataTable = localFlatEvasionAffixData;
		Family = EAffixFamily.LocalFlatEvasion;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Local Evasion Rating";
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
		AffixDataTable = localIncreasedEvasionAffixData;
		Family = EAffixFamily.LocalIncreasedEvasion;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Evasion Rating";
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
		AffixDataTable = localFlatEnergyShieldAffixData;
		Family = EAffixFamily.LocalFlatEnergyShield;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Local Energy Shield";
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
		AffixDataTable = localIncreasedEnergyShieldAffixData;
		Family = EAffixFamily.LocalIncreasedEnergyShield;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Energy Shield";
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
		AffixDataTable = increasedMeleeDamageAffixData;
		Family = EAffixFamily.IncreasedMeleeDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedMeleeDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Melee Damage";
	}
}

public class IncreasedRangedDamageAffix : Affix {
	private static readonly List<AffixData> increasedRangedDamageAffixData = [
		new(0, "Ranged1", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Ranged2", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Ranged3", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedRangedDamageAffix() {
		AffixDataTable = increasedRangedDamageAffixData;
		Family = EAffixFamily.IncreasedRangedDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedRangedDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Ranged Damage";
	}
}

public class IncreasedSpellDamageAffix : Affix {
	private static readonly List<AffixData> increasedSpellDamageAffixData = [
		new(0, "Spell1", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Spell2", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Spell3", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedSpellDamageAffix() {
		AffixDataTable = increasedSpellDamageAffixData;
		Family = EAffixFamily.IncreasedSpellDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedSpellDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Spell Damage";
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
		AffixDataTable = increasedMovementSpeedAffixData;
		Family = EAffixFamily.IncreasedMovementSpeed;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedMovementSpeed;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Movement Speed";
	}
}


// =========== SUFFIXES ===========

public class Local1HIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> local1HIncreasedAttackSpeedAffixData = [
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

	public Local1HIncreasedAttackSpeedAffix() {
		AffixDataTable = local1HIncreasedAttackSpeedAffixData;
		Family = EAffixFamily.LocalIncreasedAttackSpeed;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Attack Speed";
	}
}

public class Local2HIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> local2HIncreasedAttackSpeedAffixData = [
		new(0, "of AS1", 
			0.07, 0.11, 
			0, 0
		),
		new(0, "of AS2", 
			0.12, 0.16, 
			0, 0
		),
		new(0, "of AS3", 
			0.17, 0.22, 
			0, 0
		)
	];

	public Local2HIncreasedAttackSpeedAffix() {
		AffixDataTable = local2HIncreasedAttackSpeedAffixData;
		Family = EAffixFamily.LocalIncreasedAttackSpeed;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Attack Speed";
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
		AffixDataTable = localIncreasedCritChanceAffixData;
		Family = EAffixFamily.LocalIncreasedCritChance;
		IsLocal = true;
		IsMultiplicative = false;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Local Critical Strike Chance";
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
		AffixDataTable = globalIncreasedAttackSpeedAffixData;
		Family = EAffixFamily.IncreasedAttackSpeed;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedAttackSpeed;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Attack Speed";
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
		AffixDataTable = globalIncreasedCritChanceAffixData;
		Family = EAffixFamily.IncreasedCritChance;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedCritChance;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(ValueFirst * 100, 0)}% increased Critical Strike Chance";
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
		AffixDataTable = flatStrAffixData;
		Family = EAffixFamily.AddedStrength;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatStrength;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Strength";
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
		AffixDataTable = flatDexAffixData;
		Family = EAffixFamily.AddedDexterity;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatDexterity;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Dexterity";
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
		AffixDataTable = flatIntAffixData;
		Family = EAffixFamily.AddedIntelligence;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatIntelligence;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst} to Intelligence";
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
		AffixDataTable = fireResistanceAffixData;
		Family = EAffixFamily.FireResistance;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FireResistance;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst}% to Fire Resistance";
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
		AffixDataTable = coldResistanceAffixData;
		Family = EAffixFamily.ColdResistance;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.ColdResistance;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst}% to Cold Resistance";
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
		AffixDataTable = lightningResistanceAffixData;
		Family = EAffixFamily.LightningResistance;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.LightningResistance;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst}% to Lightning Resistance";
	}
}
