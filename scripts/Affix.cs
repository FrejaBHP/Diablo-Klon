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

	public bool IsHidden { get; protected set; } = false;

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

public class BasicStaffBlockImplicit : Affix {
	private static readonly List<AffixData> basicStaffBlockAffixData = [
		new(0, "",
			0.15, 0.15,
			0, 0
		)
	];

	public BasicStaffBlockImplicit() {
		AffixDataTable = basicStaffBlockAffixData;
		Family = EAffixFamily.None;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.BlockChance;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
        ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
    }

	public override string GetAffixTooltipText() {
		return $"+{ValueFirst:P0} to Block Chance";
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
		StatNameFirst = EStatName.FlatAttackMinPhysDamage;
		StatNameSecond = EStatName.FlatAttackMaxPhysDamage;

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

public class BasicWandSpellDamageAffix : Affix {
	private static readonly List<AffixData> increasedSpellDamageAffixData = [
		new(0, "", 
			0.08, 0.15, 
			0, 0
		)
	];

	public BasicWandSpellDamageAffix() {
		AffixDataTable = increasedSpellDamageAffixData;
		Family = EAffixFamily.IncreasedSpellDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedSpellDamage;

		SetAffixTier(0);
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Spell Damage";
	}
}

// =========== PREFIXES ===========

#region LocalDamage
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
		return $"{ValueFirst:P0} increased Local Physical Damage";
	}
}

public class Local1HFlatFireDamageAffix : Affix {
	private static readonly List<AffixData> local1HFlatFireDamageAffixData = [
		new(0, "Warm", 
			2, 3, 
			4, 5
		),
		new(0, "Hot", 
			4, 5, 
			6, 7
		),
		new(0, "Fiery", 
			5, 7, 
			8, 10
		)
	];

	public Local1HFlatFireDamageAffix() {
		AffixDataTable = local1HFlatFireDamageAffixData;
		Family = EAffixFamily.LocalFlatFireDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Fire Damage";
	}
}

public class Local2HFlatFireDamageAffix : Affix {
	private static readonly List<AffixData> local2HFlatFireDamageAffixData = [
		new(0, "Warm", 
			3, 4, 
			5, 6
		),
		new(0, "Hot", 
			6, 8, 
			9, 11
		),
		new(0, "Fiery", 
			10, 12, 
			13, 15
		)
	];

	public Local2HFlatFireDamageAffix() {
		AffixDataTable = local2HFlatFireDamageAffixData;
		Family = EAffixFamily.LocalFlatFireDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Fire Damage";
	}
}

public class Local1HFlatColdDamageAffix : Affix {
	private static readonly List<AffixData> local1HFlatColdDamageAffixData = [
		new(0, "Chilly", 
			2, 3, 
			4, 5
		),
		new(0, "Frigid", 
			4, 5, 
			6, 7
		),
		new(0, "Freezing", 
			5, 7, 
			8, 10
		)
	];

	public Local1HFlatColdDamageAffix() {
		AffixDataTable = local1HFlatColdDamageAffixData;
		Family = EAffixFamily.LocalFlatColdDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Cold Damage";
	}
}

public class Local2HFlatColdDamageAffix : Affix {
	private static readonly List<AffixData> local2HFlatColdDamageAffixData = [
		new(0, "Chilly", 
			3, 4, 
			5, 6
		),
		new(0, "Frigid", 
			6, 8, 
			9, 11
		),
		new(0, "Freezing", 
			10, 12, 
			13, 15
		)
	];

	public Local2HFlatColdDamageAffix() {
		AffixDataTable = local2HFlatColdDamageAffixData;
		Family = EAffixFamily.LocalFlatColdDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Cold Damage";
	}
}

public class Local1HFlatLightningDamageAffix : Affix {
	private static readonly List<AffixData> local1HFlatLightningDamageAffixData = [
		new(0, "Static", 
			1, 2, 
			5, 6
		),
		new(0, "Charged", 
			2, 3, 
			8, 10
		),
		new(0, "Crackling", 
			3, 4, 
			11, 14
		)
	];

	public Local1HFlatLightningDamageAffix() {
		AffixDataTable = local1HFlatLightningDamageAffixData;
		Family = EAffixFamily.LocalFlatLightningDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Lightning Damage";
	}
}

