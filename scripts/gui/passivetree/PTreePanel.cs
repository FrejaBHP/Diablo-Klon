using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreePanel : PanelContainer {
    [Signal]
    public delegate void PassiveTreeChangedEventHandler();

    private static readonly PackedScene selectionPanelScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/ptree_class_selection_panel.tscn");

    private readonly PackedScene clusterJuggernautScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_juggernaut.tscn");
    private readonly PackedScene clusterSorcererScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_sorcerer.tscn");
    private readonly PackedScene clusterOccultistScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_occultist.tscn");
    private readonly PackedScene clusterPathfinderScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_pathfinder.tscn");
    private readonly PackedScene clusterAssassinScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_assassin.tscn");

    public EActorFlags PassiveTreeActorFlags { get; protected set; }
    public Dictionary<EStatName, double> PassiveTreeStatDictionary { get; protected set; } = new();

    private PTreeClassSelectionPanel activeClassSelectionPanel = null;

    private Label pointLabel;
    private HBoxContainer clustersContainer;

    private List<PTreeCluster> clusterPool = new();

    private int passiveTreePoints = 10;
    public int PassiveTreePoints { 
        get => passiveTreePoints;
        set {
            passiveTreePoints = value;
            UpdatePointLabelText();
        }
    }

    private bool nodesCanBeAllocated = true;

    public override void _Ready() {
        pointLabel = GetNode<Label>("MarginContainer/PointLabel");
        clustersContainer = GetNode<HBoxContainer>("MarginContainer/ClustersContainer");
        UpdatePointLabelText();

        //CreateClassSelectionPanel();
    }

	public void CreateClassSelectionPanel() {
        if (Run.Instance.PlayerActor.PlayerClasses.Count < 3) {
            clustersContainer.Visible = false;

            activeClassSelectionPanel = selectionPanelScene.Instantiate<PTreeClassSelectionPanel>();
            AddChild(activeClassSelectionPanel);
            activeClassSelectionPanel.ClassSelected += PlayerClassSelected;
        }
	}

    public void PlayerClassSelected(PTreeClassSelectionBox selectionBox) {
        VBoxContainer vBox = new VBoxContainer();
        clustersContainer.AddChild(vBox);

        Label label = new Label();
        vBox.AddChild(label);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.Text = selectionBox.Cluster.ClusterName;

        selectionBox.Cluster.Reparent(vBox);
        Run.Instance.PlayerActor.PlayerClasses.Add(selectionBox.ClassEnum);

        foreach (PTreeNode node in selectionBox.Cluster.NodeList) {
            //node.Resize();
            node.NodeClicked += OnNodeClicked;
            node.NodeAllocated += OnNodeAllocated;
        }
        
        activeClassSelectionPanel.QueueFree();

        clustersContainer.Visible = true;
    }

    private void UpdatePointLabelText() {
        pointLabel.Text = $"Passive Points: {passiveTreePoints}";
    }

    private void OnNodeClicked(PTreeNode node) {
        if (passiveTreePoints > 0) {
            node.Allocate();
            PassiveTreePoints--;

            //RemoveActiveClusters();
            //GetRandomClusters();
        }
    }

    public void OnNodeAllocated(PTreeNode node) {
        for (int i = 0; i < node.StatNames.Count; i++) {
            if (PassiveTreeStatDictionary.ContainsKey(node.StatNames[i])) {
                if (Utilities.MultiplicativeStatNames.Contains(node.StatNames[i])) {
                    PassiveTreeStatDictionary[node.StatNames[i]] *= node.StatValues[i];
                }
                else {
                    PassiveTreeStatDictionary[node.StatNames[i]] += node.StatValues[i];
                }
            }
            else {
                if (!PassiveTreeStatDictionary.TryAdd(node.StatNames[i], node.StatValues[i])) {
                    GD.PrintErr($"Failed to add {node.StatNames[i]} {node.StatValues[i]} to dictionary");
                }
            }
        }

        if (node.ActorFlag != 0) {
            PassiveTreeActorFlags |= node.ActorFlag;
        }

        EmitSignal(SignalName.PassiveTreeChanged);
    }
}
