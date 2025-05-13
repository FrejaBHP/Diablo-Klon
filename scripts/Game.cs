using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	protected static readonly PackedScene goldScene = GD.Load<PackedScene>("res://scenes/worldgold.tscn");
	protected static readonly PackedScene mapTransScene = GD.Load<PackedScene>("res://scenes/map_transition.tscn");

	public static Game Instance { get; private set; }

	public Player PlayerActor;
	public MapBase CurrentMap;

	public int CurrentAct { get; protected set; } = 1;
	public int CurrentArea { get; protected set; } = 1;

	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	private MapBase mapTown; // Husk at lave en speciel klasse til Town

	private CanvasLayer worldObjectsLayer;

	private EnemyWave activeWave = null;
	private int enemiesToSpawn = 0;
	private int activeEnemies = 0;

	public override void _Ready() {
		Instance = this; // There should possibly only ever be 1 Game instance at any time, so if this somehow results in overrides, lord have mercy

		currentMapNode = GetNode<Node3D>("CurrentMap");
		mapTown = currentMapNode.GetNode<MapBase>("MapTown");
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
		PlayerActor = GetNode<Player>("Player");
		
		//GameSettings.Instance.ApplyPlayerSettings();
		
		CurrentMap = mapTown;

		SetEnemyWave(EnemyDatabase.TestWave);
	}

	public void LoadAndSetMapToTown() {
		if (!mapTown.IsInsideTree()) {
			MapBase oldMap = CurrentMap;
			oldMap.ClearEnemies();
			
			currentMapNode.AddChild(mapTown);
			CurrentMap = mapTown;
			CurrentMap.AddRegionToNav();
			OnMapLoaded();

			oldMap.QueueFree();
		}
	}

	private void UnloadTown() {
		if (mapTown.IsInsideTree()) {
			currentMapNode.CallDeferred(MethodName.RemoveChild, mapTown);
		}
	}

	/// <summary>
	/// Changes the map to a non-town scene. Do not use this for loading the town.
	/// </summary>
	/// <param name="scene"></param>
	public void ChangeMap(PackedScene scene) {
		MapBase oldMap = CurrentMap;
		oldMap.ClearEnemies();

		CurrentMap = MapDatabase.GetMap(scene);
		CurrentMap.MapReady += OnMapLoaded;
		currentMapNode.AddChild(CurrentMap);

		if (oldMap == mapTown) {
			UnloadTown();
		}
		else {
			oldMap.QueueFree();
		}

		SetEnemyWave(EnemyDatabase.TestWave);
	}

	// Potentially add exception for the town? Just to avoid spawning in the same spot every time no matter where you came from?
	private void OnMapLoaded() {
		PlayerActor.ResetNodeTarget();
		PlayerActor.Velocity = Vector3.Zero;
		PlayerActor.GlobalPosition = CurrentMap.PlayerSpawnMarker.GlobalPosition;
	}

	/// <summary>
	/// Clears all nodes in the WorldObjectsLayer. Do note these are currently global and not map-specific.
	/// </summary>
	public void RemoveAllWorldItems() {
        System.Collections.Generic.IEnumerable<Node> worldItems = worldObjectsLayer.GetChildren().Where(c => c.IsInGroup("WorldItem"));
		foreach (Node item in worldItems) {
			item.QueueFree();
		}
	}

	public void GenerateRandomItemFromCategory(EItemCategory category, Vector3 position) {
		//int currentAreaLevel = CurrentMap.AreaLevel;
		Item item = ItemGeneration.GenerateItemFromCategory(category);
		WorldItem worldItem = item.ConvertToWorldItem();
		DropItem(worldItem, position);
	}

	public void GenerateRandomSkillItem(Vector3 position) {
		Item item = ItemGeneration.GenerateRandomSkillItem();
		WorldItem worldItem = item.ConvertToWorldItem();
		DropItem(worldItem, position);
	}

	public void DropItem(WorldItem item, Vector3 position) {
		worldObjectsLayer.AddChild(item);
		item.GlobalPosition = position with { Y = position.Y + 0.25f };
		item.PostSpawn();
	}

	public void DropGold(int baseAmount, Vector3 position) {
		Gold gold = goldScene.Instantiate<Gold>();
		worldObjectsLayer.AddChild(gold);

		int goldDropped = (int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0);
		gold.SetAmount(goldDropped);

		gold.GlobalPosition = position with { Y = position.Y + 0.25f };
		gold.PostSpawn();
	}

	public void AwardExperience(double baseAmount) {
		PlayerActor.GainExperience(baseAmount);
	}

	public void SetEnemyWave(EnemyWave wave) {
		activeWave = wave.ShallowCopy();
		enemiesToSpawn = activeWave.GetWaveSpawnCount();
	}

	// Spawns all enemies from a wave at once
	public void Test() {
		foreach (EnemyWaveComponent comp in activeWave.WaveComponents) {
			for (int i = 0; i < comp.EnemyCount; i++) {
				Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
				spawnPos.Y -= 0.5f;
				EnemyBase enemy = comp.EnemyScene.Instantiate<EnemyBase>();
				CurrentMap.EnemiesNode.AddChild(enemy);
				enemy.GlobalPosition = spawnPos;

				enemiesToSpawn--;
				activeEnemies++;
				enemy.EnemyDied += DecrementEnemyCount;
			}
		}
    }

	public void DecrementEnemyCount() {
		activeEnemies--;

		if (CurrentMap != mapTown && activeEnemies == 0 && enemiesToSpawn == 0) {
			TestSpawnMapTrans();
		}
	}

	public void TestSpawnMapTrans() {
		MapTransitionObj transObj = mapTransScene.Instantiate<MapTransitionObj>();
		CurrentMap.AddChild(transObj);
		transObj.SceneToTransitionTo = MapDatabase.FEScene;
		transObj.GoesToTown = false;

		Vector3 newPos = CurrentMap.PlayerSpawnMarker.GlobalPosition;
		newPos.Y += 0.5f;
		transObj.GlobalPosition = newPos;
	}
}
