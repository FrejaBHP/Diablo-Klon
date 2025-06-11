using Godot;
using System;

public partial class RightHUD : Control {
    public Label ProgressLabel { get; protected set; }
    public VBoxContainer ObjectiveContainer { get; protected set; }

    public override void _Ready() {
        ProgressLabel = GetNode<Label>("ProgressLabel");
        ObjectiveContainer = GetNode<VBoxContainer>("ObjectiveContainer");
    }

    public void UpdateProgressLabel() {
        ProgressLabel.Text = $"Act: {Game.Instance.CurrentAct}, Area: {Game.Instance.CurrentArea}";
    }
}
