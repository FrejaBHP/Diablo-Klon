using Godot;
using System;

public partial class PTreeCluster : Control {
    public PTreeNode NodeRoot { get; protected set; }
    public PTreeNode NodeBranchLeft { get; protected set; }
    public PTreeNode NodeBranchRight { get; protected set; }
    public PTreeNode NodeLeftSplitLeft { get; protected set; }
    public PTreeNode NodeLeftSplitRight { get; protected set; }
    public PTreeNode NodeRightSplitLeft { get; protected set; }
    public PTreeNode NodeRightSplitRight { get; protected set; }
    public PTreeNode NodeNotableLeft { get; protected set; }
    public PTreeNode NodeNotableRight { get; protected set; }
    public PTreeNode NodeKeystone { get; protected set; }

    public PTreeNode[] NodeArray { get; protected set; }

    public override void _Ready() {
        NodeRoot = GetNode<PTreeNode>("PTreeNodeRoot");
        NodeBranchLeft = GetNode<PTreeNode>("PTreeNodeBranch1");
        NodeBranchRight = GetNode<PTreeNode>("PTreeNodeBranch2");
        NodeLeftSplitLeft = GetNode<PTreeNode>("PTreeNodeSplit1Node1");
        NodeLeftSplitRight = GetNode<PTreeNode>("PTreeNodeSplit1Node2");
        NodeRightSplitLeft = GetNode<PTreeNode>("PTreeNodeSplit2Node1");
        NodeRightSplitRight = GetNode<PTreeNode>("PTreeNodeSplit2Node2");
        NodeNotableLeft = GetNode<PTreeNode>("PTreeNodeNotable1");
        NodeNotableRight = GetNode<PTreeNode>("PTreeNodeNotable2");
        NodeKeystone = GetNode<PTreeNode>("PTreeNodeKeystone");

        NodeArray = [
            NodeRoot,
            NodeBranchLeft,
            NodeBranchRight,
            NodeLeftSplitLeft,
            NodeLeftSplitRight,
            NodeRightSplitLeft,
            NodeRightSplitRight,
            NodeNotableLeft,
            NodeNotableRight,
            NodeKeystone
        ];

        for (int i = 0; i < NodeArray.Length; i++) {
            if (i != 0) {
                NodeArray[i].Lock();
            }
            NodeArray[i].NodeAllocated += OnNodeAllocated;
        }
    }

    public void OnNodeAllocated(PTreeNode node) {

    }
}
