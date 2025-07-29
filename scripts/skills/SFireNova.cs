using Godot;
using System;
using System.Collections.Generic;

public class SFireNova : Skill, ISpell, IAreaSkill {
    public double BaseCastTime { get; set; } = 0.80;
    public Stat BaseCastSpeedModifiers { get; set; } = new(0, false);
    public Stat ActiveCastSpeedModifiers { get; set; } = new(0, false);

    public float BaseAreaRadius { get; set; } = 2.5f;
    public double IncreasedArea { get; set; } = 0;
    public double MoreArea { get; set; } = 1;
    public float TotalAreaRadius { get; set; }
    public bool IsNovaSkill { get; set; } = true;

    private const float pixelSize = 0.125f;

    private static readonly double[] minDamageArray = [
        10, 12, 14, 17, 20,
        24, 29, 35, 43, 51,
        61, 74, 89, 106, 128
    ];

    private static readonly double[] maxDamageArray = [
        13, 15, 18, 22, 26,
        32, 38, 46, 55, 67,
        80, 96, 115, 139, 166
    ];

    public SFireNova() {
        Name = "Fire Nova";
        Description = "Casts a nova of flame, dealing Fire damage to all surrounding enemies.";
        Effects = [];

        SkillName = ESkillName.FireNova;
        Type = ESkillType.Spell;
        Tags = ESkillTags.Spell | ESkillTags.Area | ESkillTags.Fire;
        DamageCategory = EDamageCategory.Spell;
        Texture = UILib.TextureSkillFireNova;

        ManaCost = 1;

        AddedDamageModifier = 2;

        TotalAreaRadius = BaseAreaRadius;
        CastRange = TotalAreaRadius - 0.5f;
    }

    protected override void OnSkillLevelChanged() {
        BaseDamageModifiers.Fire.SMinBase = minDamageArray[level];
        BaseDamageModifiers.Fire.SMaxBase = maxDamageArray[level];
        
        UpdateEffectStrings();
    }

    protected override void UpdateEffectStrings() {
        Effects = [
            $"Base radius is {BaseAreaRadius} metres"
        ];
    }

    public override void UseSkill() {
        if (ActorOwner != null) {
            IAreaSkill aSkill = this;
            aSkill.BasicAreaSweepSkillBehaviour(this, "fireNova", pixelSize);
        }
    }

    public void ApplyAreaSkillBehaviourToTargets(List<Actor> targets) {
        SkillDamage damage = RollForDamage(true);

        foreach (Actor actor in targets) {
            actor.ReceiveHit(DamageCategory, damage, ActorOwner.Penetrations, true);
            
            if (actor.BasicStats.CurrentLife > 0) {
                actor.ReceiveEffect(new IgniteEffect(actor, 1, damage.Fire));
            }
        }
    }
}
