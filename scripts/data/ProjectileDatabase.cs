using Godot;
using System;
using System.Collections.Generic;

public static class ProjectileDatabase {
    public static readonly PackedScene GenericProjectileScene = GD.Load<PackedScene>("res://scenes/skills/scene_projectile_generic.tscn");
    public static readonly PackedScene SoulrendProjectileScene = GD.Load<PackedScene>("res://scenes/skills/scene_projectile_soulrend.tscn");

    public static Projectile GetProjectile(ESkillProjectileType projectileType) {
        Projectile newProjectile;

        if (projectileCatalogue.TryGetValue(projectileType, out PackedScene projScene)) {
            newProjectile = projScene.Instantiate<Projectile>();
        }
        else {
            GD.PrintErr("Projectile Type not found in catalogue");
            newProjectile = GenericProjectileScene.Instantiate<Projectile>();
        }
        
        return newProjectile;
    }

    private static readonly Dictionary<ESkillProjectileType, PackedScene> projectileCatalogue = new() {
        { ESkillProjectileType.Default, GenericProjectileScene },
        { ESkillProjectileType.Soulrend, SoulrendProjectileScene },
    };
}
