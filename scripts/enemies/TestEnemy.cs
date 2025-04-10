using Godot;
using System;

public partial class TestEnemy : EnemyBase {
    public TestEnemy() {
		BasicStats.BaseLife = 20;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();
    }

    public override void _Ready() {
        base._Ready();
        GD.Print("TestEnemy _Ready() executed");
    }


    public override void _PhysicsProcess(double delta) {
        ApplyRegen();
    }

}
