using Godot;
using System;

public partial class CharacterPanel : Control {
    public Player PlayerOwner;
    public bool IsOpen = false;

    public OffencePanel OffenceTabPanel;
    public DefencePanel DefenceTabPanel;
    public Label CharacterNameLabel;
    public Label CharacterLevelLabel;
    public StatTableEntry LifeContainer;
    public StatTableEntry ManaContainer;
    public StatTableEntry StrContainer;
    public StatTableEntry DexContainer;
    public StatTableEntry IntContainer;
    private PanelContainer characterPlateContainer;
    private TabContainer statTabContainer;

    public override void _Ready() {
        characterPlateContainer = GetNode<PanelContainer>("VBoxContainer/CharacterPlateContainer");
        CharacterNameLabel = characterPlateContainer.GetNode<Label>("MarginContainer/PlateVContainer/NamePlateContainer/Name");
        CharacterLevelLabel = characterPlateContainer.GetNode<Label>("MarginContainer/PlateVContainer/NamePlateContainer/Level");

        LifeContainer = characterPlateContainer.GetNode<StatTableEntry>("MarginContainer/PlateVContainer/StatsContainer/ResourceContainer/LifeLabelsContainer");
        ManaContainer = characterPlateContainer.GetNode<StatTableEntry>("MarginContainer/PlateVContainer/StatsContainer/ResourceContainer/ManaLabelsContainer");

        StrContainer = characterPlateContainer.GetNode<StatTableEntry>("MarginContainer/PlateVContainer/StatsContainer/AttributeContainer/StrLabelsContainer");
        DexContainer = characterPlateContainer.GetNode<StatTableEntry>("MarginContainer/PlateVContainer/StatsContainer/AttributeContainer/DexLabelsContainer");
        IntContainer = characterPlateContainer.GetNode<StatTableEntry>("MarginContainer/PlateVContainer/StatsContainer/AttributeContainer/IntLabelsContainer");

        statTabContainer = GetNode<TabContainer>("VBoxContainer/TabContainer");
        OffenceTabPanel = statTabContainer.GetNode<OffencePanel>("Offence");
        DefenceTabPanel = statTabContainer.GetNode<DefencePanel>("Defence");

        LifeContainer.SetDescription("Life");
        ManaContainer.SetDescription("Mana");
        StrContainer.SetDescription("Strength");
        DexContainer.SetDescription("Dexterity");
        IntContainer.SetDescription("Intelligence");

        // Placeholder
        CharacterNameLabel.Text = "Unnamed";
        CharacterLevelLabel.Text = "Level 0 Creature";
        StrContainer.SetValue("0");
        DexContainer.SetValue("0");
        IntContainer.SetValue("0");
    }

    public void TogglePanel() {
        if (!IsOpen) {
            Visible = true;
            IsOpen = true;
        }
        else {
            Visible = false;
            IsOpen = false;
        }
    }

    public void UpdateMainHand() {

    }
}
