using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemTooltip : Control {
	public VBoxContainer PartsContainer;
	public Label NameLabel;
	public Label BaseLabel;
	public VBoxContainer BaseStatsContainer;
	public VBoxContainer ImplicitContainer;
	public VBoxContainer AffixContainer;
	public HSeparator ImplicitSeparator;
	public HSeparator AffixSeparator;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			PartsContainer = GetNode<VBoxContainer>("PartsContainer");
			NameLabel = PartsContainer.GetNode<Label>("NameContainer/ItemName");
			BaseLabel = PartsContainer.GetNode<Label>("NameContainer/ItemType");
			BaseStatsContainer = PartsContainer.GetNode<VBoxContainer>("BaseStatsContainer");
			ImplicitContainer = PartsContainer.GetNode<VBoxContainer>("ImplicitContainer");
			AffixContainer = PartsContainer.GetNode<VBoxContainer>("AffixContainer");
			ImplicitSeparator = PartsContainer.GetNode<HSeparator>("ImplicitSeparator");
			AffixSeparator = PartsContainer.GetNode<HSeparator>("AffixSeparator");
		}

        base._Notification(what);
    }
}
