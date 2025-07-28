using Godot;
using System;
using System.Collections.Generic;

public abstract class SupportGem : Item {
    protected int level = 0;
    public int Level { 
        get => level; 
        set {
            level = value;
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

        Run.Instance.GemLevelChanged += SetGemLevel;
	}

    public void SetGemLevel(int level) {
        Level = level;
    }

    protected abstract void OnGemLevelChanged();
    protected abstract void UpdateGemEffectsDescription();

    public virtual void ApplyToDamageModifiers(DamageModifiers dmgMods) {}
    public virtual void ModifyAttackSkill(IAttack attack) {}
    public virtual void ModifySpellSkill(ISpell spell) {}
    public virtual void ModifyMeleeSkill(IMeleeSkill mSkill) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
    public virtual void ModifyAreaSkill(IAreaSkill aSkill) {}
    public virtual void ModifyDurationSkill(IDurationSkill dSkill) {}
}

public partial class SuppAddedFire : SupportGem {
    private static readonly int[] minDamageArray = [
        4, 4, 5, 6, 7,
        8, 9, 10, 12, 14,
        16, 19, 22, 25, 28
    ];

    private static readonly int[] maxDamageArray = [
        8, 9, 10, 12, 14,
        16, 18, 21, 24, 28,
        32, 37, 43, 49, 56
    ];

    private int addedMinFire;
    private int addedMaxFire;

    public SuppAddedFire() {
        ItemName = "Added Fire Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinFire = minDamageArray[level];
        addedMaxFire = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {addedMinFire} to {addedMaxFire} Added Fire Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Fire.SMinAdded += addedMinFire;
        dmgMods.Fire.SMaxAdded += addedMaxFire;
    }
}

public partial class SuppAddedCold : SupportGem {
    private static readonly int[] minDamageArray = [
        5, 5, 6, 7, 8,
        10, 11, 13, 15, 17,
        20, 23, 26, 30, 35
    ];

    private static readonly int[] maxDamageArray = [
        7, 8, 9, 10, 12,
        14, 16, 18, 21, 24,
        28, 32, 37, 43, 49
    ];

    private int addedMinCold;
    private int addedMaxCold;

    public SuppAddedCold() {
        ItemName = "Added Cold Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinCold = minDamageArray[level];
        addedMaxCold = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {addedMinCold} to {addedMaxCold} Added Cold Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Cold.SMinAdded += addedMinCold;
        dmgMods.Cold.SMaxAdded += addedMaxCold;
    }
}

public partial class SuppAddedLightning : SupportGem {
    private static readonly int[] minDamageArray = [
        1, 1, 1, 1, 1,
        2, 2, 2, 3, 3,
        4, 4, 5, 6, 7
    ];

    private static readonly int[] maxDamageArray = [
        10, 11, 13, 15, 17,
        20, 23, 26, 30, 35,
        40, 46, 53, 61, 70
    ];

    private int addedMinLightning;
    private int addedMaxLightning;

    public SuppAddedLightning() {
        ItemName = "Added Lightning Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinLightning = minDamageArray[level];
        addedMaxLightning = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {addedMinLightning} to {addedMaxLightning} Added Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Lightning.SMinAdded += addedMinLightning;
        dmgMods.Lightning.SMaxAdded += addedMaxLightning;
    }
}

public partial class SuppAddedChaos : SupportGem {
    private static readonly int[] minDamageArray = [
        4, 4, 5, 6, 7,
        8, 9, 10, 12, 14,
        16, 19, 22, 25, 28
    ];

    private static readonly int[] maxDamageArray = [
        8, 9, 10, 12, 14,
        16, 18, 21, 24, 28,
        32, 37, 43, 49, 56
    ];

    private int addedMinChaos;
    private int addedMaxChaos;

    public SuppAddedChaos() {
        ItemName = "Added Chaos Damage Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinChaos = minDamageArray[level];
        addedMaxChaos = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {addedMinChaos} to {addedMaxChaos} Added Chaos Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Chaos.SMinAdded += addedMinChaos;
        dmgMods.Chaos.SMaxAdded += addedMaxChaos;
    }
}

public partial class SuppAttackSpeed : SupportGem {
    private static readonly double[] increasedAttackSpeedArray = [
        0.25, 0.26, 0.27, 0.28, 0.29,
        0.30, 0.31, 0.32, 0.33, 0.34,
        0.35, 0.36, 0.37, 0.38, 0.39
    ];

    private double attackSpeedIncrease;

    public SuppAttackSpeed() {
        ItemName = "Attack Speed Support";
        Description = "Supports Attacks";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Attack;
    }

