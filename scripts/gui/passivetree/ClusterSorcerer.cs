using Godot;

public partial class ClusterSorcerer : PTreeCluster {
    private static readonly PackedScene arcaneBuffController = GD.Load<PackedScene>("res://scenes/controllers/sorcerer_arcane_surge_controller.tscn");

    public override void OnNodeAllocated(PTreeNode node) {
        base.OnNodeAllocated(node);

        if (node.ActorFlag.HasFlag(EActorFlags.GainArcaneSurgeOnManaSpent)) {
            SorcererArcaneSurgeController asController = arcaneBuffController.Instantiate<SorcererArcaneSurgeController>();
            Run.Instance.PlayerActor.AddChild(asController);
            asController.SetPlayerOwner(Run.Instance.PlayerActor);
        }
    }
}
