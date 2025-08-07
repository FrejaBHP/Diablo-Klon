using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Vector2 = Godot.Vector2;

public partial class InventoryItem : PanelContainer {
	PackedScene itemTooltipScene = GD.Load<PackedScene>("res://scenes/gui/hud_item_tooltip.tscn");
	PackedScene skillTooltipScene = GD.Load<PackedScene>("res://scenes/gui/hud_skillitem_tooltip.tscn");

	public InventoryGrid InventoryReference;
	public bool IsClicked = false;
	public bool IsForSale = false;
	public bool IsAReward = false;

	public Item ItemReference;

	protected int gridSizeX;
	protected int gridSizeY;

	protected TextureRect itemTexture;
	protected ColorRect itemBackground;
	protected bool hasActiveTooltip = false;

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

	public void GUIInput(InputEvent @event) {
		if (@event.IsActionPressed("LeftClick")) {
			if (!IsClicked) {
				if (!IsAReward) {
					InventoryReference.OnItemClicked(this);
					if (!IsForSale) {
						RemoveTooltip();
					}
				}
				else {
					// Dirty
					//ItemRewardSlot rewardSlot = GetParent<ItemRewardSlot>();
					//rewardSlot.EmitSignal(ItemRewardSlot.SignalName.RewardSelected, this);
				}
			}
		}

		if (@event.IsActionPressed("RightClick")) {
			if (!IsClicked) {
				PrintDebug();
			}
		}
	}

	protected void PrintDebug() {
		StringBuilder sb = new();
		sb.Append($"Name: {ItemReference.ItemName}\n");
		sb.Append($"Base: {ItemReference.ItemBase}\n");
		sb.Append($"Rarity: {ItemReference.ItemRarity}\n");
		sb.Append($"Item Level: {ItemReference.ItemLevel}\n");
		sb.Append($"No of Prefixes: {ItemReference.Prefixes.Count}\n");
		sb.Append($"No of Suffixes: {ItemReference.Suffixes.Count}");
		GD.Print(sb.ToString());
	}

	public void OnMouseEntered() {
		if (!Run.Instance.PlayerActor.PlayerHUD.PlayerInventory.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackgroundHovered;
			isHovered = true;

			Vector2 anchor = GlobalPosition with { X = GlobalPosition.X + Size.X / 2, Y = GlobalPosition.Y };
			if (ItemReference.GetType().IsSubclassOf(typeof(WeaponItem)) || ItemReference.GetType().IsSubclassOf(typeof(ArmourItem)) || ItemReference.GetType().IsSubclassOf(typeof(JewelleryItem))) {
				SignalCreateEquipmentTooltip(anchor, GetGlobalRect(), true);
			}
			else if (ItemReference.GetType() == typeof(SkillItem)) {
				SignalCreateSkillItemTooltip(anchor, GetGlobalRect(), true);
			}
			else if (ItemReference.GetType().IsSubclassOf(typeof(SupportGem))) {
				SignalCreateSupportItemTooltip(anchor, GetGlobalRect(), true);
			}
		}
	}

	public void OnMouseExited() {
		if (!Run.Instance.PlayerActor.PlayerHUD.PlayerInventory.IsAnItemSelected) {
			itemBackground.Color = UILib.ColorItemBackground;
			isHovered = false;
			RemoveTooltip();
		}
	}

	public void SignalCreateEquipmentTooltip(Vector2 anchor, Rect2 rect, bool rightSide) {
		Run.Instance.PlayerActor.PlayerHUD.CreateItemTooltip(GetCustomEquipmentTooltip(), anchor, rect, rightSide);
		hasActiveTooltip = true;
	}

	public void SignalCreateSkillItemTooltip(Vector2 anchor, Rect2 rect, bool rightSide) {
		Run.Instance.PlayerActor.PlayerHUD.CreateItemTooltip(GetCustomSkillTooltip(), anchor, rect, rightSide);
		hasActiveTooltip = true;
	}

	public void SignalCreateSupportItemTooltip(Vector2 anchor, Rect2 rect, bool rightSide) {
		Run.Instance.PlayerActor.PlayerHUD.CreateItemTooltip(GetCustomSupportTooltip(), anchor, rect, rightSide);
		hasActiveTooltip = true;
	}

