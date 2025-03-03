using Godot;
using System;
using System.Collections.Generic;

public static class NameGeneration {
	public static List<string> ItemNamePrefixes = new List<string>();
	public static List<string> ItemNameSuffixes = new List<string>();

	static NameGeneration() {
		ItemNamePrefixes.Add("Agony");
		ItemNamePrefixes.Add("Apocalypse");
		ItemNamePrefixes.Add("Armageddon");
		ItemNamePrefixes.Add("Beast");
		ItemNamePrefixes.Add("Behemoth");
		ItemNamePrefixes.Add("Blight");
		ItemNamePrefixes.Add("Blood");
		ItemNamePrefixes.Add("Bramble");
		ItemNamePrefixes.Add("Brimstone");
		ItemNamePrefixes.Add("Brood");
		ItemNamePrefixes.Add("Dread");
		ItemNamePrefixes.Add("Fate");
		ItemNamePrefixes.Add("Hate");
		ItemNamePrefixes.Add("Havoc");
		ItemNamePrefixes.Add("Miracle");
		ItemNamePrefixes.Add("Rage");

		ItemNameSuffixes.Add("Bane");
		ItemNameSuffixes.Add("Call");
		ItemNameSuffixes.Add("Guard");
		ItemNameSuffixes.Add("Roar");
		ItemNameSuffixes.Add("Sanctuary");
		ItemNameSuffixes.Add("Song");
		ItemNameSuffixes.Add("Star");
		ItemNameSuffixes.Add("Watch");
	}

	public static string GenerateItemName() {
		string prefix = ItemNamePrefixes[Utilities.RNG.Next(ItemNamePrefixes.Count)];
		string suffix = ItemNameSuffixes[Utilities.RNG.Next(ItemNameSuffixes.Count)];

		return prefix + " " + suffix;
	}
}
