using Godot;
using System;

public static class Utilities {
	public readonly static Random RNG = new Random();
	
	public static int RandomDoubleInclusiveToInt(double minimum, double maximum) {
		return (int)(RNG.NextDouble() * (maximum + 1 - minimum) + minimum);
	}
}
