using Godot;
using System;
using System.Linq;

public partial class Game : Node3D {
	private Player player;
	private PlayerCamera playerCam;

	private CanvasLayer worldObjectsLayer;

	public override void _Ready() {
		worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");
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
