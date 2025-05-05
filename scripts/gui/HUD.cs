using Godot;
using System;

public partial class HUD : Control {
	PackedScene itemTooltipPopupScene = GD.Load<PackedScene>("res://scenes/gui/hud_item_tooltip_popup.tscn");
	
	public Player PlayerOwner { get; protected set; }
	public CharacterPanel PlayerPanel;
	public SkillPanel PlayerSkillPanel;
	public Inventory PlayerInventory;
	public LowerHUD PlayerLowerHUD;

	private ItemTooltipPopup activeTooltipPopup = null;

	public override void _Ready() {
		PlayerPanel = GetNode<CharacterPanel>("PlayerPanel");
		PlayerSkillPanel = GetNode<SkillPanel>("SkillPanel");
		PlayerInventory = GetNode<Inventory>("Inventory");
		PlayerLowerHUD = GetNode<LowerHUD>("LowerHUD");
	}

	public void AssignPlayer(Player player) {
		PlayerOwner = player;

		PlayerPanel.PlayerOwner = PlayerOwner;
		PlayerInventory.PlayerOwner = PlayerOwner;

		PlayerSkillPanel.AssignPlayer(PlayerOwner);
		PlayerLowerHUD.AssignPlayer(PlayerOwner);
	}

	public void TogglePlayerPanel() {
		if (PlayerSkillPanel.Visible) {
			ToggleSkillPanel();
		}

		PlayerPanel.TogglePanel();
	}

	public void ToggleSkillPanel() {
		if (PlayerPanel.Visible) {
			TogglePlayerPanel();
		}

		PlayerSkillPanel.Visible = !PlayerSkillPanel.Visible;
	}

	public void ToggleInventory() {
		PlayerInventory.ToggleInventory();
	}

    public void CreateItemTooltip(Control tooltipContent, Vector2 anchor, Rect2 itemRect, bool rightSide) {
		RemoveItemTooltip();
		
		ItemTooltipPopup tooltipPopup = itemTooltipPopupScene.Instantiate<ItemTooltipPopup>();
		tooltipPopup.ItemRect = itemRect;
		tooltipPopup.RightSide = rightSide;
		tooltipPopup.Unfocusable = true; // If false, consumes all input events while open
		AddChild(tooltipPopup);
		activeTooltipPopup = tooltipPopup;

		tooltipPopup.AddChild(tooltipContent);
		tooltipPopup.Position = (Vector2I)anchor;
    }

	public void RemoveItemTooltip() {
		if (activeTooltipPopup != null) {
			activeTooltipPopup.QueueFree();
			activeTooltipPopup = null;
		}
	}
}
