using Godot;
using System;

public partial class CharacterPanel : Control {
    public Player PlayerOwner;
    public bool IsOpen = false;

    private TabContainer statTabContainer;
    public OffencePanel OffenceTabPanel;
    public DefencePanel DefenceTabPanel;

    public override void _Ready() {
        statTabContainer = GetNode<TabContainer>("TabContainer");
        OffenceTabPanel = GetNode<OffencePanel>("TabContainer/Offence");
        DefenceTabPanel = GetNode<DefencePanel>("TabContainer/Defence");
    }

    public void TogglePanel() {
        if (!IsOpen) {
            Visible = true;
            IsOpen = true;
        }
        else {
            Visible = false;
            IsOpen = false;
        }
    }

    public void UpdateMainHand() {

    }
}
