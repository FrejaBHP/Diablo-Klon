using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PTreeCluster : Control {
    [Export]
    public string ClusterName { get; protected set; }

    public List<PTreeNode> NodeList { get; protected set; } = new();
    public bool FirstTime { get; set; } = true;

    protected TextureRect clusterCentre;
    protected Control nodes;
    protected Control lines;

    public override void _Ready() {
        clusterCentre = GetNode<TextureRect>("ClusterCentre");
        nodes = GetNode<Control>("Nodes");
        lines = GetNode<Control>("Lines");

        foreach (Node child in nodes.GetChildren()) {
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

    protected void CreateConnectorLine(Vector2 start, Vector2 end) {
        Line2D newLine = new Line2D();
        //newLine.JointMode = Line2D.LineJointMode.Round;
        newLine.Width = 3f;
        newLine.AddPoint(start);
        newLine.AddPoint(end);
        //newLine.ZIndex = -1;
        //newLine.DefaultColor = Color.Color8(255, 200, 15);
        newLine.DefaultColor = Color.Color8(100, 100, 100);

        lines.AddChild(newLine);
    }

    public virtual void OnNodeAllocated(PTreeNode node) {
        foreach (PTreeNode tnode in node.NodesNext) {
            if (tnode.IsLocked) {
                tnode.IsLocked = false;
            }
        }

        ExperimentalFillLine(node.Position + node.Size / 2);
    }

    public void ExperimentalFillLine(Vector2 endPos) {
        foreach (Line2D line in lines.GetChildren().Cast<Line2D>()) {
            if (line.Points.Last() == endPos) {
                line.DefaultColor = Color.Color8(255, 200, 15);
            }
        }
    }
}
