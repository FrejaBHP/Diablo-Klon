using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SCleave : Skill, IAttack, IMeleeSkill, IAreaSkill {
    public ESkillWeapons Weapons { get; set; } = ESkillWeapons.AllMeleeWeapons;
    public bool CanDualWield { get; set; } = true;

    public Stat BaseAttackSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveAttackSpeedModifiers { get; set; } = new(0, false);

    public float BaseAttackRange { get; set; } = 3f;

    public float BaseAreaRadius { get; set; } = 2.5f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = false;

    private const float pixelSize = 0.085f;
    private const float coneMin = 0f;

    private static readonly double[] addedDamageModArray = [
        1.30, 1.38, 1.47, 1.57, 1.67,
        1.78, 1.89, 2.02, 2.15, 2.29,
        2.44, 2.59, 2.76, 2.94, 3.13
    ];

    public SCleave() {
        Name = "Cleave";
        Description = "Attacks in a frontal arc with a melee weapon, damaging enemies in the area.";
        Effects = [];

        SkillName = ESkillName.Cleave;
        Type = ESkillType.Attack;
        Tags = ESkillTags.Attack | ESkillTags.Melee | ESkillTags.Area;
        SkillDamageTags = ESkillDamageTags.Attack | ESkillDamageTags.Melee | ESkillDamageTags.Area;
        DamageCategory = EDamageCategory.Melee;
        Texture = UILib.TextureSkillCleave;

        ManaCost = 1;

        CastRange = BaseAttackRange;

        UsesMouseAim = true;
    }

    protected override void OnSkillLevelChanged() {
        AddedDamageModifier = addedDamageModArray[level];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IAreaSkill aSkill = this;
            Vector3 facing = ActorOwner.GlobalTransform.Basis.Z.Normalized() * 20;
            Vector3 projectedPosition = ActorOwner.GlobalPosition + facing;
            aSkill.BasicConeSweepSkillBehaviour(this, "cleave", pixelSize, projectedPosition, coneMin);
        }
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        SkillInfo info = CalculateOutgoingValuesIntoInfo(true);

        foreach (Actor actor in targets) {
            actor.ReceiveHit(DamageCategory, info, ActorOwner.Penetrations, true);
        }
    }
}
