using Godot;
using System;

public partial class LowerHUD : Control {
    public Player PlayerOwner { get; protected set; }
    private Label lifeLabel;
    private Label manaLabel;
    private TextureProgressBar lifeOrb;
    private TextureProgressBar manaOrb;

    private SkillHotbar skillHotbar;

    public override void _Ready() {
        lifeLabel = GetNode<Label>("LeftSide/Control/LifeLabel");
        manaLabel = GetNode<Label>("RightSide/HBoxContainer/Control/ManaLabel");
        lifeOrb = GetNode<TextureProgressBar>("LeftSide/Control/PanelContainer/LifeOrb");
        manaOrb = GetNode<TextureProgressBar>("RightSide/HBoxContainer/Control/PanelContainer/ManaOrb");

        skillHotbar = GetNode<SkillHotbar>("RightSide/HBoxContainer/SkillHotbar");
    }

    public void AssignPlayer(Player player) {
        PlayerOwner = player;
        skillHotbar.PlayerOwner = PlayerOwner;
    }

    public SkillHotbar GetSkillHotbar() {
        return skillHotbar;
    }

    public void UpdateOrbs() {
        lifeOrb.Value = (PlayerOwner.BasicStats.CurrentLife / PlayerOwner.BasicStats.TotalLife) * 100;
        if (lifeLabel.Visible) {
            lifeLabel.Text = $"{PlayerOwner.BasicStats.CurrentLife:F0} / {PlayerOwner.BasicStats.TotalLife}";
        }
        manaOrb.Value = (PlayerOwner.BasicStats.CurrentMana / PlayerOwner.BasicStats.TotalMana) * 100;
        if (manaLabel.Visible) {
            manaLabel.Text = $"{PlayerOwner.BasicStats.CurrentMana:F0} / {PlayerOwner.BasicStats.TotalMana}";
        }
    }

    private void OnLifeOrbEntered() {
        lifeLabel.Visible = true;
        UpdateOrbs();
    }

    private void OnLifeOrbExited() {
        lifeLabel.Visible = false;
    }

    private void OnManaOrbEntered() {
        manaLabel.Visible = true;
        UpdateOrbs();
    }

    private void OnManaOrbExited() {
        manaLabel.Visible = false;
    }
}