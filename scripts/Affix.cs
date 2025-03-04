using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum EAffixPosition {
	Prefix,
	Suffix
}

public enum EAffixFamily {
	None,
	FlatMaxLife,
	PercentageMaxLife,
	FireResistance,
	ColdResistance,
	LightningResistance
}

public abstract class Affix {
	protected AffixData data;
	public AffixData Data {
		get { return data; }
	}

	protected double value;
	public double Value {
		get { return value; } 
	}

	public abstract void RollAffixTier(int itemLevel);

	public abstract void SetAffixTier(int tier);

	public abstract void RollAffixValue();

	public virtual string GetAffixName() {
		return data.Name;
	}
	public abstract string GetAffixTooltipText();
	
}

public class HealthAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.HealthAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.HealthAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			value = Utilities.RandomDoubleInclusiveToInt(Data.AffixMin, Data.AffixMax);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)value} to Maximum Life";
	}
}

public class FireResistanceAffix : Affix {
	public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.FireResistanceAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.FireResistanceAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			value = Utilities.RandomDoubleInclusiveToInt(Data.AffixMin, Data.AffixMax);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)value}% to Fire Resistance";
	}
}

public class ColdResistanceAffix : Affix {
	public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.ColdResistanceAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.ColdResistanceAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			value = Utilities.RandomDoubleInclusiveToInt(Data.AffixMin, Data.AffixMax);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)value}% to Cold Resistance";
	}
}

public class LightningResistanceAffix : Affix {
	public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LightningResistanceAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LightningResistanceAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			value = Utilities.RandomDoubleInclusiveToInt(Data.AffixMin, Data.AffixMax);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)value}% to Lightning Resistance";
	}
}
