using Godot;
using System;

public enum EDamageType {
    Untyped,
    Physical,   // DoT: Bleed/Bleeding
    Fire,       // DoT: Ignite - Burn/Burning
    Cold,       // DoT: Frostburn/Frostburning
    Lightning,  // DoT: Electrocute/Electrocuted
    Chaos       // DoT: Poison
}

public enum EEffectName {
	Ignite,
	Poison,
	Soulrend,
	Bleed,
	SpeedBoost,
	ArcaneSurge
}

public enum EEffectRating {
    Neutral,
    Negative,
    Positive
}

[Flags]
public enum EDamageInfoFlags {
	None = 0,
	Critical = 1 << 0,
	Ignites = 1 << 1
}

[Flags]
public enum EDamageFeedbackInfoFlags {
	None = 0,
	Critical = 1 << 0,
	Ignited = 1 << 1
}


public enum EMovementInputMethod {
	Mouse,
	Keyboard
}


public enum EMapType {
	Town,
	Intermission,
	Objective,
	Miniboss,
	Boss
}

public enum EMapObjective {
	None,
	Survival,
	Waves
}


public enum EEnemyType {
	TestEnemy,
	TestEnemy2,
	TestEnemy3,
}


[Flags]
public enum EActorFlags : long {
	None = 0,
	AllDamageCanBleed = 1 << 0,
	AllDamageCanIgnite = 1 << 1,
	AllDamageCanChill = 1 << 2,
	AllDamageCanShock = 1 << 3,
	AllDamageCanPoison = 1 << 4,
	DamageScalesWithBlockChance = 1 << 5,
	DoubleArmourAgainstCrits = 1 << 6,
	DamageScalesWithMaxMana = 1 << 7,
	GainArcaneSurgeOnManaSpent = 1 << 8,
}

// ======== LABELS =======

public enum ELabelColourSet {
	Default,
	Item
}

public enum ETextColour {
	Default,
	Common,
	Magic,
	Rare,
	Unique,
	Skill
}


public enum EDamageCategory {
	Melee,
	Ranged,
	Spell,
	Untyped
}

public enum ESkillName {
	BasicThrust,
	PiercingShot,
	PrismaticBolt,
	SplitArrow,
	FireNova,
	CobraShot,
	Soulrend,
	Cleave,
	Fireball,
	COUNT
}

public enum ESkillType {
    Attack,
    Spell
}

[Flags]
public enum ESkillTags {
    None = 0,
	Attack = 1 << 0,
    Spell = 1 << 1,
	
    Melee = 1 << 2,
    Ranged = 1 << 3,

	Projectile = 1 << 4,
    Area = 1 << 5,
	Duration = 1 << 6,

	Physical = 1 << 7,
	Fire = 1 << 8,
	Cold = 1 << 9,
	Lightning = 1 << 10,
	Chaos = 1 << 11
}

[Flags]
public enum ESkillDamageTags {
	None = 0,
	Attack = 1 << 0,
    Spell = 1 << 1,
	Melee = 1 << 2,
	Projectile = 1 << 3,
    Area = 1 << 4,
	DoT = 1 << 5,

	Bleed = 1 << 6,
	Burn = 1 << 7,
	Poison = 1 << 8,
}

[Flags]
public enum ESkillStatusEffectFlags {
	None = 0,
	CanBleed = 1 << 0,
	CanIgnite = 1 << 1,
	CanChill = 1 << 2,
	CanShock = 1 << 3,
	CanPoison = 1 << 4,
	CanSlow = 1 << 5,
}

public enum ESkillProjectileType {
	Default,
	Soulrend,
}

[Flags]
public enum ESkillWeapons {
    None = 0,
    Unarmed = 1 << 0,
    Melee1H = 1 << 1,
    Melee2H = 1 << 2,
    MeleeDW = 1 << 3,
    Ranged1H = 1 << 4,
    Ranged2H = 1 << 5,

    AllMelee = Unarmed | Melee1H | Melee2H | MeleeDW,
	AllMeleeWeapons = Melee1H | Melee2H | MeleeDW,
    AllRangedWeapons = Ranged1H | Ranged2H
}


public enum EActorState {
	Actionable,
	UsingSkill,
	Stunned,
	Dying,
	Dead
}

public enum EEnemyRarity {
	Normal,
	Magic,
	Rare,
	Unique,
	Boss
}

public enum EShopType {
	Equipment,
	Gems
}

public enum EItemRewardType {
	Weapons,
	Armour,
	Accessories,
	Skills,
	Supports
}


public enum EPlayerClass {
	Juggernaut,
	Sorcerer,
	Occultist,
	Assassin,
	Pathfinder,
}
