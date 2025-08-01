using Godot;
using System;
using System.Collections.Generic;

public struct MapTags(EMapType type, List<EMapObjective> objectives) {
    public readonly EMapType Type = type;
    public readonly List<EMapObjective> Objectives = objectives;
}

public static class MapDatabase {
    public static readonly PackedScene TownScene = GD.Load<PackedScene>("res://scenes/world/map_town.tscn");
    public static readonly PackedScene FEScene = GD.Load<PackedScene>("res://scenes/world/map_FirstEncounter.tscn");
    public static readonly PackedScene ShopSmallTownScene = GD.Load<PackedScene>("res://scenes/world/map_shop_smalltown.tscn");

    public static MapBase GetMap(PackedScene scene) {
        MapBase newMap = scene.Instantiate<MapBase>();
        return newMap;
    }

    public static void GetMap(PackedScene scene, out MapBase map, out MapTags tags) {
        map = scene.Instantiate<MapBase>();
        tags = mapCatalogue[scene];
    }

    private static readonly Dictionary<PackedScene, MapTags> mapCatalogue = new() {
        { TownScene, new(EMapType.Town, [EMapObjective.None]) },
        { FEScene, new(EMapType.Objective, [EMapObjective.Survival, EMapObjective.Waves]) },
        { ShopSmallTownScene, new(EMapType.Intermission, [EMapObjective.None]) },
    };
}
