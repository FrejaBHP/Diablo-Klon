using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class ItemTooltip : Control {
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
			NameLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemName");
			BaseLabel = GetNode<Label>("VBoxContainer/NameContainer/ItemType");
			BaseStatsContainer = GetNode<VBoxContainer>("VBoxContainer/BaseStatsContainer");
			ImplicitContainer = GetNode<VBoxContainer>("VBoxContainer/ImplicitContainer");
			AffixContainer = GetNode<VBoxContainer>("VBoxContainer/AffixContainer");
			ImplicitSeparator = GetNode<HSeparator>("VBoxContainer/ImplicitSeparator");
			AffixSeparator = GetNode<HSeparator>("VBoxContainer/AffixSeparator");
		}

        base._Notification(what);
    }
}
