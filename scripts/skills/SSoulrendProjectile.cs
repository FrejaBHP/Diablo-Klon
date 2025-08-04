using Godot;
using System;
using System.Collections.Generic;

public partial class SSoulrendProjectile : Projectile {
    public AreaOfEffectPersistent AoE { get; protected set; }
    public Area3D SeekerRadius { get; protected set; }

    public double SeekerUpdateInterval { get; protected set; } = 0.1;
    protected double updateTimer = 0;

    const double turnConeMinimum = 0.33;
    const float turnRate = 0.0225f;

    protected List<Node3D> actorsInSeekerRadius = new();
    protected List<Node3D> validSeekerActors = new();
    protected Vector3 seekerGoal = new();

    public override void _Ready() {
        base._Ready();
        AoE = GetNode<AreaOfEffectPersistent>("AreaOfEffect");
        SeekerRadius = GetNode<Area3D>("SeekerRadius");
    }

    public void SetAoEProperties(float newRadius) {
        AoE.SetProperties(10, newRadius, null, 0, 0);
    }

    public override void _PhysicsProcess(double delta) {
        if (!ShouldExpire()) {
            if (updateTimer <= 0) {
                updateTimer += SeekerUpdateInterval;
                validSeekerActors = UpdateValidSeekerTargets();
                
                if (validSeekerActors.Count != 0) {
                    seekerGoal = GetClosestSeekerTarget().GlobalPosition with { Y = GlobalPosition.Y };
                }
            }

            if (ShouldTurn()) {
                Turn();
            }
            MoveStraightAhead(delta);

            updateTimer -= delta;
            ProjTimeAlive += delta;
        }
    }

    protected bool ShouldTurn() {
        if (validSeekerActors.Count == 0) {
            return false;
        }

        return true;
    }

    public void Turn() {
        Vector3 target = GlobalPosition.DirectionTo(seekerGoal);
        float dotX = GlobalTransform.Basis.X.Dot(target);

        if (dotX > 0.005) {
            SetFacing(turnRate);
        }
        else if (dotX < -0.005) {
            SetFacing(-turnRate);
        }
    }

    protected List<Node3D> UpdateValidSeekerTargets() {
        List<Node3D> validActors = new();

        if (actorsInSeekerRadius.Count == 0) {
            return validActors;
        }

        foreach (Node3D body in actorsInSeekerRadius) {
            Vector3 facing = GlobalTransform.Basis.Z;
            Vector3 flat = new(body.GlobalPosition.X, GlobalPosition.Y, body.GlobalPosition.Z);
            Vector3 target = GlobalPosition.DirectionTo(flat);

            if (facing.Dot(target) >= turnConeMinimum) {
                validActors.Add(body);
            }
        }

        return validActors;
    }

    protected Node3D GetClosestSeekerTarget() {
        int shortestDistanceIndex = 0;
        float shortestDistance = 100f;

        for (int i = 0; i < validSeekerActors.Count; i++) {
            if (shortestDistance > GlobalPosition.DistanceSquaredTo(validSeekerActors[i].GlobalPosition)) {
                shortestDistanceIndex = i;
            }
        }

        return validSeekerActors[shortestDistanceIndex];
    }

    public void OnSeekerRadiusBodyEntered(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            actorsInSeekerRadius.Add(body);
        }
    }

    public void OnSeekerRadiusBodyExited(Node3D body) {
        if (body.IsInGroup("Enemy") || body.IsInGroup("Player")) {
            actorsInSeekerRadius.Remove(body);
        }
    }
}
