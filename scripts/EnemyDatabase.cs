using Godot;
using System;
using System.Collections.Generic;

public struct EnemyData(PackedScene scene, int cost) {
    public readonly PackedScene Scene = scene;
    public readonly int BudgetCost = cost;
}

public struct EnemyWaveComponent(PackedScene enemyScene, int count) {
    public PackedScene EnemyScene = enemyScene;
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
        EnemyWave copy = (EnemyWave)MemberwiseClone();

        return copy;
    }
}

public class MapEnemyWaves(List<EnemyWave> enemyWaves) {
    public readonly List<EnemyWave> EnemyWaves = new(enemyWaves);
}

public static class EnemyDatabase {
    public static readonly PackedScene TestEnemyScene = GD.Load<PackedScene>("res://scenes/enemy_test.tscn");
    public static readonly PackedScene TestEnemy2Scene = GD.Load<PackedScene>("res://scenes/enemy_test_2.tscn");

    public static readonly EnemyData TestEnemyData = new(TestEnemyScene, 2);
    public static readonly EnemyData TestEnemy2Data = new(TestEnemy2Scene, 2);

    public static readonly Dictionary<EEnemyType, EnemyData> EnemyDictionary = new() {
        { EEnemyType.TestEnemy, TestEnemyData },
        { EEnemyType.TestEnemy2, TestEnemy2Data }
    };

    public static readonly WeightedList<EEnemyType> TestSurvivalSpawnPool = new([
        new WeightedListItem<EEnemyType>(EEnemyType.TestEnemy, 100),
        new WeightedListItem<EEnemyType>(EEnemyType.TestEnemy2, 10),
    ], Utilities.RNG);




    public static readonly EnemyWave TestWave = new([
        new EnemyWaveComponent(TestEnemyScene, 10)
    ]);

    public static readonly MapEnemyWaves TestMapHorde = new([
        TestWave,
        TestWave,
        TestWave
    ]);
}
