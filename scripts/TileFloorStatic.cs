using Godot;
using System;

[Tool]
public partial class TileFloorStatic : StaticBody3D {
	[Export]
	//private Texture2D tileTexture;
	public TileTexture TextureIndex { get; private set; }
	private MeshInstance3D meshInstance;

	public override void _Ready() {
		meshInstance = GetNode<MeshInstance3D>("TileMesh");
		meshInstance.Mesh.SurfaceSetMaterial(0, TileTextureLib.TileMaterial[(int)TextureIndex]);
		//StandardMaterial3D mat = meshInstance.Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
		//mat.AlbedoTexture = TileTextureLib.TileTextures[(int)textureIndex];
	}
}
