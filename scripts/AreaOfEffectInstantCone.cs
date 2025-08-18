using Godot;
using System;
using System.Collections.Generic;

public partial class AreaOfEffectInstantCone : Node3D {
    public delegate void TargetsSweptEventHandler(List<Actor> targets);
    public event TargetsSweptEventHandler TargetsSwept;

    public ShapeCast3D ShapeCastSweep { get; protected set; }
    protected CylinderShape3D collisionShape;
    public AnimatedSprite3D AreaAnimation;
    public float ConeDotMinimum { get; protected set; }
    
    public override void _Ready() {
        ShapeCastSweep = GetNode<ShapeCast3D>("ShapeCastSweep");
        collisionShape = ShapeCastSweep.Shape as CylinderShape3D;
        AreaAnimation = GetNode<AnimatedSprite3D>("AreaAnimation");
    }

    public void SetProperties(float newRadius, string animationName, float animationPixelSize, float animationScale, float height, Vector3 direction, float coneDotMinimum) {
        collisionShape.Radius = newRadius;
        ConeDotMinimum = coneDotMinimum;

        AreaAnimation.Position = AreaAnimation.Position with { Y = height, Z = -collisionShape.Radius / 2 };
        AreaAnimation.PixelSize = animationPixelSize;
        Vector3 scale = AreaAnimation.Scale;
        scale.X *= animationScale;
        scale.Z *= animationScale;
        AreaAnimation.Scale = scale;

        Vector3 lookatDirection = direction with { Y = height };

        LookAt(lookatDirection);

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
            Actor actor = ShapeCastSweep.GetCollider(i) as Actor;

            Vector3 facing = -GlobalTransform.Basis.Z;
            Vector3 flat = new(actor.GlobalPosition.X, GlobalPosition.Y, actor.GlobalPosition.Z);
            Vector3 target = GlobalPosition.DirectionTo(flat);

            if (facing.Dot(target) >= ConeDotMinimum) {
                affectedTargets.Add(actor);
            }
        }
        
        TargetsSwept?.Invoke(affectedTargets);
    }

    protected void OnAnimationEnd() {
        QueueFree();
    }
}
