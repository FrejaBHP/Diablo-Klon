using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Item {
	PackedScene worldItemScene = GD.Load<PackedScene>("res://scenes/worlditem.tscn");
	PackedScene inventoryItemScene = GD.Load<PackedScene>("res://scenes/gui/hud_item.tscn");

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
	public List<Affix> Implicits = new List<Affix>();
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

		if (player.PlayerHUD.PlayerInventory.TryAddItemToInventory(ref inventoryItem)) {
			if (IsInWorld) {
				worldItemRef.QueueFree();
				worldItemRef = null;
			}

			IsInWorld = false;
			IsPickedUp = true;
			PlayerOwner = player;
			invItemRef = inventoryItem;
			inventoryItem.InventoryReference = player.PlayerHUD.PlayerInventory;
		}
		else {
			inventoryItem.QueueFree();
		}
	}

	public List<AffixTableType> GetValidPrefixTypes() {
		List<AffixTableType> validPrefixes = AffixDataTables.PrefixData.Where(p => Utilities.HasAnyFlags(ItemAffixFlags, p.AffixItemFlags)).ToList();
		validPrefixes.RemoveAll(p => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, p.AffixItemSpecifierFlags));

		for (int i = 0; i < Prefixes.Count; i++) {
			validPrefixes.RemoveAll(p => p.AffixFamily == Prefixes[i].AffixFamily);
		}
		
		return validPrefixes;
	}

	public List<AffixTableType> GetValidSuffixTypes() {
		List<AffixTableType> validSuffixes = AffixDataTables.SuffixData.Where(s => Utilities.HasAnyFlags(ItemAffixFlags, s.AffixItemFlags)).ToList();
		validSuffixes.RemoveAll(s => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, s.AffixItemSpecifierFlags));

		for (int i = 0; i < Suffixes.Count; i++) {
			validSuffixes.RemoveAll(s => s.AffixFamily == Suffixes[i].AffixFamily);
		}

		return validSuffixes;
	}

	public void AddImplicits(List<Type> implicitTypes) {
		foreach (Type type in implicitTypes) {
			Affix newImplicitAffix = (Affix)Activator.CreateInstance(type);
			newImplicitAffix.RollAffixValue();
			Implicits.Add(newImplicitAffix);
			ApplyAffixToDictionary(newImplicitAffix, true);
		}
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

		ApplyAffixToDictionary(newAffix, true);
	}

	protected virtual void ApplyAffixToDictionary(Affix affix, bool add) {
		if (affix.IsLocal) {
			ApplyLocalAffix(affix, add);
			return;
		}

		// First affix value
		if (StatDictionary.ContainsKey(affix.StatNameFirst)) {
			if (add) {
				if (affix.IsMultiplicative) {
					StatDictionary[affix.StatNameFirst] *= 1 + affix.ValueFirst;
				}
				else {
					StatDictionary[affix.StatNameFirst] += affix.ValueFirst;
				}
				
			}
			else {
				if (affix.IsMultiplicative) {
					StatDictionary[affix.StatNameFirst] /= 1 + affix.ValueFirst;
				}
				else {
					StatDictionary[affix.StatNameFirst] -= affix.ValueFirst;
				}
				

				if (StatDictionary[affix.StatNameFirst] == 0) {
					StatDictionary.Remove(affix.StatNameFirst);
				}
			}
		}
		else {
			StatDictionary.Add(affix.StatNameFirst, affix.ValueFirst);
		}

		// Second affix value, if affix is a range (such as added min and max damage values)
		if (affix.StatNameSecond != EStatName.None) {
			if (StatDictionary.ContainsKey(affix.StatNameSecond)) {
				if (add) {
					if (affix.IsMultiplicative) {
						StatDictionary[affix.StatNameSecond] *= 1 + affix.ValueSecond;
					}
					else {
						StatDictionary[affix.StatNameSecond] += affix.ValueSecond;
					}
					
				}
				else {
					if (affix.IsMultiplicative) {
						StatDictionary[affix.StatNameSecond] /= 1 + affix.ValueSecond;
					}
					else {
						StatDictionary[affix.StatNameSecond] -= affix.ValueSecond;
					}
					

					if (StatDictionary[affix.StatNameSecond] == 0) {
						StatDictionary.Remove(affix.StatNameSecond);
					}
				}
			}
			else {
				StatDictionary.Add(affix.StatNameSecond, affix.ValueSecond);
			}
		}
	}

	protected virtual void ApplyLocalAffix(Affix affix, bool add) {

	}

	protected static string GetRandomName() {
		return NameGeneration.GenerateItemName();
	}

	public virtual void PostGenCalculation() {

	}
}

