using Godot;
using System;

public partial class LowerHUD : Control {
    public Player PlayerOwner { get; protected set; }

    private bool alwaysShowLifeManaValues;
    public bool AlwaysShowLifeManaValues {
        get => alwaysShowLifeManaValues;
        set {
            alwaysShowLifeManaValues = value;
            UpdateOrbTextVisibility();
        }
    }

    private Label lifeLabel;
    private Label manaLabel;
    private TextureProgressBar lifeOrb;
    private TextureProgressBar manaOrb;

    private Label levelLabel;
    private Label experienceLabel;
    private int experienceBarMaxValue = 0;
    private TextureProgressBar experienceBar;

    private SkillHotbar skillHotbar;

    public override void _Ready() {
        lifeLabel = GetNode<Label>("LowerContainer/LeftSide/Control/LifeLabel");
        manaLabel = GetNode<Label>("LowerContainer/RightSide/HBoxContainer/Control/ManaLabel");
        lifeOrb = GetNode<TextureProgressBar>("LowerContainer/LeftSide/Control/PanelContainer/LifeOrb");
        manaOrb = GetNode<TextureProgressBar>("LowerContainer/RightSide/HBoxContainer/Control/PanelContainer/ManaOrb");

        levelLabel = GetNode<Label>("LowerContainer/Control/LevelLabel");
        experienceLabel = GetNode<Label>("LowerContainer/Control/ExperienceLabel");
        experienceBar = GetNode<TextureProgressBar>("LowerContainer/Control/ExperienceBar");

        skillHotbar = GetNode<SkillHotbar>("LowerContainer/RightSide/HBoxContainer/SkillHotbar");

        AlwaysShowLifeManaValues = GameSettings.Instance.AlwaysShowPlayerLifeAndManaValues;
    }

    public void AssignPlayer(Player player) {
        PlayerOwner = player;
        skillHotbar.PlayerOwner = PlayerOwner;
    }

    public SkillHotbar GetSkillHotbar() {
        return skillHotbar;
    }

    public void UpdateLifeOrb(double newValue) {
        lifeOrb.Value = newValue;
        if (lifeLabel.Visible) {
            lifeLabel.Text = $"{PlayerOwner.BasicStats.CurrentLife:F0} / {PlayerOwner.BasicStats.TotalLife}";
        }
    }

    public void UpdateManaOrb(double newValue) {
        manaOrb.Value = newValue;
        if (manaLabel.Visible) {
            manaLabel.Text = $"{PlayerOwner.BasicStats.CurrentMana:F0} / {PlayerOwner.BasicStats.TotalMana}";
        }
    }

    private void OnLifeOrbEntered() {
        if (!AlwaysShowLifeManaValues) {
            lifeLabel.Text = $"{PlayerOwner.BasicStats.CurrentLife:F0} / {PlayerOwner.BasicStats.TotalLife}";
            lifeLabel.Visible = true;
        }
    }

    private void OnLifeOrbExited() {
        if (!AlwaysShowLifeManaValues) {
            lifeLabel.Visible = false;
        }
    }

    private void OnManaOrbEntered() {
        if (!AlwaysShowLifeManaValues) {
            manaLabel.Text = $"{PlayerOwner.BasicStats.CurrentMana:F0} / {PlayerOwner.BasicStats.TotalMana}";
            manaLabel.Visible = true;
        }
    }

    private void OnManaOrbExited() {
        if (!AlwaysShowLifeManaValues) {
            manaLabel.Visible = false;
        }
    }

    private void UpdateOrbTextVisibility() {
        if (AlwaysShowLifeManaValues) {
            lifeLabel.Visible = true;
            manaLabel.Visible = true;
        }
        else {
            lifeLabel.Visible = false;
            manaLabel.Visible = false;
        }
    }

    public void UpdateLevelLabel(int newLevel) {
        levelLabel.Text = $"Level {newLevel}";
    }

    public void UpdateExperienceBar(double newExp) {
        experienceLabel.Text = $"{newExp} / {experienceBarMaxValue}";
        experienceBar.Value = (newExp / experienceBarMaxValue) * 100;
    }

    public void SetExperienceBarLimit(int newLimit) {
        experienceBarMaxValue = newLimit;
    }
}
