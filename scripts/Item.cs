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
	public EAffixItemFlags ItemAffixFlags;
	public int MinimumLevel = 0;
	public int ItemLevel = 0;

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

	public virtual List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(ItemAffixFlags)).ToList();
	}

	public virtual List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(ItemAffixFlags)).ToList();
	}

	protected static string GetRandomName() {
		return NameGeneration.GenerateItemName();
	}
}

public partial class WeaponItem : Item {
	public double BasePhysicalMinimumDamage;
	public double PhysicalMinimumDamage;
	public double BasePhysicalMaximumDamage;
	public double PhysicalMaximumDamage;
	public EItemWeaponBaseType ItemWeaponBaseType;
	public string WeaponClass;
}

public partial class ArmourItem : Item {
	public EItemDefences ItemDefences;
	public double BaseArmour;
	public double Armour;
	public double BaseEvasion;
	public double Evasion;
	public double BaseEnergyShield;
	public double EnergyShield;
	public EItemArmourBaseType ItemArmourBaseType;
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Helmet) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Helmet) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Chest) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Chest) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Gloves) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Gloves) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Boots) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Boots) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Armour)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Belt) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Jewellery)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Belt) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Jewellery)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Ring) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Jewellery)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.Ring) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Jewellery)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.OHWeapon) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Weapon)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.OHWeapon) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Weapon)).ToList();
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

	public override List<AffixTableType> GetValidPrefixTypes() {
		return AffixDataTables.PrefixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.THWeapon) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Weapon)).ToList();
	}

	public override List<AffixTableType> GetValidSuffixTypes() {
		return AffixDataTables.SuffixData.Where(i => i.AffixItemFlags.HasFlag(EAffixItemFlags.THWeapon) || i.AffixItemFlags.HasFlag(EAffixItemFlags.Weapon)).ToList();
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
