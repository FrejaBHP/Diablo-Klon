using Godot;
using System;

public partial class TestEnemy : EnemyBase {
    public TestEnemy() {
		BasicStats.BaseLife = 40;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();
    }

    public override void _Ready() {
        base._Ready();
        MovementSpeed.SBase = 4;
        Evasion.SBase = 67;
    }

    public override void _PhysicsProcess(double delta) {
        ProcessNavigation();
        ApplyRegen();

        //debugLabel.Text = $"Next Path Pos: {navAgent.GetNextPathPosition().ToString("F2")}\nTarget Pos: {navAgent.TargetPosition.ToString("F2")}\nVelocity Length: {Velocity.Length():F2}";
        //debugLabel.Text = $"Rotation: {GlobalRotationDegrees.ToString("F2")}\nVelocity Length: {Velocity.Length():F2}";
    }
}
