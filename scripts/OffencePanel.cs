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
        MainHandPhysDamage = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(MainHandPhysDamage);
        MainHandPhysDamage.SetDescription("Main Hand Physical Damage");
        stats.Add(MainHandPhysDamage);

        MainHandAttackSpeed = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(MainHandAttackSpeed);
        MainHandAttackSpeed.SetDescription("Main Hand Attacks per Second");
        stats.Add(MainHandAttackSpeed);

        MainHandCritChance = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(MainHandCritChance);
        MainHandCritChance.SetDescription("Main Hand Critical Strike Chance");
        stats.Add(MainHandCritChance);

        OffHandPhysDamage = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(OffHandPhysDamage);
        OffHandPhysDamage.SetDescription("Off Hand Physical Damage");
        stats.Add(OffHandPhysDamage);

        OffHandAttackSpeed = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(OffHandAttackSpeed);
        OffHandAttackSpeed.SetDescription("Off Hand Attacks per Second");
        stats.Add(OffHandAttackSpeed);

        OffHandCritChance = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(OffHandCritChance);
        OffHandCritChance.SetDescription("Off Hand Critical Strike Chance");
        stats.Add(OffHandCritChance);
    }
}
