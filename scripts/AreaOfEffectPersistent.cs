using Godot;
using System;
using System.Collections.Generic;

public partial class AreaOfEffectPersistent : Area3D {
    public delegate void TargetsAffectedEventHandler(List<Actor> targets);
    public event TargetsAffectedEventHandler TargetsAffected;
    
    protected CollisionShape3D hitboxCollision;
    protected SphereShape3D collisionShape;
    public AnimatedSprite3D AreaAnimation;

    public double Lifetime { get; protected set; } = 0;
    protected double remainingLifetime = 0;

    public double UpdateInterval { get; protected set; } = 0.1;
    protected double updateTimer = 0;

    protected List<Actor> affectedTargets = new();

    public override void _Ready() {
        hitboxCollision = GetNode<CollisionShape3D>("HitCollision");
        collisionShape = hitboxCollision.Shape as SphereShape3D;
        AreaAnimation = GetNode<AnimatedSprite3D>("AreaAnimation");
    }

    public override void _PhysicsProcess(double delta) {
        Tick(delta);
    }

    public virtual void Tick(double delta) {
        if (remainingLifetime > 0) {
            if (updateTimer <= 0) {
                if (affectedTargets.Count > 0) {
                    TargetsAffected?.Invoke(affectedTargets);
                }
                updateTimer += UpdateInterval;
            }

            remainingLifetime -= delta;
            updateTimer -= delta;
        }
        else {
            QueueFree();
        }
    }

    public void SetProperties(double lifetime, float newRadius, string animationName, float animationPixelSize, float animationScale) {
        Lifetime = lifetime;
        remainingLifetime = Lifetime;
        updateTimer = UpdateInterval;
        
        collisionShape.Radius = newRadius;

        AreaAnimation.PixelSize = animationPixelSize;
        Vector3 scale = AreaAnimation.Scale;
        scale.X *= animationScale;
        scale.Z *= animationScale;
        AreaAnimation.Scale = scale;

        if (animationName != null) {
            AreaAnimation.Animation = animationName;
            AreaAnimation.Visible = true;
            AreaAnimation.Play();
        }
    }

    protected virtual void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            Actor target = body as Actor;
            affectedTargets.Add(target);
        }
    }

    protected virtual void OnBodyExited(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            Actor target = body as Actor;
            affectedTargets.Remove(target);
        }
    }
}
