using Godot;
using System;

public partial class TestEnemy2 : EnemyBase {
    public TestEnemy2() {
		BasicStats.BaseLife = 14;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        goldBounty = 3;
        experienceBounty = 1;
    }

    public override void _Ready() {
        base._Ready();

        // PS dmg% = 130%
        UnarmedMinDamage = 5;
        UnarmedMaxDamage = 8;
        UnarmedAttackSpeed = 1;

        MainHandStats.PhysMinDamage = UnarmedMinDamage;
        MainHandStats.PhysMaxDamage = UnarmedMaxDamage;

        MovementSpeed.SBase = 3.5;
        Evasion.SBase = 0; // 67

        Skill skillPiercingShot = new SPiercingShot();
        skillPiercingShot.Level = 0;
        AddSkill(skillPiercingShot);
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        BasicChaseAIProcess(delta);
    }

    protected override void UsePrimarySkill() {
        currentlyUsedSkill = Skills[0];

        ActorState = EActorState.UsingSkill;
        skillTimer.WaitTime = UnarmedAttackSpeed / AttackSpeedMod.STotal;
        skillUsePointTimer.WaitTime = skillTimer.WaitTime * 0.70;
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
}
