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

	public abstract void RollAffixTier(int itemLevel);

	public abstract void SetAffixTier(int tier);

	public abstract void RollAffixValue();

	public virtual string GetAffixName() {
		return data.Name;
	}

	public abstract string GetAffixTooltipText();
}

public class LocalFlatPhysDamageAffix : Affix {
	public LocalFlatPhysDamageAffix() {
		isLocal = true;
	}

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
	public LocalPercentagePhysDamageAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalPercentagePhysDamageAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalPercentagePhysDamageAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDouble(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Physical Damage";
	}
}

public class HealthAffix : Affix {
	public HealthAffix() {
		isLocal = false;
		statNameFirst = EStatName.FlatMaxLife;
	}

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

public class LocalFlatArmourAffix : Affix {
	public LocalFlatArmourAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalFlatArmourAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalFlatArmourAffixData[tier];
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

public class LocalPercentageArmourAffix : Affix {
	public LocalPercentageArmourAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalPercentageArmourAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalPercentageArmourAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDouble(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		
		return $"{Math.Round(valueFirst * 100, 0)}% increased Armour";
	}
}

public class LocalFlatEvasionAffix : Affix {
	public LocalFlatEvasionAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalFlatEvasionAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalFlatEvasionAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDoubleInclusiveToInt(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"+{(int)valueFirst} to Evasion Rating";
	}
}

public class LocalPercentageEvasionAffix : Affix {
	public LocalPercentageEvasionAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalPercentageEvasionAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalPercentageEvasionAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDouble(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Evasion Rating";
	}
}

public class LocalFlatEnergyShieldAffix : Affix {
	public LocalFlatEnergyShieldAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalFlatEnergyShieldAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalFlatEnergyShieldAffixData[tier];
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

public class LocalPercentageEnergyShieldAffix : Affix {
	public LocalPercentageEnergyShieldAffix() {
		isLocal = true;
	}

    public override void RollAffixTier(int itemLevel) {
		List<AffixData> legalAffixData = AffixDataTables.LocalPercentageEnergyShieldAffixData.Where(a => a.MinimumLevel <= itemLevel).ToList();
        data = legalAffixData[Utilities.RNG.Next(legalAffixData.Count)];
    }

    public override void SetAffixTier(int tier) {
		data = AffixDataTables.LocalPercentageEnergyShieldAffixData[tier];
    }

	public override void RollAffixValue() {
		if (Data != null) {
			valueFirst = Utilities.RandomDouble(Data.AffixMinFirst, Data.AffixMaxFirst);
		}
	}

	public override string GetAffixTooltipText() {
		return $"{Math.Round(valueFirst * 100, 0)}% increased Energy Shield";
	}
}


public class FireResistanceAffix : Affix {
	public FireResistanceAffix() {
		isLocal = false;
		statNameFirst = EStatName.FireResistance;
	}

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
	public ColdResistanceAffix() {
		isLocal = false;
		statNameFirst = EStatName.ColdResistance;
	}

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
	public LightningResistanceAffix() {
		isLocal = false;
		statNameFirst = EStatName.LightningResistance;
	}

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
