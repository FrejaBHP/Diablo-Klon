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
    public double ManaCostMultiplier { get; protected set; } = 1;

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
    public virtual void ModifySkill(Skill skill) {}
    public virtual void ModifyAttackSkill(IAttack attack) {}
    public virtual void ModifySpellSkill(ISpell spell) {}
    public virtual void ModifyMeleeSkill(IMeleeSkill mSkill) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
    public virtual void ModifyAreaSkill(IAreaSkill aSkill) {}
    public virtual void ModifyDurationSkill(IDurationSkill dSkill) {}
}

public partial class SuppAttackSpeed : SupportGem {
    private const double moreAttackSpeed = 1.20;

    public SuppAttackSpeed() {
        ItemName = "Attack Speed Support";
        Description = "Supports Attacks";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Attack;
        ManaCostMultiplier = 1.1;
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
        ManaCostMultiplier = 1.1;
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
        ManaCostMultiplier = 1.15;
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
        pSkill.Pierces.SAdded += addedPierces;
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
        ManaCostMultiplier = 1.25;
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
        pSkill.NumberOfProjectiles.SAdded += addedProjectiles;
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
        ManaCostMultiplier = 1.15;
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
        ManaCostMultiplier = 1.15;
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
        ManaCostMultiplier = 1.15;
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
        ManaCostMultiplier = 1.1;
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
        ManaCostMultiplier = 1.1;
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
    private const double variantBleedDamageMagnitude = 1.15;

    public SuppBrutality() {
        ItemName = "Brutality Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.2;
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
            sb.Append($"\nSupported Skill has {variantBleedDamageMagnitude - 1:P0} increased Bleed Damage Multiplier");
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
            dmgMods.IncreasedBleedMagnitude += variantBleedDamageMagnitude;
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
        ManaCostMultiplier = 1.15;
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
        ManaCostMultiplier = 1.15;
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
        ManaCostMultiplier = 1.15;
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

public partial class SuppDeadlyPoison : SupportGem {
    private const double morePoisonMagnitude = 1.75;
    private const double lessHitDamage = 0.75;

    public SuppDeadlyPoison() {
        ItemName = "Deadly Poison Support";
        Description = "Supports Skills that Hit Enemies";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.15;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {morePoisonMagnitude - 1:P0} more Poison Damage Multiplier\nSupported Skill deals {1 - lessHitDamage:P0} less Damage with Hits";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MorePoisonMagnitude *= morePoisonMagnitude;
        dmgMods.HitDamageMultiplier *= lessHitDamage;
    }
}

public partial class SuppHemorrhage : SupportGem {
    private const double moreBleedMagnitude = 1.75;
    private const double lessHitDamage = 0.75;

    public SuppHemorrhage() {
        ItemName = "Hemorrhage Support";
        Description = "Supports Skills that Hit Enemies";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.15;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreBleedMagnitude - 1:P0} more Bleed Damage Multiplier\nSupported Skill deals {1 - lessHitDamage:P0} less Damage with Hits";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreBleedMagnitude *= moreBleedMagnitude;
        dmgMods.HitDamageMultiplier *= lessHitDamage;
    }
}

public partial class SuppSearingHeat : SupportGem {
    private const double moreIgniteMagnitude = 1.75;
    private const double lessHitDamage = 0.75;

    public SuppSearingHeat() {
        ItemName = "Searing Warmth Support";
        Description = "Supports Skills that Hit Enemies";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.15;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreIgniteMagnitude - 1:P0} more Ignite Damage Multiplier\nSupported Skill deals {1 - lessHitDamage:P0} less Damage with Hits";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreIgniteMagnitude *= moreIgniteMagnitude;
        dmgMods.HitDamageMultiplier *= lessHitDamage;
    }
}

public partial class SuppMoreProjSpeed : SupportGem {
    private const double moreSpeed = 1.4;

