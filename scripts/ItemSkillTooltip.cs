using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemSkillTooltip : Control {
	public Label NameLabel;
	public VBoxContainer StatsContainer;
	public VBoxContainer DescriptionContainer;
	public VBoxContainer EffectContainer;
	public HSeparator DescriptionSeparator;
	public HSeparator EffectSeparator;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			NameLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemName");
			StatsContainer = GetNode<VBoxContainer>("VBoxContainer/StatsContainer");
			DescriptionContainer = GetNode<VBoxContainer>("VBoxContainer/DescriptionContainer");
			EffectContainer = GetNode<VBoxContainer>("VBoxContainer/EffectContainer");
			DescriptionSeparator = GetNode<HSeparator>("VBoxContainer/DescriptionSeparator");
			EffectSeparator = GetNode<HSeparator>("VBoxContainer/EffectSeparator");
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
