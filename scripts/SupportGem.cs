using Godot;
using System;
using System.Collections.Generic;

public partial class SupportGem : Item {
    public string Description;
    public string DescEffects;
    public bool AffectsDamageModifiers;
	public ESkillTags SkillTags;

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

    protected virtual void UpdateGemEffectsDescription(){}

    public virtual void ApplyToDamageModifiers(DamageModifiers dmgMods) {}
    public virtual void ModifyAttackSkill(IAttack attack) {}
    public virtual void ModifySpellSkill(ISpell spell) {}
    public virtual void ModifyMeleeSkill(IMeleeSkill mSkill) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
    public virtual void ModifyAreaSkill(IAreaSkill aSkill) {}
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
    private const double attackSpeedIncrease = 0.25;

    public SAttackSpeed() {
        ItemName = "Attack Speed Support";
        Description = "Supports Attacks";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Attack;
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
    private const double castSpeedIncrease = 0.25;

    public SCastSpeed() {
        ItemName = "Cast Speed Support";
        Description = "Supports Spells";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Spell;
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
