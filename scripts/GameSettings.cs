using Godot;
using System;

public partial class GameSettings : Node {
    public static GameSettings Instance { get; private set; }

    private bool alwaysShowPlayerLifeAndManaValues = false;
    public bool AlwaysShowPlayerLifeAndManaValues {
        get => alwaysShowPlayerLifeAndManaValues;
        set {
            alwaysShowPlayerLifeAndManaValues = value;
            ShowLifeManaValuesChanged(alwaysShowPlayerLifeAndManaValues);
        }
    }

    public override void _Ready() {
        Instance = this;

        LoadSettings();
    }

    public void SaveSettings() {
        ConfigFile config = new();
        config.SetValue("Misc", "alwaysShowLifeMana", AlwaysShowPlayerLifeAndManaValues);

        config.Save("user://settings.cfg");
    }

    public void LoadSettings() {
        ConfigFile config = new();

        // Load data from a file.
        Error err = config.Load("user://settings.cfg");

        // If the file didn't load, ignore it.
        if (err != Error.Ok) {
            GD.PrintErr("Settings file not found");
            return;
        }

        alwaysShowPlayerLifeAndManaValues = (bool)config.GetValue("Misc", "alwaysShowLifeMana");
    }

    // Currently unused
    public void ApplyPlayerSettings() { 
        Run.Instance.PlayerActor.PlayerHUD.PlayerLowerHUD.AlwaysShowLifeManaValues = alwaysShowPlayerLifeAndManaValues;
    }

    protected static void ShowLifeManaValuesChanged(bool state) {
        if (Run.Instance != null && Run.Instance.PlayerActor != null) {
            Run.Instance.PlayerActor.PlayerHUD.PlayerLowerHUD.AlwaysShowLifeManaValues = state;
        }
    }
}