public class Local2HFlatLightningDamageAffix : Affix {
	private static readonly List<AffixData> local2HFlatLightningDamageAffixData = [
		new(0, "Static", 
			1, 2, 
			7, 8
		),
		new(0, "Charged", 
			2, 3, 
			13, 16
		),
		new(0, "Crackling", 
			3, 4, 
			20, 23
		)
	];

	public Local2HFlatLightningDamageAffix() {
		AffixDataTable = local2HFlatLightningDamageAffixData;
		Family = EAffixFamily.LocalFlatLightningDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Lightning Damage";
	}
}

public class Local1HFlatChaosDamageAffix : Affix {
	private static readonly List<AffixData> local1HFlatChaosDamageAffixData = [
		new(0, "Vile", 
			2, 3, 
			4, 5
		),
		new(0, "Chaotic", 
			4, 5, 
			6, 7
		),
		new(0, "Corrupted", 
			5, 7, 
			8, 10
		)
	];

	public Local1HFlatChaosDamageAffix() {
		AffixDataTable = local1HFlatChaosDamageAffixData;
		Family = EAffixFamily.LocalFlatChaosDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Chaos Damage";
	}
}

public class Local2HFlatChaosDamageAffix : Affix {
	private static readonly List<AffixData> local2HFlatChaosDamageAffixData = [
		new(0, "Vile", 
			3, 4, 
			5, 6
		),
		new(0, "Chaotic", 
			6, 8, 
			9, 11
		),
		new(0, "Corrupted", 
			10, 12, 
			13, 15
		)
	];

	public Local2HFlatChaosDamageAffix() {
		AffixDataTable = local2HFlatChaosDamageAffixData;
		Family = EAffixFamily.LocalFlatChaosDamage;
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
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Local Chaos Damage";
	}
}
#endregion

#region FlatAttackDamage
public class FlatAttackFireDamageAccessoryAffix : Affix {
	private static readonly List<AffixData> flatAttackFireDamageAffixData = [
		new(0, "Heated", 
			1, 2, 
			3, 5
		),
		new(0, "Smoking", 
			2, 3, 
			5, 7
		),
		new(0, "Scorching", 
			4, 6, 
			8, 11
		)
	];

	public FlatAttackFireDamageAccessoryAffix() {
		AffixDataTable = flatAttackFireDamageAffixData;
		Family = EAffixFamily.FlatFireDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatAttackMinFireDamage;
		StatNameSecond = EStatName.FlatAttackMaxFireDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Fire Damage to Attacks";
	}
}

public class FlatAttackColdDamageAccessoryAffix : Affix {
	private static readonly List<AffixData> flatAttackColdDamageAffixData = [
		new(0, "Chilled", 
			1, 2, 
			3, 5
		),
		new(0, "Frigid", 
			2, 3, 
			5, 7
		),
		new(0, "Frozen", 
			4, 6, 
			8, 11
		)
	];

	public FlatAttackColdDamageAccessoryAffix() {
		AffixDataTable = flatAttackColdDamageAffixData;
		Family = EAffixFamily.FlatColdDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatAttackMinColdDamage;
		StatNameSecond = EStatName.FlatAttackMaxColdDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Cold Damage to Attacks";
	}
}

public class FlatAttackLightningDamageAccessoryAffix : Affix {
	private static readonly List<AffixData> flatAttackLightningDamageAffixData = [
		new(0, "Humming", 
			1, 2, 
			4, 7
		),
		new(0, "Crackling", 
			1, 2, 
			6, 9
		),
		new(0, "Sparking", 
			1, 3, 
			11, 16
		)
	];

	public FlatAttackLightningDamageAccessoryAffix() {
		AffixDataTable = flatAttackLightningDamageAffixData;
		Family = EAffixFamily.FlatLightningDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatAttackMinLightningDamage;
		StatNameSecond = EStatName.FlatAttackMaxLightningDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Lightning Damage to Attacks";
	}
}
#endregion

#region FlatSpellDamage
public class FlatSpellFireDamageWandAffix : Affix {
	private static readonly List<AffixData> flatSpellFireDamageAffixData = [
		new(0, "Heated", 
			2, 4, 
			5, 7
		),
		new(0, "Smoking", 
			5, 7, 
			9, 12
		),
		new(0, "Scorching", 
			8, 11, 
			12, 15
		)
	];

