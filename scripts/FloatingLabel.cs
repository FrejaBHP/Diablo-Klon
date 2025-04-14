using Godot;
using System;

public partial class FloatingLabel : Control {
	public bool IsSticky = false;

	private const int margin = 8;

	private PanelContainer labelContainer;
	private ColorRect labelBackground;
	private Label nameLabel;
	private Label typeLabel;

	private Camera3D camera;
	private Marker3D parentAnchor;

	private Color primaryColour;
	private Color hoveredColour;
	private Color textColour;
	private bool isHovered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		labelContainer = GetNode<PanelContainer>("Container");
		labelBackground = labelContainer.GetNode<ColorRect>("LabelBackground");
		nameLabel = labelContainer.GetNode<Label>("MarginContainer/VBoxContainer/NameLabel");
		typeLabel = labelContainer.GetNode<Label>("MarginContainer/VBoxContainer/TypeLabel");

		camera = GetViewport().GetCamera3D();
		parentAnchor = GetParent<Marker3D>();
	}

	public void OnClicked(InputEvent @event) {
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			Player player = camera.GetParent<Player>();
			if (!player.PlayerHUD.PlayerInventory.IsAnItemSelected) {
				player.SetDestinationNode(parentAnchor.GetParent<StaticBody3D>());
			}
		}
	}

	public void OnMouseEntered() {
		isHovered = true;
		labelBackground.Color = hoveredColour;
		//GD.Print("Mouse entered!");
	}

	public void OnMouseExited() {
		isHovered = false;
		labelBackground.Color = primaryColour;
		//GD.Print("Mouse exited!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// Oversat fra gdscript via https://github.com/godotengine/godot-demo-projects/tree/master/3d/waypoints

		// If the camera we have isn't the current one, get the current camera.
		if (!camera.Current) {
			camera = GetViewport().GetCamera3D();
		}

		Vector3 parentAnchorPosition = parentAnchor.GlobalTransform.Origin;
		Transform3D cameraTransform = camera.GlobalTransform;
		Vector3 cameraPosition = camera.GlobalTransform.Origin;

		// We would use "camera.is_position_behind(parent_position)", except
		// that it also accounts for the near clip plane, which we don't want.
		bool isBehind = camera.Transform.Basis.Z.Dot(parentAnchorPosition - cameraPosition) > 0;

		// `get_size_override()` will return a valid size only if the stretch mode is `2d`.
		// Otherwise, the viewport size is used directly.
		Vector2 unprojectedPosition = camera.UnprojectPosition(parentAnchorPosition);
		Vector2I viewportBaseSize;

		if (GetViewport().GetWindow().ContentScaleSize > Vector2I.Zero) {
			viewportBaseSize = GetViewport().GetWindow().ContentScaleSize;
		}
		else {
			viewportBaseSize = GetViewport().GetWindow().Size;
		}

		// For non-sticky waypoints, we don't need to clamp and calculate
		// the position if the waypoint goes off screen.
		if (!IsSticky) {
			Position = unprojectedPosition;
			Visible = !isBehind;
		}

		// We need to handle the axes differently.
		// For the screen's X axis, the projected position is useful to us,
		// but we need to force it to the side if it's also behind.
		if (isBehind) {
			if (unprojectedPosition.X < viewportBaseSize.X / 2) {
				unprojectedPosition.X = viewportBaseSize.X - margin;
			}
			else {
				unprojectedPosition.X = margin;
			}
		}

		// For the screen's Y axis, the projected position is NOT useful to us
		// because we don't want to indicate to the user that they need to look
		// up or down to see something behind them. Instead, here we approximate
		// the correct position using difference of the X axis Euler angles
		// (up/down rotation) and the ratio of that with the camera's FOV.
		// This will be slightly off from the theoretical "ideal" position.
		if (isBehind || unprojectedPosition.X < margin || unprojectedPosition.X > viewportBaseSize.X - margin) {
			Transform3D look = camera.Transform.LookingAt(parentAnchorPosition, Vector3.Up);
			float diff = Mathf.AngleDifference(look.Basis.GetEuler().X, camera.Transform.Basis.GetEuler().X);
			unprojectedPosition.Y = (float)(viewportBaseSize.Y * (0.5 + (diff / Mathf.DegToRad(camera.Fov))));
		}

		Position = new Vector2(
			Mathf.Clamp(unprojectedPosition.X, margin, viewportBaseSize.X - margin),
			Mathf.Clamp(unprojectedPosition.Y, margin, viewportBaseSize.Y - margin)
		);

		labelContainer.Visible = true;
		Rotation = 0;

		// Used to display a diagonal arrow when the waypoint is displayed in
		// one of the screen corners.
		int overflow = 0;

		if (Position.X <= margin) {
			// Left overflow
			overflow = (int)(Mathf.Tau / 8);
			labelContainer.Visible = false;
			Rotation = Mathf.Tau / 4;
		}
		else if (Position.X >= viewportBaseSize.X - margin) {
			// Right overflow
			overflow = (int)Mathf.Tau / 8;
			labelContainer.Visible = false;
			Rotation = Mathf.Tau * 3 / 4;
		}

		if (Position.Y <= margin) {
			// Top overflow
			labelContainer.Visible = false;
			Rotation = Mathf.Tau / 2 + overflow;
		}
		else if (Position.Y >= viewportBaseSize.Y - margin) {
			// Bottom overflow
			labelContainer.Visible = false;
			Rotation = -overflow;
		}
	}

	public void SetLabelText(string name, string type) {
		nameLabel.Text = name;
		typeLabel.Text = type;
	}

	public void ApplyColourSet(ELabelColourSet set) {
		switch (set) {
			case ELabelColourSet.Default:
				primaryColour.R8 = 55;
				primaryColour.G8 = 55;
				primaryColour.B8 = 55;
				primaryColour.A8 = 175;

				hoveredColour.R8 = 85;
				hoveredColour.G8 = 85;
				hoveredColour.B8 = 85;
				hoveredColour.A8 = 175;
				break;

			case ELabelColourSet.Item:
				primaryColour.R8 = 65;
				primaryColour.G8 = 65;
				primaryColour.B8 = 65;
				primaryColour.A8 = 175;

				hoveredColour.R8 = 95;
				hoveredColour.G8 = 95;
				hoveredColour.B8 = 95;
				hoveredColour.A8 = 175;
				break;
			
			default:
				break;
		}

		if (isHovered) {
			labelBackground.Color = hoveredColour;
		}
		else {
			labelBackground.Color = primaryColour;
		}
	}

	public void ApplyTextColour(ETextColour colour) {
		switch (colour) {
			case ETextColour.Default:
				textColour = UILib.ColorWhite;
				break;

			case ETextColour.Common:
				textColour = UILib.ColorWhite;
				break;

			case ETextColour.Magic:
				textColour = UILib.ColorMagic;
				break;
			
			case ETextColour.Rare:
				textColour = UILib.ColorRare;
				// If item is of Rare rarity, show type, as random name obscures it
				typeLabel.Visible = true;
				break;

			case ETextColour.Unique:
				textColour = UILib.ColorUnique;
				//typeLabel.Visible = true;
				break;

			case ETextColour.Skill:
				textColour = UILib.ColorSkill;
				break;
			
			default:
				break;
		}

		nameLabel.AddThemeColorOverride("font_color", textColour);
		typeLabel.AddThemeColorOverride("font_color", textColour);
	}
}
