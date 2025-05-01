using Godot;
using System;

public partial class MapBase : Node3D {
    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }

    public override void _Ready() {
        NavRegion = GetNode<NavigationRegion3D>("NavigationRegion3D");

        CallDeferred(MethodName.AddRegionToNav);
    }

    public void AddRegionToNav() {
        NavigationServer3D.RegionSetMap(NavRegion.GetRid(), GetWorld3D().NavigationMap);
    }
}
