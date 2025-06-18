using Godot;
using System;

public partial class SkillPanel : Control {
    public Player PlayerOwner { get; protected set; }
    public bool IsOpen = false;

    public SkillSlotCluster[] SkillSlotClusters { get; protected set; } = new SkillSlotCluster[4];

    public override void _Ready() {
        for (int i = 0; i < SkillSlotClusters.Length; i++) {
            SkillSlotClusters[i] = GetNode<SkillSlotCluster>($"SkillSlotCluster{i + 1}");
            SkillSlotClusters[i].ClusterChanged += OnSkillSlotClusterChanged;
            SkillSlotClusters[i].ActiveSkillEquipped += OnSkillEquippedFromInventory;
            SkillSlotClusters[i].ActiveSkillUnequipped += OnSkillUnequipped;
            
            SkillSlotClusters[i].SupportEquipped += OnSupportEquippedFromInventory;
            SkillSlotClusters[i].SupportUnequipped += OnSupportUnequipped;
        }
    }

    public void AssignPlayer(Player player) {
        PlayerOwner = player;

        for (int i = 0; i < SkillSlotClusters.Length; i++) {
            SkillSlotClusters[i].SetInventoryReference(PlayerOwner.PlayerHUD.PlayerInventory);
        }
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

    public void OnSkillSlotClusterChanged(SkillSlotCluster cluster) {

    }

    public void OnSkillEquippedFromInventory(SkillSlotCluster cluster, Control slot, InventoryItem item) {
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();

        SkillItem skillItem = (SkillItem)item.ItemReference;

        skillItem.SkillReference.ActorOwner = PlayerOwner;
        skillItem.SkillReference.HousingSkillCluster = cluster;
        PlayerOwner.AddSkill(skillItem.SkillReference);
    }

    public void OnSkillUnequipped(SkillSlotCluster cluster, Control slot, InventoryItem item) {
        item.RemoveTooltip();
        slot.RemoveChild(item);
        
        if (!PlayerOwner.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItemOnFloor(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }

        SkillItem skillItem = (SkillItem)item.ItemReference;

        skillItem.SkillReference.ActorOwner = null;
        skillItem.SkillReference.HousingSkillCluster = null;
        PlayerOwner.RemoveSkill(skillItem.SkillReference);
    }

    public void OnSupportEquippedFromInventory(Control slot, InventoryItem item) {
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();
    }

    public void OnSupportUnequipped(Control slot, InventoryItem item) {
        item.RemoveTooltip();
        slot.RemoveChild(item);
        
        if (!PlayerOwner.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItemOnFloor(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }
    }
}
