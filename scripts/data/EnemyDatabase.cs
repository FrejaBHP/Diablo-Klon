using Godot;
using System;
using System.Collections.Generic;

public struct EnemyData(PackedScene scene, int cost) {
    public readonly PackedScene Scene = scene;
    public readonly int BudgetCost = cost;
}

public struct EnemyWaveComponent(EEnemyType enemyType, int count) {
    public EEnemyType EnemyType = enemyType;
    public int EnemyCount = count;
}

public class EnemyWave(List<EnemyWaveComponent> waveComponents) {
    public List<EnemyWaveComponent> WaveComponents = new(waveComponents);

    public int GetWaveSpawnCount() {
        int count = 0;

        foreach (EnemyWaveComponent waveComponent in WaveComponents) {
            count += waveComponent.EnemyCount;
        }

        return count;
    }

    public EnemyWave ShallowCopy() {
        return (EnemyWave)MemberwiseClone();
    }
}

public class MapEnemyWaves(List<EnemyWave> enemyWaves) {
    public readonly List<EnemyWave> EnemyWaves = new(enemyWaves);
}

public static class EnemyDatabase {
    public static readonly PackedScene TestEnemyScene = GD.Load<PackedScene>("res://scenes/enemy_test.tscn");
    public static readonly PackedScene TestEnemy2Scene = GD.Load<PackedScene>("res://scenes/enemy_test_2.tscn");
    public static readonly PackedScene TestEnemy3Scene = GD.Load<PackedScene>("res://scenes/enemy_test_3.tscn");

    public static readonly EnemyData TestEnemyData = new(TestEnemyScene, 2);
    public static readonly EnemyData TestEnemy2Data = new(TestEnemy2Scene, 2);
    public static readonly EnemyData TestEnemy3Data = new(TestEnemy3Scene, 2);

    public static readonly Dictionary<EEnemyType, EnemyData> EnemyDictionary = new() {
        { EEnemyType.TestEnemy, TestEnemyData },
        { EEnemyType.TestEnemy2, TestEnemy2Data },
        { EEnemyType.TestEnemy3, TestEnemy3Data }
    };


    // Spawn pools for random endless enemy spawns

    public static readonly WeightedList<EEnemyType> TestSurvivalSpawnPool = new([
        new WeightedListItem<EEnemyType>(EEnemyType.TestEnemy, 100),
        new WeightedListItem<EEnemyType>(EEnemyType.TestEnemy2, 25),
        new WeightedListItem<EEnemyType>(EEnemyType.TestEnemy2, 15),
    ], Utilities.RNG);



    // Waves for preset enemy spawns

    public static readonly EnemyWave TestWave = new([
        new EnemyWaveComponent(EEnemyType.TestEnemy, 5),
        new EnemyWaveComponent(EEnemyType.TestEnemy2, 2),
        new EnemyWaveComponent(EEnemyType.TestEnemy3, 1)
    ]);

    public static readonly MapEnemyWaves TestMapHorde = new([
        TestWave,
        TestWave,
        TestWave
    ]);
}




public class EnemyRarityTable {
    public double RareChance { get; set; }
    public double MagicChance { get; set; }
}

public static class EnemyRarityData {
    #region Tables
    private static EnemyRarityTable actOneFirstTable = new() {
        RareChance = 0.04,
        MagicChance = 0.1
    };

    private static EnemyRarityTable actOneSecondTable = new() {
        RareChance = 0.044,
        MagicChance = 0.11
    };

    private static EnemyRarityTable actTwoFirstTable = new() {
        RareChance = 0.048,
        MagicChance = 0.12
    };

    private static EnemyRarityTable actTwoSecondTable = new() {
        RareChance = 0.052,
        MagicChance = 0.13
    };

    private static EnemyRarityTable actThreeFirstTable = new() {
        RareChance = 0.056,
        MagicChance = 0.14
    };

    private static EnemyRarityTable actThreeSecondTable = new() {
        RareChance = 0.06,
        MagicChance = 0.15
    };
    #endregion

    public static EnemyRarityTable GetCurrentRarityTable(int level) {
        EnemyRarityTable table;

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

    public static readonly Dictionary<EStatName, double> MagicStatDictionary = new() {
        { EStatName.MoreMaxLife, 2.5 },
        { EStatName.MoreAttackSpeed, 1.15 },
        { EStatName.MoreCastSpeed, 1.15 },
        { EStatName.MoreAllDamage, 1.1 },
        { EStatName.IncreasedMovementSpeed, 0.1 },
    };

    public static readonly Dictionary<EStatName, double> RareStatDictionary = new() {
        { EStatName.MoreMaxLife, 4.5 },
        { EStatName.MoreAttackSpeed, 1.25 },
        { EStatName.MoreCastSpeed, 1.25 },
        { EStatName.MoreAllDamage, 1.15 },
        { EStatName.IncreasedMovementSpeed, 0.15 },
    };
}
