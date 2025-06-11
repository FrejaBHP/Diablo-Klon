using Godot;
using System;

public partial class MapObjectiveHUD : Control {
    public Label ObjectiveLabel { get; protected set; }
    protected VBoxContainer rewardsContainer;
    public Label GoldRewardLabel { get; protected set; }
    public Label ItemRewardLabel { get; protected set; }

    public override void _Ready() {
        ObjectiveLabel = GetNode<Label>("ObjectiveLabel");
        rewardsContainer = GetNode<VBoxContainer>("RewardsContainer");
        GoldRewardLabel = rewardsContainer.GetNode<Label>("GoldRewardLabel");
        ItemRewardLabel = rewardsContainer.GetNode<Label>("ItemRewardLabel");
    }

    public void SetObjectiveText(string text) {
        ObjectiveLabel.Text = text;
    }

    public void SetGoldReward(int gold) {
        GoldRewardLabel.Text = $"Gold: {gold}";
    }

    public void SetItemReward(int items) {
        ItemRewardLabel.Text = $"Items: {items}";
    }
}
