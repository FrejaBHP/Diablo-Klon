using Godot;

public enum TileTexture {
    NoTexture,
    RockMosaic,
    Snow,
    StoneBrick
}

public static class TileTextureLib {
    public static readonly StandardMaterial3D[] TileMaterial = {
        null,
        GD.Load<StandardMaterial3D>("res://textures/world/rock_mosaic.tres"),
        GD.Load<StandardMaterial3D>("res://textures/world/snow.tres"),
        GD.Load<StandardMaterial3D>("res://textures/world/stone_brick.tres")
    };
}
