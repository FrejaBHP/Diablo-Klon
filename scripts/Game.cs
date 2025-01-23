using Godot;
using System;

public partial class Game : Node3D {
	private Player player;
	private PlayerCamera playerCam;

	public override void _Ready() {
		player = GetNode<CharacterBody3D>("Player") as Player;
		playerCam = GetNode<Camera3D>("PlayerCamera") as PlayerCamera;

		player.AssignCamera(playerCam);
		playerCam.AssignPlayer(player);
	}
}
