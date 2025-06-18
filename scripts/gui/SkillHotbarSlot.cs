using Godot;
using System;

public partial class SkillHotbarSlot : Control {
    [Signal]
    public delegate void SkillSlotClickedEventHandler(SkillHotbarSlot slot);

    [Signal]
    public delegate void SkillSlotEnteredEventHandler(SkillHotbarSlot slot);

    [Signal]
    public delegate void SkillSlotExitedEventHandler(SkillHotbarSlot slot);

    public Skill AssignedSkill { get; protected set; } = null;

    protected Label skillHotkeyHint;
    protected TextureRect skillTextureRect;

    protected bool isHovered = false;

    public override void _Ready() {
        skillHotkeyHint = GetNode<Label>("SkillHotbarHint");
        skillTextureRect = GetNode<TextureRect>("SkillHotbarContainer/SkillTexture");
    }

    public ESkillName GetSkillName() {
        return AssignedSkill.SkillName;
    }

    public void UpdateHint(string newHint) {
        skillHotkeyHint.Text = newHint;
    }

    public void AssignSkillToSlot(Skill skill) {
        AssignedSkill = skill;
        
        if (skill != null) {
            skillTextureRect.Texture = AssignedSkill.Texture;
            AssignedSkill.RecalculateSkillValues();
        }
        else {
            skillTextureRect.Texture = UILib.TextureSkillNONE;
        }
    }

    public void GUIInput(InputEvent @event) {
        if (@event.IsActionPressed("LeftClick")) {
            EmitSignal(SignalName.SkillSlotClicked, this);
        }
    }

    public void OnMouseEntered() {
        if (AssignedSkill != null) {
            EmitSignal(SignalName.SkillSlotEntered, this);
        }
    }

    public void OnMouseExited() {
        if (AssignedSkill != null) {
            EmitSignal(SignalName.SkillSlotExited, this);
        }
    }

    public bool TryUseSkill() {
        if (AssignedSkill != null && AssignedSkill.CanUseSkill()) {
            UseSkill();
            return true;
        }
        return false;
    }

    protected void UseSkill() {
        AssignedSkill.UseSkill();
    }
}
