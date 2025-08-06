using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	public bool CanRarityChange = true;
	public int Price = 50;

	public int MagicMaxPrefixes { get; protected set; } = 1;
	public int MagicMaxSuffixes { get; protected set; } = 1;
	public int RareMaxPrefixes { get; protected set; } = 3;
	public int RareMaxSuffixes { get; protected set; } = 3;
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

	public void ConvertToInventoryItem(InventoryGrid grid, Player owner = null) {
		InventoryItem inventoryItem = inventoryItemScene.Instantiate<InventoryItem>();
		inventoryItem.SetItemReference(this);
		inventoryItem.SetGridSize(gridSizeX, gridSizeY);

		if (grid.TryAddItemToInventory(ref inventoryItem)) {
			if (IsInWorld) {
				worldItemRef.QueueFree();
				worldItemRef = null;
			}

			IsInWorld = false;
			IsPickedUp = true;
			PlayerOwner = owner;
			invItemRef = inventoryItem;
		}
		else {
			inventoryItem.QueueFree();
		}
	}

	public InventoryItem ConvertToRewardItem() {
		InventoryItem inventoryItem = inventoryItemScene.Instantiate<InventoryItem>();
		inventoryItem.SetItemReference(this);
		inventoryItem.SetGridSize(gridSizeX, gridSizeY);

		IsPickedUp = true;
		invItemRef = inventoryItem;

		return inventoryItem;
	}

	public List<AffixTableType> GetValidPrefixTypes() {
		List<AffixTableType> validPrefixes = AffixDataTables.PrefixData.Where(p => Utilities.HasAnyFlags(ItemAffixFlags, p.AffixItemFlags)).ToList();
		validPrefixes.RemoveAll(p => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, p.AffixItemSpecifierFlags));

		for (int i = 0; i < Prefixes.Count; i++) {
			validPrefixes.RemoveAll(p => p.AffixFamily == Prefixes[i].Family);
		}
		
		return validPrefixes;
	}

	public List<AffixTableType> GetValidSuffixTypes() {
		List<AffixTableType> validSuffixes = AffixDataTables.SuffixData.Where(s => Utilities.HasAnyFlags(ItemAffixFlags, s.AffixItemFlags)).ToList();
		validSuffixes.RemoveAll(s => !Utilities.HasAnyFlags(ItemBaseSpecifierFlags, s.AffixItemSpecifierFlags));

		for (int i = 0; i < Suffixes.Count; i++) {
			validSuffixes.RemoveAll(s => s.AffixFamily == Suffixes[i].Family);
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
		}
		else {
			Suffixes.Add(newAffix);
		}

		if (ItemRarity == EItemRarity.Magic) {
			UpdateMagicItemName();
		}

		ApplyAffixToDictionary(newAffix, true);
	}

	protected void UpdateMagicItemName() {
		StringBuilder sb = new();

		if (Prefixes.Count > 0) {
			sb.Append(Prefixes[0].GetAffixName() + " ");
		}

		sb.Append(ItemBase);

		if (Suffixes.Count > 0) {
			sb.Append(" " + Suffixes[0].GetAffixName());
		}

		ItemName = sb.ToString();
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
					StatDictionary[affix.StatNameFirst] *= affix.ValueFirst;
				}
				else {
					StatDictionary[affix.StatNameFirst] += affix.ValueFirst;
				}
			}
			else {
				if (affix.IsMultiplicative) {
					StatDictionary[affix.StatNameFirst] /= affix.ValueFirst;
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
						StatDictionary[affix.StatNameSecond] *= affix.ValueSecond;
					}
					else {
						StatDictionary[affix.StatNameSecond] += affix.ValueSecond;
					}
				}
				else {
					if (affix.IsMultiplicative) {
						StatDictionary[affix.StatNameSecond] /= affix.ValueSecond;
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

public partial class SkillItem : Item {
	public ESkillName SkillName;
	public Type SkillType;
	public Skill SkillReference;

	public SkillItem() {
		ItemAllBaseType = EItemAllBaseType.SkillActive;
		CanRarityChange = false;
		MagicMaxPrefixes = 0;
		MagicMaxSuffixes = 0;
		RareMaxPrefixes = 0;
		RareMaxSuffixes = 0;

		gridSizeX = 1;
		gridSizeY = 1;

		if (Run.Instance.Rules.ExistingGemsScaleWithGameProgress) {
			Run.Instance.GemLevelChanged += SetGemLevel;
		}
	}

	public void SetGemLevel(int level) {
		if (SkillReference != null) {
			SkillReference.Level = level;
		}
	}
}

public partial class WeaponItem : Item {
	public EItemWeaponBaseType ItemWeaponBaseType;
	public string WeaponClass;

	public int BasePhysicalMinimumDamage;
	public int BasePhysicalMaximumDamage;
	public int AddedPhysicalMinimumDamage = 0;
	public int AddedPhysicalMaximumDamage = 0;
	public double PercentagePhysicalDamage = 1;
	public int PhysicalMinimumDamage;
	public int PhysicalMaximumDamage;

	public int AddedFireMinimumDamage = 0;
	public int AddedFireMaximumDamage = 0;
	public int AddedColdMinimumDamage = 0;
	public int AddedColdMaximumDamage = 0;
	public int AddedLightningMinimumDamage = 0;
	public int AddedLightningMaximumDamage = 0;
	public int AddedChaosMinimumDamage = 0;
	public int AddedChaosMaximumDamage = 0;

	public double BaseAttackSpeed;
	public double PercentageAttackSpeed = 1;
	public double AttackSpeed;

	public double BaseCriticalStrikeChance;
	public double PercentageCriticalStrikeChance = 1;
	public double CriticalStrikeChance;
	
	public WeaponItem() {
		
	}

	public override void PostGenCalculation() {
		CalculatePhysicalDamage();
		CalculateAttackSpeed();
		CalculateCriticalStrikeChance();
	}

	public void CalculatePhysicalDamage() {
		PhysicalMinimumDamage = (int)Math.Round((BasePhysicalMinimumDamage + AddedPhysicalMinimumDamage) * PercentagePhysicalDamage, 0);
		PhysicalMaximumDamage = (int)Math.Round((BasePhysicalMaximumDamage + AddedPhysicalMaximumDamage) * PercentagePhysicalDamage, 0);
	}

	public void CalculateAttackSpeed() {
		AttackSpeed = BaseAttackSpeed / PercentageAttackSpeed;
	}

	public string GetAttackSpeed() {
		return $"{(1 / AttackSpeed):F2}";
	}

	public void CalculateCriticalStrikeChance() {
		CriticalStrikeChance = BaseCriticalStrikeChance * PercentageCriticalStrikeChance;
	}

	public string GetCritChance() {
		return $"{CriticalStrikeChance * 100:F2}%";
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.Family) {
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
					PercentagePhysicalDamage += affix.ValueFirst;
				}
				else {
					PercentagePhysicalDamage -= affix.ValueFirst;
				}
				CalculatePhysicalDamage();
				break;

			case EAffixFamily.LocalFlatFireDamage:
				if (add) {
					AddedFireMinimumDamage += (int)affix.ValueFirst;
					AddedFireMaximumDamage += (int)affix.ValueSecond;
				}
				else {
					AddedFireMinimumDamage -= (int)affix.ValueFirst;
					AddedFireMaximumDamage -= (int)affix.ValueSecond;
				}
				break;

			case EAffixFamily.LocalFlatColdDamage:
				if (add) {
					AddedColdMinimumDamage += (int)affix.ValueFirst;
					AddedColdMaximumDamage += (int)affix.ValueSecond;
				}
				else {
					AddedColdMinimumDamage -= (int)affix.ValueFirst;
					AddedColdMaximumDamage -= (int)affix.ValueSecond;
				}
				break;
			
			case EAffixFamily.LocalFlatLightningDamage:
				if (add) {
					AddedLightningMinimumDamage += (int)affix.ValueFirst;
					AddedLightningMaximumDamage += (int)affix.ValueSecond;
				}
				else {
					AddedLightningMinimumDamage -= (int)affix.ValueFirst;
					AddedLightningMaximumDamage -= (int)affix.ValueSecond;
				}
				break;
			
			case EAffixFamily.LocalFlatChaosDamage:
				if (add) {
					AddedChaosMinimumDamage += (int)affix.ValueFirst;
					AddedChaosMaximumDamage += (int)affix.ValueSecond;
				}
				else {
					AddedChaosMinimumDamage -= (int)affix.ValueFirst;
					AddedChaosMaximumDamage -= (int)affix.ValueSecond;
				}
				break;

			case EAffixFamily.LocalIncreasedAttackSpeed:
				if (add) {
					PercentageAttackSpeed += affix.ValueFirst;
				}
				else {
					PercentageAttackSpeed -= affix.ValueFirst;
				}
				CalculateAttackSpeed();
				break;

			case EAffixFamily.LocalIncreasedCritChance:
				if (add) {
					PercentageCriticalStrikeChance += affix.ValueFirst;
				}
				else {
					PercentageCriticalStrikeChance -= affix.ValueFirst;
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
	public double PercentageArmour = 1;
	public int Armour;
	public int BaseEvasion;
	public int AddedEvasion;
	public double PercentageEvasion = 1;
	public int Evasion;
	public int BaseEnergyShield;
	public int AddedEnergyShield;
	public double PercentageEnergyShield = 1;
	public int EnergyShield;

	public double BaseBlockChance = 0;
	public double AddedBlockChance = 0;
	public double BlockChance = 0;

	public ArmourItem() {
		StatDictionary.Add(EStatName.FlatArmour, 0);
		StatDictionary.Add(EStatName.FlatEvasion, 0);
		StatDictionary.Add(EStatName.FlatEnergyShield, 0);
		StatDictionary.Add(EStatName.BlockChance, 0);
	}

	public override void PostGenCalculation() {
		CalculateDefences();
	}

	public void CalculateDefences() {
		Armour = (int)Math.Round((BaseArmour + AddedArmour) * PercentageArmour, 0);
		Evasion = (int)Math.Round((BaseEvasion + AddedEvasion) * PercentageEvasion, 0);
		EnergyShield = (int)Math.Round((BaseEnergyShield + AddedEnergyShield) * PercentageEnergyShield, 0);
		BlockChance = BaseBlockChance + AddedBlockChance;

		StatDictionary[EStatName.FlatArmour] = Armour;
		StatDictionary[EStatName.FlatEvasion] = Evasion;
		StatDictionary[EStatName.FlatEnergyShield] = EnergyShield;
		StatDictionary[EStatName.BlockChance] = BlockChance;
	}

	protected override void ApplyLocalAffix(Affix affix, bool add) {
		switch (affix.Family) {
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
					PercentageArmour += affix.ValueFirst;
				}
				else {
					PercentageArmour -= affix.ValueFirst;
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
					PercentageEvasion += affix.ValueFirst;
				}
				else {
					PercentageEvasion -= affix.ValueFirst;
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
					PercentageEnergyShield += affix.ValueFirst;
				}
				else {
					PercentageEnergyShield -= affix.ValueFirst;
				}
				CalculateDefences();
				break;
			
			case EAffixFamily.AddedBlockChance:
				if (add) {
					AddedBlockChance += affix.ValueFirst;
				}
				else {
					AddedBlockChance -= affix.ValueFirst;
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

// Weapon Types

public partial class OneHandedSwordItem : WeaponItem {
	public OneHandedSwordItem() {
		gridSizeX = 1;
		gridSizeY = 3;
		ItemAllBaseType = EItemAllBaseType.Weapon1H;
		ItemWeaponBaseType = EItemWeaponBaseType.WeaponMelee1H;
		ItemAffixFlags = EAffixItemFlags.OHWeapon | EAffixItemFlags.Weapon;
		WeaponClass = "One Handed Sword";
	}
}

public partial class TwoHandedSwordItem : WeaponItem {
	public TwoHandedSwordItem() {
		gridSizeX = 2;
		gridSizeY = 4;
		ItemAllBaseType = EItemAllBaseType.Weapon2H;
		ItemWeaponBaseType = EItemWeaponBaseType.WeaponMelee2H;
		ItemAffixFlags = EAffixItemFlags.THWeapon | EAffixItemFlags.Weapon;
		WeaponClass = "Two Handed Sword";
	}
}

public partial class WandItem : WeaponItem {
	public WandItem() {
		gridSizeX = 1;
		gridSizeY = 3;
		ItemAllBaseType = EItemAllBaseType.Weapon1H;
		ItemWeaponBaseType = EItemWeaponBaseType.WeaponRanged1H;
		ItemAffixFlags = EAffixItemFlags.OHWeapon | EAffixItemFlags.Weapon | EAffixItemFlags.Wand;
		WeaponClass = "Wand";
	}
}

public partial class StaffItem : WeaponItem {
	public StaffItem() {
		gridSizeX = 2;
		gridSizeY = 4;
		ItemAllBaseType = EItemAllBaseType.Weapon2H;
		ItemWeaponBaseType = EItemWeaponBaseType.WeaponMelee2H;
		ItemAffixFlags = EAffixItemFlags.THWeapon | EAffixItemFlags.Weapon | EAffixItemFlags.Staff;
		WeaponClass = "Staff";
	}
}

public partial class BowItem : WeaponItem {
	public BowItem() {
		gridSizeX = 2;
		gridSizeY = 4;
		ItemAllBaseType = EItemAllBaseType.Weapon2H;
		ItemWeaponBaseType = EItemWeaponBaseType.WeaponRanged2H;
		ItemAffixFlags = EAffixItemFlags.THWeapon | EAffixItemFlags.Weapon | EAffixItemFlags.Bow;
		WeaponClass = "Bow";
	}
}

// Armour Types

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

// Accessory Types

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

public partial class QuiverItem : JewelleryItem {
	public QuiverItem() {
		gridSizeX = 2;
		gridSizeY = 3;
		ItemAllBaseType = EItemAllBaseType.Quiver;
		ItemJewelleryBaseType = EItemJewelleryBaseType.Quiver;
		ItemAffixFlags = EAffixItemFlags.Quiver;
	}
}
