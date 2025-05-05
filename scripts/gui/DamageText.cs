using Godot;
using System;

public partial class DamageText : Label3D {
    private Tween tween;

    public void Start() {
        tween = CreateTween();
        tween.Finished += OnTweenFinished;

        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "position", new Vector3(0f, 0.75f, 0f), 0.6).AsRelative();
        tween.Play();
    }

    private void OnTweenFinished() {
        QueueFree();
    }
}
