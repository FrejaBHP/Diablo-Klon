using Godot;
using System;

public partial class EnemyBase : Actor {
    private Marker3D resBarAnchor;

    public override void _Ready() {
        base._Ready();

        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        AddFloatingBars(resBarAnchor);

        AddToGroup("Enemy");
    }

    public override void OnDamageTaken(double damage, bool isCritical, bool createDamageText) {
        if (createDamageText) {
            ShowDamageText(damage, isCritical);
        }
    }

    public override void OnDamageEvaded() {
        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber("Evaded!");
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }

    protected void ShowDamageText(double damage, bool isCritical) {
        string labelText;
        if (isCritical) {
            labelText = $"{Math.Round(damage, 0)}!";
        }
        else {
            labelText = $"{Math.Round(damage, 0)}";
        }

        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber(labelText);
        GetTree().Root.GetChild(0).AddChild(damageLabel);
        damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }
}
