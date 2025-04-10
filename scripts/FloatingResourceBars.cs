using Godot;
using System;

public partial class FloatingResourceBars : Control {
    private TextureProgressBar lifeBar;
    private TextureProgressBar manaBar;

    private bool isLifeHidden = false;
    private bool isManaHidden = false;

    private Camera3D camera;
	private Marker3D parentAnchor;
    private const int margin = 8;

    public override void _Ready() {
        lifeBar = GetNode<TextureProgressBar>("LifeBar");
        manaBar = GetNode<TextureProgressBar>("ManaBar");

        camera = GetViewport().GetCamera3D();
		parentAnchor = GetParent<Marker3D>();
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
			Mathf.Clamp(unprojectedPosition.X - (Size.X / 2), margin, viewportBaseSize.X - margin),
			Mathf.Clamp(unprojectedPosition.Y, margin, viewportBaseSize.Y - margin)
		);

		Visible = true;
		Rotation = 0;
	}

    public void SetLifePercentage(double percentage) {
        lifeBar.Value = percentage * 100;

        if (lifeBar.Value == 100) {
            lifeBar.Visible = false;
        }
        else {
            lifeBar.Visible = true;
        }
    }

    public void SetManaPercentage(double percentage) {
        manaBar.Value = percentage * 100;

        if (!isManaHidden) {
            if (manaBar.Value == 100) {
                manaBar.Visible = false;
            }
            else {
                manaBar.Visible = true;
            }
        }
    }

    public void SetManaBarVisibility(bool visible) {
        if (!visible) {
            manaBar.Visible = false;
            isManaHidden = true;
        }
        else {
            isManaHidden = false;
        }
    }
}