	public void ToggleBackground() {
		if (IsClicked) {
			itemBackground.Color = UILib.ColorTransparent;
		}
		else {
			itemBackground.Color = UILib.ColorItemBackground;
		}
	}

	public void ToggleIsReward() {
		if (!IsAReward) {
			IsAReward = true;
			MouseFilter = MouseFilterEnum.Ignore;
			itemBackground.Color = UILib.ColorTransparent;
		}
		else {
			IsAReward = false;
			MouseFilter = MouseFilterEnum.Stop;
			itemBackground.Color = UILib.ColorItemBackground;
		}
	}

	public void RemoveTooltip() {
		Run.Instance.PlayerActor.PlayerHUD.RemoveItemTooltip();
		isHovered = false;
		hasActiveTooltip = false;
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
		occupiedInventorySlots = list.GetRange(0, list.Count);

		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Sets the list of occupied slots as empty without taking a new list. Used for when moving items around
	public void OpenOccupiedSlots() {
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = true;
        }
	}

	// Sets the list of occupied slots as used without clearing the list. Used for when moving items around
	public void CloseOccupiedSlots() {
		foreach (InventoryGridCell slot in occupiedInventorySlots) {
            slot.IsEmpty = false;
        }
	}

	// Empties the list of occupied slots
	public void ClearOccupiedSlots() {
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

	public Control GetCustomEquipmentTooltip() {
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
			weaponClassLabel.AddThemeFontSizeOverride("font_size", 15);
			weaponClassLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
			weaponClassLabel.HorizontalAlignment = HorizontalAlignment.Center;
			tooltipContent.BaseStatsContainer.AddChild(weaponClassLabel);

			if (item.PhysicalMinimumDamage > 0) {
				HBoxContainer physLabel = GenerateBaseStatLabel("Physical Damage:", $"{item.PhysicalMinimumDamage} - {item.PhysicalMaximumDamage}", item.PhysicalMinimumDamage > item.BasePhysicalMinimumDamage);
				tooltipContent.BaseStatsContainer.AddChild(physLabel);
			}
			if (item.AddedFireMinimumDamage > 0) {
				HBoxContainer fireLabel = GenerateColouredBaseStatLabel("Fire Damage:", $"{item.AddedFireMinimumDamage} - {item.AddedFireMaximumDamage}", UILib.ColorFire);
				tooltipContent.BaseStatsContainer.AddChild(fireLabel);
			}
			if (item.AddedColdMinimumDamage > 0) {
				HBoxContainer coldLabel = GenerateColouredBaseStatLabel("Cold Damage:", $"{item.AddedColdMinimumDamage} - {item.AddedColdMaximumDamage}", UILib.ColorCold);
				tooltipContent.BaseStatsContainer.AddChild(coldLabel);
			}
			if (item.AddedLightningMinimumDamage > 0) {
				HBoxContainer lightningLabel = GenerateColouredBaseStatLabel("Lightning Damage:", $"{item.AddedLightningMinimumDamage} - {item.AddedLightningMaximumDamage}", UILib.ColorLightning);
				tooltipContent.BaseStatsContainer.AddChild(lightningLabel);
			}
			if (item.AddedChaosMinimumDamage > 0) {
				HBoxContainer chaosLabel = GenerateColouredBaseStatLabel("Chaos Damage:", $"{item.AddedChaosMinimumDamage} - {item.AddedChaosMaximumDamage}", UILib.ColorChaos);
				tooltipContent.BaseStatsContainer.AddChild(chaosLabel);
			}

			HBoxContainer critLabel = GenerateBaseStatLabel("Critical Strike Chance:", item.GetCritChance(), item.CriticalStrikeChance > item.BaseCriticalStrikeChance);
			tooltipContent.BaseStatsContainer.AddChild(critLabel);

			HBoxContainer asLabel = GenerateBaseStatLabel("Attacks Per Second:", item.GetAttackSpeed(), item.BaseAttackSpeed > item.AttackSpeed);
			tooltipContent.BaseStatsContainer.AddChild(asLabel);
		}
		else if (ItemReference.GetType().IsSubclassOf(typeof(ArmourItem))) {
			ArmourItem item = ItemReference as ArmourItem;

			if (item.BlockChance != 0) {
				HBoxContainer armourLabel = GenerateBaseStatLabel("Block Chance:", item.BlockChance.ToString("P0"), item.BlockChance > item.BaseBlockChance);
				tooltipContent.BaseStatsContainer.AddChild(armourLabel);
			}

			if (item.ItemDefences.HasFlag(EItemDefences.Armour)) {
				HBoxContainer armourLabel = GenerateBaseStatLabel("Armour:", item.Armour.ToString(), item.Armour > item.BaseArmour);
				tooltipContent.BaseStatsContainer.AddChild(armourLabel);
			}
			if (item.ItemDefences.HasFlag(EItemDefences.Evasion)) {
				HBoxContainer evasionLabel = GenerateBaseStatLabel("Evasion Rating:", item.Evasion.ToString(), item.Evasion > item.BaseEvasion);
				tooltipContent.BaseStatsContainer.AddChild(evasionLabel);
			}
			if (item.ItemDefences.HasFlag(EItemDefences.EnergyShield)) {
				HBoxContainer esLabel = GenerateBaseStatLabel("Energy Shield:", item.EnergyShield.ToString(), item.EnergyShield > item.BaseEnergyShield);
				tooltipContent.BaseStatsContainer.AddChild(esLabel);
			}
		}

		// Allows for hidden implicit modifiers, in case that's desired
		int visibleImplicits = 0;

		if (ItemReference.Implicits.Count > 0) {
			foreach (Affix impl in ItemReference.Implicits) {
				if (!impl.IsHidden) {
					Label implicitLabel = GenerateAffixLabel(impl.GetAffixTooltipText());
					tooltipContent.ImplicitContainer.AddChild(implicitLabel);

					visibleImplicits++;
				}
			}
		}

		if (visibleImplicits == 0) {
			tooltipContent.ImplicitSeparator.Visible = false;
			tooltipContent.ImplicitContainer.Visible = false;
		}

		if (ItemReference.Prefixes.Count > 0 || ItemReference.Suffixes.Count > 0) {
			foreach (Affix prefix in ItemReference.Prefixes) {
				Label prefixLabel = GenerateAffixLabel(prefix.GetAffixTooltipText());
				tooltipContent.AffixContainer.AddChild(prefixLabel);
			}

			foreach (Affix suffix in ItemReference.Suffixes) {
				Label suffixLabel = GenerateAffixLabel(suffix.GetAffixTooltipText());
				tooltipContent.AffixContainer.AddChild(suffixLabel);
			}
		}
		else {
			tooltipContent.AffixSeparator.Visible = false;
			tooltipContent.AffixContainer.Visible = false;
		}

		if (IsForSale) {
			tooltipContent.PartsContainer.AddChild(GeneratePriceTag(ItemReference.Price));
		}
		
		return tooltipContent;
	}

