using Godot;
using System;

public abstract class AIState {
    public virtual void OnEnter(AIController aic){}
    public virtual void OnUpdate(AIController aic, double delta){}
    public virtual void OnExit(AIController aic){}
    public abstract string GetStateName(); // Mostly for debugging purposes
}

public class BasicIdleState : AIState {
    public override void OnEnter(AIController aic) {
        //aic.SetTarget(null);
        aic.IsIdle = true;
    }

    public override void OnUpdate(AIController aic, double delta) {
        
    }

    public override void OnExit(AIController aic) {
        aic.IsIdle = false;
    }

    public override string GetStateName() {
        return "Idle";
    }    
}

public class BasicAttackState : AIState {
    // Temporary measure to try and prevent turret behaviour for ranged enemies
    private double shuffleTimer = 0;
    private double shuffleLockout = 0;

    public override void OnEnter(AIController aic) {
        aic.IsAttackingTarget = true;
        aic.IsChasingTarget = true;
    }

    public override void OnUpdate(AIController aic, double delta) {
        if (shuffleLockout > 0) {
            shuffleLockout -= delta;
        }

        if (shuffleTimer > 0) {
            shuffleTimer -= delta;

            if (shuffleTimer <= 0) {
                shuffleLockout = 3;
            }
        }

        if (aic.ActorTarget == null) {
            //aic.Idle();
            return;
        }
        else if (aic.LinkedEnemy.ActorState != EActorState.Actionable) {
            return;
        }

        bool shouldFaceTarget = false;
        bool shouldMove = true;

        // Am I actively chasing (moving towards) a target
        if (aic.IsChasingTarget) {
            // Can I see them
            if (aic.LinkedEnemy.CanSeeTarget()) {
                shouldFaceTarget = true;

                if (aic.LinkedEnemy.IsWithinSkillRangeOfTarget(0)) {
                    // Am I shuffling the shuffle
                    if (shuffleTimer <= 0) {
                        // Am I looking at them
                        if (aic.LinkedEnemy.IsFacingTarget()) {
                            aic.LinkedEnemy.UsePrimarySkill();
                            shouldMove = false;
                        }
                        // Am I allowed to shuffle again
                        else if (shuffleLockout <= 0 && aic.LinkedEnemy.GetSkillUseRange(0) > 4f) {
                            int random = Utilities.RNG.Next(0, 12);

                            if (random == 0) {
                                Vector3 shuffleDirection = new((float)Utilities.RNG.NextDouble(), 0f, (float)Utilities.RNG.NextDouble());
                                aic.LinkedEnemy.SetNavigationTarget(aic.LinkedEnemy.GlobalPosition + shuffleDirection * 10);
                                shuffleTimer = Utilities.RandomDouble(0.3, 0.5);
                            }
                            else {
                                shouldMove = false;
                            }
                        }
                        else {
                            shouldMove = false;
                        }
                    }
                }
            }
        }

        aic.ShouldMove = shouldMove;

        if (shouldFaceTarget) {
            aic.LinkedEnemy.TurnTowardsTargetPosition();
        }
        else {
            aic.LinkedEnemy.TurnTowardsPathPosition();
        }
    }

    public override void OnExit(AIController aic) {
        aic.IsAttackingTarget = false;
        aic.IsChasingTarget = false;
    }

    public override string GetStateName() {
        return "Attacking";
    }    
}

public class BasicRetreatState : AIState {
    public override void OnEnter(AIController aic) {
        aic.IsFleeing = true;
        aic.IsChasingTarget = false;
        aic.FleeTime = Utilities.RandomDouble(0.7, 1.2);
    }

    public override void OnUpdate(AIController aic, double delta) {
        aic.ShouldMove = true;
        aic.LinkedEnemy.TurnTowardsPathPosition();
    }

    public override void OnExit(AIController aic) {
        aic.IsFleeing = false;
        aic.FleeLockout = Utilities.RandomDouble(3, 4);
    }

    public override string GetStateName() {
        return "Retreating";
    }    
}

public partial class AIController {
    public AIState CurrentAIState { get; protected set; }
    public EnemyBase LinkedEnemy { get; protected set; }

