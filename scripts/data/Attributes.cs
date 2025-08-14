using Godot;
using System;

public partial class Stat {
    public delegate void StatTotalChangedEventHandler(double newStatTotal);
    public event StatTotalChangedEventHandler StatTotalChanged;

    public readonly bool ShouldRoundToWholeNumber;

    private bool isMinCapped = false;
    private bool isMaxCapped = false;
    private double minValueCap = 0;
    private double maxValueCap = 0;

    private Stat() {}

    public Stat(double baseValue, bool shouldRound) : this() {
        ShouldRoundToWholeNumber = shouldRound;
        SBase = baseValue;
    }

    public Stat(double baseValue, bool shouldRound, double minCap) : this(baseValue, shouldRound) {
        SetMinCap(minCap);
    }

    public Stat(double baseValue, bool shouldRound, double minCap, double maxCap) : this(baseValue, shouldRound) {
        SetMinCap(minCap);
        SetMaxCap(maxCap);
    }

    private double sBase;
    public double SBase {
		get => sBase;
		set {
            if (ShouldRoundToWholeNumber) {
                sBase = Math.Round(value, 0);
            }
            else {
                sBase = value;
            }
            CalculateStats();
		}
	}

    private double sAdded = 0;
	public double SAdded {
		get => sAdded;
		set {
			if (ShouldRoundToWholeNumber) {
                sAdded = Math.Round(value, 0);
            }
            else {
                sAdded = value;
            }
            CalculateStats();
		}
	}

    private double sIncreased = 0;
	public double SIncreased {
		get => sIncreased;
		set {
			sIncreased = value;
            CalculateStats();
		}
	}

    private double sMore = 1;
	public double SMore {
		get => sMore;
		set {
			sMore = value;
            CalculateStats();
		}
	}

    private double sTotal;
	public double STotal {
		get => sTotal;
		set {
            double incValue;

            if (isMinCapped && value < minValueCap) {
                incValue = minValueCap;
            }
            else if (isMaxCapped && value > maxValueCap) {
                incValue = maxValueCap;
            }
            else {
                incValue = value;
            }

			if (ShouldRoundToWholeNumber) {
                sTotal = Math.Round(incValue, 0);
            }
            else {
                sTotal = incValue;
            }
		}
	}

    public void SetAddedIncreasedMore(double added, double increased = 0, double more = 1) {
        sAdded = added;
        sIncreased = increased;
        sMore = more;
        CalculateStats();
    }

    private void CalculateStats() {
        sTotal = (sBase + sAdded) * (1 + sIncreased) * sMore;
        StatTotalChanged?.Invoke(sTotal);
    }

    public void SetMinCap(double min) {
        minValueCap = min;
        isMinCapped = true;
    }

    public void RemoveMinCap() {
        isMinCapped = false;
    }

    public void SetMaxCap(double max) {
        maxValueCap = max;
        isMaxCapped = true;
    }

    public void RemoveMaxCap() {
        isMaxCapped = false;
    }

    public Stat ShallowCopy() {
        return (Stat)MemberwiseClone();
    }

    public static Stat operator +(Stat a, Stat b) {
        Stat c = new Stat(a.sBase + b.sBase, a.ShouldRoundToWholeNumber);

        if (a.isMinCapped) {
            c.isMinCapped = true;
            c.minValueCap = a.minValueCap;
        }

        if (a.isMaxCapped) {
            c.isMaxCapped = true;
            c.maxValueCap = a.maxValueCap;
        }

        c.sAdded = a.sAdded + b.sAdded;
        c.sIncreased = a.sIncreased + b.sIncreased;
        c.SMore = a.SMore * b.SMore;
        
        return c;
    }

    public static Stat operator -(Stat a, Stat b) {
        Stat c = new Stat(a.sBase - b.sBase, a.ShouldRoundToWholeNumber | b.ShouldRoundToWholeNumber);

        if (a.isMinCapped) {
            c.isMinCapped = true;
            c.minValueCap = a.minValueCap;
        }

        if (a.isMaxCapped) {
            c.isMaxCapped = true;
            c.maxValueCap = a.maxValueCap;
        }

        c.sAdded = a.sAdded - b.sAdded;
        c.sIncreased = a.sIncreased - b.sIncreased;
        c.SMore = a.SMore / b.SMore;

        return c;
    }

