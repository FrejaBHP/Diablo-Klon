using Godot;
using System;

public static class MapDatabase {
    public static readonly PackedScene TownScene = GD.Load<PackedScene>("res://scenes/world/map_town.tscn");
    public static readonly PackedScene FEScene = GD.Load<PackedScene>("res://scenes/world/map_firstEncounter.tscn");

    public static MapBase GetMap(PackedScene scene) {
        MapBase newMap = scene.Instantiate<MapBase>();
        return newMap;
    }
}
