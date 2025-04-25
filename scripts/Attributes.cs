using Godot;
using System;

public partial class Stat(double baseValue, bool shouldRound) {
    public delegate void StatTotalChangedEventHandler(double newStatTotal);
    public event StatTotalChangedEventHandler StatTotalChanged;

    public readonly bool ShouldRoundToWholeNumber = shouldRound;

    private bool isMinCapped = false;
    private bool isMaxCapped = false;
    private double minValueCap = 0;
    private double maxValueCap = 0;

    private double sBase = baseValue;
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

    private double sMore = 0;
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

    private void CalculateStats() {
        sTotal = (sBase + sAdded) * (1 + sIncreased) * (1 + sMore);
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
        c.SMore = a.SMore * (1 + b.SMore);
        
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
        c.SMore = a.SMore / (1 + b.SMore);

        return c;
    }
}

public partial class DamageStat() {
    public delegate void DamageStatsTotalChangedEventHandler(double newMin, double newMax);
    public event DamageStatsTotalChangedEventHandler DamageStatsTotalChanged;

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
            CalculateStats();
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
            CalculateStats();
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
            CalculateStats();
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

    private double sMore = 0;
	public double SMore {
		get => sMore;
		set {
			sMore = value;
            CalculateStats();
		}
	}

    private double sMinTotal;
	public double SMinTotal {
		get => sMinTotal;
		set {
            double incValue;

            if (value < 0) {
                incValue = 0;
            }
            else {
                incValue = value;
            }

			if (ShouldRoundToWholeNumber) {
                sMinTotal = Math.Round(incValue, 0);
            }
            else {
                sMinTotal = incValue;
            }
		}
	}

    private double sMaxTotal;
	public double SMaxTotal {
		get => sMaxTotal;
		set {
            double incValue;

            if (value < 0) {
                incValue = 0;
            }
            else {
                incValue = value;
            }

			if (ShouldRoundToWholeNumber) {
                sMaxTotal = Math.Round(incValue, 0);
            }
            else {
                sMaxTotal = incValue;
            }
		}
	}

    private void CalculateStats() {
        sMinTotal = (sMinBase + sMinAdded) * (1 + sIncreased) * (1 + sMore);
        sMaxTotal = (sMaxBase + sMaxAdded) * (1 + sIncreased) * (1 + sMore);
        DamageStatsTotalChanged?.Invoke(sMinTotal, sMaxTotal);
    }

    public static DamageStat operator +(DamageStat a, DamageStat b) {
        DamageStat c = new DamageStat();

        c.sMinBase = a.sMinBase + b.sMinBase;
        c.sMaxBase = a.sMaxBase + b.sMaxBase;
        c.sMinAdded = a.sMinAdded + b.sMinAdded;
        c.sMaxAdded = a.sMaxAdded + b.sMaxAdded;
        c.sIncreased = a.sIncreased + b.sIncreased;
        c.SMore = a.SMore * (1 + b.SMore);

        /*
        GD.Print($"MinB: {a.sMinBase} + {b.sMinBase} = {c.sMinBase}");
        GD.Print($"MaxB: {a.sMaxBase} + {b.sMaxBase} = {c.sMaxBase}");
        GD.Print($"MinA: {a.sMinAdded} + {b.sMinAdded} = {c.sMinAdded}");
        GD.Print($"MaxA: {a.sMaxAdded} + {b.sMaxAdded} = {c.sMaxAdded}");
        GD.Print($"Inc: {a.sIncreased} + {b.sIncreased} = {c.sIncreased}");
        GD.Print($"More: {a.sMore} + {b.sMore} = {c.sMore}");
        GD.Print($"Total: {c.sMinTotal} - {c.sMaxTotal}");
        */
        
        return c;
    }

    public static DamageStat operator -(DamageStat a, DamageStat b) {
        DamageStat c = new DamageStat();

        c.sMinBase = a.sMinBase - b.sMinBase;
        c.sMaxBase = a.sMaxBase - b.sMaxBase;
        c.sMinAdded = a.sMinAdded - b.sMinAdded;
        c.sMaxAdded = a.sMaxAdded - b.sMaxAdded;
        c.sIncreased = a.sIncreased - b.sIncreased;
        c.SMore = a.SMore / (1 + b.SMore);
        
        return c;
    }

    public override string ToString() {
        return $"MinBase: {SMinBase}\nMaxBase: {SMaxBase}\nMinAdded: {SMinAdded}\nMaxAdded: {sMaxAdded}\nInc: {SIncreased}\nMore: {SMore}";
    }
}

public partial class Attributes {
    
}
