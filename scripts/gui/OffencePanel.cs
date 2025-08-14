using Godot;
using System;

public partial class OffencePanel : StatPanel {
    public StatTableEntry AddedAttackPhysDamage;
    public StatTableEntry AddedSpellPhysDamage;
    public StatTableEntry IncreasedPhysDamage;
    public StatTableEntry PhysicalPenetration;

    public StatTableEntry AddedAttackFireDamage;
    public StatTableEntry AddedSpellFireDamage;
    public StatTableEntry IncreasedFireDamage;
    public StatTableEntry FirePenetration;

    public StatTableEntry AddedAttackColdDamage;
    public StatTableEntry AddedSpellColdDamage;
    public StatTableEntry IncreasedColdDamage;
    public StatTableEntry ColdPenetration;

    public StatTableEntry AddedAttackLightningDamage;
    public StatTableEntry AddedSpellLightningDamage;
    public StatTableEntry IncreasedLightningDamage;
    public StatTableEntry LightningPenetration;

    public StatTableEntry AddedAttackChaosDamage;
    public StatTableEntry AddedSpellChaosDamage;
    public StatTableEntry IncreasedChaosDamage;
    public StatTableEntry ChaosPenetration;

    public StatTableEntry IncreasedAttackDamage;
    public StatTableEntry IncreasedSpellDamage;
    public StatTableEntry IncreasedMeleeDamage;
    public StatTableEntry IncreasedProjectileDamage;
    public StatTableEntry IncreasedAreaDamage;
    public StatTableEntry IncreasedDamageOverTime;

    public StatTableEntry IncreasedBleedDamage;
    public StatTableEntry IncreasedBleedDuration;
    public StatTableEntry IncreasedIgniteDamage;
    public StatTableEntry IncreasedIgniteDuration;
    public StatTableEntry IncreasedPoisonDamage;
    public StatTableEntry IncreasedPoisonDuration;

    public StatTableEntry IncreasedAttackSpeed;
    public StatTableEntry IncreasedCastSpeed;
    public StatTableEntry IncreasedCritChance;
    public StatTableEntry CritMulti;
    
    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        AddEntry(ref AddedAttackPhysDamage, "Added Physical Damage to Attacks");
        AddEntry(ref AddedSpellPhysDamage, "Added Physical Damage to Spells");
        AddEntry(ref IncreasedPhysDamage, "Physical Damage Modifier");
        AddEntry(ref PhysicalPenetration, "Physical Penetration");

        AddEntry(ref AddedAttackFireDamage, "Added Fire Damage to Attacks");
        AddEntry(ref AddedSpellFireDamage, "Added Fire Damage to Spells");
        AddEntry(ref IncreasedFireDamage, "Fire Damage Modifier");
        AddEntry(ref FirePenetration, "Fire Penetration");

        AddEntry(ref AddedAttackColdDamage, "Added Cold Damage to Attacks");
        AddEntry(ref AddedSpellColdDamage, "Added Cold Damage to Spells");
        AddEntry(ref IncreasedColdDamage, "Cold Damage Modifier");
        AddEntry(ref ColdPenetration, "Cold Penetration");

        AddEntry(ref AddedAttackLightningDamage, "Added Lightning Damage to Attacks");
        AddEntry(ref AddedSpellLightningDamage, "Added Lightning Damage to Spells");
        AddEntry(ref IncreasedLightningDamage, "Lightning Damage Modifier");
        AddEntry(ref LightningPenetration, "Lightning Penetration");

        AddEntry(ref AddedAttackChaosDamage, "Added Chaos Damage to Attacks");
        AddEntry(ref AddedSpellChaosDamage, "Added Chaos Damage to Spells");
        AddEntry(ref IncreasedChaosDamage, "Chaos Damage Modifier");
        AddEntry(ref ChaosPenetration, "Chaos Penetration");

        AddEntry(ref IncreasedAttackDamage, "Attack Damage Modifier");
        AddEntry(ref IncreasedSpellDamage, "Spell Damage Modifier");
        AddEntry(ref IncreasedMeleeDamage, "Melee Damage Modifier");
        AddEntry(ref IncreasedProjectileDamage, "Projectile Damage Modifier");
        AddEntry(ref IncreasedAreaDamage, "Area Damage Modifier");
        AddEntry(ref IncreasedDamageOverTime, "Damage Over Time Modifier");

        AddEntry(ref IncreasedBleedDamage, "Bleed Damage Modifier");
        AddEntry(ref IncreasedIgniteDamage, "Ignite Damage Modifier");
        AddEntry(ref IncreasedPoisonDamage, "Poison Damage Modifier");

        AddEntry(ref IncreasedBleedDuration, "Bleed Duration Modifier");
        AddEntry(ref IncreasedIgniteDuration, "Ignite Duration Modifier");
        AddEntry(ref IncreasedPoisonDuration, "Poison Duration Modifier");

        AddEntry(ref IncreasedAttackSpeed, "Attack Speed Modifier");
        AddEntry(ref IncreasedCastSpeed, "Cast Speed Modifier");
        AddEntry(ref IncreasedCritChance, "Critical Strike Chance Modifier");
        AddEntry(ref CritMulti, "Critical Strike Multiplier");
    }
}
