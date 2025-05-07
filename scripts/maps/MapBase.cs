using Godot;
using System;

public partial class MapBase : Node3D {
    protected static readonly PackedScene testEnemyScene = GD.Load<PackedScene>("res://scenes/enemy_test.tscn");

    [Signal]
    public delegate void MapReadyEventHandler();

    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }
    public Marker3D PlayerSpawnMarker { get; protected set; }

    public override void _Ready() {
        NavRegion = GetNode<NavigationRegion3D>("NavigationRegion3D");
        EnemiesNode = GetNode<Node3D>("Enemies");
        PlayerSpawnMarker = GetNode<Marker3D>("PlayerSpawn");

        CallDeferred(MethodName.AddRegionToNav);
        CallDeferred(MethodName.OnMapReady);
    }

    public void ClearEnemies() {
        foreach (Node enemy in EnemiesNode.GetChildren()) {
            enemy.QueueFree();
        }
    }

    public void AddRegionToNav() {
        NavigationServer3D.RegionSetMap(NavRegion.GetRid(), GetWorld3D().NavigationMap);
    }

    protected void OnMapReady() {
        EmitSignal(SignalName.MapReady);
    }

    public void Test() {
        Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
        TestEnemy testEnemy = testEnemyScene.Instantiate<TestEnemy>();
        EnemiesNode.AddChild(testEnemy);
        testEnemy.GlobalPosition = spawnPos;
    }
}
