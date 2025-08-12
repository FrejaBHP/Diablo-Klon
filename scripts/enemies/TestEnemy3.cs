using Godot;
using System;

public partial class TestEnemy3 : EnemyBase {
    private double baseCastTime;

    public TestEnemy3() {
		BasicStats.BaseLife = 10;
		BasicStats.BaseMana = 0;
		RefreshLifeMana();

        CastSpeedMod.SMore = 0.8;

        goldBounty = 3;
        experienceBounty = 1;
    }

    public override void _Ready() {
        base._Ready();

        MovementSpeed.SBase = 3.25;
        Evasion.SBase = 0; // 67

        Skill skillSoulrend = new SSoulrend();
        skillSoulrend.Level = 0;
        baseCastTime = ((ISpell)skillSoulrend).BaseCastTime;
        AddSkill(skillSoulrend);
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        BasicChaseAIProcess(delta);
    }

    protected override void UsePrimarySkill() {
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
