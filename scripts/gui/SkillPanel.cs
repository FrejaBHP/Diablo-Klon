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
            SkillSlotClusters[i].ActiveSkillsSwapped += OnSkillsSwapped;
            
            SkillSlotClusters[i].SupportGemEquipped += OnSupportEquippedFromInventory;
            SkillSlotClusters[i].SupportGemUnequipped += OnSupportUnequipped;
            SkillSlotClusters[i].SupportGemsSwapped += OnSupportsSwapped;
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

    public void OnSkillsSwapped(SkillSlotCluster cluster, Control slot, InventoryItem oldSkill, InventoryItem newSkill) {
        oldSkill.RemoveTooltip();

        newSkill.ClearOccupiedSlots();
        slot.RemoveChild(oldSkill);
        newSkill.GetParent().RemoveChild(newSkill);
        slot.AddChild(newSkill);
        
        if (!PlayerOwner.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref oldSkill)) {
            PlayerOwner.DropItemOnFloor(oldSkill.ItemReference.ConvertToWorldItem());
        }
        else {
            oldSkill.ToggleClickable();
            oldSkill.ToggleBackground();
        }

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();

        SkillItem oldSkillItem = (SkillItem)oldSkill.ItemReference;
        oldSkillItem.SkillReference.ActorOwner = null;
        oldSkillItem.SkillReference.HousingSkillCluster = null;
        PlayerOwner.RemoveSkill(oldSkillItem.SkillReference);

        SkillItem newSkillItem = (SkillItem)newSkill.ItemReference;
        newSkillItem.SkillReference.ActorOwner = PlayerOwner;
        newSkillItem.SkillReference.HousingSkillCluster = cluster;
        PlayerOwner.AddSkill(newSkillItem.SkillReference);
    }

    public void OnSupportEquippedFromInventory(SkillSlotSupport slot, InventoryItem item) {
        item.GetParent().RemoveChild(item);
        slot.AddChild(item);

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();
    }

    public void OnSupportUnequipped(SkillSlotSupport slot, InventoryItem item) {
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

    public void OnSupportsSwapped(SkillSlotSupport slot, InventoryItem oldSupport, InventoryItem newSupport) {
        oldSupport.RemoveTooltip();
        
        newSupport.ClearOccupiedSlots();
        slot.RemoveChild(oldSupport);
        newSupport.GetParent().RemoveChild(newSupport);
        slot.AddChild(newSupport);
        
        if (!PlayerOwner.PlayerHUD.PlayerInventory.InventoryGrid.TryAddItemToInventory(ref oldSupport)) {
            PlayerOwner.DropItemOnFloor(oldSupport.ItemReference.ConvertToWorldItem());
        }
        else {
            oldSupport.ToggleClickable();
            oldSupport.ToggleBackground();
        }

        PlayerOwner.PlayerHUD.PlayerInventory.ClearSelectedItem();
    }
}
