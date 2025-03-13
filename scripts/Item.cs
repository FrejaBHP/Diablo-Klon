using Godot;
using System;
using System.Collections.Generic;
using System.Linq;



public partial class Item {
	PackedScene worldItemScene = GD.Load<PackedScene>("res://worlditem.tscn");
	PackedScene inventoryItemScene = GD.Load<PackedScene>("res://hud_item.tscn");

	public Player PlayerOwner = null;

	public string ItemName;
	public string ItemBase;
	public EItemRarity ItemRarity;
	public EItemAllBaseType ItemAllBaseType = EItemAllBaseType.None;
	public EItemBaseSpecifierFlags ItemBaseSpecifierFlags = EItemBaseSpecifierFlags.NoFlags;
	public EAffixItemFlags ItemAffixFlags;
	public int MinimumLevel = 0;
	public int ItemLevel = 0;

	public int MagicMaxPrefixes = 1;
	public int MagicMaxSuffixes = 1;
	public int RareMaxPrefixes = 3;
	public int RareMaxSuffixes = 3;
	public List<Affix> Prefixes = new List<Affix>();
	public List<Affix> Suffixes = new List<Affix>();

	public Texture2D ItemTexture;
	public bool IsInWorld = false;
	public bool IsPickedUp = false;

	public Dictionary<EStatName, double> StatDictionary = new();

	protected int gridSizeX = 1;
	protected int gridSizeY = 1;
	
	protected WorldItem worldItemRef;
	protected InventoryItem invItemRef;

	public Vector2I GetGridSize() {
		return new Vector2I(gridSizeX, gridSizeY);
	}

	public WorldItem ConvertToWorldItem() {
		PlayerOwner = null;
		WorldItem worldItem = worldItemScene.Instantiate<WorldItem>();
		worldItem.SetItemReference(this);

		worldItemRef = worldItem;

		if (IsPickedUp) {
			invItemRef.QueueFree();
			invItemRef = null;
		}

		IsInWorld = true;
		IsPickedUp = false;

		return worldItem;
	}

	public void ConvertToInventoryItem(Player player) {
		InventoryItem inventoryItem = inventoryItemScene.Instantiate<InventoryItem>();
		inventoryItem.SetItemReference(this);
		inventoryItem.SetGridSize(gridSizeX, gridSizeY);

		if (player.PlayerInventory.TryAddItemToInventory(ref inventoryItem)) {
			if (IsInWorld) {
				worldItemRef.QueueFree();
				worldItemRef = null;
			}

			IsInWorld = false;
			IsPickedUp = true;
			PlayerOwner = player;
			invItemRef = inventoryItem;
			inventoryItem.InventoryReference = player.PlayerInventory;
		}
		else {
			inventoryItem.QueueFree();
		}
	}

	public List<AffixTableType> GetValidPrefixTypes() {
		List<AffixTableType> validPrefixes = AffixDataTables.PrefixData.Where(p => Utilities.HasAnyFlags(ItemAffixFlags, p.AffixItemFlags)).ToList();
		validPrefixes.RemoveAll(p => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, p.AffixItemSpecifierFlags));

		for (int i = 0; i < Prefixes.Count; i++) {
			validPrefixes.RemoveAll(p => p.AffixTable[0].AffixFamily == Prefixes[i].Data.AffixFamily);
		}
		
