using Godot;
using System;
using System.Collections.Generic;
using System.Text;

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
    public bool AffectsStatusModifiers { get; protected set; }
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

        if (Run.Instance.Rules.ExistingGemsScaleWithGameProgress) {
			Run.Instance.GemLevelChanged += SetGemLevel;
		}
	}

    public void SetGemLevel(int level) {
        Level = level;
    }

    protected abstract void OnGemLevelChanged();
    protected abstract void UpdateGemEffectsDescription();

    public virtual void ApplyToDamageModifiers(DamageModifiers dmgMods) {}
    public virtual void ApplyToStatusModifiers(StatusEffectModifiers seMods) {}
    public virtual void ModifyAttackSkill(IAttack attack) {}
    public virtual void ModifySpellSkill(ISpell spell) {}
    public virtual void ModifyMeleeSkill(IMeleeSkill mSkill) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
    public virtual void ModifyAreaSkill(IAreaSkill aSkill) {}
    public virtual void ModifyDurationSkill(IDurationSkill dSkill) {}
}

public partial class SuppAddedFire : SupportGem {
    private static readonly int[] minDamageArray = [
        4, 4, 5, 5, 6,
        7, 8, 9, 10, 11,
        12, 13, 15, 17, 19
    ];

    private static readonly int[] maxDamageArray = [
        8, 9, 10, 11, 12,
        13, 15, 17, 19, 22,
        25, 28, 31, 35, 39
    ];

    private int addedMinFire;
    private int addedMaxFire;

    public SuppAddedFire() {
        ItemName = "Added Fire Damage Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinFire = minDamageArray[level];
        addedMaxFire = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {addedMinFire} to {addedMaxFire} Added Fire Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Fire.SMinAdded += addedMinFire;
        dmgMods.Fire.SMaxAdded += addedMaxFire;
    }
}

public partial class SuppAddedCold : SupportGem {
    private static readonly int[] minDamageArray = [
        5, 5, 6, 7, 8,
        9, 10, 11, 12, 13,
        15, 17, 19, 21, 24
    ];

    private static readonly int[] maxDamageArray = [
        7, 8, 8, 9, 10,
        11, 13, 15, 17, 19,
        21, 24, 27, 30, 34
    ];

    private int addedMinCold;
    private int addedMaxCold;

    public SuppAddedCold() {
        ItemName = "Added Cold Damage Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinCold = minDamageArray[level];
        addedMaxCold = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {addedMinCold} to {addedMaxCold} Added Cold Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Cold.SMinAdded += addedMinCold;
        dmgMods.Cold.SMaxAdded += addedMaxCold;
    }
}

public partial class SuppAddedLightning : SupportGem {
    private static readonly int[] minDamageArray = [
        1, 1, 1, 1, 1,
        1, 1, 2, 2, 2,
        2, 3, 3, 4, 4
    ];

    private static readonly int[] maxDamageArray = [
        10, 11, 12, 13, 15,
        17, 19, 21, 24, 27,
        30, 34, 38, 43, 48
    ];

    private int addedMinLightning;
    private int addedMaxLightning;

    public SuppAddedLightning() {
        ItemName = "Added Lightning Damage Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinLightning = minDamageArray[level];
        addedMaxLightning = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {addedMinLightning} to {addedMaxLightning} Added Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Lightning.SMinAdded += addedMinLightning;
        dmgMods.Lightning.SMaxAdded += addedMaxLightning;
    }
}

public partial class SuppAddedChaos : SupportGem {
    private static readonly int[] minDamageArray = [
        4, 4, 5, 5, 6,
        7, 8, 9, 10, 11,
        12, 13, 15, 17, 19
    ];

    private static readonly int[] maxDamageArray = [
        8, 9, 10, 11, 12,
        13, 15, 17, 19, 22,
        25, 28, 31, 35, 39
    ];

    private int addedMinChaos;
    private int addedMaxChaos;

    public SuppAddedChaos() {
        ItemName = "Added Chaos Damage Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        addedMinChaos = minDamageArray[level];
        addedMaxChaos = maxDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {addedMinChaos} to {addedMaxChaos} Added Chaos Damage";
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
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Attack;
    }

    protected override void OnGemLevelChanged() {
        attackSpeedIncrease = increasedAttackSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {attackSpeedIncrease:P0} increased Attack Speed";
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
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Spell;
    }

