using Godot;
using System;

public partial class WavesController : ObjectiveController {
    public MapEnemyWaves ActiveWaveList = null;
	public EnemyWave ActiveWave = null;
	public int ActiveWaveNumber = 0;

    public override void _Ready() {
        MapObjective = EMapObjective.Waves;
        GoldRewardPool += GoldReward;

        SetMapWaveList(EnemyDatabase.TestMapHorde);
		SetEnemyWave(ActiveWaveList.EnemyWaves[0]);
    }

    public override void _PhysicsProcess(double delta) {
        if (IsObjectiveActive) {
            
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
        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Visible = true;
        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Text = $"Wave {ActiveWaveNumber + 1} / {ActiveWaveList.EnemyWaves.Count}";

        SpawnWave();

        Run.Instance.PlayerActor.PlayerHUD.RightHUD.UpdateEnemyDebugLabelWaves(RemainingEnemies, ActiveWave.GetWaveSpawnCount());
        Run.Instance.PlayerActor.PlayerHUD.RightHUD.EnemyDebugLabel.Visible = true;

        IsObjectiveActive = true;
    }

    public override void EndObjective() {
        IsObjectiveActive = false;
        
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

        Run.Instance.PlayerActor.PlayerHUD.RightHUD.UpdateEnemyDebugLabelWaves(RemainingEnemies, ActiveWave.GetWaveSpawnCount());

        if (RemainingEnemies == 0 && EnemiesToSpawn == 0) {
			if (ActiveWaveNumber < ActiveWaveList.EnemyWaves.Count - 1) {
				SetAndStartNextWave();
			}
			else {
				EndObjective();
                LinkedMap.OnMapObjectiveFinished();
			}
		}
    }

    // Waves-specific
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
				SpawnEnemy(comp.EnemyType);

                EnemiesToSpawn--;
                RemainingEnemies++;
			}
		}
    }

    public void SetAndStartNextWave() {
		ActiveWaveNumber++;
        Run.Instance.PlayerActor.PlayerHUD.UpperHUD.ObjTimeLabel.Text = $"Wave {ActiveWaveNumber + 1} / {ActiveWaveList.EnemyWaves.Count}";
		SetEnemyWave(ActiveWaveList.EnemyWaves[ActiveWaveNumber]);
		SpawnWave();
	}
}
