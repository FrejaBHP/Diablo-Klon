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

public partial class DamageStat() {
    public readonly bool ShouldRoundToWholeNumber = false;

    private double sMinBase = 0;
	public double SMinBase {
		get => sMinBase;
		set {
            if (ShouldRoundToWholeNumber) {
                sMinBase = Math.Round(value, 0);
            }
            else {
                sMinBase = value;
            }
		}
	}

    private double sMaxBase = 0;
	public double SMaxBase {
		get => sMaxBase;
		set {
            if (ShouldRoundToWholeNumber) {
                sMaxBase = Math.Round(value, 0);
            }
            else {
                sMaxBase = value;
            }
		}
	}

    private double sMinAdded = 0;
	public double SMinAdded {
		get => sMinAdded;
		set {
			if (ShouldRoundToWholeNumber) {
                sMinAdded = Math.Round(value, 0);
            }
            else {
                sMinAdded = value;
            }
		}
	}

    private double sMaxAdded = 0;
	public double SMaxAdded {
		get => sMaxAdded;
		set {
			if (ShouldRoundToWholeNumber) {
                sMaxAdded = Math.Round(value, 0);
            }
            else {
                sMaxAdded = value;
            }
		}
	}

    private double sIncreased = 0;
	public double SIncreased {
		get => sIncreased;
		set {
			sIncreased = value;
		}
	}

    private double sMore = 1;
	public double SMore {
		get => sMore;
		set {
			sMore = value;
		}
	}

    public void SetAddedIncreasedMore(double minAdded, double maxAdded, double increased = 0, double more = 1) {
        sMinAdded = minAdded;
        sMaxAdded = maxAdded;
        sIncreased = increased;
        sMore = more;
    }

    public void CalculateTotal(out double totalMin, out double totalMax) {
        totalMin = (sMinBase + sMinAdded) * (1 + sIncreased) * sMore;
        totalMax = (sMaxBase + sMaxAdded) * (1 + sIncreased) * sMore;

        if (totalMin < 0) {
            totalMin = 0;
        }

        if (totalMax < 0) {
            totalMax = 0;
        }

        if (ShouldRoundToWholeNumber) {
            totalMin = Math.Round(totalMin, 0);
            totalMax = Math.Round(totalMax, 0);
        }
    }

    public void CalculateTotalWithBase(double baseMin, double baseMax, double multiplier, out double totalMin, out double totalMax) {
        totalMin = ((baseMin * multiplier) + sMinAdded) * (1 + sIncreased) * sMore;
        totalMax = ((baseMax * multiplier) + sMaxAdded) * (1 + sIncreased) * sMore;

        if (totalMin < 0) {
            totalMin = 0;
        }

        if (totalMax < 0) {
            totalMax = 0;
        }

        if (ShouldRoundToWholeNumber) {
            totalMin = Math.Round(totalMin, 0);
            totalMax = Math.Round(totalMax, 0);
        }
    }

    public DamageStat ShallowCopy() {
        return (DamageStat)MemberwiseClone();
    }

    public static DamageStat operator +(DamageStat a, DamageStat b) {
        DamageStat c = new DamageStat();

        c.sMinBase = a.sMinBase + b.sMinBase;
        c.sMaxBase = a.sMaxBase + b.sMaxBase;
        c.sMinAdded = a.sMinAdded + b.sMinAdded;
        c.sMaxAdded = a.sMaxAdded + b.sMaxAdded;
        c.sIncreased = a.sIncreased + b.sIncreased;
        c.SMore = a.SMore * b.SMore;
        
        return c;
    }

    public static DamageStat operator -(DamageStat a, DamageStat b) {
        DamageStat c = new DamageStat();

        c.sMinBase = a.sMinBase - b.sMinBase;
        c.sMaxBase = a.sMaxBase - b.sMaxBase;
        c.sMinAdded = a.sMinAdded - b.sMinAdded;
        c.sMaxAdded = a.sMaxAdded - b.sMaxAdded;
        c.sIncreased = a.sIncreased - b.sIncreased;
        c.SMore = a.SMore / b.SMore;
        
        return c;
    }

    public override string ToString() {
        return $"Base: {SMinBase} - {sMaxBase}\nAdded: {SMinAdded} - {sMaxAdded}\nInc: {SIncreased}% / More: {SMore}%";
    }
}
