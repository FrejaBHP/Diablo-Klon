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

		ApplyRarityAndAffixes(ref item);

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
			case EItemWeaponBaseType.Weapon1H:
				OneHandedSwordItem ohSwordItem = new();
				GetBaseFromTable(ref ohSwordItem, 0);

				weaponItem = ohSwordItem;
				break;

			default:
				TwoHandedSwordItem thSwordItem = new();
				GetBaseFromTable(ref thSwordItem, 0);

				weaponItem = thSwordItem;
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
				beltItem.ItemTexture = UILib.TextureItemD2Sash;

				jewelleryItem = beltItem;
				break;

			case EItemJewelleryBaseType.Ring:
				RingItem ringItem = new();
				ringItem.ItemTexture = UILib.TextureItemD2Ring0;

				jewelleryItem = ringItem;
				break;

			// case EItemJewelleryBaseType.Amulet:
			default:
				AmuletItem amuletItem = new();
				amuletItem.ItemTexture = UILib.TextureItemD2Amulet0;

				jewelleryItem = amuletItem;
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
				case EItemWeaponBaseType.Weapon1H:
					legalWeaponData = ItemDataTables.OHSwordWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					
					ApplyWeaponBaseStats(ref weaponItem, data); // Flyt ned når alle branches er udfyldt
					break;

				// case EItemWeaponBaseType.Weapon2H:
				default:
					legalWeaponData = ItemDataTables.THSwordWeaponData.Where(i => i.MinimumLevel >= minLevel).ToList();
					data = legalWeaponData[Utilities.RNG.Next(legalWeaponData.Count)];
					
					ApplyWeaponBaseStats(ref weaponItem, data); // Flyt ned når alle branches er udfyldt
					break;
			}
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

			ApplyArmourBaseStats(ref armourItem, data);
		}
	}

	private static void ApplyWeaponBaseStats(ref WeaponItem item, WeaponItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.BasePhysicalMinimumDamage = data.BasePhysicalMinimumDamage;
		item.BasePhysicalMaximumDamage = data.BasePhysicalMaximumDamage;

		// indtil der bliver implementeret affixes:
		item.CalculatePhysicalDamage();
	}

	private static void ApplyArmourBaseStats(ref ArmourItem item, ArmourItemData data) {
		item.ItemBase = data.BaseName;
		item.ItemBaseSpecifierFlags |= data.ItemSpecifierFlags;
		item.ItemTexture = data.Texture;
		item.MinimumLevel = data.MinimumLevel;

		item.ItemDefences = data.ItemDefences;
		item.BaseArmour = data.BaseArmour;
		item.BaseEvasion = data.BaseEvasion;
		item.BaseEnergyShield = data.BaseEnergyShield;

		// indtil der bliver implementeret affixes:
		item.CalculateDefences();
	}

	private static void ApplyRarityAndAffixes(ref Item item) {
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
}
