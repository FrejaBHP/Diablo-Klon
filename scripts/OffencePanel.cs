using Godot;
using System;

public partial class OffencePanel : StatPanel {
    public StatTableEntry MainHandPhysDamage;
    public StatTableEntry OffHandPhysDamage;
    
    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        MainHandPhysDamage = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(MainHandPhysDamage);
        MainHandPhysDamage.SetDescription("Main Hand Physical Damage");
        stats.Add(MainHandPhysDamage);

        OffHandPhysDamage = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(OffHandPhysDamage);
        OffHandPhysDamage.SetDescription("Off Hand Physical Damage");
        stats.Add(OffHandPhysDamage);
    }
}
