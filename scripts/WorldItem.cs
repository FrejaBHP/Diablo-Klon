using Godot;
using System;

public partial class WorldItem : StaticBody3D {
	protected static readonly PackedScene floatingLabelScene = GD.Load<PackedScene>("res://scenes/gui/hud_floating_label.tscn");

	public Item ItemReference = null;
	protected RayCast3D floorCast;
	protected Marker3D labelAnchor;
	protected FloatingLabel floatingLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		floorCast = GetNode<RayCast3D>("FloorCast");
		labelAnchor = GetNode<Marker3D>("LabelAnchor");

		AddToGroup("WorldItem");
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
		floatingLabel.SetLabelText(ItemReference.ItemName, ItemReference.ItemBase);

		switch (ItemReference.ItemRarity) {
			case EItemRarity.Common:
				floatingLabel.ApplyTextColour(ETextColour.Common);
				break;
			
			case EItemRarity.Magic:
				floatingLabel.ApplyTextColour(ETextColour.Magic);
				break;

			case EItemRarity.Rare:
				floatingLabel.ApplyTextColour(ETextColour.Rare);
				break;

			case EItemRarity.Unique:
				floatingLabel.ApplyTextColour(ETextColour.Unique);
				break;

			case EItemRarity.Skill:
				floatingLabel.ApplyTextColour(ETextColour.Skill);
				break;

			default:
				floatingLabel.ApplyTextColour(ETextColour.Default);
				break;
		}

		floatingLabel.ApplyColourSet(ELabelColourSet.Item);
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