    public override string ToString() {
        return $"Base: {SBase}\nAdded: {SAdded}\nInc: {SIncreased}% / More: {SMore}%";
    }
}

public partial class DamageTypeStat() {
	public double SMinBase { get; set; } = 0;
	public double SMaxBase { get; set; } = 0;
    public double SMinAdded { get; set; } = 0;
	public double SMaxAdded { get; set; } = 0;
	public double SAttackMinAdded { get; set; } = 0;
	public double SAttackMaxAdded { get; set; } = 0;
	public double SSpellMinAdded { get; set; } = 0;
	public double SSpellMaxAdded { get; set; } = 0;
	public double SIncreased { get; set; } = 0;
    public double SMore { get; set; } = 1;

    public void SetAddedIncreasedMore(double minAdded, double maxAdded, double minAtkAdded, double maxAtkAdded, double minSpeAdded, double maxSpeAdded, double increased = 0, double more = 1) {
        SMinAdded = minAdded;
        SMaxAdded = maxAdded;
        SAttackMinAdded = minAtkAdded;
        SAttackMaxAdded = maxAtkAdded;
        SSpellMinAdded = minSpeAdded;
        SSpellMaxAdded = maxSpeAdded;
        SIncreased = increased;
        SMore = more;
    }

    public void CalculateTotal(out double totalMin, out double totalMax) {
        totalMin = (SMinBase + SMinAdded + SAttackMinAdded) * (1 + SIncreased) * SMore;
        totalMax = (SMaxBase + SMaxAdded + SAttackMaxAdded) * (1 + SIncreased) * SMore;

        if (totalMin < 0) {
            totalMin = 0;
        }

        if (totalMax < 0) {
            totalMax = 0;
        }
    }

    public void CalculateTotalWithBase(double baseMin, double baseMax, double multiplier, out double totalMin, out double totalMax) {
        totalMin = ((baseMin * multiplier) + SMinAdded + SAttackMinAdded) * (1 + SIncreased) * SMore;
        totalMax = ((baseMax * multiplier) + SMaxAdded + SAttackMaxAdded) * (1 + SIncreased) * SMore;

        if (totalMin < 0) {
            totalMin = 0;
        }

        if (totalMax < 0) {
            totalMax = 0;
        }
    }

