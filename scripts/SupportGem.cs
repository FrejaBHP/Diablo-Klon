using Godot;
using System;
using System.Collections.Generic;

public partial class SupportGem : Item {
    protected const int maxLevel = 14;

    protected int level = 0;
    public int Level { 
        get => level; 
        set {
            if (value > maxLevel) {
                level = maxLevel;
            }
            else if (value < 0) {
                level = 0;
            }
            else {
                level = value;
            }

            OnGemLevelChanged();
        } 
    }

    public string Description { get; protected set; }
    public string DescEffects { get; protected set; }
    public bool AffectsDamageModifiers { get; protected set; }
	public ESkillTags SkillTags { get; protected set; }

	public SupportGem() {
		ItemAllBaseType = EItemAllBaseType.SkillSupport;
		CanRarityChange = false;
		MagicMaxPrefixes = 0;
		MagicMaxSuffixes = 0;
		RareMaxPrefixes = 0;
		RareMaxSuffixes = 0;

		gridSizeX = 1;
		gridSizeY = 1;
	}

    protected virtual void OnGemLevelChanged() {}
    protected virtual void UpdateGemEffectsDescription(){}

    public virtual void ApplyToDamageModifiers(DamageModifiers dmgMods) {}
    public virtual void ModifyAttackSkill(IAttack attack) {}
    public virtual void ModifySpellSkill(ISpell spell) {}
    public virtual void ModifyMeleeSkill(IMeleeSkill mSkill) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
    public virtual void ModifyAreaSkill(IAreaSkill aSkill) {}
    public virtual void ModifyDurationSkill(IDurationSkill dSkill) {}
}

public partial class SAddedFire : SupportGem {
    private const int addedMinFire = 4;
    private const int addedMaxFire = 8;

    public SAddedFire() {
        ItemName = "Added Fire Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Adds {addedMinFire} to {addedMaxFire} Fire Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Fire.SMinAdded += addedMinFire;
        dmgMods.Fire.SMaxAdded += addedMaxFire;
    }
}

public partial class SAddedCold : SupportGem {
    private const int addedMinCold = 5;
    private const int addedMaxCold = 7;

    public SAddedCold() {
        ItemName = "Added Cold Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Adds {addedMinCold} to {addedMaxCold} Cold Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Cold.SMinAdded += addedMinCold;
        dmgMods.Cold.SMaxAdded += addedMaxCold;
    }
}

public partial class SAddedLightning : SupportGem {
    private const int addedMinLightning = 1;
    private const int addedMaxLightning = 10;

    public SAddedLightning() {
        ItemName = "Added Lightning Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Adds {addedMinLightning} to {addedMaxLightning} Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Lightning.SMinAdded += addedMinLightning;
        dmgMods.Lightning.SMaxAdded += addedMaxLightning;
    }
}

public partial class SAddedChaos : SupportGem {
    private const int addedMinChaos = 4;
    private const int addedMaxChaos = 8;

    public SAddedChaos() {
        ItemName = "Added Chaos Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Adds {addedMinChaos} to {addedMaxChaos} Chaos Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Chaos.SMinAdded += addedMinChaos;
        dmgMods.Chaos.SMaxAdded += addedMaxChaos;
    }
}

public partial class SAttackSpeed : SupportGem {
    private static readonly double[] increasedAttackSpeedArray = [
        0.25, 0.26, 0.27, 0.28, 0.29,
        0.30, 0.31, 0.32, 0.33, 0.34,
        0.35, 0.36, 0.37, 0.38, 0.39
    ];

    private double attackSpeedIncrease = increasedAttackSpeedArray[0];

    public SAttackSpeed() {
        ItemName = "Attack Speed Support";
        Description = "Supports Attacks";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Attack;
        UpdateGemEffectsDescription();
    }

    protected override void OnGemLevelChanged() {
        attackSpeedIncrease = increasedAttackSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"{attackSpeedIncrease:P0} increased Attack Speed";
    }

    public override void ModifyAttackSkill(IAttack attack) {
        attack.ActiveAttackSpeedModifiers.SIncreased += attackSpeedIncrease;
    }
}

public partial class SCastSpeed : SupportGem {
    private static readonly double[] increasedCastSpeedArray = [
        0.25, 0.26, 0.27, 0.28, 0.29,
        0.30, 0.31, 0.32, 0.33, 0.34,
        0.35, 0.36, 0.37, 0.38, 0.39
    ];

    private double castSpeedIncrease = increasedCastSpeedArray[0];

    public SCastSpeed() {
        ItemName = "Cast Speed Support";
        Description = "Supports Spells";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Spell;
        UpdateGemEffectsDescription();
    }

    protected override void OnGemLevelChanged() {
        castSpeedIncrease = increasedCastSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"{castSpeedIncrease:P0} increased Cast Speed";
    }

    public override void ModifySpellSkill(ISpell spell) {
        spell.ActiveCastSpeedModifiers.SIncreased += castSpeedIncrease;
    }
}

public partial class SPierce : SupportGem {
    private const int addedPierces = 2;

    public SPierce() {
        ItemName = "Pierce Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Projectile;
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Projectiles Pierce {addedPierces} additional targets";
    }

    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.AddedPierces += addedPierces;
    }
}

public partial class SIncreasedDuration : SupportGem {
    private static readonly double[] increasedDurationArray = [
        0.40, 0.41, 0.42, 0.43, 0.44,
        0.45, 0.46, 0.47, 0.48, 0.49,
        0.50, 0.51, 0.52, 0.53, 0.54
    ];

    private double incDuration = increasedDurationArray[0];

    public SIncreasedDuration() {
        ItemName = "Increased Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Duration;
        UpdateGemEffectsDescription();
    }

    protected override void OnGemLevelChanged() {
        incDuration = increasedDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"{incDuration:P0} increased Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.IncreasedDuration += incDuration;
    }
}

public partial class SLessDuration : SupportGem {
    private static readonly double[] lessDurationArray = [
        0.50, 0.51, 0.51, 0.52, 0.53, 
        0.53, 0.54, 0.55, 0.56, 0.57, 
        0.57, 0.58, 0.59, 0.59, 0.60
    ];

    private double lessDuration = lessDurationArray[0];

    public SLessDuration() {
        ItemName = "Less Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Duration;
        UpdateGemEffectsDescription();
    }

    protected override void OnGemLevelChanged() {
        lessDuration = lessDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"{1 - lessDuration:P0} less Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.MoreDuration *= lessDuration;
    }
}
