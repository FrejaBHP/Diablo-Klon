using Godot;
using System;

public partial class RightHUD : Control {
    public Label ProgressLabel { get; protected set; }
    public VBoxContainer ObjectiveContainer { get; protected set; }

    public Label EnemyDebugLabel { get; protected set; }

    public override void _Ready() {
        ProgressLabel = GetNode<Label>("ProgressLabel");
        ObjectiveContainer = GetNode<VBoxContainer>("ObjectiveContainer");

        EnemyDebugLabel = GetNode<Label>("EnemyDebugLabel");
    }

    public void UpdateProgressLabel() {
        ProgressLabel.Text = $"Act: {Game.Instance.CurrentAct}, Area: {Game.Instance.CurrentArea}\nArea Level: {Game.Instance.CurrentMap.AreaLevel}, Scaling: {1 * Math.Pow(Game.EnemyScalingFactor, Game.Instance.CurrentMap.AreaLevel - 1)}";
    }

    public void UpdateEnemyDebugLabel(double denMod, int spawnAmount) {
        EnemyDebugLabel.Text = $"Den. Mod: {denMod:F2}, Amount: {spawnAmount}";
    }
}