	public Control GetCustomSkillTooltip() {
		SkillItem skillItem = ItemReference as SkillItem;

		ItemSkillTooltip tooltipContent = skillTooltipScene.Instantiate<ItemSkillTooltip>();
		tooltipContent.NameLabel.Text = ItemReference.ItemName;
		tooltipContent.NameLabel.AddThemeColorOverride("font_color", UILib.ColorSkill);

        List<string> tagList = Enum.GetValues(typeof(ESkillTags)).Cast<ESkillTags>().Where(t => (skillItem.SkillReference.Tags & t) == t).Select(t => t.ToString()).ToList();
		StringBuilder sbTags = new();

		for (int i = 1; i < tagList.Count; i++) {
			if (i == 1) {
				sbTags.Append(tagList[1]);
			}
			else {
				sbTags.Append($", {tagList[i]}");
			}
		}

		Label tagsLabel = GenerateGreyLabel(sbTags.ToString());
		tooltipContent.StatsContainer.AddChild(tagsLabel);

		HBoxContainer levelLabel = GenerateBaseStatLabel("Level:", $"{skillItem.SkillReference.Level + 1}", false);
		tooltipContent.StatsContainer.AddChild(levelLabel);

		HBoxContainer costLabel = GenerateBaseStatLabel("Cost:", skillItem.SkillReference.ManaCost.ToString() + " Mana", false);
		tooltipContent.StatsContainer.AddChild(costLabel);

		if (skillItem.SkillReference.Type == ESkillType.Attack) {
			HBoxContainer asLabel = GenerateBaseStatLabel("Attack Speed:", skillItem.SkillReference.GetAttackSpeedModifier() + "%", false);
			tooltipContent.StatsContainer.AddChild(asLabel);

			HBoxContainer dmgLabel = GenerateBaseStatLabel("Attack Damage:", skillItem.SkillReference.GetDamageModifier() + "%", false);
			tooltipContent.StatsContainer.AddChild(dmgLabel);
		}

		if (skillItem.SkillReference.Type == ESkillType.Spell) {
			if (skillItem.SkillReference is ISpell spell) {
				HBoxContainer castTimeLabel = GenerateBaseStatLabel("Cast Time:", spell.BaseCastTime.ToString() + " sec", false);
				tooltipContent.StatsContainer.AddChild(castTimeLabel);
			}
			
			HBoxContainer dmgLabel = GenerateBaseStatLabel("Added Damage Effectiveness:", skillItem.SkillReference.GetDamageModifier() + "%", false);
			tooltipContent.StatsContainer.AddChild(dmgLabel);
		}

		tooltipContent.DescriptionLabel.Text = skillItem.SkillReference.Description;

		if (skillItem.SkillReference.BaseDamageModifiers.Physical.SMinBase > 0) {
            Label physLabel = GenerateAffixLabel($"Deals {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Physical.SMinBase, 0)} - {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Physical.SMaxBase, 0)} Physical Damage");
            tooltipContent.EffectContainer.AddChild(physLabel);
        }
		if (skillItem.SkillReference.BaseDamageModifiers.Fire.SMinBase > 0) {
            Label fireLabel = GenerateAffixLabel($"Deals {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Fire.SMinBase, 0)} - {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Fire.SMaxBase, 0)} Fire Damage");
            tooltipContent.EffectContainer.AddChild(fireLabel);
        }
		if (skillItem.SkillReference.BaseDamageModifiers.Cold.SMinBase > 0) {
            Label coldLabel = GenerateAffixLabel($"Deals {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Cold.SMinBase, 0)} - {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Cold.SMaxBase, 0)} Cold Damage");
            tooltipContent.EffectContainer.AddChild(coldLabel);
        }
		if (skillItem.SkillReference.BaseDamageModifiers.Lightning.SMinBase > 0) {
            Label lightningLabel = GenerateAffixLabel($"Deals {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Lightning.SMinBase, 0)} - {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Lightning.SMaxBase, 0)} Lightning Damage");
            tooltipContent.EffectContainer.AddChild(lightningLabel);
        }
		if (skillItem.SkillReference.BaseDamageModifiers.Chaos.SMinBase > 0) {
            Label chaosLabel = GenerateAffixLabel($"Deals {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Chaos.SMinBase, 0)} - {Math.Round(skillItem.SkillReference.BaseDamageModifiers.Chaos.SMaxBase, 0)} Chaos Damage");
            tooltipContent.EffectContainer.AddChild(chaosLabel);
        }

		for (int i = 0; i < skillItem.SkillReference.Effects.Length; i++) {
			Label effectLabel = GenerateAffixLabel(skillItem.SkillReference.Effects[i]);
            tooltipContent.EffectContainer.AddChild(effectLabel);
		}

		if (IsForSale) {
			tooltipContent.PartsContainer.AddChild(GeneratePriceTag(ItemReference.Price));
		}
		
		return tooltipContent;
	}

