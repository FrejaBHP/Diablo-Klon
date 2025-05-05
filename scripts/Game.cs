using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	public Player PlayerActor;
	public MapBase CurrentMap;

	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	private MapBase mapTown; // Husk at lave en speciel klasse til Town

	private CanvasLayer worldObjectsLayer;

	public override void _Ready() {
		currentMapNode = GetNode<Node3D>("CurrentMap");
		mapTown = currentMapNode.GetNode<MapBase>("MapTown");
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
		PlayerActor = GetNode<Player>("Player");
		
		CurrentMap = mapTown;
	}

	public void LoadAndSetMapToTown() {
		if (!mapTown.IsInsideTree()) {
			currentMapNode.AddChild(mapTown);
			CurrentMap = mapTown;
		}
	}

	private void UnloadTown() {
		if (mapTown.IsInsideTree()) {
			currentMapNode.CallDeferred(MethodName.RemoveChild, mapTown);
		}
	}

	// Changes the map to a non-town scene. Do not use this for loading the town
	public void ChangeMap(PackedScene scene) {
		if (mapTown.IsInsideTree()) {
			UnloadTown();
		}
		CurrentMap.ClearEnemies();

		CurrentMap = MapDatabase.GetMap(scene);
		CurrentMap.MapReady += OnMapLoaded;
		currentMapNode.AddChild(CurrentMap);
	}

	private void OnMapLoaded() {
		PlayerActor.ResetNodeTarget();
		PlayerActor.Velocity = Vector3.Zero;
		PlayerActor.GlobalPosition = CurrentMap.PlayerSpawnMarker.GlobalPosition;
	}

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
}
