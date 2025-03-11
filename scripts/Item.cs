using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum EItemRarity {
	Common,
	Magic,
	Rare,
	Unique,
	What
}

public enum EItemEquipmentSlot {
	None,
	Head,
	Chest,
	Hands,
	Feet,
	Belt,
	Ring,
	Amulet,
	WeaponLeft,
	WeaponRight,
	COUNT
}

public enum EItemAllBaseType {
	None,
	Head,
	Chest,
	Hands,
	Feet,
	Belt,
	Ring,
	Amulet,
	Weapon1H,
	Weapon2H,
	Shield,
	COUNT
}

public enum EItemArmourBaseType {
	Helmet,
	Chestplate,
	Gloves,
	Boots,
	COUNT
}

public enum EItemWeaponBaseType {
	Weapon1H,
	Weapon2H,
	Shield,
	COUNT
}

public enum EItemJewelleryBaseType {
	Belt,
	Ring,
	Amulet,
	COUNT
}

public enum EItemCategory {
	None,
	Armour,
	Weapon,
	Jewellery,
	COUNT
}

[Flags]
public enum EItemBaseSpecifierFlags {
	NONE = 0,
	NoFlags = 1 << 0,
	AArmour = 1 << 1,
	AEvasion = 1 << 2,
	AEnergyShield = 1 << 3,
	AArmourEvasion = 1 << 4,
	AEvasionEnergyShield = 1 << 5,
	AEnergyShieldArmour = 1 << 6,
	AAll = 1 << 7,

	W1HSword = 1 << 8,
	W2HSword = 1 << 9
}

[Flags]
public enum EItemDefences {
	None = 0,
	Armour = 1,
	Evasion = 2,
	EnergyShield = 4,

	ArmourEvasion = Armour | Evasion,
	EvasionEnergyShield = Evasion | EnergyShield,
	EnergyShieldArmour = EnergyShield | Armour,

	All = Armour | Evasion | EnergyShield
}

public partial class Item {
	PackedScene worldItemScene = GD.Load<PackedScene>("res://worlditem.tscn");
	PackedScene inventoryItemScene = GD.Load<PackedScene>("res://hud_item.tscn");

	public Player PlayerOwner = null;

	// Alle disse burde flyttes væk herfra og ind i en separat tabel
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
			//GD.Print($"Valid prefixes: {item.GetValidPrefixTypes().Count}");
			//tableTypes = GetValidPrefixTypes().GetRange(0, GetValidPrefixTypes().Count);
			tableTypes = GetValidPrefixTypes();

			if (tableTypes.Count == 0) {
				//GD.PrintErr("No valid prefixes");
				return;
			}

			//GD.Print($"{tableTypes.Count} valid prefixes with item specifier flags ({ItemBaseSpecifierFlags})");
		}
		else {
			//GD.Print($"Valid suffixes: {item.GetValidPrefixTypes().Count}");
			//tableTypes = GetValidSuffixTypes().GetRange(0, GetValidSuffixTypes().Count);
			tableTypes = GetValidSuffixTypes();
			
			if (tableTypes.Count == 0) {
				//GD.PrintErr("No valid suffixes");
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

	}

	protected static string GetRandomName() {
		return NameGeneration.GenerateItemName();
	}
}

public partial class WeaponItem : Item {
	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
	public int AddedPhysicalMinimumDamage = 0;
	public int AddedPhysicalMaximumDamage = 0;
	public int PercentageIncreasedPhysicalDamage = 0;
	public int PhysicalMinimumDamage;
	public int PhysicalMaximumDamage;
	public EItemWeaponBaseType ItemWeaponBaseType;
	public string WeaponClass;

	public void CalculatePhysicalDamage() {
		PhysicalMinimumDamage = (int)((BasePhysicalMinimumDamage + AddedPhysicalMinimumDamage) * (1 + ((double)PercentageIncreasedPhysicalDamage / 100)));
		PhysicalMaximumDamage = (int)((BasePhysicalMaximumDamage + AddedPhysicalMaximumDamage) * (1 + ((double)PercentageIncreasedPhysicalDamage / 100))); 
	}

