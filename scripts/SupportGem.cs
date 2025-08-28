using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public abstract class SupportGem : Item {
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
	}

    public abstract void RollForVariant();
    protected abstract void OnVariantChosen();
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

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        addedMinFire = minDamageArray[0];
        addedMaxFire = maxDamageArray[0];
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

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        addedMinCold = minDamageArray[0];
        addedMaxCold = maxDamageArray[0];
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

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        addedMinLightning = minDamageArray[0];
        addedMaxLightning = maxDamageArray[0];
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

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        addedMinChaos = minDamageArray[0];
        addedMaxChaos = maxDamageArray[0];
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
    private const double moreAttackSpeed = 1.20;

    public SuppAttackSpeed() {
        ItemName = "Attack Speed Support";
        Description = "Supports Attacks";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Attack;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreAttackSpeed - 1:P0} more Attack Speed";
    }

    public override void ModifyAttackSkill(IAttack attack) {
        attack.ActiveAttackSpeedModifiers.SMore *= moreAttackSpeed;
    }
}

public partial class SuppCastSpeed : SupportGem {
    private const double moreCastSpeed = 1.20;

    public SuppCastSpeed() {
        ItemName = "Cast Speed Support";
        Description = "Supports Spells";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Spell;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreCastSpeed - 1:P0} more Cast Speed";
    }

    public override void ModifySpellSkill(ISpell spell) {
        spell.ActiveCastSpeedModifiers.SMore *= moreCastSpeed;
    }
}

public partial class SuppPierce : SupportGem {
    private const int addedPierces = 2;

