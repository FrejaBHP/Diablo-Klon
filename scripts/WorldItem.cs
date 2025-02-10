using Godot;
using System;

public partial class WorldItem : StaticBody3D {
	PackedScene floatingLabelScene = GD.Load<PackedScene>("res://hud_floating_label.tscn");

	public Item ItemReference = null;
	private RayCast3D floorCast;
	private Marker3D labelAnchor;
	private FloatingLabel floatingLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		floorCast = GetNode<RayCast3D>("FloorCast");
		labelAnchor = GetNode<Marker3D>("LabelAnchor");

		AddToGroup("WorldItem");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}

	public void SetItemReference(Item item) {
		ItemReference = item;
	}

	public void PostSpawn() {
		if (!FindAndSnapToFloor()) {
			GD.PrintErr("Floor not found");
		}

		floatingLabel = floatingLabelScene.Instantiate<FloatingLabel>();
		labelAnchor.AddChild(floatingLabel);
		floatingLabel.SetLabelText(ItemReference.ItemName);

		switch (ItemReference.ItemRarity) {
			case EItemRarity.Common:
				floatingLabel.ApplyColourSet(ELabelColourSet.Common);
				break;
			
			case EItemRarity.Magic:
				floatingLabel.ApplyColourSet(ELabelColourSet.Magic);
				break;

			case EItemRarity.Rare:
				floatingLabel.ApplyColourSet(ELabelColourSet.Rare);
				break;

			case EItemRarity.Unique:
				floatingLabel.ApplyColourSet(ELabelColourSet.Unique);
				break;

			default:
				floatingLabel.ApplyColourSet(ELabelColourSet.Default);
				break;
		}
	}

	public bool FindAndSnapToFloor() {
		floorCast.ForceRaycastUpdate();

		if (!floorCast.IsColliding()) {
            return false;
		}

		Vector3 floorIntersect = floorCast.GetCollisionPoint();

		Vector3 newPosition = floorIntersect;
		newPosition.Y += 0.05f;
		Position = newPosition;

		floorCast.Enabled = false;

		return true;
	}
}
