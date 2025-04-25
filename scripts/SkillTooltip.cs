using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class SkillTooltip : Control {
	public Label NameLabel;
	public HBoxContainer TopContainer;
	public TextureRect CostTexture;
	public Label CostLabel;
	public TextureRect TimeTexture;
	public Label TimeLabel;

	public VBoxContainer DamageContainer;

	/*
	public VBoxContainer DescriptionContainer;
	public VBoxContainer EffectContainer;
	public HSeparator DescriptionSeparator;
	public HSeparator EffectSeparator;
	*/

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			TopContainer = GetNode<HBoxContainer>("VBoxContainer/TopContainer");
			NameLabel = TopContainer.GetNode<Label>("NameLabel");
			CostTexture = TopContainer.GetNode<TextureRect>("CostContainer/CostTexture");
			CostLabel = TopContainer.GetNode<Label>("CostContainer/CostLabel");
			TimeTexture = TopContainer.GetNode<TextureRect>("TimeContainer/TimeTexture");
			TimeLabel = TopContainer.GetNode<Label>("TimeContainer/TimeLabel");

			DamageContainer = GetNode<VBoxContainer>("VBoxContainer/DamageContainer");

			/*
			DescriptionContainer = GetNode<VBoxContainer>("VBoxContainer/DescriptionContainer");
			EffectContainer = GetNode<VBoxContainer>("VBoxContainer/EffectContainer");
			DescriptionSeparator = GetNode<HSeparator>("VBoxContainer/DescriptionSeparator");
			EffectSeparator = GetNode<HSeparator>("VBoxContainer/EffectSeparator");
			*/
		}

        base._Notification(what);
    }
}