    public bool IsNonZero() {
        CalculateTotal(out double min, out double max);

        if (min == 0 && max == 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public DamageTypeStat ShallowCopy() {
        return (DamageTypeStat)MemberwiseClone();
    }

    public static DamageTypeStat operator +(DamageTypeStat a, DamageTypeStat b) {
        DamageTypeStat c = new DamageTypeStat();

        c.SMinBase = a.SMinBase + b.SMinBase;
        c.SMaxBase = a.SMaxBase + b.SMaxBase;
        c.SMinAdded = a.SMinAdded + b.SMinAdded;
        c.SMaxAdded = a.SMaxAdded + b.SMaxAdded;
        c.SAttackMinAdded = a.SAttackMinAdded + b.SAttackMinAdded;
        c.SAttackMaxAdded = a.SAttackMaxAdded + b.SAttackMaxAdded;
        c.SSpellMinAdded = a.SSpellMinAdded + b.SSpellMinAdded;
        c.SSpellMaxAdded = a.SSpellMaxAdded + b.SSpellMaxAdded;
        c.SIncreased = a.SIncreased + b.SIncreased;
        c.SMore = a.SMore * b.SMore;
        
        return c;
    }

    public static DamageTypeStat operator -(DamageTypeStat a, DamageTypeStat b) {
        DamageTypeStat c = new DamageTypeStat();

        c.SMinBase = a.SMinBase - b.SMinBase;
        c.SMaxBase = a.SMaxBase - b.SMaxBase;
        c.SMinAdded = a.SMinAdded - b.SMinAdded;
        c.SMaxAdded = a.SMaxAdded - b.SMaxAdded;
        c.SAttackMinAdded = a.SAttackMinAdded - b.SAttackMinAdded;
        c.SAttackMaxAdded = a.SAttackMaxAdded - b.SAttackMaxAdded;
        c.SSpellMinAdded = a.SSpellMinAdded - b.SSpellMinAdded;
        c.SSpellMaxAdded = a.SSpellMaxAdded - b.SSpellMaxAdded;
        c.SIncreased = a.SIncreased - b.SIncreased;
        c.SMore = a.SMore / b.SMore;
        
        return c;
    }

    public override string ToString() {
        return $"Base: {SMinBase} - {SMaxBase}\nAdded: {SAttackMinAdded} - {SAttackMaxAdded}\nInc: {SIncreased}% / More: {SMore}%";
    }
}




public partial class StatusEffectStats() {
    private double sBaseChance = 0;
	public double SBaseChance {
		get => sBaseChance;
		set {
            sBaseChance = value;
		}
	}

    private double sAddedChance = 0;
	public double SAddedChance {
		get => sAddedChance;
		set {
            sAddedChance = value;
		}
	}

    private double sIncreasedChance = 0;
	public double SIncreasedChance {
		get => sIncreasedChance;
		set {
			sIncreasedChance = value;
		}
	}

    private double sMoreChance = 1;
	public double SMoreChance {
		get => sMoreChance;
		set {
			sMoreChance = value;
		}
	}

    public void SetAddedIncreasedMoreChance(double added, double increased = 0, double more = 1) {
        sAddedChance = added;
        sIncreasedChance = increased;
        sMoreChance = more;
    }

    public double CalculateTotalChance() {
        double totalChance = (sBaseChance + sAddedChance) * (1 + sIncreasedChance) * sMoreChance;

        if (totalChance < 0) {
            totalChance = 0;
        }
        else if (totalChance > 1) {
            totalChance = 1;
        }

        return totalChance;
    }

    public bool RollForProc() {
        double chance = CalculateTotalChance();
        return Utilities.RollForChance(chance);
    }

    public StatusEffectStats ShallowCopy() {
        return (StatusEffectStats)MemberwiseClone();
    }

    private double sIncreasedDuration = 0;
	public double SIncreasedDuration {
		get => sIncreasedDuration;
		set {
			sIncreasedDuration = value;
		}
	}

    private double sMoreDuration = 1;
	public double SMoreDuration {
		get => sMoreDuration;
		set {
			sMoreDuration = value;
		}
	}

    private double sFasterTicking = 0;
	public double SFasterTicking {
		get => sFasterTicking;
		set {
			sFasterTicking = value;
		}
	}

    public double CalculateDurationModifier() {
        return (1 + sIncreasedDuration) * sMoreDuration / (1 + sFasterTicking);
    }

    public static StatusEffectStats operator +(StatusEffectStats a, StatusEffectStats b) {
        StatusEffectStats c = new StatusEffectStats();

        c.sBaseChance = a.sBaseChance + b.sBaseChance;
        c.sAddedChance = a.sAddedChance + b.sAddedChance;
        c.sIncreasedChance = a.sIncreasedChance + b.sIncreasedChance;
        c.sMoreChance = a.SMoreChance * b.sMoreChance;
        c.sFasterTicking = a.sFasterTicking + b.sFasterTicking;

        c.sIncreasedDuration = a.sIncreasedDuration + b.sIncreasedDuration;
        c.sMoreDuration = a.sMoreDuration * b.sMoreDuration;
        
        return c;
    }

    public static StatusEffectStats operator -(StatusEffectStats a, StatusEffectStats b) {
        StatusEffectStats c = new StatusEffectStats();

        c.sBaseChance = a.sBaseChance - b.sBaseChance;
        c.sAddedChance = a.sAddedChance - b.sAddedChance;
        c.sIncreasedChance = a.sIncreasedChance - b.sIncreasedChance;
        c.sMoreChance = a.sMoreChance / b.sMoreChance;
        c.sFasterTicking = a.sFasterTicking - b.sFasterTicking;

        c.sIncreasedDuration = a.sIncreasedDuration - b.sIncreasedDuration;
        c.sMoreDuration = a.sMoreDuration / b.sMoreDuration;
        
        return c;
    }

    public override string ToString() {
        return $"BaseC: {SBaseChance}\nAddedC: {sAddedChance}\nIncC: {SIncreasedChance}% / MoreC: {SMoreChance}%";
    }
}
