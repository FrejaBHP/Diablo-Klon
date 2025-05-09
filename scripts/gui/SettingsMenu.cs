using Godot;
using System;

public partial class SettingsMenu : Control {
    [Signal]
    public delegate void BackEventHandler();

    protected VBoxContainer settingsContainer;

    protected CheckBox cbShowLifeManaValues;
    protected bool stateShowLifeManaValues;

    public override void _Ready() {
        settingsContainer = GetNode<VBoxContainer>("SettingsContainer");

        cbShowLifeManaValues = settingsContainer.GetNode<CheckBox>("CBShowLifeManaValues");
        cbShowLifeManaValues.SetPressedNoSignal(GameSettings.Instance.AlwaysShowPlayerLifeAndManaValues);
        stateShowLifeManaValues = GameSettings.Instance.AlwaysShowPlayerLifeAndManaValues;
    }

    public void OnShowLifeManaValuesToggled(bool state) {
        stateShowLifeManaValues = state;
    }

    public void OnCancelPressed() {
        EmitSignal(SignalName.Back);
        QueueFree();
    }

    public void OnApplyPressed() {
        GameSettings.Instance.AlwaysShowPlayerLifeAndManaValues = stateShowLifeManaValues;
        GameSettings.Instance.SaveSettings();
    }
}
