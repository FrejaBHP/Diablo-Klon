using Godot;
using System;
using System.Collections.Generic;

public partial class ItemRewardPanel : Control {
    protected static readonly PackedScene itemRewardSlotScene = GD.Load<PackedScene>("res://scenes/gui/item_reward_slot.tscn");

    [Signal]
    public delegate void RewardTakenEventHandler();

    protected Label rewardLabel;
    protected HBoxContainer rewardContainer;
    protected bool rewardSelected = false;

    public override void _Ready() {
        rewardLabel = GetNode<Label>("MarginContainer/VBoxContainer/RewardLabel");
        rewardContainer = GetNode<HBoxContainer>("MarginContainer/VBoxContainer/RewardMarginContainer/RewardBoxContainer");
    }

    public void GenerateStarterWeapons() {
        rewardLabel.Text = "Choose a starting weapon";

        ItemRewardSlot melee1HSlot = itemRewardSlotScene.Instantiate<ItemRewardSlot>();
        melee1HSlot.SetItem(ItemGeneration.GenerateWeaponFromType(EItemWeaponBaseType.WeaponMelee1H, -1, EItemRarity.Common).ConvertToRewardItem());
        ItemRewardSlot melee2HSlot = itemRewardSlotScene.Instantiate<ItemRewardSlot>();
        melee2HSlot.SetItem(ItemGeneration.GenerateWeaponFromType(EItemWeaponBaseType.WeaponMelee2H, -1, EItemRarity.Common).ConvertToRewardItem());
        ItemRewardSlot ranged1HSlot = itemRewardSlotScene.Instantiate<ItemRewardSlot>();
        ranged1HSlot.SetItem(ItemGeneration.GenerateWeaponFromType(EItemWeaponBaseType.WeaponRanged1H, -1, EItemRarity.Common).ConvertToRewardItem());
        ItemRewardSlot ranged2HSlot = itemRewardSlotScene.Instantiate<ItemRewardSlot>();
        ranged2HSlot.SetItem(ItemGeneration.GenerateWeaponFromType(EItemWeaponBaseType.WeaponRanged2H, -1, EItemRarity.Common).ConvertToRewardItem());

        rewardContainer.AddChild(melee1HSlot);
        rewardContainer.AddChild(melee2HSlot);
        rewardContainer.AddChild(ranged1HSlot);
        rewardContainer.AddChild(ranged2HSlot);

        melee1HSlot.RewardSelected += RewardSelected;
        melee2HSlot.RewardSelected += RewardSelected;
        ranged1HSlot.RewardSelected += RewardSelected;
        ranged2HSlot.RewardSelected += RewardSelected;
    }

    public void GenerateStarterSkills() {
        rewardLabel.Text = "Choose a starting skill";

        List<SkillItem> skillGems = ItemGeneration.GenerateRandomSkillGems(5, false);

        foreach (SkillItem skillGem in skillGems) {
            ItemRewardSlot skillGemSlot = itemRewardSlotScene.Instantiate<ItemRewardSlot>();
            skillGemSlot.SetItem(skillGem.ConvertToRewardItem());
            rewardContainer.AddChild(skillGemSlot);
            skillGemSlot.RewardSelected += RewardSelected;
        }
    }

    public void GenerateRewards(EItemRewardType rewardType, int amount, EItemRarity rarityOverride = EItemRarity.None) {
        switch (rewardType) {
            case EItemRewardType.Weapons:
                break;

            case EItemRewardType.Armour:
                break;

            default:
                break;
        }
    }

    public void RewardSelected(InventoryItem item) {
        if (!rewardSelected) {
            rewardSelected = true;
            item.Owner = Run.Instance.PlayerActor;
            item.MouseFilter = MouseFilterEnum.Stop;
            item.IsAReward = false;
            item.GetParent().RemoveChild(item);
            Run.Instance.PlayerActor.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref item);

            EmitSignal(SignalName.RewardTaken);

            QueueFree();
        }
    }
}
