using Godot;
using System;

public partial class TestEnemy : EnemyBase {
    public TestEnemy() {
		BasicStats.BaseLife = 18;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        GoldBounty.SBase = 3;
        ExperienceBounty.SBase = 1;
    }

    public override void _Ready() {
        base._Ready();

        // Thrust dmg% = 160%
        UnarmedMinDamage = 4;
        UnarmedMaxDamage = 7;
        UnarmedAttackSpeed = 1.1;

        MainHandStats.PhysMinDamage = UnarmedMinDamage;
        MainHandStats.PhysMaxDamage = UnarmedMaxDamage;

        MovementSpeed.SBase = 4;
        Evasion.SBase = 0; // 67
        BlockChance.SBase = 0.20;

        Skill skillThrust = new SThrust();
        skillThrust.Level = 0;
        AddSkill(skillThrust);
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

    //public override void OnNoLifeLeft() {}
}
