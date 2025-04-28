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
        MovementSpeed.SBase = 5;
    }

    public override void _PhysicsProcess(double delta) {
        ApplyRegen();
    }
}
