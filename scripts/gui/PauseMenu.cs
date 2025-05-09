using Godot;
using System;

public partial class PauseMenu : Control {
    protected static readonly PackedScene settingsMenuScene = GD.Load<PackedScene>("res://scenes/gui/menu_settings.tscn");

    protected Control pauseMenuControl;

    public override void _Ready() {
        pauseMenuControl = GetNode<Control>("Control");
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event.IsActionPressed("Pause")) {
            GetTree().Root.SetInputAsHandled();
			OnResumePressed();
		}
    }

    public void OnSettingsPressed() {
        SettingsMenu settingsMenu = settingsMenuScene.Instantiate<SettingsMenu>();
        pauseMenuControl.Hide();
        AddChild(settingsMenu);
        settingsMenu.Back += OnSettingsClosed;
    }

    public void OnSettingsClosed() {
        pauseMenuControl.Show();
    }

    public void OnResumePressed() {
        Hide();
        GetTree().Paused = false;
    }
}
