using Godot;
using System;

public partial class StatTableEntry : PanelContainer {
    private string description = "";
    private string value = "";

    private Label descriptionLabel;
    private Label valueLabel;

    public override void _Ready() {
        descriptionLabel = GetNode<Label>("HBoxContainer/Description");
        valueLabel = GetNode<Label>("HBoxContainer/Value");
    }

    public void SetDescription(string newDesc) {
        description = newDesc;
        descriptionLabel.Text = description;
    }

    public void SetValue(string newValue) {
        value = newValue;
        valueLabel.Text = value;
    }
}
