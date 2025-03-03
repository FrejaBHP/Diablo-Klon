using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemTooltip : Control {
	public Label NameLabel;
	public Label BaseLabel;
	public VBoxContainer BaseStatsContainer;
	public VBoxContainer AffixContainer;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			NameLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemName");
			BaseLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemType");
			BaseStatsContainer = GetNode<VBoxContainer>("VBoxContainer/BaseStatsContainer");
			AffixContainer = GetNode<VBoxContainer>("VBoxContainer/AffixContainer");
		}

        base._Notification(what);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}
}