	public FlatSpellFireDamageWandAffix() {
		AffixDataTable = flatSpellFireDamageAffixData;
		Family = EAffixFamily.FlatFireDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinFireDamage;
		StatNameSecond = EStatName.FlatSpellMaxFireDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Fire Damage to Spells";
	}
}

public class FlatSpellFireDamageStaffAffix : Affix {
	private static readonly List<AffixData> flatSpellFireDamageAffixData = [
		new(0, "Heated", 
			4, 8, 
			10, 14
		),
		new(0, "Smoking", 
			10, 14, 
			18, 24
		),
		new(0, "Scorching", 
			16, 22, 
			24, 30
		)
	];

	public FlatSpellFireDamageStaffAffix() {
		AffixDataTable = flatSpellFireDamageAffixData;
		Family = EAffixFamily.FlatFireDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinFireDamage;
		StatNameSecond = EStatName.FlatSpellMaxFireDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Fire Damage to Spells";
	}
}

public class FlatSpellColdDamageWandAffix : Affix {
	private static readonly List<AffixData> flatSpellColdDamageAffixData = [
		new(0, "Chilled", 
			3, 4, 
			5, 6
		),
		new(0, "Frigid", 
			6, 7, 
			9, 11
		),
		new(0, "Frozen", 
			9, 11, 
			12, 14
		)
	];

	public FlatSpellColdDamageWandAffix() {
		AffixDataTable = flatSpellColdDamageAffixData;
		Family = EAffixFamily.FlatColdDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinColdDamage;
		StatNameSecond = EStatName.FlatSpellMaxColdDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Cold Damage to Spells";
	}
}

public class FlatSpellColdDamageStaffAffix : Affix {
	private static readonly List<AffixData> flatSpellColdDamageAffixData = [
		new(0, "Chilled", 
			5, 8, 
			10, 13
		),
		new(0, "Frigid", 
			11, 14, 
			18, 23
		),
		new(0, "Frozen", 
			15, 22, 
			24, 29
		)
	];

	public FlatSpellColdDamageStaffAffix() {
		AffixDataTable = flatSpellColdDamageAffixData;
		Family = EAffixFamily.FlatColdDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinColdDamage;
		StatNameSecond = EStatName.FlatSpellMaxColdDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Cold Damage to Spells";
	}
}

public class FlatSpellLightningDamageWandAffix : Affix {
	private static readonly List<AffixData> flatSpellLightningDamageAffixData = [
		new(0, "Humming", 
			1, 2, 
			6, 9
		),
		new(0, "Crackling", 
			1, 3, 
			11, 17
		),
		new(0, "Sparking", 
			1, 4, 
			15, 21
		)
	];

	public FlatSpellLightningDamageWandAffix() {
		AffixDataTable = flatSpellLightningDamageAffixData;
		Family = EAffixFamily.FlatLightningDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinLightningDamage;
		StatNameSecond = EStatName.FlatSpellMaxLightningDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Lightning Damage to Spells";
	}
}

public class FlatSpellLightningDamageStaffAffix : Affix {
	private static readonly List<AffixData> flatSpellLightningDamageAffixData = [
		new(0, "Humming", 
			1, 2, 
			12, 18
		),
		new(0, "Crackling", 
			1, 3, 
			22, 34
		),
		new(0, "Sparking", 
			1, 4, 
			30, 42
		)
	];

	public FlatSpellLightningDamageStaffAffix() {
		AffixDataTable = flatSpellLightningDamageAffixData;
		Family = EAffixFamily.FlatLightningDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.FlatSpellMinLightningDamage;
		StatNameSecond = EStatName.FlatSpellMaxLightningDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
			ValueSecond = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinSecond, TierData.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)ValueFirst} to {(int)ValueSecond} Lightning Damage to Spells";
	}
}
#endregion

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

#region LocalDefence
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
		return $"{ValueFirst:P0} increased Local Armour";
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
		return $"{ValueFirst:P0} increased Local Evasion Rating";
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
		return $"{ValueFirst:P0} increased Local Energy Shield";
	}
}
#endregion

