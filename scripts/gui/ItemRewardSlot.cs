using Godot;
using System;

public partial class ItemRewardSlot : Control {
    [Signal]
    public delegate void RewardSelectedEventHandler(InventoryItem item);

    public InventoryItem itemInSlot { get; protected set; }
    protected ColorRect highlight;

    public override void _Ready() {
		highlight = GetNode<ColorRect>("Highlight");
	}

    public void SetItem(InventoryItem item) {
        AddChild(item);
		item.ToggleIsReward(); // This is called here instead of in the ConvertToRewardItem() function, because it is an orphan at the time, and will not work
        itemInSlot = item;
    }

    public void GUIInput(InputEvent @event) {
		// On left-click
		if (@event.IsActionPressed("LeftClick")) {
            EmitSignal(SignalName.RewardSelected, itemInSlot);
        }
    }

    public void OnMouseEntered() {
		if (itemInSlot != null) {
			HighlightSlot();

			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + (Size.X / 2) };
            if (itemInSlot.ItemReference.GetType().IsSubclassOf(typeof(WeaponItem)) || itemInSlot.ItemReference.GetType().IsSubclassOf(typeof(ArmourItem)) || itemInSlot.ItemReference.GetType().IsSubclassOf(typeof(JewelleryItem))) {
				itemInSlot.SignalCreateEquipmentTooltip(anchor, GetGlobalRect(), false);
			}
			else if (itemInSlot.ItemReference.GetType() == typeof(SkillItem)) {
				itemInSlot.SignalCreateSkillItemTooltip(anchor, GetGlobalRect(), false);
			}
			else if (itemInSlot.ItemReference.GetType().IsSubclassOf(typeof(SupportGem))) {
				itemInSlot.SignalCreateSupportItemTooltip(anchor, GetGlobalRect(), false);
			}
		}
	}

	public void OnMouseExited() {
		if (itemInSlot != null) {
			RemoveHighlight();

            Run.Instance.PlayerActor.PlayerHUD.RemoveItemTooltip();
		}
	}

    public void HighlightSlot() {
		highlight.Color = UILib.ColorItemBackgroundHovered;
	}

	public void RemoveHighlight() {
		highlight.Color = UILib.ColorTransparent;
	}
}
