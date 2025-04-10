using Godot;
using System;

public partial class EnemyBase : Actor {
    private Marker3D resBarAnchor;

    public override void _Ready() {
        base._Ready();

        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        AddFloatingBars(resBarAnchor);

        AddToGroup("Enemy");

        GD.Print("EnemyBase _Ready() executed");
    }
}
