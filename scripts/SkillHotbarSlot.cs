using Godot;
using System;

public partial class SkillHotbarSlot : Control {
    [Signal]
    public delegate void SkillSlotClickedEventHandler(SkillHotbarSlot slot);

    public Skill AssignedSkill { get; protected set; } = null;
    protected TextureRect skillTextureRect;

    public override void _Ready() {
        skillTextureRect = GetNode<TextureRect>("SkillTexture");
    }

    public ESkillName GetSkillName() {
        return AssignedSkill.SkillName;
    }

    public void AssignSkillToSlot(Skill skill) {
        AssignedSkill = skill;
        if (skill != null) {
            skillTextureRect.Texture = AssignedSkill.Texture;
        }
        else {
            skillTextureRect.Texture = null;
        }
    }

    public void GUIInput(InputEvent @event) {
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
            EmitSignal(SignalName.SkillSlotClicked, this);
        }
    }

    public bool TryUseSkill() {
        if (AssignedSkill != null) {
            UseSkill();
            return true;
        }

        return false;
    }

    protected void UseSkill() {
        AssignedSkill.UseSkill();
    }
}