		return validPrefixes;
	}

	public List<AffixTableType> GetValidSuffixTypes() {
		List<AffixTableType> validSuffixes = AffixDataTables.SuffixData.Where(s => Utilities.HasAnyFlags(ItemAffixFlags, s.AffixItemFlags)).ToList();
		validSuffixes.RemoveAll(s => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, s.AffixItemSpecifierFlags));

		for (int i = 0; i < Suffixes.Count; i++) {
			validSuffixes.RemoveAll(s => s.AffixTable[0].AffixFamily == Suffixes[i].Data.AffixFamily);
		}

		return validSuffixes;
	}

	public void AddRandomAffix(EAffixPosition position) {
		List<AffixTableType> tableTypes;

		if (position == EAffixPosition.Prefix) {
			tableTypes = GetValidPrefixTypes();

			if (tableTypes.Count == 0) {
				return;
			}
		}
		else {
			tableTypes = GetValidSuffixTypes();
			
			if (tableTypes.Count == 0) {
				return;
			}
		}

		AffixTableType affixTableType = tableTypes[Utilities.RNG.Next(tableTypes.Count)];

		Affix newAffix = (Affix)Activator.CreateInstance(affixTableType.AffixClassType);
		newAffix.RollAffixTier(ItemLevel);
		newAffix.RollAffixValue();

		if (position == EAffixPosition.Prefix) {
			Prefixes.Add(newAffix);

			if (ItemRarity == EItemRarity.Magic) {
				string newName = $"{newAffix.GetAffixName()} {ItemName}";
				ItemName = newName;
			}
		}
		else {
			Suffixes.Add(newAffix);

			if (ItemRarity == EItemRarity.Magic) {
				string newName = $"{ItemName} {newAffix.GetAffixName()}";
				ItemName = newName;
			}
		}

		ApplyAffix(newAffix, true);
	}

	protected virtual void ApplyAffix(Affix affix, bool add) {
		if (affix.IsLocal) {
			ApplyLocalAffix(affix, add);
			return;
		}

		// First affix value
		if (StatDictionary.ContainsKey(affix.StatNameFirst)) {
			if (add) {
				StatDictionary[affix.StatNameFirst] += (double)affix.ValueFirst;
			}
			else {
				StatDictionary[affix.StatNameFirst] -= (double)affix.ValueFirst;

				if (StatDictionary[affix.StatNameFirst] == 0) {
					StatDictionary.Remove(affix.StatNameFirst);
				}
			}
		}
		else {
			StatDictionary.Add(affix.StatNameFirst, (double)affix.ValueFirst);
		}

		// Second affix value, if affix is a range (such as added min and max damage values)
		if (affix.StatNameSecond != EStatName.None) {
			if (StatDictionary.ContainsKey(affix.StatNameSecond)) {
				if (add) {
					StatDictionary[affix.StatNameSecond] += (double)affix.ValueSecond;
				}
				else {
					StatDictionary[affix.StatNameSecond] -= (double)affix.ValueSecond;

					if (StatDictionary[affix.StatNameSecond] == 0) {
						StatDictionary.Remove(affix.StatNameSecond);
					}
				}
			}
			else {
				StatDictionary.Add(affix.StatNameSecond, (double)affix.ValueSecond);
			}
		}
	}

	protected virtual void ApplyLocalAffix(Affix affix, bool add) {

	}

	protected static string GetRandomName() {
		return NameGeneration.GenerateItemName();
	}
}

public partial class WeaponItem : Item {
	public EItemWeaponBaseType ItemWeaponBaseType;
	public string WeaponClass;

	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
	public int AddedPhysicalMinimumDamage = 0;
	public int AddedPhysicalMaximumDamage = 0;
	public float PercentageIncreasedPhysicalDamage = 0;
	public int PhysicalMinimumDamage;
	public int PhysicalMaximumDamage;
	
	public WeaponItem() {

	}

	public void CalculatePhysicalDamage() {
		PhysicalMinimumDamage = (int)MathF.Round((BasePhysicalMinimumDamage + AddedPhysicalMinimumDamage) * (1 + PercentageIncreasedPhysicalDamage), 0);
		PhysicalMaximumDamage = (int)MathF.Round((BasePhysicalMaximumDamage + AddedPhysicalMaximumDamage) * (1 + PercentageIncreasedPhysicalDamage), 0);
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.Data.AffixFamily) {
			case EAffixFamily.LocalFlatPhysDamage:
				if (add) {
					AddedPhysicalMinimumDamage += (int)affix.ValueFirst;
					AddedPhysicalMaximumDamage += (int)affix.ValueSecond;
				}
				else {
					AddedPhysicalMinimumDamage -= (int)affix.ValueFirst;
					AddedPhysicalMaximumDamage -= (int)affix.ValueSecond;
				}
				CalculatePhysicalDamage();
				break;

			case EAffixFamily.LocalPercentagePhysDamage:
				if (add) {
					PercentageIncreasedPhysicalDamage += (float)affix.ValueFirst;
				}
				else {
					PercentageIncreasedPhysicalDamage -= (float)affix.ValueFirst;
				}
				CalculatePhysicalDamage();
				break;
			
			default:
				break;
		}
	}
}

