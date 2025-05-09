using Godot;
using System;
using System.Collections.Generic;

public partial class StatPanel : ScrollContainer {
    protected static readonly PackedScene statEntryScene = GD.Load<PackedScene>("res://scenes/gui/hud_stat_labels_container.tscn");

    protected VBoxContainer statContainer;
    
    protected List<StatTableEntry> stats = new List<StatTableEntry>();

    public override void _Ready() {
        statContainer = GetNode<VBoxContainer>("StatContainer");
    }

    protected void ApplyAlternatingBackground() {
        StyleBoxFlat styleBoxFlat;

        int statsColoured = 0;
        for (int i = 0; i < stats.Count; i++) {
            styleBoxFlat = stats[i].GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;

            if (stats[i].Visible) {
                if (statsColoured % 2 == 0) {
                styleBoxFlat.BgColor = UILib.ColorItemBackgroundHovered;
                }
                else {
                    styleBoxFlat.BgColor = UILib.ColorItemBackground;
                }
                
                stats[i].AddThemeStyleboxOverride("panel", styleBoxFlat);
                statsColoured++;
            }
        }
	}

    protected void AddEntry(ref StatTableEntry entry, string description) {
        entry = statEntryScene.Instantiate<StatTableEntry>();
        statContainer.AddChild(entry);
        entry.SetDescription(description);
        stats.Add(entry);
    }
}
