using Godot;
using System;

public partial class SkillAssignable : TextureRect {
    public delegate void SkillSelectedEventHandler(Skill skill);
    public event SkillSelectedEventHandler SkillSelected;

    private Skill assignableSkill;

    public void SetAssignableSkill(Skill skill) {
        if (skill != null) {
            assignableSkill = skill;
            Texture = skill.Texture;
        }
        else {
            assignableSkill = null;
            Texture = UILib.TextureRemoveSkill;
        }
    }

    public void GUIInput(InputEvent @event) {
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed) {
            GetViewport().SetInputAsHandled();
            SkillSelected?.Invoke(assignableSkill);
        }
    }
}
