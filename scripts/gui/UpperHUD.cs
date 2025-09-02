using Godot;
using System;
using System.Collections.Generic;

public partial class UpperHUD : Control {
    protected readonly PackedScene hudStatusIconScene = GD.Load<PackedScene>("res://scenes/gui/hud_status.tscn");

    public MarginContainer BuffMarginContainer { get; protected set; }
    public GridContainer BuffGridContainer { get; protected set; }
    public Label ObjTimeLabel { get; protected set; }

    private List<HUDStatusIcon> statusIcons = new();

    public override void _Ready() {
        BuffMarginContainer = GetNode<MarginContainer>("BuffMarginContainer");
        BuffGridContainer = BuffMarginContainer.GetNode<GridContainer>("BuffGridContainer");
        ObjTimeLabel = GetNode<Label>("ObjTimeLabel");
    }

    public bool TryAddStatus(AttachedEffect statusEffect) {
        if (statusIcons.Exists(s => s.EffectName == statusEffect.EffectName)) {
            HUDStatusIcon statusIcon = statusIcons.Find(s => s.EffectName == statusEffect.EffectName);
            statusIcon.OverrideTimeLeft(statusEffect.RemainingTime);

            if (statusEffect is IUniqueStackableEffect usEffect) {
                statusIcon.UpdateStacks(usEffect.StackAmount);
            }
            
            return false;
        }

        HUDStatusIcon newStatusIcon = hudStatusIconScene.Instantiate<HUDStatusIcon>();
        BuffGridContainer.AddChild(newStatusIcon);
        newStatusIcon.SetStatusType(statusEffect);
        statusIcons.Add(newStatusIcon);

        return true;
    }

    public bool TryUpdateRepeatableEffectInstances(EEffectName effectName, int amount) {
        if (statusIcons.Exists(s => s.EffectName == effectName)) {
            HUDStatusIcon statusIcon = statusIcons.Find(s => s.EffectName == effectName);
            statusIcon.UpdateInstances(amount);

            return true;
        }

        return false;
    }

    public bool TryRemoveStatus(EEffectName statusEffectName) {
        if (statusIcons.Exists(s => s.EffectName == statusEffectName)) {
            HUDStatusIcon statusIcon = statusIcons.Find(s => s.EffectName == statusEffectName);
            
            BuffGridContainer.RemoveChild(statusIcon);
            statusIcons.Remove(statusIcon);
            statusIcon.QueueFree();

            return true;
        }

        return false;
    }
}
