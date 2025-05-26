using Godot;
using System;

public partial class TestEnemy2 : EnemyBase {
    public TestEnemy2() {
		BasicStats.BaseLife = 20;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        goldBounty = 3;
        experienceBounty = 1;
    }

    public override void _Ready() {
        base._Ready();

        UnarmedMinDamage = 8;
        UnarmedMaxDamage = 12;
        UnarmedAttackSpeed = 1.25;

        DamageMods.Physical.SMinBase = UnarmedMinDamage;
        DamageMods.Physical.SMaxBase = UnarmedMaxDamage;

        MovementSpeed.SBase = 4;
        Evasion.SBase = 0; // 67

        Skill skillShoot = new SShoot();
        AddSkill(skillShoot);
    }

    public override void _PhysicsProcess(double delta) {
        ApplyRegen(delta);

        if (isChasingTarget && actorTarget != null) {
            if (GlobalPosition.DistanceTo(actorTarget.GlobalPosition) < Skills[0].CastRange - 0.25f && ActorState != EActorState.Attacking) {
                //Vector3 targetVector = actorTarget.GlobalPosition - GlobalPosition;
                //targetVector = targetVector.Normalized() * 20;
                //lineOfSightCast.TargetPosition = targetVector;
                
                lineOfSightCast.ForceRaycastUpdate();

                if (lineOfSightCast.IsColliding()) {
                    UseShoot();
                }
            }
        }

        if (ActorState == EActorState.Actionable) {
            ProcessNavigation();
        }

        DoGravity(delta);
        MoveAndSlide();
    }

    public void UseShoot() {
        currentlyUsedSkill = Skills[0];

        ActorState = EActorState.Attacking;
        skillTimer.WaitTime = UnarmedAttackSpeed * AttackSpeedMod.STotal;
        skillUsePointTimer.WaitTime = skillTimer.WaitTime / 2;
        skillTimer.Start();
        skillUsePointTimer.Start();

        Vector3 velocity = Velocity;
        velocity.X = 0f;
        velocity.Z = 0f;
        Velocity = velocity;
    }

    public override void OnSkillUsePointTimerTimeout() {
        if (currentlyUsedSkill != null && ActorState == EActorState.Attacking) {
            currentlyUsedSkill.UseSkill();
        }
    }
}
