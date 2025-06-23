using Godot;
using System;

public partial class OffencePanel : StatPanel {
    public StatTableEntry AddedPhysDamage;
    public StatTableEntry IncreasedPhysDamage;
    public StatTableEntry AddedFireDamage;
    public StatTableEntry IncreasedFireDamage;
    public StatTableEntry AddedColdDamage;
    public StatTableEntry IncreasedColdDamage;
    public StatTableEntry AddedLightningDamage;
    public StatTableEntry IncreasedLightningDamage;
    public StatTableEntry AddedChaosDamage;
    public StatTableEntry IncreasedChaosDamage;

    public StatTableEntry IncreasedMeleeDamage;
    public StatTableEntry IncreasedRangedDamage;
    public StatTableEntry IncreasedSpellDamage;

    public StatTableEntry IncreasedAttackSpeed;
    public StatTableEntry IncreasedCastSpeed;
    public StatTableEntry IncreasedCritChance;
    public StatTableEntry CritMulti;

    //protected bool isOHEquipped;
    
    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    public void SetOffhandVisibility(bool ohEquipped) {
        /*
        if (!ohEquipped) {
            OffHandPhysDamage.Visible = false;
            OffHandAttackSpeed.Visible = false;
            OffHandCritChance.Visible = false;
        }
        else {
            OffHandPhysDamage.Visible = true;
            OffHandAttackSpeed.Visible = true;
            OffHandCritChance.Visible = true;
        }
        */
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        AddEntry(ref AddedPhysDamage, "Added Physical Damage");
        AddEntry(ref IncreasedPhysDamage, "Physical Damage Modifier");
        AddEntry(ref AddedFireDamage, "Added Fire Damage");
        AddEntry(ref IncreasedFireDamage, "Fire Damage Modifier");
        AddEntry(ref AddedColdDamage, "Added Cold Damage");
        AddEntry(ref IncreasedColdDamage, "Cold Damage Modifier");
        AddEntry(ref AddedLightningDamage, "Added Lightning Damage");
        AddEntry(ref IncreasedLightningDamage, "Lightning Damage Modifier");
        AddEntry(ref AddedChaosDamage, "Added Chaos Damage");
        AddEntry(ref IncreasedChaosDamage, "Chaos Damage Modifier");

        AddEntry(ref IncreasedMeleeDamage, "Melee Damage Modifier");
        AddEntry(ref IncreasedRangedDamage, "Ranged Damage Modifier");
        AddEntry(ref IncreasedSpellDamage, "Spell Damage Modifier");

        AddEntry(ref IncreasedAttackSpeed, "Attack Speed Modifier");
        AddEntry(ref IncreasedCastSpeed, "Cast Speed Modifier");
        AddEntry(ref IncreasedCritChance, "Critical Strike Chance Modifier");
        AddEntry(ref CritMulti, "Critical Strike Multiplier");
    }
}
