using Godot;
using System;

public static class Utilities {
	private static readonly PackedScene damageTextScene = GD.Load<PackedScene>("res://scenes/damage_text.tscn");

	public readonly static Random RNG = new Random();
	
	public static int RandomDoubleInclusiveToInt(double minimum, double maximum) {
		return (int)(RNG.NextDouble() * (maximum + 1 - minimum) + minimum);
	}

	public static double RandomDouble(double minimum, double maximum) {
		return RNG.NextDouble() * (maximum - minimum) + minimum;
	}

	public static bool HasAnyFlags(this Enum input, Enum comparison) {
        return (Convert.ToInt32(input) & Convert.ToInt32(comparison)) != 0;
    }

	public static DamageText CreateDamageNumber(string text) {
		DamageText damageLabel = damageTextScene.Instantiate<DamageText>();
		damageLabel.Text = text;

		return damageLabel;
	}
}
