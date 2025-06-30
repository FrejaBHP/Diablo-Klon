using Godot;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

// Run instansen skal sepereres fra Game når der bliver introduceret en hovedmenu, og i stedet instantieres derfra

public partial class Game : Node {
	public static Game Instance { get; private set; }

    public override void _EnterTree() {
		// Changes percentage formatting (<num>:P<dec>) to "x%" instead of "x %"
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
		CultureInfo ci = new(cultureName);
		ci.NumberFormat.PercentPositivePattern = 1;
		ci.NumberFormat.PercentNegativePattern = 1;
		Thread.CurrentThread.CurrentCulture = ci;
    }

	public override void _Ready() {
		Instance = this; // There should possibly only ever be 1 Game instance at any time, so if this somehow results in overrides, lord have mercy
	}
}
