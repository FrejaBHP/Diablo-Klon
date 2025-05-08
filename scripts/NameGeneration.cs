using Godot;
using System;
using System.Collections.Generic;

public static class NameGeneration {
	public static readonly List<string> ItemNamePrefixes = new List<string>() {
		"Agony",
		"Apocalypse",
		"Armageddon",
		"Beast",
		"Behemoth",
		"Blight",
		"Blood",
		"Bramble",
		"Brimstone",
		"Brood",
		"Dread",
		"Fate",
		"Hate",
		"Havoc",
		"Miracle",
		"Rage"
	};
	
	public static readonly List<string> ItemNameSuffixes = new List<string>() {
		"Bane",
		"Call",
		"Guard",
		"Roar",
		"Sanctuary",
		"Song",
		"Star",
		"Watch"
	};

	public static string GenerateItemName() {
		string prefix = ItemNamePrefixes[Utilities.RNG.Next(ItemNamePrefixes.Count)];
		string suffix = ItemNameSuffixes[Utilities.RNG.Next(ItemNameSuffixes.Count)];

		return prefix + " " + suffix;
	}
}
