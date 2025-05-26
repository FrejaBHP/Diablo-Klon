using Godot;
using System;

public partial class Projectile : Node3D {
    public Area3D Hitbox;
    private CollisionShape3D hitboxCollision;
    private CapsuleShape3D collisionCapsule;
    public Sprite3D ProjectileSprite;

    private Vector3 direction;

    public int Pierces { get; protected set; } = 0;
    public float ProjSpeed { get; protected set; } = 0f;
    public double ProjLifetime { get; protected set; } = 0;
    public double ProjTimeAlive { get; protected set; } = 0;
    public float Range { get; protected set; } = 0f;
    public float DistanceTravelled { get; protected set; } = 0f;

    private SkillDamage damage;
    private ActorPenetrations pens;
    private EDamageCategory dmgCategory;

    public override void _Ready() {
        Hitbox = GetNode<Area3D>("Hitbox");
        hitboxCollision = Hitbox.GetNode<CollisionShape3D>("HitCollision");
        collisionCapsule = hitboxCollision.Shape as CapsuleShape3D;
        ProjectileSprite = Hitbox.GetNode<Sprite3D>("ProjectileSprite");
    }

    public override void _PhysicsProcess(double delta) {
        if (Range > 0f) {
            if (DistanceTravelled >= Range) {
                QueueFree();
                return;
            }
        }
        else if (ProjLifetime > 0) {
            if (ProjTimeAlive >= ProjLifetime) {
                QueueFree();
                return;
            }
        }
        
        Vector3 newPos = GlobalPosition;
        float distanceCovered = (float)(ProjSpeed * delta);
        newPos += direction * distanceCovered;
        GlobalPosition = newPos;

        DistanceTravelled += distanceCovered;
        ProjTimeAlive += delta;
    }

    public void SetFacing(float angle) {
        RotateY(angle);
        direction = Transform.Basis * new Vector3(0f, 0f, 1f).Normalized();
    }

    public void SetFacing(Vector3 position) {
        LookAt(position, Vector3.Up, true);
        direction = Transform.Basis * new Vector3(0f, 0f, 1f).Normalized();
    }

    public void SetProperties(EDamageCategory dmgCat, SkillDamage sDamage, ActorPenetrations sPens, float speed = 10f, double lifetime = 0, int pierce = 0, float range = 15f) {
        damage = sDamage;
        pens = sPens;
        dmgCategory = dmgCat;

        ProjSpeed = speed;
        Pierces = pierce;
        ProjLifetime = lifetime;
        Range = range;
    }

    protected void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            Actor target = body as Actor;
            OnCollideWithTarget(target);
        }
        else {
            QueueFree();
        }
    }

    protected virtual void OnCollideWithTarget(Actor actor) {
        actor.TakeDamage(dmgCategory, damage, pens, true, damage.IsCritical, true);

        if (Pierces < 1) {
            QueueFree();
        }
        else {
            Pierces--;
        }
    }

    protected void OnBodyExited(Node3D body) {
        
    }
}
