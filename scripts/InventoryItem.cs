using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class InventoryItem : PanelContainer {
	PackedScene itemTooltipScene = GD.Load<PackedScene>("res://hud_item_tooltip.tscn");

	public Inventory InventoryReference;
	public bool IsClicked = false;

	public Item ItemReference;

	protected int gridSizeX;
	protected int gridSizeY;

	protected TextureRect itemTexture;
	protected ColorRect itemBackground;
	protected ItemTooltip itemTooltip;

	protected List<InventoryGridCell> occupiedInventorySlots = new List<InventoryGridCell>();

	protected const int margin = 6; // Don't touch

	protected bool isHovered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemTexture = GetNode<TextureRect>("ItemTexture");
		itemTexture.Texture = ItemReference.ItemTexture;

		itemBackground = GetNode<ColorRect>("ItemBackground");
		itemBackground.Color = UILib.ColorItemBackground;

		ApplyBorder();

		// Each grid slot is 32x32 units
		Vector2 newMinSize = GetGridSize() * 32;
		newMinSize.X -= margin;
		newMinSize.Y -= margin;
		itemTexture.CustomMinimumSize = newMinSize;
	}

	public void OnClicked(InputEvent @event) {
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
			if (!IsClicked) {
				InventoryReference.ItemClickSelect(this);
			}
		}
	}

	public void OnMouseEntered() {
		if (!InventoryReference.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackgroundHovered;
			isHovered = true;

			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			InventoryReference.PlayerOwner.PlayerHUD.CreateItemTooltip(GetCustomTooltip(), anchor);
		}
	}

	public void OnMouseExited() {
		if (!InventoryReference.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackground;
			isHovered = false;
			
			InventoryReference.PlayerOwner.PlayerHUD.RemoveItemTooltip();
		}
	}

	public void ToggleBackground() {
		if (IsClicked) {
			itemBackground.Color = UILib.ColorTransparent;
		}
		else {
			itemBackground.Color = UILib.ColorItemBackground;
		}
	}

	// For adjusting click functionality
	// A physics frame delay is inserted to prevent things from overlapping in unintended ways
	public async void ToggleClickable() {
		if (MouseFilter == MouseFilterEnum.Stop) {
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			MouseFilter = MouseFilterEnum.Ignore;
		}
		else if (MouseFilter == MouseFilterEnum.Ignore) {
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			MouseFilter = MouseFilterEnum.Stop;
		}
	}

	public void SetItemReference(Item item) {
		ItemReference = item;
	}

	public Vector2I GetGridSize() {
		return new Vector2I(gridSizeX, gridSizeY);
	}

	public void SetGridSize(int x, int y) {
		gridSizeX = x;
		gridSizeY = y;
	}

	// Sets the list of slots that this item will occupy
	public void SetOccupiedSlots(List<InventoryGridCell> list) {
		//GD.Print("Slots set");
		occupiedInventorySlots = list.GetRange(0, list.Count);

		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Sets the list of occupied slots as empty without taking a new list. Used for when moving items around
	public void OpenOccupiedSlots() {
		//GD.Print("Slots opened");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }
	}

	// Sets the list of occupied slots as used without clearing the list. Used for when moving items around
	public void CloseOccupiedSlots() {
		//GD.Print("Slots closed");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Empties the list of occupied slots
	public void ClearOccupiedSlots() {
		//GD.Print("Slots cleared");
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }

		occupiedInventorySlots.Clear();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (IsClicked) {
			float newX = GetGlobalMousePosition().X - (Size.X / 2) * Scale.X;
			float newY = GetGlobalMousePosition().Y - (Size.Y / 2) * Scale.Y;
			Vector2 newPosition = new Vector2(newX, newY);
			GlobalPosition = newPosition;
		}
	}

	public void ApplyBorder() {
		StyleBoxFlat styleBoxFlat = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
		const int marginFactor = margin / 2;
		styleBoxFlat.BorderWidthLeft = marginFactor;
		styleBoxFlat.BorderWidthTop = marginFactor;
		styleBoxFlat.BorderWidthRight = marginFactor;
		styleBoxFlat.BorderWidthBottom = marginFactor;
		
		styleBoxFlat.BorderColor = GetRarityColour();

		AddThemeStyleboxOverride("panel", styleBoxFlat);
	}

	private Color GetRarityColour() {
		switch (ItemReference.ItemRarity) {
			case EItemRarity.Common:
				return UILib.ColorWhite;

			case EItemRarity.Magic:
				return UILib.ColorMagic;

			case EItemRarity.Rare:
				return UILib.ColorRare;

			case EItemRarity.Unique:
				return UILib.ColorUnique;
			
			default:
				return UILib.ColorTransparent;
		}
	}

	public Control GetCustomTooltip() {
		ItemTooltip tooltipContent = itemTooltipScene.Instantiate<ItemTooltip>();

		tooltipContent.NameLabel.Text = ItemReference.ItemName;
		tooltipContent.NameLabel.AddThemeColorOverride("font_color", GetRarityColour());

		if (ItemReference.ItemRarity == EItemRarity.Rare || ItemReference.ItemRarity == EItemRarity.Unique) {
			tooltipContent.BaseLabel.Text = ItemReference.ItemBase;
			tooltipContent.BaseLabel.AddThemeColorOverride("font_color", GetRarityColour());
		}
		else {
			tooltipContent.BaseLabel.Visible = false;
		}

		if (ItemReference.GetType().IsSubclassOf(typeof(WeaponItem))) {
			WeaponItem item = ItemReference as WeaponItem;

			Label weaponClassLabel = new Label();
			weaponClassLabel.Text = item.WeaponClass;
			weaponClassLabel.MouseFilter = MouseFilterEnum.Ignore;
			weaponClassLabel.AddThemeFontSizeOverride("font_size", 15);
			weaponClassLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
			weaponClassLabel.HorizontalAlignment = HorizontalAlignment.Center;
			tooltipContent.BaseStatsContainer.AddChild(weaponClassLabel);

			if (item.PhysicalMinimumDamage > 0) {
				bool shouldHighlight = false;
				if (item.PhysicalMinimumDamage > item.BasePhysicalMinimumDamage) {
					shouldHighlight = true;
				}
				HBoxContainer physLabel = GenerateBaseStatLabel("Physical Damage:", item.PhysicalMinimumDamage.ToString() + "-" + item.PhysicalMaximumDamage.ToString(), shouldHighlight);
				tooltipContent.BaseStatsContainer.AddChild(physLabel);
			}
		}
		else if (ItemReference.GetType().IsSubclassOf(typeof(ArmourItem))) {
			ArmourItem item = ItemReference as ArmourItem;

			if (item.ItemDefences.HasFlag(EItemDefences.Armour)) {
				bool shouldHighlight = false;
				if (item.Armour > item.BaseArmour) {
					shouldHighlight = true;
				}
				HBoxContainer armourLabel = GenerateBaseStatLabel("Armour:", item.Armour.ToString(), shouldHighlight);
				tooltipContent.BaseStatsContainer.AddChild(armourLabel);
			}

			if (item.ItemDefences.HasFlag(EItemDefences.Evasion)) {
				bool shouldHighlight = false;
				if (item.Evasion > item.BaseEvasion) {
					shouldHighlight = true;
				}
				HBoxContainer evasionLabel = GenerateBaseStatLabel("Evasion Rating:", item.Evasion.ToString(), shouldHighlight);
				tooltipContent.BaseStatsContainer.AddChild(evasionLabel);
			}

			if (item.ItemDefences.HasFlag(EItemDefences.EnergyShield)) {
				bool shouldHighlight = false;
				if (item.EnergyShield > item.BaseEnergyShield) {
					shouldHighlight = true;
				}
				HBoxContainer esLabel = GenerateBaseStatLabel("Energy Shield:", item.EnergyShield.ToString(), shouldHighlight);
				tooltipContent.BaseStatsContainer.AddChild(esLabel);
			}
		}

		foreach (Affix prefix in ItemReference.Prefixes) {
			Label prefixLabel = GenerateAffixLabel(prefix.GetAffixTooltipText());
			tooltipContent.AffixContainer.AddChild(prefixLabel);
		}

		foreach (Affix suffix in ItemReference.Suffixes) {
			Label suffixLabel = GenerateAffixLabel(suffix.GetAffixTooltipText());
			tooltipContent.AffixContainer.AddChild(suffixLabel);
		}

		return tooltipContent;
	}

	private HBoxContainer GenerateBaseStatLabel(string statName, string statValue, bool highlight) {
		HBoxContainer labelContainer = new HBoxContainer();
		labelContainer.MouseFilter = MouseFilterEnum.Ignore;

		Label baseStatNameLabel = new Label();
		baseStatNameLabel.Text = statName;
		baseStatNameLabel.MouseFilter = MouseFilterEnum.Ignore;
		baseStatNameLabel.AddThemeFontSizeOverride("font_size", 15);
		baseStatNameLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
		labelContainer.AddChild(baseStatNameLabel);

		Label baseStatValueLabel = new Label();
		baseStatValueLabel.Text = statValue;
		baseStatValueLabel.MouseFilter = MouseFilterEnum.Ignore;
		baseStatValueLabel.AddThemeFontSizeOverride("font_size", 15);
		if (highlight) {
			baseStatValueLabel.AddThemeColorOverride("font_color", UILib.ColorBlurple);
		}
		labelContainer.AddChild(baseStatValueLabel);

		return labelContainer;
	}

	private Label GenerateAffixLabel(string affixText) {
		Label affixTextLabel = new Label();

		affixTextLabel.Text = affixText;
		affixTextLabel.MouseFilter = MouseFilterEnum.Ignore;
		affixTextLabel.AddThemeFontSizeOverride("font_size", 15);
		affixTextLabel.AddThemeColorOverride("font_color", UILib.ColorBlurple);
		affixTextLabel.HorizontalAlignment = HorizontalAlignment.Center;

		return affixTextLabel;
	}
}
