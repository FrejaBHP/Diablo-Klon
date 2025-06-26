using Godot;
using System;

public partial class TestEnemy : EnemyBase {
    public TestEnemy() {
		BasicStats.BaseLife = 15;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        goldBounty = 3;
        experienceBounty = 1;
    }

    public override void _Ready() {
        base._Ready();

        UnarmedMinDamage = 6;
        UnarmedMaxDamage = 9;
        UnarmedAttackSpeed = 1.1;

        MainHandStats.PhysMinDamage = UnarmedMinDamage;
        MainHandStats.PhysMaxDamage = UnarmedMaxDamage;

        MovementSpeed.SBase = 4;
        Evasion.SBase = 0; // 67

        Skill skillThrust = new SThrust();
        AddSkill(skillThrust);
        //CalculateStats();
    }

    public override void _PhysicsProcess(double delta) {
        ApplyRegen(delta);

        if (isChasingTarget && actorTarget != null) {
            if (GlobalPosition.DistanceTo(actorTarget.GlobalPosition) < Skills[0].CastRange - 0.25f && ActorState != EActorState.UsingSkill) {
                //Vector3 targetVector = actorTarget.GlobalPosition - GlobalPosition;
                //targetVector = targetVector.Normalized() * 20;
                //lineOfSightCast.TargetPosition = targetVector;
                
                lineOfSightCast.ForceRaycastUpdate();

                if (lineOfSightCast.IsColliding()) {
                    UseThrust();
                }
            }
        }

        if (ActorState == EActorState.Actionable) {
            ProcessNavigation();
        }

        DoGravity(delta);
        MoveAndSlide();

        //debugLabel.Text = $"Next Path Pos: {navAgent.GetNextPathPosition().ToString("F2")}\nTarget Pos: {navAgent.TargetPosition.ToString("F2")}\nVelocity Length: {Velocity.Length():F2}";
        //debugLabel.Text = $"Rotation: {GlobalRotationDegrees.ToString("F2")}\nVelocity Length: {Velocity.Length():F2}";
    }

    public void UseThrust() {
        currentlyUsedSkill = Skills[0];

        ActorState = EActorState.UsingSkill;
        skillTimer.WaitTime = UnarmedAttackSpeed / AttackSpeedMod.STotal;
        skillUsePointTimer.WaitTime = skillTimer.WaitTime / 2;
        skillTimer.Start();
        skillUsePointTimer.Start();

        Vector3 velocity = Velocity;
        velocity.X = 0f;
        velocity.Z = 0f;
        Velocity = velocity;
    }

    public override void OnSkillUsePointTimerTimeout() {
        if (currentlyUsedSkill != null && ActorState == EActorState.UsingSkill) {
            currentlyUsedSkill.UseSkill();
        }
    }

    //public override void OnNoLifeLeft() {}
}
