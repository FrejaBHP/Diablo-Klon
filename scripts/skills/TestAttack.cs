using Godot;
using System;

public partial class TestAttack : Node3D {
    private Area3D hitbox;
    private CollisionShape3D hitboxCollision;
    private CapsuleShape3D collisionCapsule;
    private Sprite3D effectSprite;

    private Tween tween;

    private SkillDamage damage;

    public override void _Ready() {
        hitbox = GetNode<Area3D>("Hitbox");
        hitboxCollision = hitbox.GetNode<CollisionShape3D>("HitCollision");
        collisionCapsule = hitboxCollision.Shape as CapsuleShape3D;
        effectSprite = hitbox.GetNode<Sprite3D>("EffectSprite");
    }

    public void StartAttack(SkillDamage sDamage, float scale, float range, float speed) {
        damage = sDamage;

        hitbox.Scale = new Vector3(scale, scale, scale);
        hitbox.Position = hitbox.Position with { Z = hitbox.Position.Z + (collisionCapsule.Height / 2) * (scale - 1) };
        float travelTime = range / speed;
        float travelDistance = range - (collisionCapsule.Height / 2) * (scale - 1);

        tween = CreateTween();
        tween.Finished += OnTweenFinished;

        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(hitbox, "position", new Vector3(0f, 0f, travelDistance), travelTime);

        AnimateTween();
    }

    protected void AnimateTween() {
        tween.Play();
    }

    protected void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy")) {
            Actor enemy = body as Actor;
            enemy.TakeDamage(damage.Physical); // Giv støtte til alle typer senere!!
        }
    }

    protected void OnBodyExited(Node3D body) {
        
    }

    protected void OnTweenFinished() {
        QueueFree();
    }
}