    public SuppPierce() {
        ItemName = "Pierce Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Projectile;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
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
    private const int addedProjectiles = 2;
    private const double damagePenalty = 0.75;

    public SuppMultipleProjectiles() {
        ItemName = "Multiple Projectiles Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Projectile;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
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

public partial class SuppMoreDuration : SupportGem {
    private const double moreDuration = 1.4;

    public SuppMoreDuration() {
        ItemName = "More Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreDuration - 1:P0} more Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.MoreDuration *= moreDuration;
    }
}

public partial class SuppLessDuration : SupportGem {
    private const double moreDuration = 0.65;

    public SuppLessDuration() {
        ItemName = "Less Duration Support";
        Description = "Supports Duration Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Duration;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {1 - moreDuration:P0} less Duration of Skill Effects";
    }

    public override void ModifyDurationSkill(IDurationSkill dSkill) {
        dSkill.MoreDuration *= moreDuration;
    }
}

public partial class SuppMoreAoE : SupportGem {
    private const double moreArea = 1.3;

    public SuppMoreAoE() {
        ItemName = "More Area of Effect Support";
        Description = "Supports Area Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Area;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreArea - 1:P0} more Area of Effect";
    }

    public override void ModifyAreaSkill(IAreaSkill aSkill) {
        aSkill.MoreArea *= moreArea;
    }
}

public partial class SuppConcAoE : SupportGem {
    private const double moreAreaDamage = 1.3;
    private const double areaMultiplier = 0.6;

    public SuppConcAoE() {
        ItemName = "Concentrated Effect Support";
        Description = "Supports Area Skills";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Area;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
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

public partial class SuppBleedChance : SupportGem {
    private const double addedBleedChance = 0.5;

    private int variant = 0;
    private const double variantIncreasedBleedDuration = 0.2;

    public SuppBleedChance() {
        ItemName = "Chance to Bleed Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = true;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        variant = Utilities.RNG.Next(0, 2);

        if (variant == 1) {
            ItemName = "Chance to Bleed Support of Exsanguination";
        }

        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        StringBuilder sb = new();
        sb.Append($"Supported Skill has {addedBleedChance:P0} chance to Bleed on Hit");

        if (variant == 1) {
            sb.Append($"\nSupported Skill has {variantIncreasedBleedDuration:P0} increased Bleed Duration");
        }

        DescEffects = sb.ToString();
    }

    public override void ApplyToStatusModifiers(StatusEffectModifiers seMods) {
        seMods.Bleed.SAddedChance += addedBleedChance;

        if (variant == 1) {
            seMods.Bleed.SIncreasedDuration += variantIncreasedBleedDuration;
        }
    }
}

public partial class SuppPoisonChance : SupportGem {
    private const double addedPoisonChance = 0.5;

    private int variant = 0;
    private const double variantIncreasedPoisonDuration = 0.2;

    public SuppPoisonChance() {
        ItemName = "Chance to Poison Support";
        Description = "Supports Skills that deal damage";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = true;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        variant = Utilities.RNG.Next(0, 2);

        if (variant == 1) {
            ItemName = "Chance to Poison Support of Wasting";
        }

        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        StringBuilder sb = new();
        sb.Append($"Supported Skill has {addedPoisonChance:P0} chance to Poison on Hit");

        if (variant == 1) {
            sb.Append($"\nSupported Skill has {variantIncreasedPoisonDuration:P0} increased Poison Duration");
        }

        DescEffects = sb.ToString();
    }

    public override void ApplyToStatusModifiers(StatusEffectModifiers seMods) {
        seMods.Poison.SAddedChance += addedPoisonChance;

        if (variant == 1) {
            seMods.Poison.SIncreasedDuration += variantIncreasedPoisonDuration;
        }
    }
}

public partial class SuppBrutality : SupportGem {
    private const double morePhysDamage = 1.3;
    private const double moreNonPhysDamage = 0;

    private int variant = 0;
    private const double variantBleedDamageMagnitude = 1.05;

    public SuppBrutality() {
        ItemName = "Brutality Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        variant = Utilities.RNG.Next(0, 2);

        if (variant == 1) {
            ItemName = "Brutality Support of Deep Cuts";
        }

        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        StringBuilder sb = new();
        sb.Append($"Supported Skill deals {morePhysDamage - 1:P0} more Physical Damage\nSupported Skill deals no non-Physical Damage");

        if (variant == 1) {
            sb.Append($"\nSupported Skill has {variantBleedDamageMagnitude - 1:P0} increased Bleed Magnitude");
        }

        DescEffects = sb.ToString();
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Physical.SMore *= morePhysDamage;
        dmgMods.Fire.SMore *= moreNonPhysDamage;
        dmgMods.Cold.SMore *= moreNonPhysDamage;
        dmgMods.Lightning.SMore *= moreNonPhysDamage;
        dmgMods.Chaos.SMore *= moreNonPhysDamage;

        if (variant == 1) {
            dmgMods.BleedMagnitude += variantBleedDamageMagnitude;
        }
    }
}

public partial class SuppBlisteringHeat : SupportGem {
    private const double damageAsExtraFire = 0.25;
    private const double moreOtherEleDamage = 0.5;

    public SuppBlisteringHeat() {
        ItemName = "Blistering Heat Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {damageAsExtraFire:P0} of Damage as Extra Fire Damage\nSupported Skill deals {1 - moreOtherEleDamage:P0} less Cold and Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.ExtraFire += damageAsExtraFire;
        dmgMods.Cold.SMore *= moreOtherEleDamage;
        dmgMods.Lightning.SMore *= moreOtherEleDamage;
    }
}

public partial class SuppSheerCold : SupportGem {
    private const double damageAsExtraCold = 0.25;
    private const double moreOtherEleDamage = 0.5;

    public SuppSheerCold() {
        ItemName = "Sheer Cold Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {damageAsExtraCold:P0} of Damage as Extra Cold Damage\nSupported Skill deals {1 - moreOtherEleDamage:P0} less Fire and Lightning Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.ExtraCold += damageAsExtraCold;
        dmgMods.Fire.SMore *= moreOtherEleDamage;
        dmgMods.Lightning.SMore *= moreOtherEleDamage;
    }
}

public partial class SuppVolatileCurrent : SupportGem {
    private const double damageAsExtraLightning = 0.25;
    private const double moreOtherEleDamage = 0.5;

    public SuppVolatileCurrent() {
        ItemName = "Volatile Current Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill gains {damageAsExtraLightning:P0} of Damage as Extra Lightning Damage\nSupported Skill deals {1 - moreOtherEleDamage:P0} less Fire and Cold Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.ExtraLightning += damageAsExtraLightning;
        dmgMods.Fire.SMore *= moreOtherEleDamage;
        dmgMods.Cold.SMore *= moreOtherEleDamage;
    }
}
