using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ItemGeneration {
	private static readonly int magicMaxPrefixes = 1;
	private static readonly int magicMaxSuffixes = 1;
	private static readonly int rareMaxPrefixes = 3;
	private static readonly int rareMaxSuffixes = 3;

	static ItemGeneration() {

	}

	public static Item GenerateItemFromCategory(EItemCategory category) {
		Item item;
		EItemRarity rarity = CalculateRarity();

		// If category not specified, pick one at random
		if (category == EItemCategory.None) {
			category = (EItemCategory)Utilities.RNG.Next(1, (int)EItemCategory.COUNT);
		}

		switch (category) {
			case EItemCategory.Weapon:
				EItemWeaponBaseType weaponType = (EItemWeaponBaseType)Utilities.RNG.Next((int)EItemWeaponBaseType.COUNT);
				item = GenerateWeaponItem(weaponType, rarity);
				break;

			case EItemCategory.Armour:
				EItemArmourBaseType armourType = (EItemArmourBaseType)Utilities.RNG.Next((int)EItemArmourBaseType.COUNT);
				item = GenerateArmourItem(armourType, rarity);
				break;
			
			//case EItemCategory.Jewellery:
			default:
				EItemJewelleryBaseType jewelleryType = (EItemJewelleryBaseType)Utilities.RNG.Next((int)EItemJewelleryBaseType.COUNT);
				item = GenerateJewelleryItem(jewelleryType, rarity);
				break;
		}

		ApplyRarityAndAffixes(item);
		return item;
	}

	/*
	public static Item GenerateItemFromSlot(EItemEquipmentSlot slot) {
		if (slot == EItemEquipmentSlot.None) {
			slot = (EItemEquipmentSlot)rnd.Next((int)EItemEquipmentSlot.COUNT);
		}
	}
	*/

	private static WeaponItem GenerateWeaponItem(EItemWeaponBaseType weaponType, EItemRarity rarity) {
		WeaponItem weaponItem;

		switch (weaponType) {
			case EItemWeaponBaseType.WeaponMelee1H:
				OneHandedSwordItem ohSwordItem = new();
				GetBaseFromTable(ref ohSwordItem, 0);

				weaponItem = ohSwordItem;
				break;

			case EItemWeaponBaseType.WeaponMelee2H:
				TwoHandedSwordItem thSwordItem = new();
				GetBaseFromTable(ref thSwordItem, 0);

				weaponItem = thSwordItem;
				break;

			// case EItemWeaponBaseType.WeaponRanged1H & 2H:
			default:
				BowItem bowItem = new();
				GetBaseFromTable(ref bowItem, 0);

				weaponItem = bowItem;
				break;
		}

		if (String.IsNullOrEmpty(weaponItem.ItemBase)) {
			weaponItem.ItemBase = weaponItem.ItemWeaponBaseType.ToString();
		}

		return weaponItem;
	}

	private static ArmourItem GenerateArmourItem(EItemArmourBaseType armourType, EItemRarity rarity) {
		ArmourItem armourItem;

		switch (armourType) {
			case EItemArmourBaseType.Helmet:
				HeadItem headItem = new();
				GetBaseFromTable(ref headItem, 0);

				armourItem = headItem;
				break;
			
			case EItemArmourBaseType.Chestplate:
				ChestItem chestItem = new();
				GetBaseFromTable(ref chestItem, 0);

				armourItem = chestItem;
				break;

			case EItemArmourBaseType.Gloves:
				HandsItem handsItem = new();
				GetBaseFromTable(ref handsItem, 0);

				armourItem = handsItem;
				break;

			case EItemArmourBaseType.Boots:
				FeetItem feetItem = new();
				GetBaseFromTable(ref feetItem, 0);

				armourItem = feetItem;
				break;

			//case EItemArmourBaseType.Shield:
			default:
				int shieldType = Utilities.RNG.Next(0, 3);
				switch (shieldType) {
					default:
						SmallShieldItem smallShieldItem = new();
						GetBaseFromTable(ref smallShieldItem, 0);

						armourItem = smallShieldItem;
						break;
				}
				break;
		}

		if (String.IsNullOrEmpty(armourItem.ItemBase)) {
			armourItem.ItemBase = armourItem.ItemArmourBaseType.ToString();
		}

		return armourItem;
	}

	private static JewelleryItem GenerateJewelleryItem(EItemJewelleryBaseType jewelleryType, EItemRarity rarity) {
		JewelleryItem jewelleryItem;

		switch (jewelleryType) {
			case EItemJewelleryBaseType.Belt:
				BeltItem beltItem = new();
				GetBaseFromTable(ref beltItem, 0);

				jewelleryItem = beltItem;
				break;

			case EItemJewelleryBaseType.Ring:
				RingItem ringItem = new();
				GetBaseFromTable(ref ringItem, 0);

				jewelleryItem = ringItem;
				break;

			case EItemJewelleryBaseType.Amulet:
				AmuletItem amuletItem = new();
				GetBaseFromTable(ref amuletItem, 0);

				jewelleryItem = amuletItem;
				break;

			default:
				QuiverItem quiverItem = new();
				GetBaseFromTable(ref quiverItem, 0);

				jewelleryItem = quiverItem;
				break;
		}

		if (String.IsNullOrEmpty(jewelleryItem.ItemBase)) {
			jewelleryItem.ItemBase = jewelleryItem.ItemJewelleryBaseType.ToString();
		}

		return jewelleryItem;
	}

	private static EItemRarity CalculateRarity() {
		int rarity = Utilities.RNG.Next(4);

		switch (rarity) {
			case 0:
				return EItemRarity.Common;

			case 1:
				return EItemRarity.Magic;

			case 2:
				return EItemRarity.Rare;

			// case 3:
			default:
				return EItemRarity.Unique;
		}
	}

	private static void GetBaseFromTable<T>(ref T item, int minLevel) {
		if (item.GetType().IsSubclassOf(typeof(WeaponItem))) {
			WeaponItem weaponItem = item as WeaponItem;
			WeaponItemData data;

			List<WeaponItemData> legalWeaponData = new List<WeaponItemData>();

			switch (weaponItem.ItemWeaponBaseType) {
				case EItemWeaponBaseType.WeaponMelee1H:
					legalWeaponData = ItemDataTables.OHSwordWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					break;

				case EItemWeaponBaseType.WeaponMelee2H:
					legalWeaponData = ItemDataTables.THSwordWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					break;

				case EItemWeaponBaseType.WeaponRanged1H:
					legalWeaponData = ItemDataTables.BowWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					break;

				// case EItemWeaponBaseType.WeaponRanged2H
				default:
					legalWeaponData = ItemDataTables.BowWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					break;
			}

			weaponItem.AddImplicits(data.ImplicitTypes);
			ApplyWeaponBaseStats(weaponItem, data);
		}
		else if (item.GetType().IsSubclassOf(typeof(ArmourItem))) {
			ArmourItem armourItem = item as ArmourItem;
			ArmourItemData data;

			List<ArmourItemData> legalArmourData = new List<ArmourItemData>();

			switch (armourItem.ItemArmourBaseType) {
				case EItemArmourBaseType.Helmet:
					legalArmourData = ItemDataTables.HeadArmourData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
					break;

				case EItemArmourBaseType.Chestplate:
					legalArmourData = ItemDataTables.ChestArmourData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
					break;

				case EItemArmourBaseType.Gloves:
					legalArmourData = ItemDataTables.HandsArmourData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
					break;

				case EItemArmourBaseType.Boots:
					legalArmourData = ItemDataTables.FeetArmourData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
					break;

				// case EItemArmourBaseType.Shield:
				default:
					legalArmourData = ItemDataTables.SmallShieldArmourData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalArmourData[Utilities.RNG.Next(legalArmourData.Count)];
					break;
			}

			armourItem.AddImplicits(data.ImplicitTypes);
			ApplyArmourBaseStats(armourItem, data);
		}
		else if (item.GetType().IsSubclassOf(typeof(JewelleryItem))) {
			JewelleryItem jewelItem = item as JewelleryItem;
			ItemData data;

			List<ItemData> legalJewelleryData = new List<ItemData>();

			switch (jewelItem.ItemJewelleryBaseType) {
				case EItemJewelleryBaseType.Amulet:
					legalJewelleryData = ItemDataTables.AmuletJewelleryData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
					break;

				case EItemJewelleryBaseType.Ring:
					legalJewelleryData = ItemDataTables.RingJewelleryData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
					break;

				case EItemJewelleryBaseType.Belt:
					legalJewelleryData = ItemDataTables.BeltJewelleryData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
					break;

				//case EItemJewelleryBaseType.Belt:
				default:
					legalJewelleryData = ItemDataTables.QuiverJewelleryData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalJewelleryData[Utilities.RNG.Next(legalJewelleryData.Count)];
					break;
			}

			jewelItem.AddImplicits(data.ImplicitTypes);
			ApplyJewelleryBaseStats(jewelItem, data);
		}
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

		item.PostGenCalculation();
	}

	private static void ApplyJewelleryBaseStats(JewelleryItem item, ItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.PostGenCalculation();
	}

	private static void ApplyRarityAndAffixes(Item item) {
		EItemRarity rarity = CalculateRarity();
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

	public static SkillItem GenerateRandomSkillItem() {
		SkillItem item = new();
		SkillItemData data;

		List<SkillItemData> legalSkillItemData = new List<SkillItemData>();
		legalSkillItemData = ItemDataTables.SkillData.ToList();
		data = legalSkillItemData[Utilities.RNG.Next(legalSkillItemData.Count)];

		item.ItemRarity = EItemRarity.Skill;

		item.ItemBase = data.BaseName;
		item.SkillName = data.SkillName;
		item.ItemTexture = data.Texture;
		item.SkillType = data.SkillType;

		Skill newSkill = (Skill)Activator.CreateInstance(item.SkillType);
		item.SkillReference = newSkill;

		item.ItemName = item.SkillReference.Name;

		return item;
	}
}