    public SuppMoreProjSpeed() {
        ItemName = "More Projectile Speed Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Projectile;
        ManaCostMultiplier = 1.1;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreSpeed - 1:P0} more Projectile Speed";
    }

    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.ProjectileSpeed.SMore *= moreSpeed;
    }
}

public partial class SuppLessProjSpeed : SupportGem {
    private const double moreSpeed = 0.6;

    public SuppLessProjSpeed() {
        ItemName = "Less Projectile Speed Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.Projectile;
        ManaCostMultiplier = 1.1;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {1 - moreSpeed:P0} less Projectile Speed";
    }

    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.ProjectileSpeed.SMore *= moreSpeed;
    }
}

public partial class SuppCritChance : SupportGem {
    private const double increasedCritChance = 1;

    public SuppCritChance() {
        ItemName = "Critical Chance Support";
        Description = "Supports Skills that Hit Enemies";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.1;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {increasedCritChance:P0} increased Critical Strike Chance";
    }

    public override void ModifySkill(Skill skill) {
        skill.CriticalStrikeChance.SIncreased += increasedCritChance;
    }
}

public partial class SuppChain : SupportGem {
    private const int addedChains = 2;
    private const double damagePenalty = 0.65;

    public SuppChain() {
        ItemName = "Chain Support";
        Description = "Supports Projectile Skills";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.25;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill Chains {addedChains} additional times with Projectiles\nSupported Skill deals {1 - damagePenalty:P0} less Damage with Hits";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.HitDamageMultiplier *= damagePenalty;
    }

    public override void ModifySkill(Skill skill) {
        skill.Chains.SAdded += addedChains;
    }
}

public partial class SuppExecute : SupportGem {
    private const double moreLowLifeDamage = 1.35;

    public SuppExecute() {
        ItemName = "Execute Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.20;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill deals {moreLowLifeDamage - 1:P0} more Damage with Hits against Enemies with 50% Life or less";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.MoreLowLife *= moreLowLifeDamage;
    }
}

public partial class SuppCritChanceLowLife : SupportGem {
    private const double moreCritChance = 1.4;

    public SuppCritChanceLowLife() {
        ItemName = "Assassin Support";
        Description = "Supports Skills that Hit Enemies";
        AffectsDamageModifiers = false;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.15;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {moreCritChance - 1:P0} more Critical Strike Chance against Enemies at 50% Life or less";
    }

    public override void ModifySkill(Skill skill) {
        skill.MoreCriticalStrikeChanceToLowLife *= moreCritChance;
    }
}

public partial class SuppPhysToFire : SupportGem {
    private const double physToFireConversion = 0.5;

    public SuppPhysToFire() {
        ItemName = "Physical to Fire Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.10;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {physToFireConversion:P0} of Physical Damage Converted to Fire Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Conversion.Physical.ToFire.Values[1] += physToFireConversion;
    }
}

public partial class SuppEleToChaos : SupportGem {
    private const double eleToChaosConversion = 0.33;

    public SuppEleToChaos() {
        ItemName = "Elemental Chaos Support";
        Description = "Supports Skills that deal Damage";
        AffectsDamageModifiers = true;
        AffectsStatusModifiers = false;
        SkillTags = ESkillTags.None;
        ManaCostMultiplier = 1.10;
    }

    public override void RollForVariant() {
        OnVariantChosen();
    }

    protected override void OnVariantChosen() {
        UpdateGemEffectsDescription();
    }

    protected override void UpdateGemEffectsDescription() {
        DescEffects = $"Supported Skill has {eleToChaosConversion:P0} of Elemental Damage Converted to Chaos Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Conversion.Fire.ToChaos.Values[1] += eleToChaosConversion;
        dmgMods.Conversion.Cold.ToChaos.Values[1] += eleToChaosConversion;
        dmgMods.Conversion.Lightning.ToChaos.Values[1] += eleToChaosConversion;
    }
}
