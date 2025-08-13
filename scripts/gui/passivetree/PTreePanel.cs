using Godot;
using System;
using System.Collections.Generic;

public partial class PTreePanel : PanelContainer {
    [Signal]
    public delegate void PassiveTreeChangedEventHandler();

    private Label pointLabel;
    public PTreeCluster ClusterBurn { get; protected set; }

    private int passiveTreePoints = 1;
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

        ClusterBurn = GetNode<PTreeCluster>("MarginContainer/Control/ClusterBurn");
        foreach (PTreeNode node in ClusterBurn.NodeArray) {
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
        if (node.NodeFirstStatName != EStatName.None) {
            PassiveTreeStatDictionary[node.NodeFirstStatName] += node.NodeFirstStatValue;
            //PassiveTreeStatDictionary.TryAdd(node.NodeFirstStatName, node.NodeFirstStatValue);
            GD.Print($"Added {node.NodeFirstStatValue} {node.NodeFirstStatName}");
        }
        if (node.NodeSecondStatName != EStatName.None) {
            PassiveTreeStatDictionary[node.NodeSecondStatName] += node.NodeSecondStatValue;
            //PassiveTreeStatDictionary.TryAdd(node.NodeSecondStatName, node.NodeSecondStatValue);
            GD.Print($"Added {node.NodeSecondStatValue} {node.NodeSecondStatName}");
        }
        if (node.NodeThirdStatName != EStatName.None) {
            PassiveTreeStatDictionary[node.NodeThirdStatName] += node.NodeThirdStatValue;
            //PassiveTreeStatDictionary.TryAdd(node.NodeThirdStatName, node.NodeThirdStatValue);
            GD.Print($"Added {node.NodeThirdStatValue} {node.NodeThirdStatName}");
        }

        EmitSignal(SignalName.PassiveTreeChanged);
    }

    public Dictionary<EStatName, double> PassiveTreeStatDictionary { get; protected set; } = new() {
		{ EStatName.FlatStrength, 					0 },
		{ EStatName.FlatDexterity, 					0 },
		{ EStatName.FlatIntelligence, 				0 },

		{ EStatName.FlatMaxLife, 					0 },
		{ EStatName.IncreasedMaxLife, 				0 },
		{ EStatName.AddedLifeRegen, 				0 },
		{ EStatName.PercentageLifeRegen, 			0 },

		{ EStatName.FlatMaxMana, 					0 },
		{ EStatName.IncreasedMaxMana, 				0 },
		{ EStatName.AddedManaRegen, 				0 },
		{ EStatName.IncreasedManaRegen, 			0 },

		{ EStatName.FlatMinPhysDamage, 				0 },
		{ EStatName.FlatMaxPhysDamage, 				0 },
        { EStatName.FlatAttackMinPhysDamage, 		0 },
		{ EStatName.FlatAttackMaxPhysDamage, 		0 },
        { EStatName.FlatSpellMinPhysDamage, 		0 },
		{ EStatName.FlatSpellMaxPhysDamage, 		0 },
		{ EStatName.IncreasedPhysDamage, 			0 },

		{ EStatName.FlatMinFireDamage, 				0 },
		{ EStatName.FlatMaxFireDamage, 				0 },
        { EStatName.FlatAttackMinFireDamage, 		0 },
		{ EStatName.FlatAttackMaxFireDamage, 		0 },
        { EStatName.FlatSpellMinFireDamage, 		0 },
		{ EStatName.FlatSpellMaxFireDamage, 		0 },
		{ EStatName.IncreasedFireDamage, 			0 },

		{ EStatName.FlatMinColdDamage, 				0 },
		{ EStatName.FlatMaxColdDamage, 				0 },
        { EStatName.FlatAttackMinColdDamage, 		0 },
		{ EStatName.FlatAttackMaxColdDamage, 		0 },
        { EStatName.FlatSpellMinColdDamage, 		0 },
		{ EStatName.FlatSpellMaxColdDamage, 		0 },
		{ EStatName.IncreasedColdDamage, 			0 },

		{ EStatName.FlatMinLightningDamage, 		0 },
		{ EStatName.FlatMaxLightningDamage, 		0 },
        { EStatName.FlatAttackMinLightningDamage, 	0 },
		{ EStatName.FlatAttackMaxLightningDamage, 	0 },
        { EStatName.FlatSpellMinLightningDamage, 	0 },
		{ EStatName.FlatSpellMaxLightningDamage, 	0 },
		{ EStatName.IncreasedLightningDamage, 		0 },

		{ EStatName.FlatMinChaosDamage, 			0 },
		{ EStatName.FlatMaxChaosDamage, 			0 },
        { EStatName.FlatAttackMinChaosDamage, 		0 },
		{ EStatName.FlatAttackMaxChaosDamage, 		0 },
        { EStatName.FlatSpellMinChaosDamage, 		0 },
		{ EStatName.FlatSpellMaxChaosDamage, 		0 },
		{ EStatName.IncreasedChaosDamage, 			0 },

        { EStatName.IncreasedAttackDamage, 			0 },
        { EStatName.IncreasedSpellDamage, 			0 },
        { EStatName.IncreasedMeleeDamage, 			0 },
		{ EStatName.IncreasedProjectileDamage, 		0 },
		{ EStatName.IncreasedAreaDamage, 			0 },
        { EStatName.IncreasedDamageOverTime, 		0 },

		{ EStatName.IncreasedAttackSpeed, 			0 },
		{ EStatName.IncreasedCastSpeed, 			0 },
		{ EStatName.IncreasedCritChance, 			0 },
		{ EStatName.AddedCritMulti, 				0 },

		{ EStatName.IncreasedMovementSpeed, 		0 },
		{ EStatName.BlockChance, 					0 },

		{ EStatName.FlatArmour, 					0 },
		{ EStatName.IncreasedArmour, 				0 },
		{ EStatName.FlatEvasion, 					0 },
		{ EStatName.IncreasedEvasion, 				0 },
		{ EStatName.FlatEnergyShield, 				0 },
		{ EStatName.IncreasedEnergyShield, 			0 },

		{ EStatName.PhysicalResistance, 			0 },
		{ EStatName.FireResistance, 				0 },
		{ EStatName.ColdResistance, 				0 },
		{ EStatName.LightningResistance, 			0 },
		{ EStatName.ChaosResistance, 				0 },
	};
}
