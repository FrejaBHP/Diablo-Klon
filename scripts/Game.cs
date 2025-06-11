using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	protected static readonly PackedScene goldScene = GD.Load<PackedScene>("res://scenes/worldgold.tscn");
	protected static readonly PackedScene mapTransScene = GD.Load<PackedScene>("res://scenes/map_transition.tscn");

	public static Game Instance { get; private set; }

	public Player PlayerActor;
	public MapBase CurrentMap;

	public int CurrentAct { get; protected set; } = 0;
	public int CurrentArea { get; protected set; } = 0;
	private int areasPerAct = 2; // Areas refer to both combat maps and shop/breather maps. Ideally the rotation is C-C-S, and act ends with a boss. 3-4 rotations should be good here in the end

	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	private MapBase mapTown; // Husk at lave en speciel klasse til Town

	private CanvasLayer worldObjectsLayer;
	private Timer mapStartTimer;

	public override void _Ready() {
		Instance = this; // There should possibly only ever be 1 Game instance at any time, so if this somehow results in overrides, lord have mercy

		currentMapNode = GetNode<Node3D>("CurrentMap");
		mapTown = currentMapNode.GetNode<MapBase>("MapTown");
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
		mapStartTimer = GetNode<Timer>("MapStartTimer");
		PlayerActor = GetNode<Player>("Player");
		
		//GameSettings.Instance.ApplyPlayerSettings();
		
		CurrentMap = mapTown;
		mapTown.MapObjective = EMapObjective.None;
		MapTransitionObj transObj = mapTown.GetNode<MapTransitionObj>("MapTransition");
		transObj.SceneToTransitionTo = MapDatabase.FEScene;

		//SetMapWaveList(EnemyDatabase.TestMapHorde);
	}

	public void LoadAndSetMapToTown() {
		if (!mapTown.IsInsideTree()) {
			PlayerActor.PlayerCamera.OcclusionCast.Enabled = false;

			MapBase oldMap = CurrentMap;
			oldMap.ClearEnemies();
			
			currentMapNode.AddChild(mapTown);
			CurrentMap = mapTown;
			CurrentMap.AddRegionToNav();
			OnMapLoaded();

			oldMap.QueueFree();

			CurrentArea = 0;
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
		PlayerActor.PlayerCamera.OcclusionCast.Enabled = false;

		MapBase oldMap = CurrentMap;
		oldMap.ClearEnemies();

        //CurrentMap = MapDatabase.GetMap(scene);
        MapDatabase.GetMapTest(scene, out CurrentMap, out EMapObjective mapObjective);
        CurrentMap.MapObjective = mapObjective;
		CurrentMap.MapReady += OnMapLoaded;
		CurrentMap.MapFinished += OnMapCompletion;
		currentMapNode.AddChild(CurrentMap);

		if (oldMap == mapTown) {
			UnloadTown();
		}
		else {
			oldMap.QueueFree();
		}

		CurrentArea++;
		//activeWaveNumber = 0;
		//SetMapWaveList(EnemyDatabase.TestMapHorde);
		//SetEnemyWave(activeWaveList.EnemyWaves[0]);

		//PlayerActor.debugLabel.Text = $"Wave {CurrentMap.ActiveWaveNumber + 1} / {CurrentMap.ActiveWaveList.EnemyWaves.Count}";
		//SetEnemyWave(EnemyDatabase.TestWave);
	}

	// Potentially add exception for the town? Just to avoid spawning in the same spot every time no matter where you came from?
	private void OnMapLoaded() {
		PlayerActor.ResetNodeTarget();
		PlayerActor.Velocity = Vector3.Zero;
		PlayerActor.GlobalPosition = CurrentMap.PlayerSpawnMarker.GlobalPosition;
		PlayerActor.PlayerCamera.OcclusionCast.Enabled = true;

		if (CurrentMap != mapTown) {
			CurrentMap.CreateObjectiveGUI();

			mapStartTimer.WaitTime = 2;
			mapStartTimer.Start();
		}
	}

	private void OnMapStartTimerTimeout() {
		CurrentMap.StartObjective();

		//if (activeWave != null) {
		//	SpawnWave();
		//}
	}

	/// <summary>
	/// Clears all nodes in the WorldObjectsLayer and maps' NameplateLayer
	/// </summary>
	public void RemoveAllWorldItems() {
        System.Collections.Generic.IEnumerable<Node> worldItems = worldObjectsLayer.GetChildren().Where(c => c.IsInGroup("WorldItem"));
		worldItems = worldItems.Concat(CurrentMap.NameplateLayer.GetChildren().Where(c => c.IsInGroup("WorldItem")));
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
		//worldObjectsLayer.AddChild(item);
		CurrentMap.NameplateLayer.AddChild(item);
		item.GlobalPosition = position with { Y = position.Y + 0.25f };
		item.PostSpawn();
	}

	public void DropGold(int baseAmount, Vector3 position, bool isRandom) {
		Gold gold = goldScene.Instantiate<Gold>();
		//worldObjectsLayer.AddChild(gold);
		CurrentMap.NameplateLayer.AddChild(gold);

		if (isRandom) {
			gold.SetAmount((int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0));
		}
		else {
			gold.SetAmount(baseAmount);
		}

		gold.GlobalPosition = position with { Y = position.Y + 0.25f };
		gold.PostSpawn();
	}

	public void AwardExperience(double baseAmount) {
		PlayerActor.GainExperience(baseAmount);
	}

	public void TestSpawnMapTrans() {
		MapTransitionObj transObj = mapTransScene.Instantiate<MapTransitionObj>();
		CurrentMap.AddChild(transObj);

		Vector3 newPos = CurrentMap.PlayerSpawnMarker.GlobalPosition;
		newPos.Y += 0.5f;
		transObj.GlobalPosition = newPos;

		if (CurrentArea < areasPerAct) {
			transObj.SceneToTransitionTo = MapDatabase.FEScene;
			transObj.GoesToTown = false;
		}
		else {
			transObj.GoesToTown = true;
		}

		if (transObj.GoesToTown) {
			transObj.UseRedPortal = false;
		}
		else {
			transObj.UseRedPortal = true;
		}
		
		transObj.UpdatePortalAnimationAndVisibility();
	}

	public void OnMapCompletion() {
		TestSpawnMapTrans();

		/*
		var objectives = PlayerActor.PlayerHUD.PlayerRightHUD.ObjectiveContainer.GetChildren();
		foreach (var item in objectives) {
			item.QueueFree();
		}
		*/
    }
}
