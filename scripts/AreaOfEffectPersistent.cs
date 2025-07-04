using Godot;
using System;

public partial class AreaOfEffectPersistent : Area3D {
    protected CollisionShape3D hitboxCollision;
    protected CylinderShape3D collisionShape;
    public AnimatedSprite3D AreaAnimation;

    protected SkillDamage damage;
    protected ActorPenetrations pens;
    protected EDamageCategory dmgCategory;

    public override void _Ready() {
        hitboxCollision = GetNode<CollisionShape3D>("HitCollision");
        collisionShape = hitboxCollision.Shape as CylinderShape3D;
        AreaAnimation = GetNode<AnimatedSprite3D>("AreaAnimation");
    }

    public void SetProperties(EDamageCategory dmgCat, SkillDamage sDamage, ActorPenetrations sPens) {
        damage = sDamage;
        pens = sPens;
        dmgCategory = dmgCat;
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
        actor.TakeDamage(dmgCategory, damage, pens, true, true);
    }

    protected void OnBodyExited(Node3D body) {
        
    }
}
