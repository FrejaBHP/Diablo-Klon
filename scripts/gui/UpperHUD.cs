using Godot;
using System;

public partial class UpperHUD : Control {
    public Label ObjTimeLabel;

    public override void _Ready() {
        ObjTimeLabel = GetNode<Label>("ObjTimeLabel");
    }
}