	protected override void ApplyAffix(Affix affix, bool add) {
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
					PercentageIncreasedPhysicalDamage += (int)affix.ValueFirst;
				}
				else {
					PercentageIncreasedPhysicalDamage -= (int)affix.ValueFirst;
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
	public int BaseArmour;
	public int AddedArmour;
	public int IncreasedArmour;
	public int Armour;
	public int BaseEvasion;
	public int AddedEvasion;
	public int IncreasedEvasion;
	public int Evasion;
	public int BaseEnergyShield;
	public int AddedEnergyShield;
	public int IncreasedEnergyShield;
	public int EnergyShield;
	public EItemArmourBaseType ItemArmourBaseType;

	public void CalculateDefences() {
		Armour = (int)((BaseArmour + AddedArmour) * (1 + ((double)IncreasedArmour / 100)));
		Evasion = (int)((BaseEvasion + AddedEvasion) * (1 + ((double)IncreasedEvasion / 100)));
		EnergyShield = (int)((BaseEnergyShield + AddedEnergyShield) * (1 + ((double)IncreasedEnergyShield / 100)));
	}

	protected override void ApplyAffix(Affix affix, bool add) {
		switch (affix.Data.AffixFamily) {
			case EAffixFamily.FlatArmour:
				if (add) {
					AddedArmour += (int)affix.ValueFirst;
				}
				else {
					AddedArmour -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.PercentageArmour:
				if (add) {
					IncreasedArmour += (int)affix.ValueFirst;
				}
				else {
					IncreasedArmour -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;
			
			case EAffixFamily.FlatEvasion:
				if (add) {
					AddedEvasion += (int)affix.ValueFirst;
				}
				else {
					AddedEvasion -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.PercentageEvasion:
				if (add) {
					IncreasedEvasion += (int)affix.ValueFirst;
				}
				else {
					IncreasedEvasion -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.FlatEnergyShield:
				if (add) {
					AddedEnergyShield += (int)affix.ValueFirst;
				}
				else {
					AddedEnergyShield -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.PercentageEnergyShield:
				if (add) {
					IncreasedEnergyShield += (int)affix.ValueFirst;
				}
				else {
					IncreasedEnergyShield -= (int)affix.ValueFirst;
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


// DEBUG

public partial class TestItem23 : ChestItem {
	public TestItem23() {
		ItemTexture = UILib.TextureItemD2LeatherArmour;
		ItemRarity = EItemRarity.Common;
		ItemName = "Leather Armour";
		ItemBase = "Leather Armour";
	}
}

public partial class TestItem22 : HandsItem {
	public TestItem22() {
		ItemTexture = UILib.TextureItemD2LeatherGloves;
		ItemRarity = EItemRarity.Rare;
		ItemName = GetRandomName();
		ItemBase = "Leather Gloves";
	}
}

public partial class TestItem11 : JewelleryItem {
	public TestItem11() {
		gridSizeX = 1;
		gridSizeY = 1;
		ItemTexture = UILib.TextureItemD2Ring0;
		ItemRarity = EItemRarity.Unique;
		ItemAllBaseType = EItemAllBaseType.Ring;
		ItemJewelleryBaseType = EItemJewelleryBaseType.Ring;
		ItemName = "Ring";
		ItemBase = "Ring";
	}
}

public partial class TestItem21 : BeltItem {
	public TestItem21() {
		ItemTexture = UILib.TextureItemD2Sash;
		ItemRarity = EItemRarity.Magic;
		ItemName = GetRandomName();
		ItemBase = "Sash";
	}
}

public partial class TestItemWeapon : OneHandedSwordItem {
	public TestItemWeapon() {
		ItemTexture = UILib.TextureItemD2ShortSword;
		ItemRarity = EItemRarity.Rare;
		ItemName = GetRandomName();
		ItemBase = "Short Sword";
	}
}
