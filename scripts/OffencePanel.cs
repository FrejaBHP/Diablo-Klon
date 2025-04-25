using Godot;
using System;

public partial class OffencePanel : StatPanel {
    public StatTableEntry MainHandPhysDamage;
    public StatTableEntry MainHandAttackSpeed;
    public StatTableEntry MainHandCritChance;

    public StatTableEntry OffHandPhysDamage;
    public StatTableEntry OffHandAttackSpeed;
    public StatTableEntry OffHandCritChance;

    protected bool isOHEquipped;
    
    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    public void SetOffhandVisibility(bool ohEquipped) {
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
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        AddEntry(ref MainHandPhysDamage, "MH Physical Damage");
        AddEntry(ref MainHandAttackSpeed, "MH Attacks per Second");
        AddEntry(ref MainHandCritChance, "MH Critical Strike Chance");
        AddEntry(ref OffHandPhysDamage, "OH Physical Damage");
        AddEntry(ref OffHandAttackSpeed, "OH Attacks per Second");
        AddEntry(ref OffHandCritChance, "OH Critical Strike Chance");
    }
}
