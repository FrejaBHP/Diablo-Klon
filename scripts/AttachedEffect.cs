using Godot;
using System;
using System.Collections.Generic;

public abstract class AttachedEffect {
    public Actor AffectedActor { get; set; }
    public Texture2D EffectIcon { get; protected set; }
    public EEffectName EffectName { get; protected set; }
    public double EffectLength { get; protected set; }
    public double RemainingTime { get; protected set; }
    public double EffectValue { get; protected set; }
    public bool HasExpired { get; protected set; } = false;
    public bool CreatesStatusIcon { get; protected set; } = false;
    
    public virtual  void OnGained() {
        if (CreatesStatusIcon) {
            AffectedActor.AddStatusToFloatingBars(EffectIcon, EffectName);
        }
    }

    public virtual void OnExpired() {
        //AffectedActor.RemoveStatusFromFloatingBars(EffectName);
        HasExpired = true;
    }
    
    public abstract void Tick(double deltaTime);

    public virtual void OverrideTimer(double newTime) {
        RemainingTime = newTime;
    }
}

// Effect that only ever has one instance on an Actor at any time
// Some of them may allow their effects getting replaced with a superior, incoming version
public interface IUniqueEffect {
    bool ShouldReplaceCurrentEffect(double duration, double value);
}

// Same as the above, but the effect may "stack" instead of fighting for the same spot
public interface IUniqueStackableEffect {
    int StacksPerApplication { get; protected set; }
    int StackLimit { get; protected set; }

    void AddStack(int stacks);
    void SetStacks(int stacks);
}

// Effect that an Actor can have any number of at any time
// They all work independently and do not interact with one another
public interface IRepeatableEffect {

}

public interface IDamageOverTimeEffect {
    
}

public interface IStatAlteringEffect {
    public Dictionary<EStatName, double> EffectStatDictionary { get; protected set; }
}

public class BleedEffect : AttachedEffect, IUniqueEffect, IDamageOverTimeEffect {
    public static EDamageType DamageType { get; protected set; } = EDamageType.Physical;
    private const double damageFactor = 0.75;
    private const double bleedDuration = 8;

    public BleedEffect(Actor actor, double durationModifier, double dps) {
        EffectIcon = UILib.DamageBleed;
        CreatesStatusIcon = true;
        EffectName = EEffectName.Bleed;

        AffectedActor = actor;
        EffectLength = bleedDuration * durationModifier;
        RemainingTime = EffectLength;
        EffectValue = dps * damageFactor;
    }

    public override void OnGained() {
        base.OnGained();
    }

    public override void OnExpired() {
        base.OnExpired();
    }

    public bool ShouldReplaceCurrentEffect(double duration, double value) {
        return (duration * value) > (RemainingTime * EffectValue);
    }

    public override void Tick(double deltaTime) {
        double damage = EffectValue * deltaTime;

        if (AffectedActor.Velocity.LengthSquared() > 0) {
            damage *= 2;
        }

        AffectedActor.TallyDamageOverTimeForNextTick(DamageType, damage);
        RemainingTime -= deltaTime;
    }
}

public class IgniteEffect : AttachedEffect, IUniqueEffect, IDamageOverTimeEffect {
    public static EDamageType DamageType { get; protected set; } = EDamageType.Fire;
    private const double damageFactor = 0.5;
    private const double igniteDuration = 4;

    public IgniteEffect(Actor actor, double durationModifier, double dps) {
        EffectIcon = UILib.DamageFireBurn;
        CreatesStatusIcon = true;
        EffectName = EEffectName.Ignite;

        AffectedActor = actor;
        EffectLength = igniteDuration * durationModifier;
        RemainingTime = EffectLength;
        EffectValue = dps * damageFactor;
    }

    public override void OnGained() {
        base.OnGained();
    }

    public override void OnExpired() {
        base.OnExpired();
    }

    public bool ShouldReplaceCurrentEffect(double duration, double value) {
        return (duration * value) > (RemainingTime * EffectValue);
    }

    public override void Tick(double deltaTime) {
        double damage = EffectValue * deltaTime;
        AffectedActor.TallyDamageOverTimeForNextTick(DamageType, damage);
        RemainingTime -= deltaTime;
    }
}

public class PoisonEffect : AttachedEffect, IRepeatableEffect, IDamageOverTimeEffect {
    public static EDamageType DamageType { get; protected set; } = EDamageType.Chaos;
    private const double damageFactor = 0.2;
    private const double poisonDuration = 2;

    public PoisonEffect(Actor actor, double durationModifier, double dps) {
        EffectIcon = UILib.DamagePoison;
        CreatesStatusIcon = true;
        EffectName = EEffectName.Poison;

        AffectedActor = actor;
        EffectLength = poisonDuration * durationModifier;
        RemainingTime = EffectLength;
        EffectValue = dps * damageFactor;
    }

    public override void OnGained() {
        base.OnGained();
    }

    public override void OnExpired() {
        base.OnExpired();
    }

    public override void Tick(double deltaTime) {
        double damage = EffectValue * deltaTime;
        AffectedActor.TallyDamageOverTimeForNextTick(DamageType, damage);
        RemainingTime -= deltaTime;
    }
}

public class SpeedBurstTestEffect : AttachedEffect, IUniqueEffect, IStatAlteringEffect {
    public Dictionary<EStatName, double> EffectStatDictionary { get; set; } = new() {
        { EStatName.IncreasedMovementSpeed, 0.5 }
    };
    private const double boostDuration = 2;

    public SpeedBurstTestEffect(Actor actor) {
        EffectIcon = UILib.TextureSkillSoulrend;
        CreatesStatusIcon = true;
        EffectName = EEffectName.SpeedBoost;

        AffectedActor = actor;
        EffectLength = boostDuration;
        RemainingTime = EffectLength;
    }

    public override void OnGained() {
        base.OnGained();
        AffectedActor.OnStatAlteringEffectGained(this);
    }

    public override void OnExpired() {
        base.OnExpired();
        AffectedActor.OnStatAlteringEffectLost(this);
    }

    public bool ShouldReplaceCurrentEffect(double duration, double value) {
        return duration > RemainingTime;
    }

    public override void Tick(double deltaTime) {
        RemainingTime -= deltaTime;
    }
}
