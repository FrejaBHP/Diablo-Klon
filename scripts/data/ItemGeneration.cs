using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ItemGeneration {
	private static readonly int magicMaxPrefixes = 1;
	private static readonly int magicMaxSuffixes = 1;
	private static readonly int rareMaxPrefixes = 3;
	private static readonly int rareMaxSuffixes = 3;

	public static Item GenerateItemFromCategory(EItemCategory category, int itemLevel = -1, EItemRarity rarity = EItemRarity.None) {
		Item item;

		if (itemLevel == -1) {
			itemLevel = Run.Instance.AreaLevel;
		}

		if (rarity == EItemRarity.None) {
			rarity = CalculateRarity();
		}

		// If category not specified, pick one at random
		if (category == EItemCategory.None) {
			category = (EItemCategory)Utilities.RNG.Next(1, (int)EItemCategory.COUNT);
		}

		switch (category) {
			case EItemCategory.Weapon:
				EItemWeaponBaseType weaponType = (EItemWeaponBaseType)Utilities.RNG.Next((int)EItemWeaponBaseType.COUNT);
				item = GenerateWeaponItem(weaponType, itemLevel);
				break;

			case EItemCategory.Armour:
				EItemArmourBaseType armourType = (EItemArmourBaseType)Utilities.RNG.Next((int)EItemArmourBaseType.COUNT);
				item = GenerateArmourItem(armourType, itemLevel);
				break;
			
			//case EItemCategory.Jewellery:
			default:
				EItemJewelleryBaseType jewelleryType = (EItemJewelleryBaseType)Utilities.RNG.Next((int)EItemJewelleryBaseType.COUNT);
				item = GenerateJewelleryItem(jewelleryType, itemLevel);
				break;
		}

		ApplyRarityAndAffixes(item, rarity);

		return item;
	}

	/*
	public static Item GenerateItemFromSlot(EItemEquipmentSlot slot) {
		if (slot == EItemEquipmentSlot.None) {
			slot = (EItemEquipmentSlot)rnd.Next((int)EItemEquipmentSlot.COUNT);
		}
	}
	*/

	private static EItemRarity CalculateRarity() {
		WeightedList<EItemRarity> rarityList = new([
			new WeightedListItem<EItemRarity>(EItemRarity.Common, 0),
			new WeightedListItem<EItemRarity>(EItemRarity.Magic, 75),
			new WeightedListItem<EItemRarity>(EItemRarity.Rare, 25),
			new WeightedListItem<EItemRarity>(EItemRarity.Unique, 0),
		], Utilities.RNG);

		return rarityList.GetRandomItem();
	}

	private static WeaponItem GenerateWeaponItem(EItemWeaponBaseType weaponType, int itemLevel) {
		WeaponItem weaponItem;
		WeaponItemData data;

		List<WeaponItemData> legalWeaponData = new List<WeaponItemData>();

		switch (weaponType) {
			case EItemWeaponBaseType.WeaponMelee1H:
				weaponItem = new OneHandedSwordItem();
				legalWeaponData = ItemDataTables.OHSwordWeaponData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
				break;

			case EItemWeaponBaseType.WeaponMelee2H:
				int meleeType = Utilities.RNG.Next(0, 2);

				switch (meleeType) {
					case 0:
						weaponItem = new TwoHandedSwordItem();
						legalWeaponData = ItemDataTables.THSwordWeaponData.Where(i => i.MinimumLevel <= itemLevel).ToList();
						data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
						break;
					
					default:
						weaponItem = new StaffItem();
						legalWeaponData = ItemDataTables.StaffWeaponData.Where(i => i.MinimumLevel <= itemLevel).ToList();
						data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
						break;
				}
				break;

			case EItemWeaponBaseType.WeaponRanged1H:
				weaponItem = new WandItem();
				legalWeaponData = ItemDataTables.WandWeaponData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
				break;

			// case EItemWeaponBaseType.WeaponRanged2H:
			default:
				weaponItem = new BowItem();
				legalWeaponData = ItemDataTables.BowWeaponData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
				break;
		}

		weaponItem.ItemLevel = itemLevel;
		weaponItem.AddImplicits(data.ImplicitTypes);
		ApplyWeaponBaseStats(weaponItem, data);

		if (String.IsNullOrEmpty(weaponItem.ItemBase)) {
			weaponItem.ItemBase = weaponItem.ItemWeaponBaseType.ToString();
		}

		return weaponItem;
	}

	private static ArmourItem GenerateArmourItem(EItemArmourBaseType armourType, int itemLevel) {
		ArmourItem armourItem;
		ArmourItemData data;

		List<ArmourItemData> legalArmourData = new List<ArmourItemData>();

		switch (armourType) {
			case EItemArmourBaseType.Helmet:
				armourItem = new HeadItem();
				legalArmourData = ItemDataTables.HeadArmourData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
				break;
			
			case EItemArmourBaseType.Chestplate:
				armourItem = new ChestItem();
				legalArmourData = ItemDataTables.ChestArmourData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
				break;

			case EItemArmourBaseType.Gloves:
				armourItem = new HandsItem();
				legalArmourData = ItemDataTables.HandsArmourData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
				break;

			case EItemArmourBaseType.Boots:
				armourItem = new FeetItem();
				legalArmourData = ItemDataTables.FeetArmourData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
				break;

			//case EItemArmourBaseType.Shield:
			default:
				int shieldType = Utilities.RNG.Next(0, 3);
				switch (shieldType) {
					default:
						armourItem = new SmallShieldItem();
						legalArmourData = ItemDataTables.SmallShieldArmourData.Where(i => i.MinimumLevel <= itemLevel).ToList();
						data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
						break;
				}
				break;
		}

		armourItem.ItemLevel = itemLevel;
		armourItem.AddImplicits(data.ImplicitTypes);
		ApplyArmourBaseStats(armourItem, data);

		if (String.IsNullOrEmpty(armourItem.ItemBase)) {
			armourItem.ItemBase = armourItem.ItemArmourBaseType.ToString();
		}

		return armourItem;
	}

	private static JewelleryItem GenerateJewelleryItem(EItemJewelleryBaseType jewelleryType, int itemLevel) {
		JewelleryItem jewelleryItem;
		ItemData data;

		List<ItemData> legalJewelleryData = new List<ItemData>();

		switch (jewelleryType) {
			case EItemJewelleryBaseType.Amulet:
				jewelleryItem = new AmuletItem();
				legalJewelleryData = ItemDataTables.AmuletJewelleryData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
				break;

			case EItemJewelleryBaseType.Ring:
				jewelleryItem = new RingItem();
				legalJewelleryData = ItemDataTables.RingJewelleryData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
				break;

			case EItemJewelleryBaseType.Belt:
				jewelleryItem = new BeltItem();
				legalJewelleryData = ItemDataTables.BeltJewelleryData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
				break;

			default:
				jewelleryItem = new QuiverItem();
				legalJewelleryData = ItemDataTables.QuiverJewelleryData.Where(i => i.MinimumLevel <= itemLevel).ToList();
				data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
				break;
		}

		jewelleryItem.ItemLevel = itemLevel;
		jewelleryItem.AddImplicits(data.ImplicitTypes);
		ApplyJewelleryBaseStats(jewelleryItem, data);

		if (String.IsNullOrEmpty(jewelleryItem.ItemBase)) {
			jewelleryItem.ItemBase = jewelleryItem.ItemJewelleryBaseType.ToString();
		}

		return jewelleryItem;
	}

	private static void ApplyWeaponBaseStats(WeaponItem item, WeaponItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.BasePhysicalMinimumDamage = data.BasePhysicalMinimumDamage;
		item.BasePhysicalMaximumDamage = data.BasePhysicalMaximumDamage;
		item.BaseAttackSpeed = data.BaseAttackSpeed;
		item.BaseCriticalStrikeChance = data.BaseCritChance;

		item.PostGenCalculation();
	}

	private static void ApplyArmourBaseStats(ArmourItem item, ArmourItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.ItemDefences = data.ItemDefences;
		item.BaseArmour = data.BaseArmour;
		item.BaseEvasion = data.BaseEvasion;
		item.BaseEnergyShield = data.BaseEnergyShield;
		item.BaseBlockChance = data.BaseBlockChance;

		item.PostGenCalculation();
	}

	private static void ApplyJewelleryBaseStats(JewelleryItem item, ItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.PostGenCalculation();
	}

	private static void ApplyRarityAndAffixes(Item item, EItemRarity rarity) {
		item.ItemRarity = rarity;

		switch (rarity) {
			case EItemRarity.Magic:
				item.ItemName = item.ItemBase;
				int affixesToRollM = Utilities.RNG.Next(1, 3);

				for (int i = 0; i < affixesToRollM; i++) {
					if (item.Prefixes.Count != magicMaxPrefixes && item.Suffixes.Count != magicMaxSuffixes) {
						item.AddRandomAffix((EAffixPosition)Utilities.RNG.Next(0, 2));
					}
					else if (item.Prefixes.Count == magicMaxPrefixes && item.Suffixes.Count == 0) {
						item.AddRandomAffix(EAffixPosition.Suffix);
					}
					else {
						item.AddRandomAffix(EAffixPosition.Prefix);
					}
				}
				break;

			case EItemRarity.Rare:
				item.ItemName = NameGeneration.GenerateItemName();
				int affixesToRollR = Utilities.RNG.Next(3, 7);

				for (int i = 0; i < affixesToRollR; i++) {
					if (item.Prefixes.Count < rareMaxPrefixes && item.Suffixes.Count < rareMaxSuffixes) {
						item.AddRandomAffix((EAffixPosition)Utilities.RNG.Next(0, 2));
					}
					else if (item.Prefixes.Count == rareMaxPrefixes) {
						item.AddRandomAffix(EAffixPosition.Suffix);
					}
					else {
						item.AddRandomAffix(EAffixPosition.Prefix);
					}
				}
				break;

			default:
				if (!string.IsNullOrEmpty(item.ItemBase)) {
					item.ItemName = item.ItemBase;
				}
				else {
					item.ItemName = item.ItemAllBaseType.ToString();
				}
				break;
		}
	}

	public static SkillItem GenerateRandomSkillGem() {
		SkillItem item = new();

		List<SkillItemData> legalSkillItemData = ItemDataTables.SkillData.ToList();
		SkillItemData data = legalSkillItemData[Utilities.RNG.Next(legalSkillItemData.Count)];

		item.ItemRarity = EItemRarity.Skill;
		item.ItemBase = data.BaseName;
		item.SkillName = data.SkillName;
		item.ItemTexture = data.Texture;
		item.SkillType = data.SkillType;

		// Lav om senere, så denne gøres, når et skill gem samles op, og ikke allerede har et allokeret skill
		Skill newSkill = (Skill)Activator.CreateInstance(item.SkillType);
		newSkill.Level = Run.Instance.GemLevel;

		item.SkillReference = newSkill;

		item.ItemName = item.SkillReference.Name;

		return item;
	}

	public static List<SkillItem> GenerateRandomSkillGems(int amount, bool allowRepeats) {
		List<SkillItemData> legalSkillGemData = ItemDataTables.SkillData.ToList();
		List<SkillItem> generatedSkillGems = new();

		for (int i = 0; i < amount; i++) {
			if (legalSkillGemData.Count != 0) {
				SkillItem item = new();
				int index = Utilities.RNG.Next(legalSkillGemData.Count);

				SkillItemData data = legalSkillGemData[index];

				item.ItemRarity = EItemRarity.Skill;
				item.ItemBase = data.BaseName;
				item.SkillName = data.SkillName;
				item.ItemTexture = data.Texture;
				item.SkillType = data.SkillType;

				Skill newSkill = (Skill)Activator.CreateInstance(item.SkillType);
				newSkill.Level = Run.Instance.GemLevel;

				item.SkillReference = newSkill;

				item.ItemName = item.SkillReference.Name;

				generatedSkillGems.Add(item);

				if (!allowRepeats) {
					legalSkillGemData.RemoveAt(index);
				}
			}
		}
		
		return generatedSkillGems;
	}

	public static SupportGem GenerateRandomSupportGem() {
		List<SupportGemData> legalSupportGemData = ItemDataTables.SupportGemData.ToList();
		SupportGemData data = legalSupportGemData[Utilities.RNG.Next(legalSupportGemData.Count)];

		SupportGem item = (SupportGem)Activator.CreateInstance(data.SupportType);

		item.ItemRarity = EItemRarity.Skill;
		item.ItemBase = data.BaseName;
		item.ItemTexture = data.Texture;

		item.Level = Run.Instance.GemLevel;

		return item;
	}

	public static List<SupportGem> GenerateRandomSupportGems(int amount, bool allowRepeats) {
		List<SupportGemData> legalSupportGemData = ItemDataTables.SupportGemData.ToList();
		List<SupportGem> generatedSupportGems = new();

		for (int i = 0; i < amount; i++) {
			if (legalSupportGemData.Count != 0) {
				int index = Utilities.RNG.Next(legalSupportGemData.Count);

				SupportGemData data = legalSupportGemData[index];

				SupportGem item = (SupportGem)Activator.CreateInstance(data.SupportType);

				item.ItemRarity = EItemRarity.Skill;
				item.ItemBase = data.BaseName;
				item.ItemTexture = data.Texture;

				item.Level = Run.Instance.GemLevel;

				generatedSupportGems.Add(item);

				if (!allowRepeats) {
					legalSupportGemData.RemoveAt(index);
				}
			}
		}
		
		return generatedSupportGems;
	}
}
