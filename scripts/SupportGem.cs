using Godot;
using System;
using System.Collections.Generic;

public partial class SupportGem : Item {
    public string Description;
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

    protected virtual void UpdateGemDescription(){}

    public virtual void ApplyToDamageModifiers(DamageModifiers dmgMods) {}
    public virtual void ModifyProjectileSkill(IProjectileSkill pSkill) {}
}

public partial class SAddedCold : SupportGem {
    private const int addedMinCold = 5;
    private const int addedMaxCold = 7;

    public SAddedCold() {
        ItemName = "Added Cold Damage Support";
        AffectsDamageModifiers = true;
        SkillTags = ESkillTags.None;
        UpdateGemDescription();
    }

    protected override void UpdateGemDescription() {
        Description = $"Adds {addedMinCold} to {addedMaxCold} Cold Damage";
    }

    public override void ApplyToDamageModifiers(DamageModifiers dmgMods) {
        dmgMods.Cold.SMinAdded += addedMinCold;
        dmgMods.Cold.SMaxAdded += addedMaxCold;
    }
}

public partial class SPierce : SupportGem {
    private const int addedPierces = 2;

    public SPierce() {
        ItemName = "Pierce Support";
        AffectsDamageModifiers = false;
        SkillTags = ESkillTags.Projectile;
        UpdateGemDescription();
    }

    protected override void UpdateGemDescription() {
        Description = $"Projectiles Pierce an additional {addedPierces} targets";
    }

    // Problemer her kan potentielt undgås ved at give Skill Gems adgang til vedhæftede supports, og så lægge tingene sammen der og tilføje det hele til sidst
    // Bare for at der ikke permanent skal tilføjes til fx TotalPierces, men det i stedet kan blive udregnet som fx AddedPierces += addedPierces + whatever
    // Vil nok kræve et separat struct, som kan holde på denne info, og så lægge det sammen med de andre værdier
    public override void ModifyProjectileSkill(IProjectileSkill pSkill) {
        pSkill.AddedPierces += addedPierces;
    }
}
