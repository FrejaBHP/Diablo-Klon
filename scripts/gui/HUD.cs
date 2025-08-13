using Godot;
using System;

public partial class HUD : Control {
	PackedScene itemTooltipPopupScene = GD.Load<PackedScene>("res://scenes/gui/hud_item_tooltip_popup.tscn");
	
	public Player PlayerOwner { get; protected set; }
	public CharacterPanel PlayerPanel { get; protected set; }
	public SkillPanel SkillPanel { get; protected set; }
	public PlayerInventory Inventory { get; protected set; }
	public UpperHUD UpperHUD { get; protected set; }
	public RightHUD RightHUD { get; protected set; }
	public LowerHUD LowerHUD { get; protected set; }
	public PTreePanel PassiveTreePanel { get; protected set; }

	private ItemTooltipPopup activeTooltipPopup = null;

	public override void _Ready() {
		PlayerPanel = GetNode<CharacterPanel>("PlayerPanel");
		SkillPanel = GetNode<SkillPanel>("SkillPanel");
		Inventory = GetNode<PlayerInventory>("Inventory");
		UpperHUD = GetNode<UpperHUD>("UpperHUD");
		RightHUD = GetNode<RightHUD>("RightHUD");
		LowerHUD = GetNode<LowerHUD>("LowerHUD");
		PassiveTreePanel = GetNode<PTreePanel>("PtreePanel");
	}

	public void AssignPlayer(Player player) {
		PlayerOwner = player;

		PlayerPanel.PlayerOwner = PlayerOwner;
		Inventory.PlayerOwner = PlayerOwner;

		SkillPanel.AssignPlayer(PlayerOwner);
		LowerHUD.AssignPlayer(PlayerOwner);
	}

	public void TogglePlayerPanel() {
		if (SkillPanel.Visible) {
			ToggleSkillPanel();
		}

		PlayerPanel.TogglePanel();
	}

	public void ToggleSkillPanel() {
		if (PlayerPanel.Visible) {
			TogglePlayerPanel();
		}

		SkillPanel.Visible = !SkillPanel.Visible;
	}

	public void ToggleInventory() {
		Inventory.ToggleInventory();
	}

	public void TogglePassiveTree() {
		PassiveTreePanel.Visible = !PassiveTreePanel.Visible;
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
