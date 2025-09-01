using Godot;
using System;

public partial class HUDStatusIcon : PanelContainer {
    public TextureRect StatusIcon { get; protected set; }
    public Label StatusLabel { get; protected set; }

    public EEffectName EffectName { get; protected set; }
    public string EffectNameString { get; protected set; }
    public string EffectDescString { get; protected set; }

    public int Stacks { get; protected set; } = 1;
    public double TimeLeft { get; protected set; }

    public bool ShouldDisplayStacksInstead { get; protected set; } = false;
    private bool isConfigured = false;

    public override void _Ready() {
        StatusIcon = GetNode<TextureRect>("MarginContainer/VBoxContainer/StatusIcon");
        StatusLabel = GetNode<Label>("MarginContainer/VBoxContainer/CenterContainer/StatusLabel");
    }

    public override void _PhysicsProcess(double delta) {
        if (isConfigured) {
            if (!ShouldDisplayStacksInstead) {
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

            TimeLeft -= delta;
        }
    }

    public void SetStatusType(AttachedEffect attachedEffect) {
        EffectName = attachedEffect.EffectName;
        StatusIcon.Texture = attachedEffect.EffectIcon;
        EffectNameString = attachedEffect.EffectString;
        EffectDescString = attachedEffect.EffectDescription;
        TimeLeft = attachedEffect.RemainingTime;

        if (attachedEffect is IUniqueStackableEffect usEffect) {
            Stacks = usEffect.StackAmount;
        }

        if (Stacks > 1) {
            ShouldDisplayStacksInstead = true;
        }

        isConfigured = true;
    }

    public void OverrideTimeLeft(double time) {
        TimeLeft = time;
    }

    public override string _GetTooltip(Vector2 atPosition) {
        return $"{EffectNameString}\n{EffectDescString}";
    }
}
