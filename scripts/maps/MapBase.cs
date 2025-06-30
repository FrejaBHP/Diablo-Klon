using Godot;
using System;
using System.Collections.Generic;

public partial class MapBase : Node3D {
    protected static readonly PackedScene mapObjectiveHUDScene = GD.Load<PackedScene>("res://scenes/gui/hud_objective.tscn");

    [Signal]
    public delegate void MapReadyEventHandler();

    [Signal]
    public delegate void MapObjectiveStartedEventHandler(EMapObjective objectiveType);

    [Signal]
    public delegate void MapObjectiveFinishedEventHandler();

    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }
    public Marker3D PlayerSpawnMarker { get; protected set; }
    public Timer MapStartTimer { get; protected set; }
    public Timer ObjectiveTimer { get; protected set; }
    public CanvasLayer NameplateLayer { get; protected set; }

    public EMapType MapType;
    public EMapObjective MapObjective = EMapObjective.None;

    // For the future when item level will be set and matter
    // Maybe I'll even add enemy scaling
    public int LocalAreaLevel = 1;
    public int GoldReward { get; protected set; } = 25;
    public int ExpReward { get; protected set; } = 5;

    public int GoldRewardPool { get; protected set; } = 0;
    public List<Item> ItemRewardPool { get; protected set; } = new();

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

    protected MapObjectiveHUD objective;

    public override void _Ready() {
        NavRegion = GetNode<NavigationRegion3D>("NavigationRegion3D");
        EnemiesNode = GetNode<Node3D>("Enemies");
        PlayerSpawnMarker = GetNode<Marker3D>("PlayerSpawn");
        MapStartTimer = GetNode<Timer>("MapStartTimer");
        ObjectiveTimer = GetNode<Timer>("ObjectiveTimer");
        NameplateLayer = GetNode<CanvasLayer>("NameplateLayer");

        SetEnemySpawnPool(EnemyDatabase.TestSurvivalSpawnPool);
        SetMapWaveList(EnemyDatabase.TestMapHorde);
		SetEnemyWave(ActiveWaveList.EnemyWaves[0]);

        CallDeferred(MethodName.CalculateAndUpdateAreaLevel);
        CallDeferred(MethodName.AddRegionToNav);
        CallDeferred(MethodName.OnMapReady);

        if (MapObjective == EMapObjective.Survival) {
            GoldRewardPool += GoldReward;
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (IsObjectiveActive()) {
            if (MapObjective == EMapObjective.Survival) {
                spawnTimer -= delta;

                if (spawnTimer <= 0) {
                    spawnTimer += spawnInterval;
                    SpawnFromPool(GetAmountToSpawn(), false);
                    
                    Run.Instance.PlayerActor.PlayerHUD.PlayerRightHUD.UpdateEnemyDebugLabel(
                        GetDensityTimeModifier(), 
                        GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier())
                    );
                }
            }

            if (ObjectiveTimer.TimeLeft > 10) {
                Run.Instance.PlayerActor.PlayerHUD.PlayerUpperHUD.ObjTimeLabel.Text = $"{ObjectiveTimer.TimeLeft:F0}";
            }
            else {
                Run.Instance.PlayerActor.PlayerHUD.PlayerUpperHUD.ObjTimeLabel.Text = $"{ObjectiveTimer.TimeLeft:F1}";
            }
        }
    }

    public void CalculateAndUpdateAreaLevel() {
        LocalAreaLevel = Run.Instance.AreaLevel + Run.Instance.AreaLevelMod;
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

        if (MapType == EMapType.Intermission) {
            EmitSignal(SignalName.MapObjectiveFinished);
        }
        else if (MapObjective == EMapObjective.Survival) {
			CreateObjectiveGUI();

			MapStartTimer.WaitTime = 2;
			MapStartTimer.Start();
        }
    }

    public void CreateObjectiveGUI() {
        objective = mapObjectiveHUDScene.Instantiate<MapObjectiveHUD>();
		Run.Instance.PlayerActor.PlayerHUD.PlayerRightHUD.ObjectiveContainer.AddChild(objective);
        objective.SetObjectiveText(MapObjective.ToString());
        objective.SetGoldReward(GoldRewardPool);
        objective.SetItemReward(ItemRewardPool.Count);
        objective.Visible = true;
    }

    public void OnMapStartTimerTimeout() {
        StartObjective();
    }

    public void StartObjective() {
        SetObjectiveTimerLength();
        // Timere skal flyttes til Run eller lign
        Run.Instance.PlayerActor.PlayerHUD.PlayerUpperHUD.ObjTimeLabel.Visible = true;
        Run.Instance.PlayerActor.PlayerHUD.PlayerUpperHUD.ObjTimeLabel.Text = $"{Math.Round(ObjectiveTimer.WaitTime, 0)}";

        if (MapObjective == EMapObjective.Survival) {
            EnemySpawnCeiling = CalculateEnemySpawnCeiling(Run.Instance.CurrentAct, Run.Instance.CurrentArea, 1);
            EnemySpawnDensity = CalculateBaseEnemySpawnDensity(Run.Instance.CurrentAct, Run.Instance.CurrentArea, 1);
            Run.Instance.PlayerActor.PlayerHUD.PlayerRightHUD.EnemyDebugLabel.Visible = true;
        }

        ObjectiveTimer.Start();

        hasMapStarted = true;
    }

    protected double GetDensityTimeModifier() {
        // Assuming objective type is Survival!!! Need cases for others later as they're made
        const double modFloor = 0.33;
        // Should return anywhere between 0 and 0,67, where 0,67 is reached when 67% of the survival time has elapsed
        double ratio = Math.Clamp(1 - (ObjectiveTimer.TimeLeft / ObjectiveTimer.WaitTime), 0, 0.67);
        
        // At max time ratio, spawn density should be 1
        return modFloor + ratio + 0.001;
    }

    protected void SetObjectiveTimerLength() {
        double waitTime;

        if (Run.Instance.CurrentAct < 2) {
            if (Run.Instance.CurrentArea > 5) {
                waitTime = 40 + 10 * Run.Instance.CurrentAct;
            }
            else {
                waitTime = 30 + 10 * Run.Instance.CurrentAct;
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

    public void OnObjectiveTimerTimeout() {
        if (MapObjective == EMapObjective.Survival) {
            hasMapFinished = true;
            EmitSignal(SignalName.MapObjectiveFinished);
            ClearEnemies();
            Run.Instance.PlayerActor.PlayerHUD.PlayerUpperHUD.ObjTimeLabel.Visible = false;

            if (objective != null) {
                Run.Instance.PlayerActor.PlayerHUD.PlayerRightHUD.ObjectiveContainer.RemoveChild(objective);
                objective.QueueFree();

                Run.Instance.PlayerActor.PlayerHUD.PlayerRightHUD.EnemyDebugLabel.Visible = false;
            }

            if (GoldRewardPool > 0) {
			    Run.Instance.DropGold(GoldRewardPool, PlayerSpawnMarker.GlobalPosition, false);
		    }

            if (ExpReward > 0) {
                Run.Instance.AwardExperience(ExpReward);
            }

            if (ItemRewardPool.Count > 0) {
                Vector3 distVec = new(0f, 0, 2f);

                for (int i = 0; i < ItemRewardPool.Count; i++) {
                    double randomAngle = Math.Tau * Utilities.RNG.NextDouble();
                    Vector3 newPos = distVec.Rotated(Vector3.Up, (float)randomAngle) + PlayerSpawnMarker.GlobalPosition;
                    
                    WorldItem wi = ItemRewardPool[i].ConvertToWorldItem();
                    Run.Instance.DropItem(wi, newPos);
                }
            }
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
        
        int enemiesToSpawn;
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
                EmitSignal(SignalName.MapObjectiveFinished);
				//TestSpawnMapTrans();
			}
		}
	}

	public void SetAndStartNextWave() {
		ActiveWaveNumber++;
		//Game.Instance.PlayerActor.debugLabel.Text = $"Wave {ActiveWaveNumber + 1} / {ActiveWaveList.EnemyWaves.Count}";
		SetEnemyWave(ActiveWaveList.EnemyWaves[ActiveWaveNumber]);
		SpawnWave();
	}

    public bool IsObjectiveActive() {
        return hasMapStarted && !hasMapFinished;
    }

    public void AddGoldToRewards(int baseAmount, bool isRandom) {
        int goldAmount;

		if (isRandom) {
			goldAmount = (int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0);
		}
		else {
			goldAmount = baseAmount;
		}

        GoldRewardPool += goldAmount;
        objective.SetGoldReward(GoldRewardPool);
	}

    public void AddItemToRewards(Item item) {
        ItemRewardPool.Add(item);
        objective.SetItemReward(ItemRewardPool.Count);
    }
}
