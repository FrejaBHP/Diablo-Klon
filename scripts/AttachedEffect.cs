using Godot;
using System;
using System.Collections.Generic;

public abstract class AttachedEffect {
    public Actor AffectedActor { get; set; }
    public Texture2D EffectIcon { get; protected set; }
    public EEffectName EffectName { get; protected set; }
    public EEffectRating EffectRating { get; protected set; }
    
    public string EffectString { get; protected set; }
    public string EffectDescription { get; protected set; }

    public double EffectLength { get; protected set; }
    public double RemainingTime { get; protected set; }
    public double EffectValue { get; protected set; }

    public bool HasExpired { get; protected set; } = false;
    public bool CreatesStatusIcon { get; protected set; } = false;
    public bool CreatesPlayerStatusIcon { get; protected set; } = false;
    
    public virtual void OnGained() {
        if (CreatesStatusIcon) {
            AffectedActor.AddStatusToFloatingBars(EffectIcon, EffectName);
        }

        if (AffectedActor is Player player && CreatesPlayerStatusIcon) {
            player.PlayerHUD.UpperHUD.TryAddStatus(this);
        }
    }

    public virtual void OnExpired() {
        HasExpired = true;
    }
    
    public virtual void Tick(double deltaTime) {
        RemainingTime -= deltaTime;
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
    int StackAmount { get; protected set; }

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
    private const double damageFactor = 0.6;
    private const double bleedDuration = 6;

    public BleedEffect(Actor actor, double durationModifier, double dps) {
        EffectIcon = UILib.DamageBleed;
        CreatesStatusIcon = true;
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.Bleed;
        EffectRating = EEffectRating.Negative;

        EffectString = "Bleeding";
        EffectDescription = "Taking Physical Damage over Time. Damage is doubled while Moving";

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
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.Ignite;
        EffectRating = EEffectRating.Negative;

        EffectString = "Ignited";
        EffectDescription = "Taking Fire Damage over Time";

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
    private const double damageFactor = 0.15;
    private const double poisonDuration = 2;

    public PoisonEffect(Actor actor, double durationModifier, double dps) {
        EffectIcon = UILib.DamagePoison;
        CreatesStatusIcon = true;
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.Poison;
        EffectRating = EEffectRating.Negative;

        EffectString = "Poisoned";
        EffectDescription = "Taking Chaos Damage over Time";

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
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.SpeedBoost;
        EffectRating = EEffectRating.Positive;

        EffectString = "Speed Burst";
        EffectDescription = "Increased Movement Speed";

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
}

public class ArcaneSurgeEffect : AttachedEffect, IUniqueEffect, IStatAlteringEffect {
    public Dictionary<EStatName, double> EffectStatDictionary { get; set; } = new() {
        { EStatName.IncreasedCastSpeed, 0.2 },
        { EStatName.IncreasedManaRegen, 0.5 }
    };

    private const double boostDuration = 4;

    public ArcaneSurgeEffect(Actor actor) {
        EffectIcon = UILib.AEffectArcaneSurge;
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.ArcaneSurge;
        EffectRating = EEffectRating.Positive;

        EffectString = "Arcane Surge";
        EffectDescription = "Increased Cast Speed and Mana Regeneration Rate";

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
}

public class TestStackEffect : AttachedEffect, IUniqueStackableEffect {
    private const double length = 10;
    private const int stacksPerApplication = 1;

    public int StacksPerApplication { get; set; } = stacksPerApplication;
    public int StackLimit { get; set; } = 10;
    public int StackAmount { get; set; }

    public TestStackEffect(Actor actor, int startingStacks = stacksPerApplication) {
        EffectIcon = UILib.DamageBleed;
        CreatesStatusIcon = true;
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.Bleed;
        EffectRating = EEffectRating.Negative;

        EffectString = "Test";
        EffectDescription = "Numbers Go Up";

        StackAmount = startingStacks;

        AffectedActor = actor;
        EffectLength = length;
        RemainingTime = EffectLength;
    }

    public override void OnGained() {
        base.OnGained();
    }

    public override void OnExpired() {
        base.OnExpired();
    }

    public void AddStack(int stacks) {
        int newStackAmount = StackAmount + stacks;

        if (newStackAmount > StackLimit) {
            StackAmount = StackLimit;
        }
        else {
            StackAmount = newStackAmount;
        }

        RemainingTime = length;
    }

    public void SetStacks(int stacks) {
        if (stacks > StackLimit) {
            StackAmount = StackLimit;
        }
        else {
            StackAmount = stacks;
        }

        RemainingTime = length;
    }

    public override void Tick(double deltaTime) {
        RemainingTime -= deltaTime;
    }
}

public class TestRepeatableEffect : AttachedEffect, IRepeatableEffect {
    public static EDamageType DamageType { get; protected set; } = EDamageType.Chaos;
    private const double duration = 5;

    public TestRepeatableEffect(Actor actor) {
        EffectIcon = UILib.DamagePoison;
        CreatesStatusIcon = true;
        CreatesPlayerStatusIcon = true;
        EffectName = EEffectName.Poison;
        EffectRating = EEffectRating.Neutral;

        EffectString = "Test Repeatable Effect";
        EffectDescription = "Numbers Still Going Up";

        AffectedActor = actor;
        EffectLength = duration;
        RemainingTime = EffectLength;
    }

    public override void OnGained() {
        base.OnGained();
    }

    public override void OnExpired() {
        base.OnExpired();
    }

    public override void Tick(double deltaTime) {
        RemainingTime -= deltaTime;
    }
}
