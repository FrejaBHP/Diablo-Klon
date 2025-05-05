using Godot;
using System;

public partial class EnemyBase : Actor {
    protected Marker3D resBarAnchor;
    protected NavigationAgent3D navAgent;
    protected Timer navUpdateTimer;

    protected Label3D debugLabel;

    protected Actor actorTarget;
    protected bool isChasingTarget = false;

    public override void _Ready() {
        base._Ready();

        debugLabel = GetNode<Label3D>("Label3D");

        navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        navAgent.VelocityComputed += OnVelocityComputed;

        navUpdateTimer = GetNode<Timer>("NavigationUpdateTimer");

        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        AddFloatingBars(resBarAnchor);

        AddToGroup("Enemy");

        CallDeferred(MethodName.NavSetup);
    }

    public void NavSetup() {
        Game game = (Game)GetTree().Root.GetChild(0);
        navAgent.SetNavigationMap(GetWorld3D().NavigationMap);
        
        // Temp
        SetActorTarget(game.PlayerActor);
    }

    public override void _PhysicsProcess(double delta) {
        
    }

    public void OnNavigationUpdateTimeout() {
        if (isChasingTarget && actorTarget != null) {
            SetNavigationTarget(actorTarget.GlobalPosition);
        }
    }

    public void SetActorTarget(Actor target) {
        actorTarget = target;
        SetNavigationTarget(target.GlobalPosition);
        isChasingTarget = true;
        navUpdateTimer.Start();
    }

    public void SetNavigationTarget(Vector3 targetPosition) {
        navAgent.TargetPosition = targetPosition;
    }

    public void ProcessNavigation() {
        // Do not query when the map has never synchronized and is empty.
        if (NavigationServer3D.MapGetIterationId(navAgent.GetNavigationMap()) == 0) {
            return;
        }

        if (navAgent.IsNavigationFinished()) {
            return;
        }

        Vector3 nextPathPosition = navAgent.GetNextPathPosition();
        Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition with { Y = GlobalPosition.Y }) * (float)MovementSpeed.STotal;
        FacePathPosition();

        if (navAgent.AvoidanceEnabled) {
            navAgent.Velocity = newVelocity;
        }
        else {
            OnVelocityComputed(newVelocity);
        }
    }

    public void OnVelocityComputed(Vector3 newVelocity) {
        Velocity = newVelocity;
    }

    protected void FacePathPosition() {
        if (isChasingTarget && actorTarget != null) {
            Vector3 direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition() with { Y = GlobalPosition.Y });

            if (direction != Vector3.Zero) {
                Basis lookTarget = Basis.LookingAt(direction, null, true);
                Basis slerpTarget = Basis.Slerp(lookTarget, 0.08f);
                Basis = slerpTarget.Orthonormalized();
            }
        }
    }

    public override void OnDamageTaken(double damage, bool isCritical, bool createDamageText) {
        if (createDamageText) {
            ShowDamageText(damage, isCritical);
        }

        if (BasicStats.CurrentLife <= 0) {
            OnNoLifeLeft();
        }
    }

    public override void OnDamageEvaded() {
        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber("Evaded!");
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }

    protected void ShowDamageText(double damage, bool isCritical) {
        string labelText;
        if (isCritical) {
            labelText = $"{Math.Round(damage, 0)}!";
        }
        else {
            labelText = $"{Math.Round(damage, 0)}";
        }

        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber(labelText);
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }

    public override void OnNoLifeLeft() {
        QueueFree();
    }

}
