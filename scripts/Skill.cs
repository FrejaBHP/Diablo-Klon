using System;

public enum SkillType {
    Attack,
    Spell
}

[Flags]
public enum SkillTags {
    None = 0,
    Melee = 1 << 0,
    Projectile = 1 << 1,
    Area = 1 << 2,
    Spell = 1 << 3
}

[Flags]
public enum SkillWeapons {
    None = 0,
    Sword = 1 << 0,
    Axe = 1 << 1,
    Mace = 1 << 2,
    Dagger = 1 << 3,
    Sword2H = 1 << 4,
    Axe2H = 1 << 5,
    Mace2H = 1 << 6,
    Staff = 1 << 7,
    Bow = 1 << 8,
    Wand = 1 << 9,

    AllMeleeOneHand = Sword | Axe | Mace | Dagger,
    AllMeleeTwoHand = Sword2H | Axe2H | Mace2H | Staff,
    AllMeleeWeapons = Sword | Axe | Mace | Dagger | Sword2H | Axe2H | Mace2H | Staff,
    AllRangedWeapons = Bow | Wand
}

interface ISkill {
    static string Name { get; }
    static string Description { get; }

    static SkillType Type { get; }
    static SkillTags Tags { get; }

    float AddedDamageModifier { get; }
    float SpeedModifier { get; }

    int ManaCost { get; }
}

interface IAttack {
    static SkillWeapons Weapons { get; }
    static bool CanDualWield { get; }
}

interface ISpell {
    static float BaseCastTime { get; }
}

interface IMeleeSkill {
    float BaseAttackRange { get; }
}

interface IProjectileSkill {
    float BaseProjectileSpeed { get; }
    float BaseProjectileLifetime { get; }
}

interface IAreaSkill {
    float BaseAreaRadius { get; }
}

public class SkillDefaultMeleeAttack : ISkill, IAttack, IMeleeSkill {
    public static string Name { get; } = "Default Melee Attack";
    public static string Description { get; } = "A default Attack";

    public static SkillType Type { get; } = SkillType.Attack;
    public static SkillTags Tags { get; } = SkillTags.Melee;
    public static SkillWeapons Weapons { get; } = SkillWeapons.AllMeleeWeapons;
    public static bool CanDualWield { get; } = true;

    public float BaseAttackRange { get; } = 0.5f;

    public float AddedDamageModifier { get; private set; } = 1f;
    public float SpeedModifier { get; private set; } = 1f;

    public int ManaCost { get; private set; } = 0;
}

public class SkillDefaultProjectileAttack : ISkill, IAttack, IProjectileSkill {
    public static string Name { get; } = "Default Ranged Attack";
    public static string Description { get; } = "A default Attack";

    public static SkillType Type { get; } = SkillType.Attack;
    public static SkillTags Tags { get; } = SkillTags.Projectile;
    public static SkillWeapons Weapons { get; } = SkillWeapons.AllRangedWeapons;
    public static bool CanDualWield { get; } = true;

    public float BaseProjectileSpeed { get; } = 6f;
    public float BaseProjectileLifetime { get; } = 2f;

    public float AddedDamageModifier { get; private set; } = 1f;
    public float SpeedModifier { get; private set; } = 1f;

    public int ManaCost { get; private set; } = 0;
}
