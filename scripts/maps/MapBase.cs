using Godot;
using System;

public partial class MapBase : Node3D {
    [Signal]
    public delegate void MapReadyEventHandler();

    [Signal]
    public delegate void MapFinishedEventHandler();

    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }
    public Marker3D PlayerSpawnMarker { get; protected set; }
    public Timer SecondTimer { get; protected set; }
    public Timer ObjectiveTimer { get; protected set; }

    public EMapObjective MapObjective = EMapObjective.None;

    // For the future when item level will be set and matter
    // Maybe I'll even add enemy scaling
    public int AreaLevel = 1;
    public int GoldReward { get; protected set; } = 25;
    public int ExpReward { get; protected set; } = 5;

    public WeightedList<EEnemyType> ActiveSpawnPool = null;
    public int EnemySpawnCeiling { get; protected set; } = 0;
    public int EnemySpawnDensity { get; protected set; } = 0;
    protected double elapsedTimeDensityMod = 0;
    public int EnemySpawnBudget = 0; // Intended for usage in terms of spawning non-Normal enemies and enemies that are considered stronger than others

    public MapEnemyWaves ActiveWaveList = null;
	public EnemyWave ActiveWave = null;
	public int ActiveWaveNumber = 0;
	public int EnemiesToSpawn = 0;
	public int ActiveEnemies = 0;

    protected bool hasMapStarted = false;
    protected bool hasMapFinished = false;

    protected double spawnInterval = 5;
    protected double spawnTimer = 0;

    protected int objectiveLength = 0;
    protected int secondsPassed = 0;

    public override void _Ready() {
        NavRegion = GetNode<NavigationRegion3D>("NavigationRegion3D");
        EnemiesNode = GetNode<Node3D>("Enemies");
        PlayerSpawnMarker = GetNode<Marker3D>("PlayerSpawn");
        SecondTimer = GetNode<Timer>("SecondTimer");
        ObjectiveTimer = GetNode<Timer>("ObjectiveTimer");

        SetEnemySpawnPool(EnemyDatabase.TestSurvivalSpawnPool);
        SetMapWaveList(EnemyDatabase.TestMapHorde);
		SetEnemyWave(ActiveWaveList.EnemyWaves[0]);

        CallDeferred(MethodName.AddRegionToNav);
        CallDeferred(MethodName.OnMapReady);
    }

    public override void _PhysicsProcess(double delta) {
        if (IsObjectiveActive()) {
            if (MapObjective == EMapObjective.Survival) {
                if (spawnTimer <= 0) {
                    SpawnFromPool(GetAmountToSpawn(), false);
                    spawnTimer += spawnInterval;
                }
                else {
                    spawnTimer -= delta;
                }
            }

            if (ObjectiveTimer.TimeLeft > 10) {
                Game.Instance.PlayerActor.debugLabel.Text = $"{ObjectiveTimer.TimeLeft:F0} seconds left";
            }
            else {
                Game.Instance.PlayerActor.debugLabel.Text = $"{ObjectiveTimer.TimeLeft:F1} seconds left";
            }
        }
    }


    public void ClearEnemies() {
        foreach (Node enemy in EnemiesNode.GetChildren()) {
            enemy.QueueFree();
        }
    }

    public void AddRegionToNav() {
        NavigationServer3D.RegionSetMap(NavRegion.GetRid(), GetWorld3D().NavigationMap);
    }

    protected void OnMapReady() {
        EmitSignal(SignalName.MapReady);
    }

    public void StartObjective() {
        SetObjectiveTimerLength();
        Game.Instance.PlayerActor.debugLabel.Text = $"{Math.Round(ObjectiveTimer.WaitTime, 0)} seconds left";

        EnemySpawnCeiling = CalculateEnemySpawnCeiling(Game.Instance.CurrentAct, Game.Instance.CurrentArea, 1);
        EnemySpawnDensity = CalculateBaseEnemySpawnDensity(Game.Instance.CurrentAct, Game.Instance.CurrentArea, 1);
        ObjectiveTimer.Start();
        SecondTimer.Start();

        hasMapStarted = true;
    }

    protected double GetDensityTimeModifier() {
        // Assuming objective type is Survival!!! Need cases for others later as they're made
        const double modFloor = 0.33;
        // Should return anywhere between 0 and 0,67, where 0,67 is reached when 67% of the survival time has elapsed
        double ratio = Math.Clamp(1 - (ObjectiveTimer.TimeLeft / ObjectiveTimer.WaitTime), 0, 0.67);
        
        // At max time ratio, spawn density should be 1
        return modFloor + ratio;
    }

    protected void SetObjectiveTimerLength() {
        double waitTime;

        if (Game.Instance.CurrentAct < 2) {
            if (Game.Instance.CurrentArea > 5) {
                waitTime = 40 + 10 * Game.Instance.CurrentAct;
            }
            else {
                waitTime = 30 + 10 * Game.Instance.CurrentAct;
            }
        }
        else {
            waitTime = 60;
        }

        ObjectiveTimer.WaitTime = waitTime;
    }

    protected static int CalculateEnemySpawnCeiling(int act, int area, double mod) {
        const int baseCeiling = 15;
        int finalCeiling = baseCeiling + (act * 5);
        
        if (area > 8) {
            finalCeiling += 4;
        }
        else if (area > 5) {
            finalCeiling += 3;
        }
        else if (area > 2) {
            finalCeiling += 2;
        }

        finalCeiling = (int)(finalCeiling * mod);

        return finalCeiling;
    }

    protected static int CalculateBaseEnemySpawnDensity(int act, int area, double mod) {
        const int baseDensity = 8;
        int finalDensity = baseDensity + (act * 2);
        
        if (area > 8) {
            finalDensity += 2;
        }
        else if (area > 5) {
            finalDensity += 1;
        }
        else if (area > 2) {
            finalDensity += 1;
        }

        finalDensity = (int)(finalDensity * mod);

        return finalDensity;
    }

    protected static int GetTimeAdjustedSpawnDensity(int density, double timeMod) {
        return (int)(density * timeMod);
    }

    public void OnSecondPassed() {
        if (IsObjectiveActive()) {
            secondsPassed++;
        }
    }

    public void OnObjectiveTimerTimeout() {
        if (MapObjective == EMapObjective.Survival) {
            hasMapFinished = true;
            SecondTimer.Stop();
            EmitSignal(SignalName.MapFinished);
            ClearEnemies();
            Game.Instance.PlayerActor.debugLabel.Text = "";
        }
    }

    public void SetEnemySpawnPool(WeightedList<EEnemyType> spawnPool) {
        ActiveSpawnPool = spawnPool;
    }

    protected void SpawnFromPool(int amount, bool isGroup) {
        if (amount == 0) {
            return;
        }

        // If spawning as a group, all enemies are the same type (and should spawn close to each other, but not implemented yet)
        if (isGroup) {
            EEnemyType type = ActiveSpawnPool.GetRandomItem();
            for (int i = 0; i < amount; i++) {
                SpawnEnemy(type);
            }
        }
        else {
            for (int i = 0; i < amount; i++) {
                EEnemyType type = ActiveSpawnPool.GetRandomItem();
                SpawnEnemy(type);
            }
        }
    }

    protected void SpawnEnemy(EEnemyType type) {
        Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
        spawnPos.Y -= 0.5f;
        EnemyBase enemy = EnemyDatabase.EnemyDictionary[type].Scene.Instantiate<EnemyBase>();
        EnemiesNode.AddChild(enemy);
        enemy.GlobalPosition = spawnPos;

        EnemiesToSpawn--;
        ActiveEnemies++;
        enemy.EnemyDied += DecrementEnemyCount;
    }

    protected int GetAmountToSpawn() {
        if (ActiveEnemies > EnemySpawnCeiling) {
            return 0;
        }
        
        int enemiesToSpawn = 0;
        int potentialMaxEnemies = ActiveEnemies + GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier());

        if (potentialMaxEnemies > EnemySpawnCeiling) {
            enemiesToSpawn = potentialMaxEnemies - EnemySpawnCeiling;
        }
        else {
            enemiesToSpawn = GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier());
        }

        return enemiesToSpawn;
    }

    public void SetMapWaveList(MapEnemyWaves waves) {
		ActiveWaveList = waves;
	}

	protected void SetEnemyWave(EnemyWave wave) {
		ActiveWave = wave.ShallowCopy();
		EnemiesToSpawn = ActiveWave.GetWaveSpawnCount();
	}

	// Spawns all enemies from a wave at once
	public void SpawnWave() {
		foreach (EnemyWaveComponent comp in ActiveWave.WaveComponents) {
			for (int i = 0; i < comp.EnemyCount; i++) {
				Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
				spawnPos.Y -= 0.5f;
				EnemyBase enemy = comp.EnemyScene.Instantiate<EnemyBase>();
				EnemiesNode.AddChild(enemy);
				enemy.GlobalPosition = spawnPos;

				EnemiesToSpawn--;
				ActiveEnemies++;
				enemy.EnemyDied += DecrementEnemyCount;
			}
		}
    }

	public void DecrementEnemyCount() {
		ActiveEnemies--;

		if (ActiveEnemies == 0 && EnemiesToSpawn == 0) {
			if (ActiveWaveNumber < ActiveWaveList.EnemyWaves.Count - 1) {
				SetAndStartNextWave();
			}
			else {
				//OnMapCompletion();
                EmitSignal(SignalName.MapFinished);
				//TestSpawnMapTrans();
			}
		}
	}

	public void SetAndStartNextWave() {
		ActiveWaveNumber++;
		Game.Instance.PlayerActor.debugLabel.Text = $"Wave {ActiveWaveNumber + 1} / {ActiveWaveList.EnemyWaves.Count}";
		SetEnemyWave(ActiveWaveList.EnemyWaves[ActiveWaveNumber]);
		SpawnWave();
	}

    public bool IsObjectiveActive() {
        return hasMapStarted && !hasMapFinished;
    }
}