    protected override void OnGemLevelChanged() {
        castSpeedIncrease = increasedCastSpeedArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {castSpeedIncrease:P0} increased Cast Speed";
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
        AffectsStatusModifiers = false;
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
        AffectsStatusModifiers = false;
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
        DescEffects = $"Supported Skill fires {addedProjectiles} additional Projectiles\nSupported Skill deals {1 - damagePenalty:P0} less Projectile Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreProjectile *= damagePenalty;
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
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    protected override void OnGemLevelChanged() {
        incDuration = increasedDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {incDuration:P0} increased Duration of Skill Effects";
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
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    protected override void OnGemLevelChanged() {
        lessDuration = lessDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {1 - lessDuration:P0} less Duration of Skill Effects";
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
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Area;
    }

    protected override void OnGemLevelChanged() {
        incArea = increasedAreaArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {incArea:P0} increased Area of Effect";
    }

    public override void ModifyAreaSkill(IAreaSkill aSkill) {
        aSkill.IncreasedArea += incArea;
    }
}

public partial class SuppConcAoE : SupportGem {
    private static readonly double[] moreAreaDamageArray = [
        1.30, 1.31, 1.32, 1.33, 1.34,
        1.35, 1.36, 1.37, 1.38, 1.39,
        1.40, 1.41, 1.42, 1.43, 1.44
    ];

    private double moreAreaDamage;
    private const double areaMultiplier = 0.7;

    public SuppConcAoE() {
        ItemName = "Concentrated Effect Support";
        Description = "Supports Area Skills";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Area;
    }

    protected override void OnGemLevelChanged() {
        moreAreaDamage = moreAreaDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {moreAreaDamage - 1:P0} more Area Damage\nSupported Skill has {1 - areaMultiplier:P0} less Area of Effect";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreArea *= moreAreaDamage;
    }

    public override void ModifyAreaSkill(IAreaSkill aSkill) {
        aSkill.MoreArea *= areaMultiplier;
    }
}

public partial class SuppPoisonChance : SupportGem {
    private static readonly double[] increasedPoisonDurationArray = [
        0.00, 0.02, 0.04, 0.06, 0.08,
        0.10, 0.12, 0.14, 0.16, 0.18,
        0.20, 0.22, 0.24, 0.26, 0.28
    ];

    private double incPoisonDuration;
    private const double addedPoisonChance = 0.5;

    public SuppPoisonChance() {
        ItemName = "Chance to Poison Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = true;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        incPoisonDuration = increasedPoisonDurationArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        StringBuilder sb = new();
        sb.Append($"Supported Skill has {addedPoisonChance:P0} chance to Poison on Hit");
        if (level > 0) {
            sb.Append($"\nSupported Skill has {incPoisonDuration:P0} increased Poison duration");
        }

        DescEffects = sb.ToString();
    }

    public override void ApplyToStatusModifiers(StatusEffectModifiers seMods) {
        seMods.Poison.SAddedChance += addedPoisonChance;
        seMods.Poison.SIncreasedDuration += incPoisonDuration;
    }
}

public partial class SuppBrutality : SupportGem {
    private static readonly double[] morePhysDamageArray = [
        1.30, 1.31, 1.32, 1.33, 1.34,
        1.35, 1.36, 1.37, 1.38, 1.39,
        1.40, 1.41, 1.42, 1.43, 1.44
    ];

    private double morePhysDamage;
    private const double nonPhysMultiplier = 0;

    public SuppBrutality() {
        ItemName = "Brutality Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        morePhysDamage = morePhysDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {morePhysDamage - 1:P0} more Physical Damage\nSupported Skill deals no non-Physical Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Physical.SMore *= morePhysDamage;
        dmgMods.Fire.SMore *= nonPhysMultiplier;
        dmgMods.Cold.SMore *= nonPhysMultiplier;
        dmgMods.Lightning.SMore *= nonPhysMultiplier;
        dmgMods.Chaos.SMore *= nonPhysMultiplier;
    }
}

public partial class SuppSheerCold : SupportGem {
    private static readonly double[] moreColdDamageArray = [
        1.25, 1.26, 1.27, 1.28, 1.29,
        1.30, 1.31, 1.32, 1.33, 1.34,
        1.35, 1.36, 1.37, 1.38, 1.39
    ];

    private double moreColdDamage;
    private const double otherEleMultiplier = 0.5;

    public SuppSheerCold() {
        ItemName = "Sheer Cold Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    protected override void OnGemLevelChanged() {
        moreColdDamage = moreColdDamageArray[level];
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {moreColdDamage - 1:P0} more Cold Damage\nSupported Skill deals {otherEleMultiplier - 1:P0} less Fire and Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Cold.SMore *= moreColdDamage;
        dmgMods.Fire.SMore *= otherEleMultiplier;
        
        dmgMods.Lightning.SMore *= otherEleMultiplier;
    }
}
