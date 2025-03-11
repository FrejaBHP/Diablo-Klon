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
	LocalFlatPhysDamage,
	LocalPercentagePhysDamage,
	FlatMaxLife,
	PercentageMaxLife,
	FlatArmour,
	PercentageArmour,
	FlatEvasion,
	PercentageEvasion,
	FlatEnergyShield,
	PercentageEnergyShield,
	FireResistance,
	ColdResistance,
	LightningResistance
}

public abstract class Affix {
	protected AffixData data;
	public AffixData Data {
		get { return data; }
	}

	protected double? valueFirst;
	public double? ValueFirst {
		get { return valueFirst; } 
	}

	protected double? valueSecond;
	public double? ValueSecond {
		get { return valueSecond; } 
	}

	public abstract void RollAffixTier(int itemLevel);

	public abstract void SetAffixTier(int tier);

	public abstract void RollAffixValue();

	public virtual string GetAffixName() {
		return data.Name;
	}
	public abstract string GetAffixTooltipText();
	
}

public class LocalFlatPhysDamageAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalFlatPhysDamageAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalFlatPhysDamageAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
			valueSecond = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinSecond, Data.AffixMaxSecond);
		}
	}

	public override string GetAffixTooltipText() {
		return $"Adds {(int)valueFirst} to {(int)valueSecond} Physical Damage";
	}
}

public class LocalPercentagePhysDamageAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalPercentagePhysDamageAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalPercentagePhysDamageAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{(int)valueFirst}% Increased Physical Damage";
	}
}

public class HealthAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.FlatHealthAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.FlatHealthAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Maximum Life";
	}
}

public class FlatArmourAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.FlatArmourAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.FlatArmourAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Armour";
	}
}

public class PercentageArmourAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.PercentageArmourAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.PercentageArmourAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{(int)valueFirst}% Increased Armour";
	}
}

public class FlatEvasionAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.FlatEvasionAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.FlatEvasionAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Evasion";
	}
}

public class PercentageEvasionAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.PercentageEvasionAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.PercentageEvasionAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{(int)valueFirst}% Increased Evasion";
	}
}

public class FlatEnergyShieldAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.FlatEnergyShieldAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.FlatEnergyShieldAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Energy Shield";
	}
}

public class PercentageEnergyShieldAffix : Affix {
    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.PercentageEnergyShieldAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.PercentageEnergyShieldAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{(int)valueFirst}% Increased Energy Shield";
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
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Fire Resistance";
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
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Cold Resistance";
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
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst}% to Lightning Resistance";
	}
}
