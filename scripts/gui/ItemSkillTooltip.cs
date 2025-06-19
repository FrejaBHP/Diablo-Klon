using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemSkillTooltip : Control {
	public VBoxContainer PartsContainer;
	public Label NameLabel;
	public VBoxContainer StatsContainer;
	public HSeparator DescriptionSeparator;
	public Label DescriptionLabel;
	public HSeparator EffectSeparator;
	public VBoxContainer EffectContainer;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			PartsContainer = GetNode<VBoxContainer>("PartsContainer");
			NameLabel = PartsContainer.GetNode<Label>("NameContainer/ItemName");
			StatsContainer = PartsContainer.GetNode<VBoxContainer>("StatsContainer");
			DescriptionSeparator = PartsContainer.GetNode<HSeparator>("DescriptionSeparator");
			DescriptionLabel = PartsContainer.GetNode<Label>("DescriptionLabel");
			EffectSeparator = PartsContainer.GetNode<HSeparator>("EffectSeparator");
			EffectContainer = PartsContainer.GetNode<VBoxContainer>("EffectContainer");
		}

        base._Notification(what);
    }
}
