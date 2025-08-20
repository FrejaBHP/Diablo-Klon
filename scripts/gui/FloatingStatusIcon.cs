using Godot;
using System;

public partial class FloatingStatusIcon : TextureRect {
    public EEffectName StatusName { get; protected set; }

    public void SetIconAndStatusType(Texture2D texture, EEffectName effectName) {
        Texture = texture;
        StatusName = effectName;
    }
}