public partial class WeaponItem : Item {
	public EItemWeaponBaseType ItemWeaponBaseType;
	public string WeaponClass;

	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
	public int AddedPhysicalMinimumDamage = 0;
	public int AddedPhysicalMaximumDamage = 0;
	public double PercentageIncreasedPhysicalDamage = 0;
	public int PhysicalMinimumDamage;
	public int PhysicalMaximumDamage;

	public double BaseAttackSpeed;
	public double PercentageIncreasedAttackSpeed = 0;
	public double AttackSpeed;

	public double BaseCriticalStrikeChance;
	public double PercentageIncreasedCriticalStrikeChance = 0;
	public double CriticalStrikeChance;
	
	public WeaponItem() {
		
	}

	public override void PostGenCalculation() {
		CalculatePhysicalDamage();
		CalculateAttackSpeed();
		CalculateCriticalStrikeChance();
	}

	public void CalculatePhysicalDamage() {
		PhysicalMinimumDamage = (int)Math.Round((BasePhysicalMinimumDamage + AddedPhysicalMinimumDamage) * (1 + PercentageIncreasedPhysicalDamage), 0);
		PhysicalMaximumDamage = (int)Math.Round((BasePhysicalMaximumDamage + AddedPhysicalMaximumDamage) * (1 + PercentageIncreasedPhysicalDamage), 0);
	}

	public void CalculateAttackSpeed() {
		AttackSpeed = BaseAttackSpeed / (1 + PercentageIncreasedAttackSpeed);
	}

	public string GetAttackSpeed() {
		return $"{(1 / AttackSpeed):F2}";
	}

	public void CalculateCriticalStrikeChance() {
		CriticalStrikeChance = BaseCriticalStrikeChance * (1 + PercentageIncreasedCriticalStrikeChance);
	}

	public string GetCritChance() {
		return $"{CriticalStrikeChance:F2}%";
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.AffixFamily) {
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

			case EAffixFamily.LocalIncreasedPhysDamage:
				if (add) {
					PercentageIncreasedPhysicalDamage += affix.ValueFirst;
				}
				else {
					PercentageIncreasedPhysicalDamage -= affix.ValueFirst;
				}
				CalculatePhysicalDamage();
				break;

			case EAffixFamily.LocalIncreasedAttackSpeed:
				if (add) {
					PercentageIncreasedAttackSpeed += affix.ValueFirst;
				}
				else {
					PercentageIncreasedAttackSpeed -= affix.ValueFirst;
				}
				CalculateAttackSpeed();
				break;

			case EAffixFamily.LocalIncreasedCritChance:
				if (add) {
					PercentageIncreasedCriticalStrikeChance += affix.ValueFirst;
				}
				else {
					PercentageIncreasedCriticalStrikeChance -= affix.ValueFirst;
				}
				CalculateCriticalStrikeChance();
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
	public double IncreasedArmour = 0;
	public int Armour;
	public int BaseEvasion;
	public int AddedEvasion;
	public double IncreasedEvasion = 0;
	public int Evasion;
	public int BaseEnergyShield;
	public int AddedEnergyShield;
	public double IncreasedEnergyShield = 0;
	public int EnergyShield;

	public ArmourItem() {
		StatDictionary.Add(EStatName.FlatArmour, 0);
		StatDictionary.Add(EStatName.FlatEvasion, 0);
		StatDictionary.Add(EStatName.FlatEnergyShield, 0);
	}

	public override void PostGenCalculation() {
		CalculateDefences();
	}

	public void CalculateDefences() {
		Armour = (int)Math.Round((BaseArmour + AddedArmour) * (1 + IncreasedArmour), 0);
		Evasion = (int)Math.Round((BaseEvasion + AddedEvasion) * (1 + IncreasedEvasion), 0);
		EnergyShield = (int)Math.Round((BaseEnergyShield + AddedEnergyShield) * (1 + IncreasedEnergyShield), 0);

		StatDictionary[EStatName.FlatArmour] = Armour;
		StatDictionary[EStatName.FlatEvasion] = Evasion;
		StatDictionary[EStatName.FlatEnergyShield] = EnergyShield;
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.AffixFamily) {
			case EAffixFamily.LocalFlatArmour:
				if (add) {
					AddedArmour += (int)affix.ValueFirst;
				}
				else {
					AddedArmour -= (int)affix.ValueFirst;
				}
				CalculateDefences();
				break;

			case EAffixFamily.LocalIncreasedArmour:
				if (add) {
					IncreasedArmour += affix.ValueFirst;
				}
				else {
					IncreasedArmour -= affix.ValueFirst;
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

			case EAffixFamily.LocalIncreasedEvasion:
				if (add) {
					IncreasedEvasion += affix.ValueFirst;
				}
				else {
					IncreasedEvasion -= affix.ValueFirst;
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

			case EAffixFamily.LocalIncreasedEnergyShield:
				if (add) {
					IncreasedEnergyShield += affix.ValueFirst;
				}
				else {
					IncreasedEnergyShield -= affix.ValueFirst;
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
