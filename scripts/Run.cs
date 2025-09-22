using Godot;
using System;
using System.Linq;

public partial class Run : Node3D {
    protected static readonly PackedScene playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
    protected static readonly PackedScene townScene = GD.Load<PackedScene>("res://scenes/world/map_town.tscn");

    protected static readonly PackedScene goldScene = GD.Load<PackedScene>("res://scenes/worldgold.tscn");
	protected static readonly PackedScene mapTransScene = GD.Load<PackedScene>("res://scenes/map_transition.tscn");

	protected static readonly PackedScene itemRewardPanelScene = GD.Load<PackedScene>("res://scenes/gui/item_reward_panel.tscn");

	[Signal]
    public delegate void GemLevelChangedEventHandler(int newLevel);
    
    public static Run Instance { get; private set; }

    public GameRules Rules { get; private set; }

    public Player PlayerActor { get; private set; }
	public MapBase CurrentMap;

	private PlayerCamera playerCam;
	private Node3D currentMapNode;
	private MapBase mapTown; // Husk at lave en speciel klasse til Town

	private CanvasLayer worldObjectsLayer;

	private int gemLevel = 0;
	public int GemLevel { 
		get => gemLevel;
		set {
            if (value > Rules.MaxGemLevel) {
                gemLevel = Rules.MaxGemLevel;
            }
            else if (value < 0) {
                gemLevel = 0;
            }
            else {
                gemLevel = value;
            }

            EmitSignal(SignalName.GemLevelChanged, gemLevel);
			PlayerActor?.UpdateSkillValues();
        } 
	}
	
	public int AreaLevel { get; protected set; }
	public int AreaLevelMod { get; protected set; } = 0;
	public int CurrentAct { get; protected set; } = 0;
	public int CurrentArea { get; protected set; } = 0; // When acts are properly structured, this should be set to 0 by it, and only maps past the first should increment this
	private const int areasPerAct = 14; // Areas refer to both combat maps and shop/breather maps
	private bool firstMapEntered = false;

	public bool Act1Completed { get; private set; } = false;
	public bool Act2Completed { get; private set; } = false;
	public bool Act3Completed { get; private set; } = false;

	private EnemyRarityTable currentEnemyRarityTable;
	private DropDataTable currentDropDataTable;
	public int LootLuck { get; set; }
	public double ItemQuantityBonus { get; set; } = 0;
	public double ItemRarityBonus { get; set; } = 0;
	public double GoldBonus { get; set; } = 0;
	public double ExperienceBonus { get; set; } = 0;

    public override void _Ready() {
        Instance = this;

		Rules = new();

        currentMapNode = GetNode<Node3D>("CurrentMapNode");
        worldObjectsLayer = GetNode<CanvasLayer>("WorldObjects");

		//GameSettings.Instance.ApplyPlayerSettings();

		StartNewRun();
		currentEnemyRarityTable = EnemyRarityData.GetCurrentRarityTable(AreaLevel);
		currentDropDataTable = ItemDropData.GetCurrentDropDataTable(AreaLevel);
    }

	public void StartNewRun() {
		AreaLevelMod = 0;
		StartAct(0);

		mapTown = townScene.Instantiate<MapBase>();
        currentMapNode.AddChild(mapTown);
		
		CurrentMap = mapTown;
		mapTown.MapObjective = EMapObjective.None;
		MapTransitionObj transObj = mapTown.GetNode<MapTransitionObj>("MapTransition");
		transObj.SceneToTransitionTo = MapDatabase.FEScene;

        PlayerActor = playerScene.Instantiate<Player>();
        AddChild(PlayerActor);
        PlayerActor.GlobalPosition = CurrentMap.PlayerSpawnMarker.GlobalPosition;

		PlayerActor.PlayerHUD.RightHUD.UpdateProgressLabel();

		CreateStarterWeaponPanel();
	}

