using Godot;
using System;

public partial class SkillHotbar : Control {
    protected readonly PackedScene skillAssignableScene = GD.Load<PackedScene>("res://scenes/gui/hud_skill_assignable.tscn");

    public Player PlayerOwner;

    protected GridContainer skillAssignmentContainer;
    public SkillHotbarSlot SkillHotbarSlot1 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot2 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot3 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot4 { get; protected set; }
    
    protected bool isSkillBeingSelected = false;
    protected SkillHotbarSlot selectedSlot;
    
    public override void _Ready() {
        skillAssignmentContainer = GetNode<GridContainer>("SkillAssignmentGrid");
        SkillHotbarSlot1 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot1");
        SkillHotbarSlot2 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot2");
        SkillHotbarSlot3 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot3");
        SkillHotbarSlot4 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot4");

        SkillHotbarSlot1.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot2.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot3.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot4.SkillSlotClicked += SkillSlotClicked;
    }

    public void ClearInvalidSkills(ESkillName skillName) {
        if (SkillHotbarSlot1.AssignedSkill != null && SkillHotbarSlot1.GetSkillName() == skillName) {
            SkillHotbarSlot1.AssignSkillToSlot(null);
        }

        if (SkillHotbarSlot2.AssignedSkill != null && SkillHotbarSlot2.GetSkillName() == skillName) {
            SkillHotbarSlot2.AssignSkillToSlot(null);
        }

        if (SkillHotbarSlot3.AssignedSkill != null && SkillHotbarSlot3.GetSkillName() == skillName) {
            SkillHotbarSlot3.AssignSkillToSlot(null);
        }

        if (SkillHotbarSlot4.AssignedSkill != null && SkillHotbarSlot4.GetSkillName() == skillName) {
            SkillHotbarSlot4.AssignSkillToSlot(null);
        }
    }

    public void SkillSlotClicked(SkillHotbarSlot slot) {
        isSkillBeingSelected = true;
        selectedSlot = slot;
        BuildSkillAssignmentMenu();
    }

    public void AssignableSkillSelected(Skill skill) {
        if (selectedSlot != null) {
            selectedSlot.AssignSkillToSlot(skill);
            DestroySkillAssignmentMenu();
        }
    }

    public void BuildSkillAssignmentMenu() {
        if (skillAssignmentContainer.GetChildCount() > 0) {
            foreach (var item in skillAssignmentContainer.GetChildren()) {
                skillAssignmentContainer.RemoveChild(item);
                item.QueueFree();
            }
        }
        
        foreach (Skill skill in PlayerOwner.Skills) {
            SkillAssignable assignableSkill = skillAssignableScene.Instantiate<SkillAssignable>();
            assignableSkill.SetAssignableSkill(skill);

            assignableSkill.SkillSelected += AssignableSkillSelected;

            skillAssignmentContainer.AddChild(assignableSkill);
        }
    }

    protected void DestroySkillAssignmentMenu() {
        isSkillBeingSelected = false;
        selectedSlot = null;

        foreach (var item in skillAssignmentContainer.GetChildren()) {
            skillAssignmentContainer.RemoveChild(item);
            item.QueueFree();
        }
    }
}
