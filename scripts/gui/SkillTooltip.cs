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
	public Label TimeDescLabel;
	public TextureRect TimeTexture;
	public Label TimeLabel;

	public HSeparator DescriptionSeparator;
	public VBoxContainer DescriptionContainer;
	public HSeparator EffectSeparator;
	public HBoxContainer DamageContainer;
	public VBoxContainer EffectContainer;

    public override void _Notification(int what) {
		// Called when instantiated to avoid needing to get these nodes elsewhere
		if (what == NotificationSceneInstantiated) {
			TopContainer = GetNode<HBoxContainer>("VBoxContainer/TopContainer");
			NameLabel = TopContainer.GetNode<Label>("NameLabel");
			InfoContainer = TopContainer.GetNode<HBoxContainer>("InfoContainer");
			CostTexture = InfoContainer.GetNode<TextureRect>("CostContainer/CostInfoContainer/CostTexture");
			CostLabel = InfoContainer.GetNode<Label>("CostContainer/CostInfoContainer/CostLabel");
			TimeDescLabel = InfoContainer.GetNode<Label>("TimeContainer/TimeDescLabel");
			TimeTexture = InfoContainer.GetNode<TextureRect>("TimeContainer/TimeInfoContainer/TimeTexture");
			TimeLabel = InfoContainer.GetNode<Label>("TimeContainer/TimeInfoContainer/TimeLabel");

			DescriptionSeparator = GetNode<HSeparator>("VBoxContainer/DescriptionSeparator");
			DescriptionContainer = GetNode<VBoxContainer>("VBoxContainer/DescriptionContainer");
			EffectSeparator = GetNode<HSeparator>("VBoxContainer/EffectSeparator");
			DamageContainer = GetNode<HBoxContainer>("VBoxContainer/DamageEffectContainer/DamageContainer");
			EffectContainer = GetNode<VBoxContainer>("VBoxContainer/DamageEffectContainer/EffectContainer");
		}

        base._Notification(what);
    }
}