	protected void CreateStarterWeaponPanel() {
		ItemRewardPanel starterWeaponPanel = itemRewardPanelScene.Instantiate<ItemRewardPanel>();
		Run.Instance.PlayerActor.PlayerHUD.AddChild(starterWeaponPanel);
		starterWeaponPanel.RewardTaken += CreateStarterSkillPanel;
		starterWeaponPanel.GenerateStarterWeapons();
	}

	// This will not show up immediately afterwards at a later point, so signal can safely be removed then
	protected void CreateStarterSkillPanel() {
		ItemRewardPanel starterSkillPanel = itemRewardPanelScene.Instantiate<ItemRewardPanel>();
		Run.Instance.PlayerActor.PlayerHUD.AddChild(starterSkillPanel);
		starterSkillPanel.GenerateStarterSkills();
	}
	
	public void StartAct(int actNo) {
		switch (actNo) {
			case 1: // Act 2
				CurrentAct = 1;
				CurrentArea = 0;
				GemLevel = 5;
				AreaLevel = 11;
				break;
			
			case 2: // Act 3
				CurrentAct = 2;
				CurrentArea = 0;
				GemLevel = 10;
				AreaLevel = 21;
				break;
			
			default: // Otherwise load Act 1
				CurrentAct = 0;
				CurrentArea = 0;
				GemLevel = 0;
				AreaLevel = 1;
				break;
		}
	}

    public void LoadAndSetMapToTown() {
		if (!mapTown.IsInsideTree()) {
			PlayerActor.PlayerCamera.OcclusionCast.Enabled = false;

			MapBase oldMap = CurrentMap;
			oldMap.ClearEnemies();
			
			currentMapNode.AddChild(mapTown);
			CurrentMap = mapTown;
			CurrentMap.AddRegionToNav();
			OnMapLoaded();

			oldMap.QueueFree();

			CurrentArea = 0; // When acts are properly structured, this should be set to 0 by it, and only maps past the first should increment this
		}
	}

	private void UnloadTown() {
		if (mapTown.IsInsideTree()) {
			currentMapNode.CallDeferred(MethodName.RemoveChild, mapTown);
		}
	}

	/// <summary>
	/// Changes the map to a non-town scene. Do not use this for loading the town.
	/// </summary>
	/// <param name="scene"></param>
	public void ChangeMap(PackedScene scene) {
		PlayerActor.PlayerCamera.OcclusionCast.Enabled = false;

		MapBase oldMap = CurrentMap;
		oldMap.ClearEnemies();

        MapDatabase.GetMap(scene, out CurrentMap, out MapTags tags);
		CurrentMap.MapType = tags.Type;

		int randomObjectiveIndex = Utilities.RNG.Next(tags.Objectives.Count);
        CurrentMap.MapObjective = tags.Objectives[randomObjectiveIndex];
		
		CurrentMap.MapReady += OnMapLoaded;
		CurrentMap.MapObjectiveFinished += OnMapCompletion;
		currentMapNode.AddChild(CurrentMap);

		if (oldMap == mapTown) {
			UnloadTown();
		}
		else {
			oldMap.QueueFree();
		}

		ProgressAct(CurrentMap.MapType == EMapType.Objective || CurrentMap.MapType == EMapType.Miniboss);
		currentEnemyRarityTable = EnemyRarityData.GetCurrentRarityTable(AreaLevel);
		currentDropDataTable = ItemDropData.GetCurrentDropDataTable(AreaLevel);
	}

	public void SpawnPortal() {
		MapTransitionObj transObj = mapTransScene.Instantiate<MapTransitionObj>();
		CurrentMap.AddChild(transObj);

		Vector3 newPos;
		if (CurrentMap.MapType == EMapType.Intermission) {
			Marker3D portalSpawnMarker = CurrentMap.GetNode<Marker3D>("PlayerExit");
			newPos = portalSpawnMarker.GlobalPosition;
		}
		else {
			newPos = CurrentMap.PlayerSpawnMarker.GlobalPosition;
		}
		
		newPos.Y += 0.5f;
		transObj.GlobalPosition = newPos;

		if ((CurrentArea + 1) % 3 == 0) {
			transObj.SceneToTransitionTo = MapDatabase.ShopSmallTownScene;
			transObj.GoesToTown = false;
			transObj.UseRedPortal = false;
		}
		else if (CurrentArea < areasPerAct - 1) {
			transObj.SceneToTransitionTo = MapDatabase.FEScene;
			transObj.GoesToTown = false;
			transObj.UseRedPortal = true;
		}
		else {
			transObj.GoesToTown = true;
			transObj.UseRedPortal = false;
		}
		
		transObj.UpdatePortalAnimationAndVisibility();
	}

