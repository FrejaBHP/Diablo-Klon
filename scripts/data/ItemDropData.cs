using Godot;
using System;

public class DropDataTable() {
    // How often Enemies of a rarity should drop an item
    public double NormalItemChance { get; set; } = 0.1;
    public double MagicItemChance { get; set; } = 0.35;
    public double RareItemChance { get; set; } = 1;
    public double UniqueItemChance { get; set; } = 5;

    // Chances of Enemies of a rarity succeeding on rolling an item of rarity
    public double NormalUniqueChance { get; set; } = 0.0005;
    public double NormalRareChance { get; set; }
    public double NormalMagicChance { get; set; }

    public double MagicUniqueChance { get; set; } = 0.001;
    public double MagicRareChance { get; set; }
    public double MagicMagicChance { get; set; }

    public double RareUniqueChance { get; set; } = 0.002;
    public double RareRareChance { get; set; }
    public double RareMagicChance { get; set; }

    public double UniqueUniqueChance { get; set; } = 0.01;
    public double UniqueRareChance { get; set; }
    public double UniqueMagicChance { get; set; } = 1;
}

public static class ItemDropData {
    #region Tables
    private readonly static DropDataTable actOneFirstTable = new() {
        NormalRareChance = 0.03,
        NormalMagicChance = 0.1,

        MagicRareChance = 0.08,
        MagicMagicChance = 0.333,

        RareRareChance = 0.2,
        RareMagicChance = 0.6,

        UniqueRareChance = 0.4
    };

    private readonly static DropDataTable actOneSecondTable = new() {
        NormalRareChance = 0.034,
        NormalMagicChance = 0.12,

        MagicRareChance = 0.09,
        MagicMagicChance = 0.367,

        RareRareChance = 0.23,
        RareMagicChance = 0.64,

        UniqueRareChance = 0.45
    };

    private readonly static DropDataTable actTwoFirstTable = new() {
        NormalRareChance = 0.038,
        NormalMagicChance = 0.14,

        MagicRareChance = 0.1,
        MagicMagicChance = 0.4,

        RareRareChance = 0.26,
        RareMagicChance = 0.68,

        UniqueRareChance = 0.5
    };

    private readonly static DropDataTable actTwoSecondTable = new() {
        NormalRareChance = 0.042,
        NormalMagicChance = 0.16,

        MagicRareChance = 0.11,
        MagicMagicChance = 0.433,

        RareRareChance = 0.29,
        RareMagicChance = 0.72,

        UniqueRareChance = 0.55
    };

    private readonly static DropDataTable actThreeFirstTable = new() {
        NormalRareChance = 0.046,
        NormalMagicChance = 0.18,

        MagicRareChance = 0.12,
        MagicMagicChance = 0.467,

        RareRareChance = 0.32,
        RareMagicChance = 0.76,

        UniqueRareChance = 0.6
    };

    private readonly static DropDataTable actThreeSecondTable = new() {
        NormalRareChance = 0.05,
        NormalMagicChance = 0.2,

        MagicRareChance = 0.13,
        MagicMagicChance = 0.5,

        RareRareChance = 0.35,
        RareMagicChance = 0.8,

        UniqueRareChance = 0.65
    };
    #endregion

    public static DropDataTable GetCurrentDropDataTable(int level) {
        DropDataTable table;

        if (level >= 25) {
            table = actThreeSecondTable;
        }
        else if (level >= 20) {
            table = actThreeFirstTable;
        }
        else if (level >= 15) {
            table = actTwoSecondTable;
        }
        else if (level >= 10) {
            table = actTwoFirstTable;
        }
        else if (level >= 5) {
            table = actOneSecondTable;
        }
        else {
            table = actOneFirstTable;
        }

        return table;
    }
}