public partial class ArmourItem : Item {
	public EItemDefences ItemDefences;
	public EItemArmourBaseType ItemArmourBaseType;

	public int BaseArmour;
	public int AddedArmour;
	public float IncreasedArmour = 0;
	public int Armour;
	public int BaseEvasion;
	public int AddedEvasion;
	public float IncreasedEvasion = 0;
	public int Evasion;
	public int BaseEnergyShield;
	public int AddedEnergyShield;
	public float IncreasedEnergyShield = 0;
	public int EnergyShield;

	public ArmourItem() {
		StatDictionary.Add(EStatName.FlatArmour, 0);
		StatDictionary.Add(EStatName.FlatEvasion, 0);
		StatDictionary.Add(EStatName.FlatEnergyShield, 0);
	}

	public void CalculateDefences() {
		Armour = (int)MathF.Round((BaseArmour + AddedArmour) * (1 + IncreasedArmour), 0);
		Evasion = (int)MathF.Round((BaseEvasion + AddedEvasion) * (1 + IncreasedEvasion), 0);
		EnergyShield = (int)MathF.Round((BaseEnergyShield + AddedEnergyShield) * (1 + IncreasedEnergyShield), 0);

		StatDictionary[EStatName.FlatArmour] = Armour;
		StatDictionary[EStatName.FlatEvasion] = Evasion;
		StatDictionary[EStatName.FlatEnergyShield] = EnergyShield;
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.Data.AffixFamily) {
			case EAffixFamily.LocalFlatArmour:
				if (add) {
					AddedArmour += (int)affix.ValueFirst;
				}
				else {
					AddedArmour -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.LocalPercentageArmour:
				if (add) {
					IncreasedArmour += (float)affix.ValueFirst;
				}
				else {
					IncreasedArmour -= (float)affix.ValueFirst;
				}
				CalculateDefences();
				break;
			
			case EAffixFamily.LocalFlatEvasion:
				if (add) {
					AddedEvasion += (int)affix.ValueFirst;
				}
				else {
					AddedEvasion -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.LocalPercentageEvasion:
				if (add) {
					IncreasedEvasion += (float)affix.ValueFirst;
				}
				else {
					IncreasedEvasion -= (float)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.LocalFlatEnergyShield:
				if (add) {
					AddedEnergyShield += (int)affix.ValueFirst;
				}
				else {
					AddedEnergyShield -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.LocalPercentageEnergyShield:
				if (add) {
					IncreasedEnergyShield += (float)affix.ValueFirst;
				}
				else {
					IncreasedEnergyShield -= (float)affix.ValueFirst;
				}
				CalculateDefences();
				break;
			
			default:
				break;
		}
	}
}

public partial class JewelleryItem : Item {
	public EItemJewelleryBaseType ItemJewelleryBaseType;
}

public partial class HeadItem : ArmourItem {
	public HeadItem() {
		gridSizeX = 2;
		gridSizeY = 2;
		ItemAllBaseType = EItemAllBaseType.Head;
		ItemArmourBaseType = EItemArmourBaseType.Helmet;
		ItemAffixFlags = EAffixItemFlags.Helmet | EAffixItemFlags.Armour;
	}
}

public partial class ChestItem : ArmourItem {
	public ChestItem() {
		gridSizeX = 2;
		gridSizeY = 3;
		ItemAllBaseType = EItemAllBaseType.Chest;
		ItemArmourBaseType = EItemArmourBaseType.Chestplate;
		ItemAffixFlags = EAffixItemFlags.Chest | EAffixItemFlags.Armour;
	}
}

public partial class HandsItem : ArmourItem {
	public HandsItem() {
		gridSizeX = 2;
		gridSizeY = 2;
		ItemAllBaseType = EItemAllBaseType.Hands;
		ItemArmourBaseType = EItemArmourBaseType.Gloves;
		ItemAffixFlags = EAffixItemFlags.Gloves | EAffixItemFlags.Armour;
	}
}

public partial class FeetItem : ArmourItem {
	public FeetItem() {
		gridSizeX = 2;
		gridSizeY = 2;
		ItemAllBaseType = EItemAllBaseType.Feet;
		ItemArmourBaseType = EItemArmourBaseType.Boots;
		ItemAffixFlags = EAffixItemFlags.Boots | EAffixItemFlags.Armour;
	}
}

public partial class ShieldItem : ArmourItem {
	public ShieldItem() {
		ItemAllBaseType = EItemAllBaseType.Shield;
		ItemArmourBaseType = EItemArmourBaseType.Shield;
		ItemAffixFlags = EAffixItemFlags.Shield | EAffixItemFlags.Armour;
	}
}

public partial class SmallShieldItem : ShieldItem {
	public SmallShieldItem() {
		gridSizeX = 2;
		gridSizeY = 2;
	}
}

public partial class MediumShieldItem : ShieldItem {
	public MediumShieldItem() {
		gridSizeX = 2;
		gridSizeY = 3;
	}
}

public partial class LargeShieldItem : ShieldItem {
	public LargeShieldItem() {
		gridSizeX = 2;
		gridSizeY = 4;
	}
}

public partial class BeltItem : JewelleryItem {
	public BeltItem() {
		gridSizeX = 2;
		gridSizeY = 1;
		ItemAllBaseType = EItemAllBaseType.Belt;
		ItemJewelleryBaseType = EItemJewelleryBaseType.Belt;
		ItemAffixFlags = EAffixItemFlags.Belt | EAffixItemFlags.Jewellery;
	}
}

public partial class RingItem : JewelleryItem {
	public RingItem() {
		gridSizeX = 1;
		gridSizeY = 1;
		ItemAllBaseType = EItemAllBaseType.Ring;
		ItemJewelleryBaseType = EItemJewelleryBaseType.Ring;
		ItemAffixFlags = EAffixItemFlags.Ring | EAffixItemFlags.Jewellery;
	}
}

public partial class AmuletItem : JewelleryItem {
	public AmuletItem() {
		gridSizeX = 1;
		gridSizeY = 1;
		ItemAllBaseType = EItemAllBaseType.Amulet;
		ItemJewelleryBaseType = EItemJewelleryBaseType.Amulet;
		ItemAffixFlags = EAffixItemFlags.Amulet | EAffixItemFlags.Jewellery;
	}
}

public partial class OneHandedSwordItem : WeaponItem {
	public OneHandedSwordItem() {
		gridSizeX = 1;
		gridSizeY = 3;
		ItemAllBaseType = EItemAllBaseType.Weapon1H;
		ItemWeaponBaseType = EItemWeaponBaseType.Weapon1H;
		ItemAffixFlags = EAffixItemFlags.OHWeapon | EAffixItemFlags.Weapon;
		WeaponClass = "One Handed Sword";
	}
}

public partial class TwoHandedSwordItem : WeaponItem {
	public TwoHandedSwordItem() {
		gridSizeX = 2;
		gridSizeY = 4;
		ItemAllBaseType = EItemAllBaseType.Weapon2H;
		ItemWeaponBaseType = EItemWeaponBaseType.Weapon2H;
		ItemAffixFlags = EAffixItemFlags.THWeapon | EAffixItemFlags.Weapon;
		WeaponClass = "Two Handed Sword";
	}
}
