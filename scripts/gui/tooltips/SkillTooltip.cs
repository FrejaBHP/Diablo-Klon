using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class SkillTooltip : Control {
	public VBoxContainer PartsContainer;
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
			PartsContainer = GetNode<VBoxContainer>("PartsContainer");
			TopContainer = PartsContainer.GetNode<HBoxContainer>("TopContainer");
			NameLabel = TopContainer.GetNode<Label>("NameLabel");
			InfoContainer = TopContainer.GetNode<HBoxContainer>("InfoContainer");
			CostTexture = InfoContainer.GetNode<TextureRect>("CostContainer/CostInfoContainer/CostTexture");
			CostLabel = InfoContainer.GetNode<Label>("CostContainer/CostInfoContainer/CostLabel");
			TimeDescLabel = InfoContainer.GetNode<Label>("TimeContainer/TimeDescLabel");
			TimeTexture = InfoContainer.GetNode<TextureRect>("TimeContainer/TimeInfoContainer/TimeTexture");
			TimeLabel = InfoContainer.GetNode<Label>("TimeContainer/TimeInfoContainer/TimeLabel");

			DescriptionSeparator = PartsContainer.GetNode<HSeparator>("DescriptionSeparator");
			DescriptionContainer = PartsContainer.GetNode<VBoxContainer>("DescriptionContainer");
			EffectSeparator = PartsContainer.GetNode<HSeparator>("EffectSeparator");
			DamageContainer = PartsContainer.GetNode<HBoxContainer>("DamageEffectContainer/DamageContainer");
			EffectContainer = PartsContainer.GetNode<VBoxContainer>("DamageEffectContainer/EffectContainer");
		}

        base._Notification(what);
    }
}