	public Control GetCustomSupportTooltip() {
		SupportGem supportGem = ItemReference as SupportGem;

		ItemSkillTooltip tooltipContent = skillTooltipScene.Instantiate<ItemSkillTooltip>();
		tooltipContent.NameLabel.Text = ItemReference.ItemName;
		tooltipContent.NameLabel.AddThemeColorOverride("font_color", UILib.ColorSkill);

        List<string> tagList = Enum.GetValues(typeof(ESkillTags)).Cast<ESkillTags>().Where(t => (supportGem.SkillTags & t) == t).Select(t => t.ToString()).ToList();
		tagList.Add("Support");

		StringBuilder sbTags = new();
		for (int i = 1; i < tagList.Count; i++) {
			if (i == 1) {
				sbTags.Append(tagList[1]);
			}
			else {
				sbTags.Append($", {tagList[i]}");
			}
		}

		Label tagsLabel = GenerateGreyLabel(sbTags.ToString());
		tooltipContent.StatsContainer.AddChild(tagsLabel);

		HBoxContainer levelLabel = GenerateBaseStatLabel("Level:", $"{supportGem.Level + 1}", false);
		tooltipContent.StatsContainer.AddChild(levelLabel);

		tooltipContent.DescriptionLabel.Text = supportGem.Description;

		Label effectLabel = GenerateAffixLabel($"{supportGem.DescEffects}");
        tooltipContent.EffectContainer.AddChild(effectLabel);

		if (IsForSale) {
			tooltipContent.PartsContainer.AddChild(GeneratePriceTag(ItemReference.Price));
		}
		
		return tooltipContent;
	}