    protected override void OnGemLevelChanged() {
        attackSpeedIncrease = increasedAttackSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {attackSpeedIncrease:P0} increased Attack Speed";
    }

    public override void ModifyAttackSkill(IAttack attack) {
        attack.ActiveAttackSpeedModifiers.SIncreased += attackSpeedIncrease;
    }
}

public partial class SuppCastSpeed : SupportGem {
    private static readonly double[] increasedCastSpeedArray = [
        0.25, 0.26, 0.27, 0.28, 0.29,
        0.30, 0.31, 0.32, 0.33, 0.34,
        0.35, 0.36, 0.37, 0.38, 0.39
    ];

    private double castSpeedIncrease;

    public SuppCastSpeed() {
        ItemName = "Cast Speed Support";
        Description = "Supports Spells";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Spell;
    }

    protected override void OnGemLevelChanged() {
        castSpeedIncrease = increasedCastSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {castSpeedIncrease:P0} increased Cast Speed";
    }

    public override void ModifySpellSkill(ISpell spell) {
        spell.ActiveCastSpeedModifiers.SIncreased += castSpeedIncrease;
    }
}

public partial class SuppPierce : SupportGem {
    private static readonly int[] addedPierceArray = [
        2, 3, 4
    ];

    private int addedPierces;

    public SuppPierce() {
        ItemName = "Pierce Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Projectile;
    }

    protected override void OnGemLevelChanged() {
        if (level >= 10) {
            addedPierces = addedPierceArray[2];
        }
        else if (level >= 5) {
            addedPierces = addedPierceArray[1];
        }
        else {
            addedPierces = addedPierceArray[0];
        }

        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill Pierces {addedPierces} additional targets with Projectiles";
    }

    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.AddedPierces += addedPierces;
    }
}

public partial class SuppMultipleProjectiles : SupportGem {
    private static readonly int[] addedProjectilesArray = [
        2, 3, 4
    ];

    private int addedProjectiles;
    private readonly double damagePenalty = 0.75;

    public SuppMultipleProjectiles() {
        ItemName = "Multiple Projectiles Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.Projectile;
    }

    protected override void OnGemLevelChanged() {
        if (level >= 10) {
            addedProjectiles = addedProjectilesArray[2];
        }
        else if (level >= 5) {
            addedProjectiles = addedProjectilesArray[1];
        }
        else {
            addedProjectiles = addedProjectilesArray[0];
        }

        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill fires {addedProjectiles} additional Projectiles\nSupported Skill deals {1 - damagePenalty:P0} less Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreAll *= damagePenalty;
    }

    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.AddedProjectiles += addedProjectiles;
    }
}

public partial class SuppIncreasedDuration : SupportGem {
    private static readonly double[] increasedDurationArray = [
        0.40, 0.41, 0.42, 0.43, 0.44,
        0.45, 0.46, 0.47, 0.48, 0.49,
        0.50, 0.51, 0.52, 0.53, 0.54
    ];

    private double incDuration;

    public SuppIncreasedDuration() {
        ItemName = "Increased Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    protected override void OnGemLevelChanged() {
        incDuration = increasedDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {incDuration:P0} increased Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.IncreasedDuration += incDuration;
    }
}

public partial class SuppLessDuration : SupportGem {
    private static readonly double[] lessDurationArray = [
        0.50, 0.49, 0.49, 0.48, 0.47, 
        0.47, 0.46, 0.45, 0.44, 0.43, 
        0.43, 0.42, 0.41, 0.41, 0.40
    ];

    private double lessDuration;

    public SuppLessDuration() {
        ItemName = "Less Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    protected override void OnGemLevelChanged() {
        lessDuration = lessDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {1 - lessDuration:P0} less Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.MoreDuration *= lessDuration;
    }
}

public partial class SuppIncreasedAoE : SupportGem {
    private static readonly double[] increasedAreaArray = [
        0.40, 0.41, 0.42, 0.43, 0.44,
        0.45, 0.46, 0.47, 0.48, 0.49,
        0.50, 0.51, 0.52, 0.53, 0.54
    ];

    private double incArea;

    public SuppIncreasedAoE() {
        ItemName = "Increased Area of Effect Support";
        Description = "Supports Area Skills";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Area;
    }

    protected override void OnGemLevelChanged() {
        incArea = increasedAreaArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {incArea:P0} increased Area of Effect";
    }

    public override void ModifyAreaSkill(IAreaSkill aSkill) {
        aSkill.IncreasedArea += incArea;
    }
}