#region GlobalDamage
public class IncreasedMeleeDamageAffix : Affix {
	private static readonly List<AffixData> increasedMeleeDamageAffixData = [
		new(0, "Striker's", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Fighter's", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Gladiator's", 
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
		return $"{ValueFirst:P0} increased Melee Damage";
	}
}

public class IncreasedProjectileDamageAffix : Affix {
	private static readonly List<AffixData> increasedProjectileDamageAffixData = [
		new(0, "Acute", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Perforating", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Incisive", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedProjectileDamageAffix() {
		AffixDataTable = increasedProjectileDamageAffixData;
		Family = EAffixFamily.IncreasedProjectileDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedProjectileDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Projectile Damage";
	}
}

public class IncreasedSpellDamageWandAffix : Affix {
	private static readonly List<AffixData> increasedSpellDamageAffixData = [
		new(0, "Apprentice's", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Adept's", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Scholar's", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedSpellDamageWandAffix() {
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
		return $"{ValueFirst:P0} increased Spell Damage";
	}
}

public class IncreasedSpellDamageStaffAffix : Affix {
	private static readonly List<AffixData> increasedSpellDamageAffixData = [
		new(0, "Apprentice's", 
			0.20, 0.32, 
			0, 0
		),
		new(0, "Adept's", 
			0.34, 0.46, 
			0, 0
		),
		new(0, "Scholar's", 
			0.48, 0.60, 
			0, 0
		)
	];

	public IncreasedSpellDamageStaffAffix() {
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
		return $"{ValueFirst:P0} increased Spell Damage";
	}
}

public class IncreasedFireDamageWandAffix : Affix {
	private static readonly List<AffixData> increasedFireDamageAffixData = [
		new(0, "Searing", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Blistering", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Volcanic", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedFireDamageWandAffix() {
		AffixDataTable = increasedFireDamageAffixData;
		Family = EAffixFamily.IncreasedFireDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedFireDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Fire Damage";
	}
}

public class IncreasedColdDamageWandAffix : Affix {
	private static readonly List<AffixData> increasedColdDamageAffixData = [
		new(0, "Biting", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Snowy", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Hailing", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedColdDamageWandAffix() {
		AffixDataTable = increasedColdDamageAffixData;
		Family = EAffixFamily.IncreasedColdDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedColdDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Cold Damage";
	}
}

public class IncreasedLightningDamageWandAffix : Affix {
	private static readonly List<AffixData> increasedLightningDamageAffixData = [
		new(0, "Charged", 
			0.10, 0.16, 
			0, 0
		),
		new(0, "Bolting", 
			0.17, 0.23, 
			0, 0
		),
		new(0, "Coursing", 
			0.24, 0.30, 
			0, 0
		)
	];

	public IncreasedLightningDamageWandAffix() {
		AffixDataTable = increasedLightningDamageAffixData;
		Family = EAffixFamily.IncreasedLightningDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedLightningDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Lightning Damage";
	}
}

public class IncreasedFireDamageStaffAffix : Affix {
	private static readonly List<AffixData> increasedFireDamageAffixData = [
		new(0, "Searing", 
			0.20, 0.32, 
			0, 0
		),
		new(0, "Blistering", 
			0.34, 0.46, 
			0, 0
		),
		new(0, "Volcanic", 
			0.48, 0.60, 
			0, 0
		)
	];

	public IncreasedFireDamageStaffAffix() {
		AffixDataTable = increasedFireDamageAffixData;
		Family = EAffixFamily.IncreasedFireDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedFireDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Fire Damage";
	}
}

public class IncreasedColdDamageStaffAffix : Affix {
	private static readonly List<AffixData> increasedColdDamageAffixData = [
		new(0, "Biting", 
			0.20, 0.32, 
			0, 0
		),
		new(0, "Snowy", 
			0.34, 0.46, 
			0, 0
		),
		new(0, "Hailing", 
			0.48, 0.60, 
			0, 0
		)
	];

	public IncreasedColdDamageStaffAffix() {
		AffixDataTable = increasedColdDamageAffixData;
		Family = EAffixFamily.IncreasedColdDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedColdDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Cold Damage";
	}
}

public class IncreasedLightningDamageStaffAffix : Affix {
	private static readonly List<AffixData> increasedLightningDamageAffixData = [
		new(0, "Charged", 
			0.20, 0.32, 
			0, 0
		),
		new(0, "Bolting", 
			0.34, 0.46, 
			0, 0
		),
		new(0, "Coursing", 
			0.48, 0.60, 
			0, 0
		)
	];

	public IncreasedLightningDamageStaffAffix() {
		AffixDataTable = increasedLightningDamageAffixData;
		Family = EAffixFamily.IncreasedLightningDamage;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedLightningDamage;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Lightning Damage";
	}
}

public class IncreasedDamageOverTimeAccessoryAffix : Affix {
	private static readonly List<AffixData> increasedDoTAffixData = [
		new(0, "Atrophying", 
			0.14, 0.22, 
			0, 0
		),
		new(0, "Dissolving", 
			0.23, 0.30, 
			0, 0
		),
		new(0, "Wasting", 
			0.31, 0.43, 
			0, 0
		)
	];

	public IncreasedDamageOverTimeAccessoryAffix() {
		AffixDataTable = increasedDoTAffixData;
		Family = EAffixFamily.IncreasedDamageOverTime;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedDamageOverTime;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Damage Over Time";
	}
}
#endregion

public class IncreasedMovementSpeedAffix : Affix {
	private static readonly List<AffixData> increasedMovementSpeedAffixData = [
		new(0, "Runner's", 
			0.05, 0.09, 
			0, 0
		),
		new(0, "Messenger's", 
			0.10, 0.14, 
			0, 0
		),
		new(0, "Sprinter's", 
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
		return $"{ValueFirst:P0} increased Movement Speed";
	}
}

public class AddedShieldBlockAffix : Affix {
	private static readonly List<AffixData> addedShieldBlockAffixData = [
		new(0, "Steadfast",
			0.02, 0.05,
			0, 0
		),
		new(0, "Unrelenting",
			0.05, 0.07,
			0, 0
		),
		new(0, "Unyielding",
			0.07, 0.10,
			0, 0
		)
	];

	public AddedShieldBlockAffix() {
		AffixDataTable = addedShieldBlockAffixData;
		Family = EAffixFamily.AddedBlockChance;
		IsLocal = true;
		IsMultiplicative = false;
		StatNameFirst = EStatName.BlockChance;

		SetAffixTier(0);
	}

    public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
    }

	public override string GetAffixTooltipText() {
		return $"+{ValueFirst:P0} to Block Chance";
	}
}




// =========== SUFFIXES ===========

public class AddedLifeRegenAffix : Affix {
	private static readonly List<AffixData> addedLifeRegenAffixData = [
		new(0, "of Recovery", 
			1.5, 3, 
			0, 0
		),
		new(0, "of Healing", 
			3, 5, 
			0, 0
		),
		new(0, "of Regeneration", 
			5, 8, 
			0, 0
		)
	];

	public AddedLifeRegenAffix() {
		AffixDataTable = addedLifeRegenAffixData;
		Family = EAffixFamily.AddedLifeRegen;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.AddedLifeRegen;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{Math.Round(ValueFirst, 1)} Life Regenerated per second";
	}
}

public class IncreasedManaRegenAffix : Affix {
	private static readonly List<AffixData> increasedManaRegenAffixData = [
		new(0, "of the Pupil", 
			0.1, 0.2, 
			0, 0
		),
		new(0, "of the Student", 
			0.2, 0.3, 
			0, 0
		),
		new(0, "of Clarity", 
			0.3, 0.4, 
			0, 0
		)
	];

	public IncreasedManaRegenAffix() {
		AffixDataTable = increasedManaRegenAffixData;
		Family = EAffixFamily.IncreasedManaRegen;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedManaRegen;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Mana Regeneration";
	}
}

public class Local1HIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> local1HIncreasedAttackSpeedAffixData = [
		new(0, "of Skill", 
			0.05, 0.07, 
			0, 0
		),
		new(0, "of Ease", 
			0.08, 0.11, 
			0, 0
		),
		new(0, "of Mastery", 
			0.12, 0.15, 
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
		return $"{ValueFirst:P0} increased Local Attack Speed";
	}
}

public class Local2HIncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> local2HIncreasedAttackSpeedAffixData = [
		new(0, "of Skill", 
			0.07, 0.10, 
			0, 0
		),
		new(0, "of Ease", 
			0.11, 0.15, 
			0, 0
		),
		new(0, "of Mastery", 
			0.16, 0.20, 
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
		return $"{ValueFirst:P0} increased Local Attack Speed";
	}
}

public class LocalIncreasedCritChanceAffix : Affix {
	private static readonly List<AffixData> localIncreasedCritChanceAffixData = [
		new(0, "of Needling", 
			0.10, 0.13, 
			0, 0
		),
		new(0, "of Stinging", 
			0.14, 0.18, 
			0, 0
		),
		new(0, "of Piercing", 
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
		return $"{ValueFirst:P0} increased Local Critical Strike Chance";
	}
}

public class IncreasedAttackSpeedAffix : Affix {
	private static readonly List<AffixData> increasedAttackSpeedAffixData = [
		new(0, "of Skill", 
			0.05, 0.08, 
			0, 0
		),
		new(0, "of Ease", 
			0.09, 0.12, 
			0, 0
		),
		new(0, "of Mastery", 
			0.13, 0.17, 
			0, 0
		)
	];

	public IncreasedAttackSpeedAffix() {
		AffixDataTable = increasedAttackSpeedAffixData;
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
		return $"{ValueFirst:P0} increased Attack Speed";
	}
}

public class IncreasedCastSpeedAffix : Affix {
	private static readonly List<AffixData> increasedCastSpeedAffixData = [
		new(0, "of Talent", 
			0.06, 0.09, 
			0, 0
		),
		new(0, "of Expertise", 
			0.10, 0.13, 
			0, 0
		),
		new(0, "of Finesse", 
			0.14, 0.18, 
			0, 0
		)
	];

	public IncreasedCastSpeedAffix() {
		AffixDataTable = increasedCastSpeedAffixData;
		Family = EAffixFamily.IncreasedCastSpeed;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedCastSpeed;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Cast Speed";
	}
}

public class IncreasedCastSpeedStaffAffix : Affix {
	private static readonly List<AffixData> increasedCastSpeedStaffAffixData = [
		new(0, "of Talent", 
			0.12, 0.18, 
			0, 0
		),
		new(0, "of Expertise", 
			0.19, 0.26, 
			0, 0
		),
		new(0, "of Finesse", 
			0.27, 0.35, 
			0, 0
		)
	];

	public IncreasedCastSpeedStaffAffix() {
		AffixDataTable = increasedCastSpeedStaffAffixData;
		Family = EAffixFamily.IncreasedCastSpeed;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.IncreasedCastSpeed;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Math.Round(Utilities.RandomDouble(TierData.AffixMinFirst, TierData.AffixMaxFirst), 2);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{ValueFirst:P0} increased Cast Speed";
	}
}

public class IncreasedCritChanceAffix : Affix {
	private static readonly List<AffixData> increasedCritChanceAffixData = [
		new(0, "of Needling", 
			0.15, 0.19, 
			0, 0
		),
		new(0, "of Stinging", 
			0.20, 0.24, 
			0, 0
		),
		new(0, "of Piercing", 
			0.25, 0.29, 
			0, 0
		)
	];

	public IncreasedCritChanceAffix() {
		AffixDataTable = increasedCritChanceAffixData;
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
		return $"{ValueFirst:P0} increased Critical Strike Chance";
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

public class ChaosResistanceAffix : Affix {
	private static readonly List<AffixData> chaosResistanceAffixData = [
		new(0, "of the Dark", 
			5, 9, 
			0, 0
		),
		new(0, "of the Void", 
			10, 14, 
			0, 0
		),
		new(0, "of the Abyss", 
			15, 19, 
			0, 0
		)
	];

	public ChaosResistanceAffix() {
		AffixDataTable = chaosResistanceAffixData;
		Family = EAffixFamily.ChaosResistance;
		IsLocal = false;
		IsMultiplicative = false;
		StatNameFirst = EStatName.ChaosResistance;
	}

	public override void RollAffixValue() {
		if (TierData != null) {
			ValueFirst = Utilities.RandomDoubleInclusiveToInt(TierData.AffixMinFirst, TierData.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)ValueFirst}% to Chaos Resistance";
	}
}
