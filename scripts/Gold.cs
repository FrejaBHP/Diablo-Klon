using Godot;
using System;

public partial class Gold : Area3D {
    protected static readonly PackedScene floatingLabelScene = GD.Load<PackedScene>("res://scenes/gui/hud_floating_label.tscn");

	protected RayCast3D floorCast;
	protected Marker3D labelAnchor;
	protected FloatingLabel floatingLabel;
    protected Sprite3D goldSprite;

    protected int goldAmount = 0;

    public override void _Ready() {
		floorCast = GetNode<RayCast3D>("FloorCast");
		labelAnchor = GetNode<Marker3D>("LabelAnchor");
        goldSprite = GetNode<Sprite3D>("GoldSprite");

		AddToGroup("Gold");
	}

    public void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Player")) {
            Game.Instance.PlayerActor.Gold += goldAmount;
            QueueFree();
        }
    }

    public void PostSpawn() {
		if (!FindAndSnapToFloor()) {
			GD.PrintErr("Floor not found");
		}

		floatingLabel = floatingLabelScene.Instantiate<FloatingLabel>();
		labelAnchor.AddChild(floatingLabel);
		floatingLabel.SetLabelText($"{goldAmount} Gold", "");

        floatingLabel.ApplyTextColour(ETextColour.Default);
		floatingLabel.ApplyColourSet(ELabelColourSet.Default);
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

    public void SetAmount(int amount) {
        goldAmount = amount;

        switch (goldAmount) {
            case >= 100:
                goldSprite.Texture = UILib.GoldPileLarge;
                break;
            
            case < 100 and >= 50:
                goldSprite.Texture = UILib.GoldPileMedium;
                break;
            
            case < 50 and >= 20:
                goldSprite.Texture = UILib.GoldPileSmall;
                break;

            default:
                goldSprite.Texture = UILib.GoldPileTiny;
                break;
        }
    }
}
