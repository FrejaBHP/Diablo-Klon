using Godot;
using System;

public partial class DefencePanel : StatPanel {
    public StatTableEntry MovementSpeed;
    public StatTableEntry LifeRegen;
    public StatTableEntry ManaRegen;
    public StatTableEntry Armour;
    public StatTableEntry Evasion;
    public StatTableEntry BlockChance;
    public StatTableEntry BlockEffectiveness;
    public StatTableEntry PhysRes;
    public StatTableEntry FireRes;
    public StatTableEntry ColdRes;
    public StatTableEntry LightningRes;
    public StatTableEntry ChaosRes;

    public override void _Ready() {
        base._Ready();
        BuildTable();
        ApplyAlternatingBackground();
    }

    private void BuildTable() {
        AddEntry(ref MovementSpeed, "Increased Movement Speed");
        AddEntry(ref LifeRegen, "Life Regeneration per Second");
        AddEntry(ref ManaRegen, "Mana Regeneration per Second");
        AddEntry(ref Armour, "Armour");
        AddEntry(ref Evasion, "Evasion");
        AddEntry(ref BlockChance, "Block Chance");
        AddEntry(ref BlockEffectiveness, "Block Effectiveness");
        AddEntry(ref PhysRes, "Physical Resistance");
        AddEntry(ref FireRes, "Fire Resistance");
        AddEntry(ref ColdRes, "Cold Resistance");
        AddEntry(ref LightningRes, "Lightning Resistance");
        AddEntry(ref ChaosRes, "Chaos Resistance");
    }
}
