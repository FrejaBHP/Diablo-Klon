using Godot;
using System;

public partial class PauseMenu : Control {
    protected static readonly PackedScene settingsMenuScene = GD.Load<PackedScene>("res://scenes/gui/menu_settings.tscn");

    protected Control pauseMenuLeftControl;
    protected Control pauseMenuRightControl;
    protected Panel blurPanel;

    protected bool isSettingsOpen = false;

    public override void _Ready() {
        pauseMenuLeftControl = GetNode<Control>("MenuControl/MenuLeft");
        pauseMenuRightControl = GetNode<Control>("MenuControl/MenuRight");

        // BlurPanel virker ikke 100% efter hensigten og udvisker også verdenen bag sig
        blurPanel = GetNode<Panel>("MenuControl/BlurPanel");
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event.IsActionPressed("Pause")) {
            GetTree().Root.SetInputAsHandled();
			OnResumePressed();
		}
    }

    public void OnSettingsPressed() {
        SettingsMenu settingsMenu = settingsMenuScene.Instantiate<SettingsMenu>();
        isSettingsOpen = true;
        blurPanel.Show();
        pauseMenuRightControl.AddChild(settingsMenu);
        pauseMenuRightControl.Show();
        settingsMenu.Back += OnSettingsClosed;
    }

    public void OnSettingsClosed() {
        isSettingsOpen = false;
        blurPanel.Hide();
        pauseMenuRightControl.Hide();
    }

    public void OnResumePressed() {
        Hide();
        GetTree().Paused = false;
    }
}
