using Godot;
using System;
using System.Globalization;

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

	// Udskift gerne wasBlocked med en enum, så der kan laves en switch over skriftfarve
	public static DamageText CreateDamageNumber(string text, bool wasBlocked) {
		DamageText damageLabel = damageTextScene.Instantiate<DamageText>();
		damageLabel.Text = text;

		if (wasBlocked) {
			damageLabel.Modulate = UILib.ColorGrey;
		}

		return damageLabel;
	}

	public static bool RollForChance(double chance) {
		if (chance == 1) {
			return true;
		}
		else if (chance == 0) {
			return false;
		}
		else {
			double roll = RNG.NextDouble();
			return chance >= roll;
		}
	}
}
