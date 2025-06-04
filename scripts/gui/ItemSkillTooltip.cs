using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemSkillTooltip : Control {
	public Label NameLabel;
	public VBoxContainer StatsContainer;
	public HSeparator DescriptionSeparator;
	public Label DescriptionLabel;
	public HSeparator EffectSeparator;
	public VBoxContainer EffectContainer;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			NameLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemName");
			StatsContainer = GetNode<VBoxContainer>("VBoxContainer/StatsContainer");
			DescriptionSeparator = GetNode<HSeparator>("VBoxContainer/DescriptionSeparator");
			DescriptionLabel = GetNode<Label>("VBoxContainer/DescriptionLabel");
			EffectSeparator = GetNode<HSeparator>("VBoxContainer/EffectSeparator");
			EffectContainer = GetNode<VBoxContainer>("VBoxContainer/EffectContainer");
		}

        base._Notification(what);
    }
}
