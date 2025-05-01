using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	public Player PlayerActor;
	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	public MapBase CurrentMap;

	private CanvasLayer worldObjectsLayer;

	public override void _Ready() {
		currentMapNode = GetNode<Node3D>("CurrentMap");
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
		PlayerActor = GetNode<Player>("Player");

		SetMap(currentMapNode.GetChild<MapBase>(0));
	}

	public void SetMap(MapBase map) {
		CurrentMap = map;
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
