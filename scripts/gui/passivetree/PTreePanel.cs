using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreePanel : PanelContainer {
    [Signal]
    public delegate void PassiveTreeChangedEventHandler();

    private Label pointLabel;
    public PTreeCluster ClusterFire { get; protected set; }

    private int passiveTreePoints = 10;
    public int PassiveTreePoints { 
        get => passiveTreePoints;
        set {
            passiveTreePoints = value;
            UpdatePointLabelText();
        }
    }

    public override void _Ready() {
        pointLabel = GetNode<Label>("MarginContainer/PointLabel");
        UpdatePointLabelText();

        ClusterFire = GetNode<PTreeCluster>("MarginContainer/Control/ClusterFire");
        foreach (PTreeNode node in ClusterFire.NodeArray) {
            node.NodeClicked += OnNodeClicked;
            node.NodeAllocated += OnNodeAllocated;
        }
    }

    private void UpdatePointLabelText() {
        pointLabel.Text = $"Passive Points: {passiveTreePoints}";
    }

    private void OnNodeClicked(PTreeNode node) {
        if (passiveTreePoints > 0) {
            node.Allocate();
        }

        PassiveTreePoints--;
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

        EmitSignal(SignalName.PassiveTreeChanged);
    }

    public Dictionary<EStatName, double> PassiveTreeStatDictionary { get; protected set; } = new();
}
