using Godot;
using System;
using System.Text;

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
        StringBuilder sb = new();
        sb.Append($"Act: {Run.Instance.CurrentAct}, ");
        sb.Append($"Area: {Run.Instance.CurrentArea}\n");
        sb.Append($"Area Level: {Run.Instance.CurrentMap.LocalAreaLevel}\n");
        sb.Append($"Scaling L/D: {1 * Math.Pow(Run.Instance.Rules.EnemyLifeScalingFactor, Run.Instance.CurrentMap.LocalAreaLevel - 1):P1}, ");
        sb.Append($"{1 * Math.Pow(Run.Instance.Rules.EnemyDamageScalingFactor, Run.Instance.CurrentMap.LocalAreaLevel - 1):P1}");
        ProgressLabel.Text = sb.ToString();
    }

    public void UpdateEnemyDebugLabel(double denMod, int spawnAmount) {
        EnemyDebugLabel.Text = $"Den. Mod: {denMod:F2}, Amount: {spawnAmount}";
    }
}
