using Godot;
using System;

public partial class DefencePanel : StatPanel {
    public StatTableEntry Armour;
    public StatTableEntry Evasion;
    public StatTableEntry FireRes;
    public StatTableEntry ColdRes;
    public StatTableEntry LightningRes;

    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        Armour = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(Armour);
        Armour.SetDescription("Armour");
        stats.Add(Armour);

        Evasion = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(Evasion);
        Evasion.SetDescription("Evasion Rating");
        stats.Add(Evasion);

        FireRes = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(FireRes);
        FireRes.SetDescription("Fire Resistance");
        stats.Add(FireRes);

        ColdRes = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(ColdRes);
        ColdRes.SetDescription("Cold Resistance");
        stats.Add(ColdRes);

        LightningRes = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(LightningRes);
        LightningRes.SetDescription("Lightning Resistance");
        stats.Add(LightningRes);
    }
}
