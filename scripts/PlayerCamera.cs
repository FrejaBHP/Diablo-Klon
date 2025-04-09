using Godot;
using System;

public partial class PlayerCamera : Camera3D {
	private Player player;
	private bool followsPlayer = false;

	private Vector3 offset = new(10, 10, 10);

	public void AssignPlayer(Player p) {
		player = p;
		followsPlayer = true;
	}

	public override void _Ready() {
		
	}

	public override void _PhysicsProcess(double delta) {
		if (followsPlayer) {
			GlobalPosition = GlobalPosition.Lerp(player.GlobalPosition + offset, 0.5f);
		}
	}
}