	// Potentially add exception for the town? Just to avoid spawning in the same spot every time no matter where you came from?
	private void OnMapLoaded() {
		PlayerActor.ResetNodeTarget();
		PlayerActor.Velocity = Vector3.Zero;
		PlayerActor.GlobalPosition = CurrentMap.PlayerSpawnMarker.GlobalPosition;
		PlayerActor.PlayerCamera.OcclusionCast.Enabled = true;

		PlayerActor.PlayerHUD.RightHUD.UpdateProgressLabel();
	}

	public void OnMapCompletion() {
		SpawnPortal();
    }

	private void ProgressAct(bool doIncrementAreaLevel) {
		CurrentArea++;

		// Make sure not to increment anything upon entering the first area of an act
		if (CurrentArea != 1) {
			if (doIncrementAreaLevel) {
				AreaLevel++;
			}
			else {
				GemLevel++;
			}
		}
	}


	#region Utility
	
	/// <summary>
	/// Clears all nodes in the WorldObjectsLayer and current map's NameplateLayer
	/// </summary>
	public void RemoveAllWorldItems() {
        System.Collections.Generic.IEnumerable<Node> worldItems = worldObjectsLayer.GetChildren().Where(c => c.IsInGroup("WorldItem"));
		worldItems = worldItems.Concat(CurrentMap.NameplateLayer.GetChildren().Where(c => c.IsInGroup("WorldItem")));
		foreach (Node item in worldItems) {
			item.QueueFree();
		}
	}

	public void GenerateRandomItemFromCategory(EItemCategory category, Vector3 position) {
		//int currentAreaLevel = CurrentMap.AreaLevel;
		Item item = ItemGeneration.GenerateItemFromCategory(category);
		WorldItem worldItem = item.ConvertToWorldItem();
		DropItem(worldItem, position);
	}

	public void GenerateRandomSkillGem(Vector3 position) {
		Item item = ItemGeneration.GenerateRandomSkillGem();
		WorldItem worldItem = item.ConvertToWorldItem();
		DropItem(worldItem, position);
	}

	public void GenerateRandomSupportGem(Vector3 position) {
		Item item = ItemGeneration.GenerateRandomSupportGem();
		WorldItem worldItem = item.ConvertToWorldItem();
		DropItem(worldItem, position);
	}

	/// <summary>
	/// Adds WorldItem to current map's NameplateLayer at provided GlobalPosition. Useful for generated items or items thrown by the player.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="position"></param>
	public void DropItem(WorldItem item, Vector3 position) {
		CurrentMap.NameplateLayer.AddChild(item);
		item.GlobalPosition = position with { Y = position.Y + 0.25f };
		item.PostSpawn();
	}

	/// <summary>
	/// Creates a pile of Gold at provided GlobalPosition. If isRandom is true, final value is variable within a slight range.
	/// </summary>
	/// <param name="baseAmount"></param>
	/// <param name="position"></param>
	/// <param name="isRandom"></param>
	public void DropGold(int baseAmount, Vector3 position, bool isRandom) {
		Gold gold = goldScene.Instantiate<Gold>();
		CurrentMap.NameplateLayer.AddChild(gold);

		if (isRandom) {
			gold.SetAmount((int)Math.Round(Utilities.RandomDouble(baseAmount * 0.75, baseAmount * 1.25), 0));
		}
		else {
			gold.SetAmount(baseAmount);
		}

		gold.GlobalPosition = position with { Y = position.Y + 0.25f };
		gold.PostSpawn();
	}

