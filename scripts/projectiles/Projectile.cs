using Godot;
using System;
using System.Collections.Generic;

public partial class Projectile : Node3D {
    public delegate void TargetHitEventHandler(Actor target);
    public event TargetHitEventHandler TargetHit;

    public Area3D Hitbox { get; protected set; }
    public Sprite3D ProjectileSprite { get; protected set; }
    private CollisionShape3D hitboxCollision;
    private CapsuleShape3D collisionCapsule;

    private Vector3 direction;

    public int Pierces { get; protected set; } = 0;
    public float ProjSpeed { get; protected set; } = 0f;
    public double ProjLifetime { get; protected set; } = 0;
    public double ProjTimeAlive { get; protected set; } = 0;
    public float Range { get; protected set; } = 0f;
    public float DistanceTravelled { get; protected set; } = 0f;
    public bool CanShotgun { get; protected set; } = false;

    public bool CanHitActors { get; set; } = true;

    public HashSet<Actor> ActorsHitThisGroup { get; set; }

    public override void _Ready() {
        Hitbox = GetNode<Area3D>("Hitbox");
        hitboxCollision = Hitbox.GetNode<CollisionShape3D>("HitCollision");
        collisionCapsule = hitboxCollision.Shape as CapsuleShape3D;
        ProjectileSprite = Hitbox.GetNode<Sprite3D>("ProjectileSprite");
    }

    public override void _PhysicsProcess(double delta) {
        if (!ShouldExpire()) {
            MoveStraightAhead(delta);
            ProjTimeAlive += delta;
        }
    }

    protected bool ShouldExpire() {
        if (Range > 0f) {
            if (DistanceTravelled >= Range) {
                QueueFree();
                return true;
            }
        }
        else if (ProjLifetime > 0) {
            if (ProjTimeAlive >= ProjLifetime) {
                QueueFree();
                return true;
            }
        }
        
        return false;
    }

    protected void MoveStraightAhead(double delta) {
        Vector3 newPos = GlobalPosition;
        float distanceCovered = (float)(ProjSpeed * delta);
        newPos += direction * distanceCovered;
        GlobalPosition = newPos;
        DistanceTravelled += distanceCovered;
    }

    public void SetFacing(float angle) {
        RotateY(angle);
        direction = Transform.Basis * new Vector3(0f, 0f, 1f).Normalized();
    }

    public void SetFacing(Vector3 position) {
        LookAt(position, Vector3.Up, true);
        direction = Transform.Basis * new Vector3(0f, 0f, 1f).Normalized();
    }

    public void SetProperties(float speed = 10f, double lifetime = 0, int pierce = 0, float range = 15f, bool canShotgun = false) {
        ProjSpeed = speed;
        Pierces = pierce;
        ProjLifetime = lifetime;
        Range = range;
        CanShotgun = canShotgun;
    }

    protected virtual void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            Actor target = body as Actor;

            if (ActorsHitThisGroup != null && !CanShotgun) {
                if (ActorsHitThisGroup.Add(target)) {
                    OnCollideWithTarget(target);
                }
            }
            else {
                OnCollideWithTarget(target);
            }
        }
        else {
            QueueFree();
        }
    }

    protected virtual void OnCollideWithTarget(Actor actor) {
        if (CanHitActors) {
            SignalTargetHit(actor);
        
            if (Pierces < 1) {
                QueueFree();
            }
            else {
                Pierces--;
            }
        }
    }

    protected void SignalTargetHit(Actor actor) {
        TargetHit?.Invoke(actor);
    }

    protected virtual void OnBodyExited(Node3D body) {
        
    }
}
