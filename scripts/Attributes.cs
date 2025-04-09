using Godot;
using System;

public partial class Stat(double baseValue, bool shouldRound) {
    public delegate void StatTotalChangedEventHandler(object sender, double newStatTotal);
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
        StatTotalChanged?.Invoke(this, sTotal);
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
}

public partial class Attributes {
    
}
