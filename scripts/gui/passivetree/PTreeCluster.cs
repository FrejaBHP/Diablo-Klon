using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreeCluster : Control {
    public List<PTreeNode> NodeList { get; protected set; } = new();

    public bool FirstTime { get; set; } = true;

    public override void _Ready() {
        foreach (Node child in GetChildren()) {
            if (child.IsClass("TextureButton")) {
                PTreeNode treeNode = child as PTreeNode;
                NodeList.Add(treeNode);
            }
        }

        for (int i = 0; i < NodeList.Count; i++) {
            NodeList[i].NodeAllocated += OnNodeAllocated;
        }
    }

    public void OnNodeAllocated(PTreeNode node) {
        foreach (PTreeNode tnode in node.NodesNext) {
            if (tnode.IsLocked) {
                tnode.IsLocked = false;
            }
        }
    }
}
