using Godot;
using System;

public partial class HUDStatusIcon : PanelContainer {
    public TextureRect StatusIcon { get; protected set; }
    public Label StacksLabel { get; protected set; }
    public Label StatusLabel { get; protected set; }

    public EEffectName EffectName { get; protected set; }
    public string EffectNameString { get; protected set; }
    public string EffectDescString { get; protected set; }

    public int Stacks { get; protected set; } = 1;
    public double TimeLeft { get; protected set; }

    public bool ShouldShowInstancesInsteadOfTime { get; protected set; } = false;

    private bool isConfigured = false;

    public override void _Ready() {
        StatusIcon = GetNode<TextureRect>("MarginContainer/VBoxContainer/Container/StatusIcon");
        StacksLabel = GetNode<Label>("MarginContainer/VBoxContainer/Container/StacksLabel");
        StatusLabel = GetNode<Label>("MarginContainer/VBoxContainer/CenterContainer/StatusLabel");
    }

    public override void _PhysicsProcess(double delta) {
        if (isConfigured) {
            TimeLeft -= delta;

            if (!ShouldShowInstancesInsteadOfTime) {
                int minutes = (int)TimeLeft / 60;
                int seconds = (int)TimeLeft % 60;

                string secondsString;

                if (seconds < 10) {
                    secondsString = $"0{seconds}";
                }
                else {
                    secondsString = $"{seconds}";
                }

                StatusLabel.Text = $"{minutes}:{secondsString}";
            }
        }
    }

    public void SetStatusType(AttachedEffect attachedEffect) {
        OverrideBackground(attachedEffect.EffectRating);

        EffectName = attachedEffect.EffectName;
        StatusIcon.Texture = attachedEffect.EffectIcon;
        EffectNameString = attachedEffect.EffectString;
        EffectDescString = attachedEffect.EffectDescription;
        TimeLeft = attachedEffect.RemainingTime;

        if (attachedEffect is IUniqueStackableEffect usEffect) {
            UpdateStacks(usEffect.StackAmount);
            StacksLabel.Visible = true;
        }
        else {
            StacksLabel.Text = "";
            StacksLabel.Visible = false;
        }

        if (attachedEffect is IRepeatableEffect) {
            ShouldShowInstancesInsteadOfTime = true;
        }

        isConfigured = true;
    }

    private void OverrideBackground(EEffectRating effectRating) {
        Color bgColor;

        switch (effectRating) {
            case EEffectRating.Positive:
                bgColor = UILib.ColorStatusPositive;
                break;
            
            case EEffectRating.Negative:
                bgColor = UILib.ColorStatusNegative;
                break;

            default:
                bgColor = UILib.ColorStatusNeutral;
                break;
        }

        StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
        styleBoxFlat.BgColor = bgColor;
		AddThemeStyleboxOverride("panel", styleBoxFlat);
    }

    public void OverrideTimeLeft(double time) {
        TimeLeft = time;
    }

    public void UpdateStacks(int stacks) {
        Stacks = stacks;
        StacksLabel.Text = Stacks.ToString();
    }

    public void UpdateInstances(int instances) {
        StatusLabel.Text = instances.ToString();
    }

    public override string _GetTooltip(Vector2 atPosition) {
        return $"{EffectNameString}\n{EffectDescString}";
    }
}
