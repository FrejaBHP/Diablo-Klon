using Godot;
using System;

public partial class MapBase : Node3D {
    [Signal]
    public delegate void MapReadyEventHandler();

    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }
    public Marker3D PlayerSpawnMarker { get; protected set; }

    // For the future when item level will be set and matter
    // Maybe I'll even add enemy scaling
    public int AreaLevel = 1;

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
}
