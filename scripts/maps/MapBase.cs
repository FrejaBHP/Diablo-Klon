using Godot;
using System;
using System.Collections.Generic;

public partial class MapBase : Node3D {
    protected static readonly PackedScene survivalControllerScene = GD.Load<PackedScene>("res://scenes/objectives/survival_controller.tscn");
    protected static readonly PackedScene wavesControllerScene = GD.Load<PackedScene>("res://scenes/objectives/waves_controller.tscn");

    [Signal]
    public delegate void MapReadyEventHandler();

    [Signal]
    public delegate void MapObjectiveStartedEventHandler(EMapObjective objectiveType);

    [Signal]
    public delegate void MapObjectiveFinishedEventHandler();

    public NavigationRegion3D NavRegion { get; protected set; }
    public Node3D EnemiesNode { get; protected set; }
    public Marker3D PlayerSpawnMarker { get; protected set; }
    public Timer MapStartTimer { get; protected set; }
    public CanvasLayer NameplateLayer { get; protected set; }

    public EMapType MapType;
    public EMapObjective MapObjective = EMapObjective.None;
    public ObjectiveController ObjectiveController = null;

    public int LocalAreaLevel = 1;

    public int EnemySpawnBudget = 0; // Intended for usage in terms of spawning non-Normal enemies and enemies that are considered stronger than others

    protected bool hasMapStarted = false;
    protected bool hasMapFinished = false;

    public override void _Ready() {
        NavRegion = GetNode<NavigationRegion3D>("NavigationRegion3D");
        EnemiesNode = GetNode<Node3D>("Enemies");
        PlayerSpawnMarker = GetNode<Marker3D>("PlayerSpawn");
        MapStartTimer = GetNode<Timer>("MapStartTimer");
        NameplateLayer = GetNode<CanvasLayer>("NameplateLayer");

        SetupController();

        CallDeferred(MethodName.CalculateAndUpdateAreaLevel);
        CallDeferred(MethodName.AddRegionToNav);
        CallDeferred(MethodName.OnMapReady);
    }

    public void SetupController() {
        if (MapObjective == EMapObjective.None) {
            return;
        }
        else if (MapObjective == EMapObjective.Survival) {
            ObjectiveController = survivalControllerScene.Instantiate<SurvivalController>();
        }
        else if (MapObjective == EMapObjective.Waves) {
            ObjectiveController = wavesControllerScene.Instantiate<WavesController>();
        }

        if (MapObjective != EMapObjective.None && ObjectiveController == null) {
            GD.PrintErr($"Missing Objective Controller for {MapObjective}");
            return;
        }

        AddChild(ObjectiveController);
        ObjectiveController.LinkedMap = this;
    }

    public override void _PhysicsProcess(double delta) {
        
    }

    public void CalculateAndUpdateAreaLevel() {
        LocalAreaLevel = Run.Instance.AreaLevel + Run.Instance.AreaLevelMod;
    }

    public void ClearEnemies() {
        foreach (Node enemy in EnemiesNode.GetChildren()) {
            enemy.QueueFree();
        }
    }

    public void AddRegionToNav() {
        NavigationServer3D.RegionSetMap(NavRegion.GetRid(), GetWorld3D().NavigationMap);
    }

    protected void OnMapReady() {
        EmitSignal(SignalName.MapReady);

        if (MapType == EMapType.Intermission) {
            EmitSignal(SignalName.MapObjectiveFinished);
            return;
        }

        ObjectiveController?.CreateObjectiveGUI();

        if (MapObjective == EMapObjective.Survival || MapObjective == EMapObjective.Waves) {
			MapStartTimer.WaitTime = 2;
			MapStartTimer.Start();
        }
    }

    public void OnMapStartTimerTimeout() {
        ObjectiveController?.StartObjective();
        hasMapStarted = true;
    }

    public void OnMapObjectiveFinished() {
        hasMapFinished = true;
        EmitSignal(SignalName.MapObjectiveFinished);
        ObjectiveController?.DestroyController();
    }

    public void SpawnEnemy(EEnemyType type) {
        Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
        spawnPos.Y -= 0.5f;
        
        EnemyBase enemy = EnemyDatabase.EnemyDictionary[type].Scene.Instantiate<EnemyBase>();
        EnemiesNode.AddChild(enemy);
        enemy.GlobalPosition = spawnPos;

        enemy.EnemyDied += OnEnemyKilled;
    }

	public void OnEnemyKilled() {
        ObjectiveController?.OnEnemyKilled();
	}
}
