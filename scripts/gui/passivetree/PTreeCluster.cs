using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreeCluster : Control {
    public List<PTreeNode> NodeList { get; protected set; } = new();
    public bool FirstTime { get; set; } = true;

    protected TextureRect clusterCentre;

    public override void _Ready() {
        clusterCentre = GetNode<TextureRect>("ClusterCentre");

        foreach (Node child in GetChildren()) {
            if (child.IsClass("TextureButton")) {
                PTreeNode treeNode = child as PTreeNode;
                NodeList.Add(treeNode);
                treeNode.NodeAllocated += OnNodeAllocated;

                if (treeNode.ConnectsToCentre) {
                    Vector2 startPos = clusterCentre.Position + clusterCentre.Size / 2;
                    Vector2 endPos = treeNode.Position + treeNode.Size / 2;
                    CreateConnectorLine(startPos, endPos);
                }

                foreach (PTreeNode followingNode in treeNode.NodesNext) {
                    Vector2 startPos = treeNode.Position + treeNode.Size / 2;
                    Vector2 endPos = followingNode.Position + followingNode.Size / 2;
                    CreateConnectorLine(startPos, endPos);
                }
            }
        }
    }

    protected void CreateConnectorLine(Vector2 start, Vector2 end) {
        Line2D newLine = new Line2D();
        //newLine.JointMode = Line2D.LineJointMode.Round;
        newLine.Width = 3f;
        newLine.AddPoint(start);
        newLine.AddPoint(end);
        newLine.ZIndex = -1;

        AddChild(newLine);
    }

    public virtual void OnNodeAllocated(PTreeNode node) {
        foreach (PTreeNode tnode in node.NodesNext) {
            if (tnode.IsLocked) {
                tnode.IsLocked = false;
            }
        }
    }
}
