using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class SkillTooltip : Control {
	public HBoxContainer TopContainer;
	public Label NameLabel;
	public HBoxContainer InfoContainer;
	public TextureRect CostTexture;
	public Label CostLabel;
	public TextureRect TimeTexture;
	public Label TimeLabel;

	public HBoxContainer DamageContainer;

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
			InfoContainer = TopContainer.GetNode<HBoxContainer>("InfoContainer");
			CostTexture = InfoContainer.GetNode<TextureRect>("CostContainer/CostTexture");
			CostLabel = InfoContainer.GetNode<Label>("CostContainer/CostLabel");
			TimeTexture = InfoContainer.GetNode<TextureRect>("TimeContainer/TimeTexture");
			TimeLabel = InfoContainer.GetNode<Label>("TimeContainer/TimeLabel");

			DamageContainer = GetNode<HBoxContainer>("VBoxContainer/DamageContainer");

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
