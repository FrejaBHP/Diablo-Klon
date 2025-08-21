using Godot;
using System;
using System.Linq;

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

    public bool FirstTime { get; set; } = true;

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

        NodeRoot.SetIdentifier(000);
        NodeBranchLeft.SetIdentifier(100);
        NodeBranchRight.SetIdentifier(110);
        NodeLeftSplitLeft.SetIdentifier(200);
        NodeLeftSplitRight.SetIdentifier(201);
        NodeRightSplitLeft.SetIdentifier(210);
        NodeRightSplitRight.SetIdentifier(211);
        NodeNotableLeft.SetIdentifier(300);
        NodeNotableRight.SetIdentifier(310);
        NodeKeystone.SetIdentifier(400);
    }

    public void OnNodeAllocated(PTreeNode node) {
        PTreeNode[] nodes;

        switch (node.Identifier) {
            case 000:
                nodes = NodeArray.Where(x => x.Identifier == 100 || x.Identifier == 110).ToArray();
                break;
            
            case 100:
                nodes = NodeArray.Where(x => x.Identifier == 200 || x.Identifier == 201).ToArray();
                break;

            case 110:
                nodes = NodeArray.Where(x => x.Identifier == 210 || x.Identifier == 211).ToArray();
                break;

            case 200 or 201:
                nodes = NodeArray.Where(x => x.Identifier == 300).ToArray();
                break;

            case 210 or 211:
                nodes = NodeArray.Where(x => x.Identifier == 310).ToArray();
                break;

            case 300 or 310:
                nodes = NodeArray.Where(x => x.Identifier == 400).ToArray();
                break;

            default:
                if (node.Identifier != 400) {
                    GD.PrintErr("Unknown identifier");
                }

                nodes = [];
                break;
        }

        foreach (PTreeNode tnode in nodes) {
            if (tnode.IsLocked) {
                tnode.Unlock();
            }
        }
    }
}