	protected static HBoxContainer GenerateBaseStatLabel(string statName, string statValue, bool highlight) {
		HBoxContainer labelContainer = new HBoxContainer();
		labelContainer.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
		labelContainer.MouseFilter = MouseFilterEnum.Ignore;

		Label baseStatNameLabel = new Label();
		baseStatNameLabel.Text = statName;
		baseStatNameLabel.AddThemeFontSizeOverride("font_size", 15);
		baseStatNameLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
		labelContainer.AddChild(baseStatNameLabel);

		Label baseStatValueLabel = new Label();
		baseStatValueLabel.Text = statValue;
		baseStatValueLabel.AddThemeFontSizeOverride("font_size", 15);
		if (highlight) {
			baseStatValueLabel.AddThemeColorOverride("font_color", UILib.ColorBlurple);
		}
		labelContainer.AddChild(baseStatValueLabel);

		return labelContainer;
	}

	protected static HBoxContainer GenerateColouredBaseStatLabel(string statName, string statValue, Color color) {
		HBoxContainer labelContainer = new HBoxContainer();
		labelContainer.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
		labelContainer.MouseFilter = MouseFilterEnum.Ignore;

		Label baseStatNameLabel = new Label();
		baseStatNameLabel.Text = statName;
		baseStatNameLabel.AddThemeFontSizeOverride("font_size", 15);
		baseStatNameLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
		labelContainer.AddChild(baseStatNameLabel);

		Label baseStatValueLabel = new Label();
		baseStatValueLabel.Text = statValue;
		baseStatValueLabel.AddThemeFontSizeOverride("font_size", 15);
		baseStatValueLabel.AddThemeColorOverride("font_color", color);
		labelContainer.AddChild(baseStatValueLabel);

		return labelContainer;
	}

	protected static Label GenerateGreyLabel(string text) {
		Label greyTextLabel = new Label();

		greyTextLabel.Text = text;
		greyTextLabel.AddThemeFontSizeOverride("font_size", 15);
		greyTextLabel.AddThemeColorOverride("font_color", UILib.ColorGrey);
		greyTextLabel.HorizontalAlignment = HorizontalAlignment.Center;

		return greyTextLabel;
	}

	protected static Label GenerateAffixLabel(string affixText) {
		Label affixTextLabel = new Label();

		affixTextLabel.Text = affixText;
		affixTextLabel.AddThemeFontSizeOverride("font_size", 15);
		affixTextLabel.AddThemeColorOverride("font_color", UILib.ColorBlurple);
		affixTextLabel.HorizontalAlignment = HorizontalAlignment.Center;

		return affixTextLabel;
	}

	protected static MarginContainer GeneratePriceTag(int price) {
		MarginContainer container = new();
		container.AddThemeConstantOverride("margin_top", 4);
		container.AddThemeConstantOverride("margin_bottom", 2);

		Label priceLabel = new();
		priceLabel.Text = $"{price} Gold";
		priceLabel.AddThemeFontSizeOverride("font_size", 15);
		priceLabel.AddThemeColorOverride("font_color", UILib.ColorGold);
		priceLabel.HorizontalAlignment = HorizontalAlignment.Center;
		container.AddChild(priceLabel);

		return container;
	}
}
