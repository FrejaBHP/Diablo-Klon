using Godot;
using System;

public partial class PTreeClassSelectionBox : PanelContainer {
    [Signal]
    public delegate void ClassSelectedEventHandler(PTreeClassSelectionBox classSelectionBox);

    public VBoxContainer ElementsContainer { get; protected set; }
    public RichTextLabel ClassNameLabel { get; protected set; }
    public RichTextLabel ClassDescriptionLabel { get; protected set; }
    public Button SelectionButton { get; protected set; }
    private Control placeholderControl;

    public PTreeCluster Cluster { get; protected set; }
    public EPlayerClass ClassEnum { get; set; }

    public override void _Ready() {
        ElementsContainer = GetNode<VBoxContainer>("MarginContainer/ElementsVBoxContainer");
        ClassNameLabel = ElementsContainer.GetNode<RichTextLabel>("TextVBoxContainer/ClassTitle");
        ClassDescriptionLabel = ElementsContainer.GetNode<RichTextLabel>("TextVBoxContainer/ClassDescription");
        placeholderControl = ElementsContainer.GetNode<Control>("Placeholder");
        SelectionButton = ElementsContainer.GetNode<Button>("SelectionButton");
    }

    public void AssignCluster(PTreeCluster newCluster) {
        Cluster = newCluster;

        placeholderControl.ReplaceBy(Cluster);
        placeholderControl.QueueFree();
        //ElementsContainer.AddChild(Cluster);
    }

    public void OnSelectionButtonPressed() {
        EmitSignal(SignalName.ClassSelected, this);
    }
}
