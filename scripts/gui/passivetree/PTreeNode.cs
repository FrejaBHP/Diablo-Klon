using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class PTreeNode : TextureButton {
    [Signal]
    public delegate void NodeClickedEventHandler(PTreeNode node);

    [Signal]
    public delegate void NodeAllocatedEventHandler(PTreeNode node);

    public enum ETreeNodeType {
        Normal = 0,
        Notable = 1,
        Keystone = 2
    }

    protected static Dictionary<ETreeNodeType, float> sizeDictionary = new() {
        { ETreeNodeType.Normal, 48f },
        { ETreeNodeType.Notable, 64f },
        { ETreeNodeType.Keystone, 80f }
    };

    protected TextureRect OutlineTexture;

    [Export]
    public EStatName NodeFirstStatName { get; protected set; }
    [Export]
    public double NodeFirstStatValue { get; protected set; }

    [Export]
    public EStatName NodeSecondStatName { get; protected set; }
    [Export]
    public double NodeSecondStatValue { get; protected set; }

    [Export]
    public EStatName NodeThirdStatName { get; protected set; }
    [Export]
    public double NodeThirdStatValue { get; protected set; }

    protected ETreeNodeType treeNodeType = ETreeNodeType.Normal;
    [Export]
    public ETreeNodeType TreeNodeType { 
        get => treeNodeType;
        protected set {
            treeNodeType = value;
            CallDeferred(MethodName.Resize);
        }
    }

    [Export]
    public string NodeName { get; protected set; }

    [Export]
    public string NodeDescription { get; protected set; }

    public bool IsLocked { get; protected set; } = false;
    public bool IsAllocated { get; protected set; } = false;

    public override void _Ready() {
        OutlineTexture = GetNode<TextureRect>("OutlineTexture");
    }

    protected void Resize() {
        float fNewSize = sizeDictionary[treeNodeType];
        Vector2 vNewSize = new(fNewSize, fNewSize);
        Size = vNewSize;

        if (Engine.IsEditorHint()) {
            TextureRect outlineTexture = GetChild<TextureRect>(0);
            outlineTexture.Size = vNewSize;
        }
        else {
            OutlineTexture.Size = vNewSize;
        }
        
    }

    public void Lock() {
        Disabled = true;
        IsLocked = true;
    }

    public void Unlock() {
        Disabled = false;
        IsLocked = false;
    }

    public void Allocate() {
        IsAllocated = true;
        EmitSignal(SignalName.NodeAllocated, this);
        OutlineTexture.Texture = UILib.PTreeNodeActive;
    }

    public void OnPressed() {
        if (!IsAllocated) {
            EmitSignal(SignalName.NodeClicked, this);
        }
    }

    public override string _GetTooltip(Vector2 atPosition) {
        return $"{NodeName}\n{NodeDescription}";
    }
}
