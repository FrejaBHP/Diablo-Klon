using Godot;
using System;

public partial class SurvivalController : ObjectiveController {
    public Timer SurvivalTimer { get; private set; }
    public Timer SpawnTimer { get; private set; }

    public WeightedList<EEnemyType> ActiveSpawnPool { get; protected set; } = null;
    public int EnemySpawnCeiling { get; protected set; } = 0;
    public int EnemySpawnDensity { get; protected set; } = 0;
    protected double elapsedTimeDensityMod = 0;

    protected const double spawnDelay = 5;

    public override void _Ready() {
        SurvivalTimer = GetNode<Timer>("SurvivalTimer");
        SpawnTimer = GetNode<Timer>("SpawnTimer");
        SpawnTimer.WaitTime = spawnDelay;

        MapObjective = EMapObjective.Survival;
        GoldRewardPool += GoldReward;

        SetEnemySpawnPool(EnemyDatabase.TestSurvivalSpawnPool);
    }

    public override void _PhysicsProcess(double delta) {
        if (IsObjectiveActive) {
            if (SurvivalTimer.TimeLeft > 10) {
                Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Text = $"{SurvivalTimer.TimeLeft:F0}";
            }
            else {
                Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Text = $"{SurvivalTimer.TimeLeft:F1}";
            }
        }
    }

    // Generics
    public override void CreateObjectiveGUI() {
        ObjectiveHUD = mapObjectiveHUDScene.Instantiate<MapObjectiveHUD>();
		Run.Instance.PlayerActor.PlayerHUD.RightHUD.ObjectiveContainer.AddChild(ObjectiveHUD);
        ObjectiveHUD.SetObjectiveText(MapObjective.ToString());
        ObjectiveHUD.SetGoldReward(GoldRewardPool);
        ObjectiveHUD.SetItemReward(ItemRewardPool.Count);
        ObjectiveHUD.Visible = true;
    }

    public override void StartObjective() {
        SetObjectiveTimerLength();

        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Visible = true;
        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Text = $"{Math.Round(SurvivalTimer.WaitTime, 0)}";

        EnemySpawnCeiling = CalculateEnemySpawnCeiling(Run.Instance.CurrentAct, Run.Instance.CurrentArea, 1);
        EnemySpawnDensity = CalculateBaseEnemySpawnDensity(Run.Instance.CurrentAct, Run.Instance.CurrentArea, 1);
        Run.Instance.PlayerActor.PlayerHUD.RightHUD.EnemyDebugLabel.Visible = true;

        SurvivalTimer.Start();
        OnSpawnTimerTimeout();
        SpawnTimer.Start();

        IsObjectiveActive = true;
    }

    public override void EndObjective() {
        IsObjectiveActive = false;
        
        SpawnTimer.Stop();
        LinkedMap.ClearEnemies();
        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Visible = false;

        if (ObjectiveHUD != null) {
            Run.Instance.PlayerActor.PlayerHUD.RightHUD.ObjectiveContainer.RemoveChild(ObjectiveHUD);
            ObjectiveHUD.QueueFree();

            Run.Instance.PlayerActor.PlayerHUD.RightHUD.EnemyDebugLabel.Visible = false;
        }

        if (GoldRewardPool > 0) {
            Run.Instance.DropGold(GoldRewardPool, LinkedMap.PlayerSpawnMarker.GlobalPosition, false);
        }

        if (ExpReward > 0) {
            Run.Instance.AwardExperience(ExpReward);
        }

        if (ItemRewardPool.Count > 0) {
            Vector3 distVec = new(0f, 0, 2f);

            for (int i = 0; i < ItemRewardPool.Count; i++) {
                double randomAngle = Math.Tau * Utilities.RNG.NextDouble();
                Vector3 newPos = distVec.Rotated(Vector3.Up, (float)randomAngle) + LinkedMap.PlayerSpawnMarker.GlobalPosition;
                
                WorldItem wi = ItemRewardPool[i].ConvertToWorldItem();
                Run.Instance.DropItem(wi, newPos);
            }
        }
    }

    public override void OnEnemyKilled() {
        RemainingEnemies--;
    }

    // Survival-specific
    public void OnSurivalTimerTimeout() {
        EndObjective();
        LinkedMap.OnMapObjectiveFinished();
    }

    public void OnSpawnTimerTimeout() {
        SpawnFromPool(GetAmountToSpawn(), false);
                
        Run.Instance.PlayerActor.PlayerHUD.RightHUD.UpdateEnemyDebugLabelSurvival(
            GetDensityTimeModifier(), 
            GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier())
        );
    }

    protected double GetDensityTimeModifier() {
        // Assuming ObjectiveHUD type is Survival!!! Need cases for others later as they're made
        const double modFloor = 0.33;
        // Should return anywhere between 0 and 0,67, where 0,67 is reached when 67% of the survival time has elapsed
        double ratio = Math.Clamp(1 - (SurvivalTimer.TimeLeft / SurvivalTimer.WaitTime), 0, 0.67);
        
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

        SurvivalTimer.WaitTime = waitTime;
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

    protected int GetAmountToSpawn() {
        if (RemainingEnemies > EnemySpawnCeiling) {
            return 0;
        }
        
        int enemiesToSpawn;
        int potentialMaxEnemies = RemainingEnemies + GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier());

        if (potentialMaxEnemies > EnemySpawnCeiling) {
            enemiesToSpawn = potentialMaxEnemies - EnemySpawnCeiling;
        }
        else {
            enemiesToSpawn = GetTimeAdjustedSpawnDensity(EnemySpawnDensity, GetDensityTimeModifier());
        }

        return enemiesToSpawn;
    }

    public void SetEnemySpawnPool(WeightedList<EEnemyType> spawnPool) {
        ActiveSpawnPool = spawnPool;
    }

    public void SpawnFromPool(int amount, bool isGroup) {
        if (amount == 0) {
            return;
        }

        // If spawning as a group, all enemies are the same type (and should spawn close to each other, but not implemented yet)
        if (isGroup) {
            EEnemyType type = ActiveSpawnPool.GetRandomItem();
            for (int i = 0; i < amount; i++) {
                LinkedMap.SpawnEnemy(type);
            }
        }
        else {
            for (int i = 0; i < amount; i++) {
                EEnemyType type = ActiveSpawnPool.GetRandomItem();
                LinkedMap.SpawnEnemy(type);
            }
        }

        EnemiesToSpawn -= amount;
        RemainingEnemies += amount;
    }
}
