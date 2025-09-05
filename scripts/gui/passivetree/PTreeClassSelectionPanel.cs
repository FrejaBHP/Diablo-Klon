using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreeClassSelectionPanel : PanelContainer {
    private static readonly PackedScene selectionBoxScene = GD.Load<PackedScene>("res://scenes/gui/passivetree/ptree_class_selection_box.tscn");

    [Signal]
    public delegate void ClassSelectedEventHandler(PTreeClassSelectionBox classSelectionBox);

    public Label ClassSelectionLabel { get; protected set; }
    public HBoxContainer ClassSelectionBoxContainer { get; protected set; }

    private List<ClassInfo> classList = new() {
        new(EPlayerClass.Occultist, "Occultist", "Specialises in Chaos Damage and Damage over Time", "res://scenes/gui/passivetree/clusters/cluster_occultist.tscn"),
        new(EPlayerClass.Juggernaut, "Juggernaut", "Specialises in Armour and Shields", "res://scenes/gui/passivetree/clusters/cluster_juggernaut.tscn"),
        new(EPlayerClass.Assassin, "Assassin", "Specialises in Critical Strikes", "res://scenes/gui/passivetree/clusters/cluster_assassin.tscn"),
        new(EPlayerClass.Sorcerer, "Sorcerer", "Specialises in Spells and Mana-related things", "res://scenes/gui/passivetree/clusters/cluster_sorcerer.tscn"),
        new(EPlayerClass.Pathfinder, "Pathfinder", "Specialises in Attacks, Speed and Evasion", "res://scenes/gui/passivetree/clusters/cluster_pathfinder.tscn"),
    };

    private const int amountOfShownClasses = 3;

    public override void _Ready() {
        ClassSelectionLabel = GetNode<Label>("MarginContainer/ClassSelectionLabel");
        ClassSelectionBoxContainer = GetNode<HBoxContainer>("MarginContainer/MarginContainer/ClassSelectionBoxContainer");

        SelectAndPresentClassChoices(GetLegalClassList());
    }

    private List<ClassInfo> GetLegalClassList() {
        return classList.Where(c => !Run.Instance.PlayerActor.PlayerClasses.Contains(c.ClassEnum)).ToList();
    }

    private void SelectAndPresentClassChoices(List<ClassInfo> legalClassInfo) {
        List<int> randomNumbers = Enumerable.Range(0, legalClassInfo.Count).OrderBy(x => Utilities.RNG.Next()).Take(amountOfShownClasses).ToList();

        for (int i = 0; i < amountOfShownClasses; i++) {
            ClassInfo pickedClassInfo = legalClassInfo[randomNumbers[i]];

            PTreeClassSelectionBox cBox = selectionBoxScene.Instantiate<PTreeClassSelectionBox>();
            ClassSelectionBoxContainer.AddChild(cBox);
            cBox.ClassEnum = pickedClassInfo.ClassEnum;
            
            cBox.ClassNameLabel.AppendText($"[center]{pickedClassInfo.ClassName}[/center]");
            cBox.ClassDescriptionLabel.AppendText($"[center]{pickedClassInfo.ClassDescription}[/center]");

            PTreeCluster cluster = GD.Load<PackedScene>(pickedClassInfo.ClusterPath).Instantiate<PTreeCluster>();
            cBox.AssignCluster(cluster);

            foreach (PTreeNode node in cluster.NodeList) {
                node.Resize();
            }
            
            cBox.ClassSelected += PlayerClassSelected;
        }
    }

    public void PlayerClassSelected(PTreeClassSelectionBox selectionBox) {
        EmitSignal(SignalName.ClassSelected, selectionBox);
    }
}

public class ClassInfo(EPlayerClass classEnum, string className, string classDescription, string clusterPath) {
    public EPlayerClass ClassEnum { get; private set; } = classEnum;
    public string ClassName { get; private set; } = className;
    public string ClassDescription { get; private set; } = classDescription;
    public string ClusterPath { get; private set; } = clusterPath;
}
