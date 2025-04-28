using Godot;
using System;

public partial class EnemyBase : Actor {
    private Marker3D resBarAnchor;

    public override void _Ready() {
        base._Ready();

        resBarAnchor = GetNode<Marker3D>("ResBarAnchor");
        AddFloatingBars(resBarAnchor);

        AddToGroup("Enemy");

        DamageTaken += ShowDamageText;
    }

    protected void ShowDamageText(double damage) {
        Vector3 attachedPosition = resBarAnchor.GlobalPosition;
        attachedPosition.Y += 0.25f;

        DamageText damageLabel = Utilities.CreateDamageNumber();
        GetTree().Root.GetChild(0).AddChild(damageLabel);

        damageLabel.Text = $"{Math.Round(damage, 0)}";
		damageLabel.GlobalPosition = attachedPosition;

        damageLabel.Start();
    }
}
