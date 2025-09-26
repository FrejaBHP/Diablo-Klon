using Godot;
using System;

public partial class TestEnemy3 : EnemyBase {
    private double baseCastTime;

    public TestEnemy3() {
		BasicStats.BaseLife = 10;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        CastSpeedMod.SMore = 0.8;

        GoldBounty.SBase = 3;
        ExperienceBounty.SBase = 1;
    }

    public override void Setup() {
        MovementSpeed.SBase = 3;
        Evasion.SBase = 0; // 67

        Skill skillSoulrend = new SSoulrend();
        skillSoulrend.Level = 0;
        baseCastTime = ((ISpell)skillSoulrend).BaseCastTime;
        AddSkill(skillSoulrend);

        AIControllerBasicMelee aic = new AIControllerBasicMelee();
        aic.AttachToEnemy(this);
        aic.PreferredMinDistanceFromTarget = 3f;
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
    }

    public override void UsePrimarySkill() {
        currentlyUsedSkill = Skills[0];

        ActorState = EActorState.UsingSkill;
        skillTimer.WaitTime = baseCastTime / CastSpeedMod.STotal;
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