    public float PreferredMinDistanceFromTarget { get; set; } = 0f;
    public float PreferredMaxDistanceFromTarget { get; set; } = 15f;
    public double FleeTime { get; set; } = 0f;
    public double FleeLockout { get; set; } = 0f;

    public Actor ActorTarget { get; protected set; }
    public bool IsChasingTarget { get; set; } = false;
    public bool IsAttackingTarget { get; set; } = false;
    public bool IsAttacking { get; set; } = false;
    public bool IsFleeing { get; set; } = false;
    public bool IsIdle { get; set; } = false;
    
    public bool ShouldMove { get; set; } = true;

    public void SetAIState(AIState newState) {
        if (CurrentAIState != null) {
            CurrentAIState.OnExit(this);
        }

        CurrentAIState = newState;
        CurrentAIState.OnEnter(this);
    }

    public virtual void Idle() {

    }

    public void AttachToEnemy(EnemyBase enemy) {
        LinkedEnemy = enemy;
        LinkedEnemy.SetAIController(this);
        SetupValues();
    }

    protected virtual void SetupValues() {

    }

    public void SetTarget(Actor target) {
        ActorTarget = target;
        LinkedEnemy.SetActorTarget(target);
        LinkedEnemy.SetNavigationTarget(target.GlobalPosition);
    }

    public virtual void Update(double delta) {
        if (CurrentAIState != null) {
            CurrentAIState.OnUpdate(this, delta);
        }
    }
}

public partial class AIControllerBasicMelee : AIController {
    public BasicIdleState IdleState { get; protected set; } = new();
    public BasicAttackState AttackState { get; protected set; } = new();
    public BasicRetreatState RetreatState { get; protected set; } = new();

    //private float attackRange = 0f;

    public AIControllerBasicMelee() {
        Idle();
    }

    protected override void SetupValues() {
        //attackRange = LinkedEnemy.GetSkillUseRange(0);
    }

    public override void Idle() {
        SetAIState(IdleState);
    }

    public void Retreat() {
        SetAIState(RetreatState);
    }

    public void Attack() {
        SetAIState(AttackState);
    }

    public override void Update(double delta) {
        if (FleeTime > 0) {
            FleeTime -= delta;
        }
        if (FleeLockout > 0) {
            FleeLockout -= delta;
        }

        if (LinkedEnemy.ActorState == EActorState.Stunned) {
            return;
        }

        // Do I have a target and am I not fleeing
        if (!IsFleeing && ActorTarget != null) {
            // Am I trying to kill them
            if (IsAttackingTarget) {
                // If too close for comfort (eg safe zone is 5 metres away, but I'm standing 3,93 metres away)
                if (PreferredMinDistanceFromTarget > LinkedEnemy.GetDistanceToTarget()) {
                    // Am I allowed to flee, and can I see my target
                    if (FleeLockout <= 0 && LinkedEnemy.ActorState == EActorState.Actionable && LinkedEnemy.CanSeeTarget()) {
                        int random = Utilities.RNG.Next(0, 10);

                        if (random == 0) {
                            Vector3 awayDirection = LinkedEnemy.GetDirectionAwayFromTarget();
                            float diff = Math.Abs(PreferredMinDistanceFromTarget - LinkedEnemy.GetDistanceToTarget());

                            // Can I move backwards to get back to safety
                            if (!LinkedEnemy.IsThereAWall(awayDirection, diff)) {
                                LinkedEnemy.SetNavigationTarget(LinkedEnemy.GlobalPosition + awayDirection * diff);
                                // Move backwards for a bit
                                Retreat();
                            }
                        }
                    }
                }
            }
            // If I'm not trying to kill them, always a good time to start
            else {
                Attack();
            }
        }
        // If I'm fleeing
        else {
            // Am I done being scared?
            if (FleeTime <= 0) {
                // If yes, and I have a target, attack
                if (ActorTarget != null) {
                    Attack();
                }
                // If yes, but I don't have a target, go idle
                else {
                    Idle();
                }
            }
        }

        if (CurrentAIState != null) {
            CurrentAIState.OnUpdate(this, delta);
        }

        if (ShouldMove && LinkedEnemy.ActorState == EActorState.Actionable) {
            LinkedEnemy.ProcessNavigation();
        }
        else {
            LinkedEnemy.ResetPlaneVelocity();
        }
    }
}
