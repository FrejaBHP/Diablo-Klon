using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	public static Game Instance { get; private set; }

	public Player PlayerActor;
	public MapBase CurrentMap;

	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	private MapBase mapTown; // Husk at lave en speciel klasse til Town

	private CanvasLayer worldObjectsLayer;

	public override void _Ready() {
		Instance = this; // There should possibly only ever be 1 Game instance at any time, so if this somehow results in overrides, lord have mercy

		currentMapNode = GetNode<Node3D>("CurrentMap");
		mapTown = currentMapNode.GetNode<MapBase>("MapTown");
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
		PlayerActor = GetNode<Player>("Player");
		
		//GameSettings.Instance.ApplyPlayerSettings();
		
		CurrentMap = mapTown;
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

	public void DropItem(WorldItem item, Vector3 position) {
		worldObjectsLayer.AddChild(item);
		item.GlobalPosition = position with { Y = position.Y + 0.25f };
		item.PostSpawn();
	}

	// Skal flyttes senere
	protected static readonly PackedScene goldScene = GD.Load<PackedScene>("res://scenes/worldgold.tscn");
	public void DropGold(int baseAmount, Vector3 position) {
		Gold gold = goldScene.Instantiate<Gold>();
		worldObjectsLayer.AddChild(gold);

		int goldDropped = (int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0);
		gold.SetAmount(goldDropped);

		gold.GlobalPosition = position with { Y = position.Y + 0.25f };
		gold.PostSpawn();
	}
}
