using Godot;
using System;
using System.Collections.Generic;

public partial class AreaOfEffectInstant : Node3D {
    public delegate void TargetsSweptEventHandler(List<Actor> targets);
    public event TargetsSweptEventHandler TargetsSwept;

    public ShapeCast3D ShapeCastSweep { get; protected set; }
    protected CylinderShape3D collisionShape;
    public AnimatedSprite3D AreaAnimation;
    
    public override void _Ready() {
        ShapeCastSweep = GetNode<ShapeCast3D>("ShapeCastSweep");
        collisionShape = ShapeCastSweep.Shape as CylinderShape3D;
        AreaAnimation = GetNode<AnimatedSprite3D>("AreaAnimation");
    }

    public void SetProperties(float newRadius, string animationName, float animationPixelSize, float animationScale) {
        collisionShape.Radius = newRadius;

        AreaAnimation.PixelSize = animationPixelSize;
        Vector3 scale = AreaAnimation.Scale;
        scale.X *= animationScale;
        scale.Z *= animationScale;
        AreaAnimation.Scale = scale;

        AreaAnimation.Animation = animationName;
        AreaAnimation.AnimationFinished += OnAnimationEnd;
    }

    public async void Sweep() {
        AreaAnimation.Visible = true;
        AreaAnimation.Play();

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        ShapeCastSweep.ForceShapecastUpdate();

        List<Actor> affectedTargets = new();

        int amountOfCollisions = ShapeCastSweep.GetCollisionCount();
        for (int i = 0; i < amountOfCollisions; i++) {
            affectedTargets.Add(ShapeCastSweep.GetCollider(i) as Actor);
        }
        
        TargetsSwept?.Invoke(affectedTargets);
    }

    protected void OnAnimationEnd() {
        QueueFree();
    }
}
