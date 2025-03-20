using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Affix {
	protected AffixData data;
	public AffixData Data { get => data; }

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
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
	}

	public virtual void SetAffixTier(int tier) {
		data = affixDataTable[tier];
	}

	public virtual string GetAffixName() {
		return data.Name;
	}

	public abstract void RollAffixValue();

	public abstract string GetAffixTooltipText();
}

public class LocalFlatPhysDamageAffix : Affix {
	public LocalFlatPhysDamageAffix() {
		affixDataTable = AffixDataTables.LocalFlatPhysDamageAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
			valueSecond = Utilities.RandomDoubleInclusiveToInt(data.AffixMinSecond, data.AffixMaxSecond);
		}
	}

    public override string GetAffixTooltipText() {
		return $"Adds {(int)valueFirst} to {(int)valueSecond} Physical Damage";
	}
}

public class LocalPercentagePhysDamageAffix : Affix {
	public LocalPercentagePhysDamageAffix() {
		affixDataTable = AffixDataTables.LocalPercentagePhysDamageAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDouble(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Physical Damage";
	}
}

public class HealthAffix : Affix {
	public HealthAffix() {
		affixDataTable = AffixDataTables.FlatHealthAffixData;
		isLocal = false;
		statNameFirst = EStatName.FlatMaxLife;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Maximum Life";
	}
}

public class LocalFlatArmourAffix : Affix {
	public LocalFlatArmourAffix() {
		affixDataTable = AffixDataTables.LocalFlatArmourAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Armour";
	}
}

public class LocalPercentageArmourAffix : Affix {
	public LocalPercentageArmourAffix() {
		affixDataTable = AffixDataTables.LocalPercentageArmourAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDouble(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		
		return $"{Math.Round(valueFirst * 100, 0)}% increased Armour";
	}
}

public class LocalFlatEvasionAffix : Affix {
	public LocalFlatEvasionAffix() {
		affixDataTable = AffixDataTables.LocalFlatEvasionAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Evasion Rating";
	}
}

public class LocalPercentageEvasionAffix : Affix {
	public LocalPercentageEvasionAffix() {
		affixDataTable = AffixDataTables.LocalPercentageEvasionAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDouble(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Evasion Rating";
	}
}

public class LocalFlatEnergyShieldAffix : Affix {
	public LocalFlatEnergyShieldAffix() {
		affixDataTable = AffixDataTables.LocalFlatEnergyShieldAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Energy Shield";
	}
}

public class LocalPercentageEnergyShieldAffix : Affix {
	public LocalPercentageEnergyShieldAffix() {
		affixDataTable = AffixDataTables.LocalPercentageEnergyShieldAffixData;
		isLocal = true;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDouble(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Energy Shield";
	}
}


public class FireResistanceAffix : Affix {
	public FireResistanceAffix() {
		affixDataTable = AffixDataTables.FireResistanceAffixData;
		isLocal = false;
		statNameFirst = EStatName.FireResistance;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Fire Resistance";
	}
}

public class ColdResistanceAffix : Affix {
	public ColdResistanceAffix() {
		affixDataTable = AffixDataTables.ColdResistanceAffixData;
		isLocal = false;
		statNameFirst = EStatName.ColdResistance;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Cold Resistance";
	}
}

public class LightningResistanceAffix : Affix {
	public LightningResistanceAffix() {
		affixDataTable = AffixDataTables.LightningResistanceAffixData;
		isLocal = false;
		statNameFirst = EStatName.LightningResistance;
	}

	public override void RollAffixValue() {
		if (data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(data.AffixMinFirst, data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Lightning Resistance";
	}
}
