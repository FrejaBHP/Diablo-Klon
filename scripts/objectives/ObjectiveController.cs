using Godot;
using System;
using System.Collections.Generic;

public abstract partial class ObjectiveController : Node {
    protected static readonly PackedScene mapObjectiveHUDScene = GD.Load<PackedScene>("res://scenes/gui/hud_objective.tscn");

    public int EnemiesToSpawn = 0;
    public int RemainingEnemies = 0;
    public EMapObjective MapObjective { get; protected set; }
    public bool IsObjectiveActive { get; protected set; } = false;

    public MapBase LinkedMap { get; set; }
    public MapObjectiveHUD ObjectiveHUD { get; protected set; }

    public int GoldReward { get; protected set; } = 25;
    public int ExpReward { get; protected set; } = 5;

    public int GoldRewardPool { get; protected set; } = 0;
    public List<Item> ItemRewardPool { get; protected set; } = new();

    public abstract void CreateObjectiveGUI();
    public abstract void StartObjective();
    public abstract void EndObjective();

    public abstract void OnEnemyKilled();

    public virtual void AddGoldToRewards(int baseAmount, bool isRandom) {
        int goldAmount;

		if (isRandom) {
			goldAmount = (int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0);
		}
		else {
			goldAmount = baseAmount;
		}

        GoldRewardPool += goldAmount;
        ObjectiveHUD.SetGoldReward(GoldRewardPool);
	}

    public virtual void AddItemToRewards(Item item) {
        ItemRewardPool.Add(item);
        ObjectiveHUD.SetItemReward(ItemRewardPool.Count);
    }

    public void SpawnEnemy(EEnemyType type) {
        EEnemyRarity randomRarity = Run.Instance.RollForEnemyRarity();
        Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(LinkedMap.GetWorld3D().NavigationMap, 1, false);
        spawnPos.Y -= 0.5f;
        
        EnemyBase enemy = EnemyDatabase.EnemyDictionary[type].Scene.Instantiate<EnemyBase>();
        LinkedMap.EnemiesNode.AddChild(enemy);
        enemy.GlobalPosition = spawnPos;
        enemy.SetRarity(randomRarity);

        enemy.EnemyDied += OnEnemyKilled;
    }

    public void DestroyController() {
        QueueFree();
    }
}
