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

    // Comments and explanation can be found in FloatingLabel.cs, since this is just copied from there
	public override void _Process(double delta) {
		if (!camera.Current) {
			camera = GetViewport().GetCamera3D();
		}

		Vector3 parentAnchorPosition = parentAnchor.GlobalTransform.Origin;
		Vector3 cameraPosition = camera.GlobalTransform.Origin;

		bool isBehind = camera.Transform.Basis.Z.Dot(parentAnchorPosition - cameraPosition) > 0;

		Vector2 unprojectedPosition = camera.UnprojectPosition(parentAnchorPosition);
		Vector2I viewportBaseSize;

		if (GetViewport().GetWindow().ContentScaleSize > Vector2I.Zero) {
			viewportBaseSize = GetViewport().GetWindow().ContentScaleSize;
		}
		else {
			viewportBaseSize = GetViewport().GetWindow().Size;
		}

		if (isBehind) {
			if (unprojectedPosition.X < viewportBaseSize.X / 2) {
				unprojectedPosition.X = viewportBaseSize.X - margin;
			}
			else {
				unprojectedPosition.X = margin;
			}
		}

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
