using Godot;
using System;

public partial class SkillPanel : Control {
    public Player PlayerOwner { get; protected set; }
    public bool IsOpen = false;

    public SkillSlotCluster SkillSlotCluster1 { get; protected set; }
    public SkillSlotCluster SkillSlotCluster2 { get; protected set; }
    public SkillSlotCluster SkillSlotCluster3 { get; protected set; }
    public SkillSlotCluster SkillSlotCluster4 { get; protected set; }

    public override void _Ready() {
        SkillSlotCluster1 = GetNode<SkillSlotCluster>("SkillSlotCluster1");
        SkillSlotCluster2 = GetNode<SkillSlotCluster>("SkillSlotCluster2");
        SkillSlotCluster3 = GetNode<SkillSlotCluster>("SkillSlotCluster3");
        SkillSlotCluster4 = GetNode<SkillSlotCluster>("SkillSlotCluster4");

        SkillSlotCluster1.ClusterChanged += OnSkillSlotClusterChanged;
        SkillSlotCluster2.ClusterChanged += OnSkillSlotClusterChanged;
        SkillSlotCluster3.ClusterChanged += OnSkillSlotClusterChanged;
        SkillSlotCluster4.ClusterChanged += OnSkillSlotClusterChanged;

        SkillSlotCluster1.ActiveSkillEquipped += OnSkillEquippedFromInventory;
        SkillSlotCluster2.ActiveSkillEquipped += OnSkillEquippedFromInventory;
        SkillSlotCluster3.ActiveSkillEquipped += OnSkillEquippedFromInventory;
        SkillSlotCluster4.ActiveSkillEquipped += OnSkillEquippedFromInventory;

        SkillSlotCluster1.ActiveSkillUnequipped += OnSkillUnequipped;
        SkillSlotCluster2.ActiveSkillUnequipped += OnSkillUnequipped;
        SkillSlotCluster3.ActiveSkillUnequipped += OnSkillUnequipped;
        SkillSlotCluster4.ActiveSkillUnequipped += OnSkillUnequipped;
    }

    public void AssignPlayer(Player player) {
        PlayerOwner = player;

        SkillSlotCluster1.SetInventoryReference(PlayerOwner.PlayerHUD.PlayerInventory);
        SkillSlotCluster2.SetInventoryReference(PlayerOwner.PlayerHUD.PlayerInventory);
        SkillSlotCluster3.SetInventoryReference(PlayerOwner.PlayerHUD.PlayerInventory);
        SkillSlotCluster4.SetInventoryReference(PlayerOwner.PlayerHUD.PlayerInventory);
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

    public void OnSkillEquippedFromInventory(Control slot, InventoryItem item) {
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();

        SkillItem skillItem = (SkillItem)item.ItemReference;

        skillItem.SkillReference.ActorOwner = PlayerOwner;
        PlayerOwner.AddSkill(skillItem.SkillReference);
    }

    public void OnSkillUnequipped(Control slot, InventoryItem item) {
        item.RemoveTooltip();
        slot.RemoveChild(item);
        
        if (!PlayerOwner.PlayerHUD.PlayerInventory.TryAddItemToInventory(ref item)) {
            PlayerOwner.DropItemOnFloor(item.ItemReference.ConvertToWorldItem());
        }
        else {
            item.ToggleClickable();
            item.ToggleBackground();
        }

        SkillItem skillItem = (SkillItem)item.ItemReference;

        skillItem.SkillReference.ActorOwner = null;
        PlayerOwner.RemoveSkill(skillItem.SkillReference);
    }

    public void EquipSkillInCluster(SkillSlotCluster cluster, SkillSlotActive slot) {

    }

    public void UnequipSkillInCluster(SkillSlotCluster cluster, SkillSlotActive slot) {

    }

    public void EquipSupportInCluster(SkillSlotCluster cluster, SkillSlotSupport slot) {

    }

    public void UnequipSupportInCluster(SkillSlotCluster cluster, SkillSlotSupport slot) {

    }
}
