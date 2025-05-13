using Godot;
using System;
using System.Collections.Generic;

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

public static class EnemyDatabase {
    public static readonly PackedScene TestEnemyScene = GD.Load<PackedScene>("res://scenes/enemy_test.tscn");

    public static readonly EnemyWave TestWave = new([
        new EnemyWaveComponent(TestEnemyScene, 10)
    ]);
}
