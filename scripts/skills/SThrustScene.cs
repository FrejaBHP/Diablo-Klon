using Godot;
using System;

public partial class SThrustScene : Node3D {
    public delegate void TargetHitEventHandler(Actor target);
    public event TargetHitEventHandler TargetHit;

    public Area3D Hitbox;
    private CollisionShape3D hitboxCollision;
    private CapsuleShape3D collisionCapsule;
    private Sprite3D effectSprite;

    private Tween tween;

    private SkillDamage damage;
    private ActorPenetrations pens;
    private EDamageCategory dmgCategory;

    public override void _Ready() {
        Hitbox = GetNode<Area3D>("Hitbox");
        hitboxCollision = Hitbox.GetNode<CollisionShape3D>("HitCollision");
        collisionCapsule = hitboxCollision.Shape as CapsuleShape3D;
        effectSprite = Hitbox.GetNode<Sprite3D>("EffectSprite");
    }

    public void StartAttack(EDamageCategory dmgCat, SkillDamage sDamage, ActorPenetrations sPens, float scale, float range, float speed) {
        damage = sDamage;
        pens = sPens;
        dmgCategory = dmgCat;

        Hitbox.Scale = new Vector3(scale, scale, scale);
        Hitbox.Position = Hitbox.Position with { Z = Hitbox.Position.Z + (collisionCapsule.Height / 2) * (scale - 1) };
        float travelTime = range / speed;
        float travelDistance = range - (collisionCapsule.Height / 2) * (scale - 1);

        tween = CreateTween();
        tween.Finished += OnTweenFinished;

        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(Hitbox, "position", new Vector3(0f, 0f, travelDistance), travelTime).AsRelative();

        AnimateTween();
    }

    protected void AnimateTween() {
        tween.Play();
    }

    protected void OnBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            Actor actor = body as Actor;
            TargetHit?.Invoke(actor);
        }
    }

    protected void OnBodyExited(Node3D body) {
        
    }

    protected void OnTweenFinished() {
        QueueFree();
    }
}
