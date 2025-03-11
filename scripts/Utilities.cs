using Godot;
using System;

public static class Utilities {
	public readonly static Random RNG = new Random();
	
	public static int RandomDoubleInclusiveToInt(double? minimum, double? maximum) {
		if (minimum != null && maximum != null) {
			return (int)(RNG.NextDouble() * (maximum + 1 - minimum) + minimum);
		}
		else {
			return 0;
		}
	}

	public static bool HasAnyFlags(this Enum input, Enum comparison) {
        return (Convert.ToInt32(input) & Convert.ToInt32(comparison)) != 0;
    }
}
