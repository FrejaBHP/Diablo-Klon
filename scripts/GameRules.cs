using Godot;
using System;

public partial class GameRules {
    public int MaxGemLevel = 14;
    public double EnemyDamageScalingFactor = 1.09;      // 1.125
    public double EnemyLifeScalingFactor = 1.12;        // 1.175

    public bool ExistingGemsScaleWithGameProgress = false;

    // AttackSkillScalingFactor = 1.065;
    // SpellSkillScalingFactor = 1.20;
    // SupportScalingFactor = 1.12;
}
