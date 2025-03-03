using Godot;
using System;

public partial class ItemTooltipPopup : PopupPanel {

	public override void _Ready() {
		//GD.Print($"First size check: {Size}");
		//GD.Print($"First min size check: {GetContentsMinimumSize()}");
		AdjustPopupPosition();
	}

	public override void _Process(double delta) {
	
	}

	private async void AdjustPopupPosition() {
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		//await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		Vector2 correctedPosition = Position;
		correctedPosition.X -= GetContentsMinimumSize().X / 2;
		correctedPosition.Y -= GetContentsMinimumSize().Y;
		Position = (Vector2I)correctedPosition;

		//GD.Print($"Second size check: {Size}");
		//GD.Print($"Second min size check: {GetContentsMinimumSize()}");
	}
}
