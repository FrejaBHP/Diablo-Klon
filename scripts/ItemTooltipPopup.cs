using Godot;
using System;

public partial class ItemTooltipPopup : PopupPanel {

	public override void _Ready() {
		//GD.Print($"First size check: {Size}");
		//GD.Print($"First min size check: {GetContentsMinimumSize()}");
		AdjustPopupPosition();
	}

	private async void AdjustPopupPosition() {
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		Vector2 correctedPosition = Position;
		correctedPosition.X -= GetContentsMinimumSize().X / 2;
		correctedPosition.Y -= GetContentsMinimumSize().Y;

		Vector2I windowSize = GetTree().Root.GetViewport().GetWindow().Size;

		//GD.Print($"Tooltip X: {correctedPosition.X} - {correctedPosition.X + GetContentsMinimumSize().X}");
		//GD.Print($"Tooltip Y: {correctedPosition.Y} - {correctedPosition.Y + GetContentsMinimumSize().Y}");

		// If tooltip would extend past the right side of the window
		if (correctedPosition.X + GetContentsMinimumSize().X > windowSize.X) {
			correctedPosition.X += windowSize.X - (correctedPosition.X + GetContentsMinimumSize().X);
		}
		// If tooltip would extend past the left side of the window
		else if (correctedPosition.X < 0) {
			correctedPosition.X -= correctedPosition.X;
		}

		// If tooltip would extend above the window
		if (correctedPosition.Y < 0) {
			correctedPosition.Y -= correctedPosition.Y;
		}
		// If tooltip would extend below the window
		else if (correctedPosition.Y + GetContentsMinimumSize().Y > windowSize.Y) {
			correctedPosition.Y += windowSize.Y - (correctedPosition.Y + GetContentsMinimumSize().Y);
		}

		Position = (Vector2I)correctedPosition;

		//GD.Print($"Viewport size: {GetTree().Root.GetViewport().GetWindow().Size}");
		//GD.Print($"Tooltip X: {Position.X} - {Position.X + GetContentsMinimumSize().X}");
		//GD.Print($"Tooltip Y: {Position.Y} - {Position.Y + GetContentsMinimumSize().Y}");
		//GD.Print($"Position: {Position}");
		//GD.Print($"Second min size check: {GetContentsMinimumSize()}");
	}
}
