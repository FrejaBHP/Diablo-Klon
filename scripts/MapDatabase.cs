using Godot;
using System;
using System.Collections.Generic;

public static class MapDatabase {
    public static readonly PackedScene TownScene = GD.Load<PackedScene>("res://scenes/world/map_town.tscn");
    public static readonly PackedScene FEScene = GD.Load<PackedScene>("res://scenes/world/map_firstEncounter.tscn");
    public static readonly PackedScene ShopSmallTownScene = GD.Load<PackedScene>("res://scenes/world/map_shop_smalltown.tscn");

    public static MapBase GetMap(PackedScene scene) {
        MapBase newMap = scene.Instantiate<MapBase>();
        return newMap;
    }

    public static void GetMapTest(PackedScene scene, out MapBase map, out EMapObjective objective) {
        map = scene.Instantiate<MapBase>();
        int randomObjective = Utilities.RNG.Next(mapMap[scene].Count);
        objective = mapMap[scene][randomObjective];
    }

    private static readonly Dictionary<PackedScene, List<EMapObjective>> mapMap = new() {
        { TownScene, new() { EMapObjective.None } },
        { FEScene, new() { EMapObjective.Survival } },
        { ShopSmallTownScene, new() { EMapObjective.Shop } },
    };
}