	/// <summary>
	/// Gives experience to the player. baseAmount is the value before other modifiers are applied.
	/// </summary>
	/// <param name="baseAmount"></param>
	public void AwardExperience(double baseAmount) {
		PlayerActor.GainExperience(baseAmount);
	}

	/// <summary>
	/// Takes an Enemy instance, and rolls for how many items should be dropped from its death
	/// </summary>
	/// <param name="enemy"></param>
	/// <returns></returns>
	public bool RollForEnemyItems(EnemyBase enemy) {
        double chance = 0;
		double uniqueChance = 0;
		double rareChance = 0;
		double magicChance = 0;
		int itemLevel = AreaLevel;
        bool haveItemsDropped = false;

        switch (enemy.EnemyRarity) {
            case EEnemyRarity.Normal:
                chance = currentDropDataTable.NormalItemChance;
				uniqueChance = currentDropDataTable.NormalUniqueChance;
				rareChance = currentDropDataTable.NormalRareChance;
				magicChance = currentDropDataTable.NormalMagicChance;
                break;
            
            case EEnemyRarity.Magic:
                chance = currentDropDataTable.MagicItemChance;
				uniqueChance = currentDropDataTable.MagicUniqueChance;
				rareChance = currentDropDataTable.MagicRareChance;
				magicChance = currentDropDataTable.MagicMagicChance;
				itemLevel += 1;
                break;

            case EEnemyRarity.Rare:
                chance = currentDropDataTable.RareItemChance;
				uniqueChance = currentDropDataTable.RareUniqueChance;
				rareChance = currentDropDataTable.RareRareChance;
				magicChance = currentDropDataTable.RareMagicChance;
				itemLevel += 2;
                break;
			
			case EEnemyRarity.Unique:
                chance = currentDropDataTable.UniqueItemChance;
				uniqueChance = currentDropDataTable.UniqueUniqueChance;
				rareChance = currentDropDataTable.UniqueRareChance;
				magicChance = currentDropDataTable.UniqueMagicChance;
				itemLevel += 2;
                break;
            
            default:
                break;
        }

		chance *= 			1 + ItemQuantityBonus;
		uniqueChance *= 	1 + ItemRarityBonus;
		rareChance *= 		1 + ItemRarityBonus;
		magicChance *= 		1 + ItemRarityBonus;

        while (chance >= 1) {
            chance -= 1;
			CreateItem(uniqueChance, rareChance, magicChance, itemLevel);
            haveItemsDropped = true;
        }

        if (chance != 0 && chance >= Utilities.RNG.NextDouble()) {
			CreateItem(uniqueChance, rareChance, magicChance, itemLevel);
            haveItemsDropped = true;
        }
        
        return haveItemsDropped;
    }

	protected void CreateItem(double uniqueChance, double rareChance, double magicChance, int itemLevel) {
		//GD.Print($"Unique: {uniqueChance}, Rare: {rareChance}, Magic: {magicChance}");
		if (RollForRarity(uniqueChance)) {
			CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None, itemLevel, EItemRarity.Unique));
		}
		else if (RollForRarity(rareChance)) {
			CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None, itemLevel, EItemRarity.Rare));
		}
		else if (RollForRarity(magicChance)) {
			CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None, itemLevel, EItemRarity.Magic));
		}
		else {
			CurrentMap.ObjectiveController?.AddItemToRewards(ItemGeneration.GenerateItemFromCategory(EItemCategory.None, itemLevel, EItemRarity.Common));
		}
	}

	public EEnemyRarity RollForEnemyRarity() {
		if (RollForRarity(currentEnemyRarityTable.RareChance)) {
			return EEnemyRarity.Rare;
		}
		else if (RollForRarity(currentEnemyRarityTable.MagicChance)) {
			return EEnemyRarity.Magic;
		}
		else {
			return EEnemyRarity.Normal;
		}
	}

	protected static bool RollForRarity(double chance) {
		double roll = Utilities.RNG.NextDouble();
		if (roll <= chance) {
			return true;
		}
		return false;
	}

	#endregion
}
