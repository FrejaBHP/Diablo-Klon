using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreePanel : PanelContainer {
    [Signal]
    public delegate void PassiveTreeChangedEventHandler();

    private readonly PackedScene clusterJuggernautScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_juggernaut.tscn");
    private readonly PackedScene clusterSorcererScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_sorcerer.tscn");
    private readonly PackedScene clusterOccultistScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_occultist.tscn");
    private readonly PackedScene clusterPathfinderScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_pathfinder.tscn");
    private readonly PackedScene clusterAssassinScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/clusters/cluster_assassin.tscn");

    public EActorFlags PassiveTreeActorFlags { get; protected set; }
    public Dictionary<EStatName, double> PassiveTreeStatDictionary { get; protected set; } = new();

    private Label pointLabel;
    private Control[] activeClusterAttachmentNodes = new Control[3];
    private Label[] activeClusterLabel = new Label[3];

    private const int amountOfActiveClusters = 3;
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
        UpdatePointLabelText();

        activeClusterAttachmentNodes[0] = GetNode<Control>("MarginContainer/Clusters/ActiveCluster1");
        activeClusterAttachmentNodes[1] = GetNode<Control>("MarginContainer/Clusters/ActiveCluster2");
        activeClusterAttachmentNodes[2] = GetNode<Control>("MarginContainer/Clusters/ActiveCluster3");

        activeClusterLabel[0] = activeClusterAttachmentNodes[0].GetNode<Label>("Label");
        activeClusterLabel[1] = activeClusterAttachmentNodes[1].GetNode<Label>("Label");
        activeClusterLabel[2] = activeClusterAttachmentNodes[2].GetNode<Label>("Label");

        CreateClusters();
        GetRandomClusters();
    }

    private void CreateClusters() {
        SetupCluster(clusterJuggernautScene);
        SetupCluster(clusterSorcererScene);
        SetupCluster(clusterOccultistScene);
        SetupCluster(clusterPathfinderScene);
        SetupCluster(clusterAssassinScene);
    }

    private void SetupCluster(PackedScene clusterScene) {
        PTreeCluster newCluster = clusterScene.Instantiate<PTreeCluster>();
        clusterPool.Add(newCluster);
    }

    private void GetRandomClusters() {
        List<int> randomNumbers = Enumerable.Range(0, clusterPool.Count).OrderBy(x => Utilities.RNG.Next()).Take(amountOfActiveClusters).ToList();
        
        for (int i = 0; i < amountOfActiveClusters; i++) {
            //activeClusters.Add();
            PTreeCluster cluster = clusterPool[randomNumbers[i]];
            activeClusterAttachmentNodes[i].AddChild(cluster);

            activeClusterLabel[i].Text = cluster.ClusterName;

            if (cluster.FirstTime) {
                foreach (PTreeNode node in cluster.NodeList) {
                    node.Resize();
                    node.NodeClicked += OnNodeClicked;
                    node.NodeAllocated += OnNodeAllocated;
                }
                cluster.FirstTime = false;
            }
        }
    }

    private void RemoveActiveClusters() {
        for (int i = 0; i < amountOfActiveClusters; i++) {
            activeClusterAttachmentNodes[i].RemoveChild(activeClusterAttachmentNodes[i].GetChild(0));
        }
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
