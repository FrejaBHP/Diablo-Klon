using Godot;
using System;

public partial class ItemTooltipPopup : PopupPanel {
	public Rect2 ItemRect;
	public bool RightSide;

	public override void _Ready() {
		AdjustPopupPosition();
	}

	private async void AdjustPopupPosition() {
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		Vector2 correctedPosition = Position;
		correctedPosition.X -= GetContentsMinimumSize().X / 2;
		correctedPosition.Y -= GetContentsMinimumSize().Y;

		Vector2I windowSize = GetTree().Root.GetViewport().GetWindow().Size;

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

			// If tooltip would now cover the hovered item
			if (correctedPosition.Y + GetContentsMinimumSize().Y > ItemRect.Position.Y) {
				float distanceToMove = ItemRect.Size.X - (correctedPosition.X - ItemRect.Position.X);

				// If part of the right side of the window, such as the inventory panel, move it to the left
				if (RightSide) {
					correctedPosition.X -= distanceToMove;
				}
				// Otherwise, move it to the right
				else {
					correctedPosition.X += distanceToMove;
				}
			}
		}
		// If tooltip would extend below the window
		else if (correctedPosition.Y + GetContentsMinimumSize().Y > windowSize.Y) {
			correctedPosition.Y += windowSize.Y - (correctedPosition.Y + GetContentsMinimumSize().Y);
		}

		Position = (Vector2I)correctedPosition;
	}
}
