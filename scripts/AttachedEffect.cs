using Godot;
using System;

public abstract class AttachedEffect {
    public Actor AffectedActor { get; set; }
    public EEffectName EffectName { get; protected set; }
    public double EffectLength { get; protected set; }
    public double RemainingTime { get; protected set; }
    public double EffectValue { get; protected set; }
    public bool HasExpired { get; protected set; } = false;
    
    public abstract void OnGained();
    public abstract void Tick(double deltaTime);

    public virtual void OnExpired() {
        HasExpired = true;
    }

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


public class IgniteEffect : AttachedEffect, IUniqueEffect, IDamageOverTimeEffect {
    public static EDamageType DamageType { get; protected set; } = EDamageType.Fire;
    public static ESkillDamageTags DamageTags { get; protected set; } = ESkillDamageTags.DoT | ESkillDamageTags.Burn;
    private const double damageFactor = 0.5;
    private const double igniteDuration = 4;

    // duration should be changed to a multiplier, and a base duration should be set here instead
    public IgniteEffect(Actor actor, double durationModifier, double dps) {
        EffectName = EEffectName.Ignite;

        AffectedActor = actor;
        EffectLength = igniteDuration * durationModifier;
        RemainingTime = EffectLength;
        EffectValue = dps * damageFactor;
    }

    public override void OnGained() {
        
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
    public static ESkillDamageTags DamageTags { get; protected set; } = ESkillDamageTags.DoT | ESkillDamageTags.Poison;
    private const double damageFactor = 0.2;
    private const double poisonDuration = 2;

    public PoisonEffect(Actor actor, double durationModifier, double dps) {
        EffectName = EEffectName.Poison;

        AffectedActor = actor;
        EffectLength = poisonDuration * durationModifier;
        RemainingTime = EffectLength;
        EffectValue = dps * damageFactor;
    }

    public override void OnGained() {
        
    }

    public override void Tick(double deltaTime) {
        double damage = EffectValue * deltaTime;
        AffectedActor.TallyDamageOverTimeForNextTick(DamageType, damage);
        RemainingTime -= deltaTime;
    }
}
