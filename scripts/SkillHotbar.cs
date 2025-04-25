using Godot;
using System;

public partial class SkillHotbar : Control {
    protected readonly PackedScene skillAssignableScene = GD.Load<PackedScene>("res://scenes/gui/hud_skill_assignable.tscn");
    protected readonly PackedScene skillTooltipScene = GD.Load<PackedScene>("res://scenes/gui/hud_skill_tooltip.tscn");

    public Player PlayerOwner;

    protected VBoxContainer skillAssignmentContainer;
    protected HBoxContainer skillsContainer;
    public SkillHotbarSlot SkillHotbarSlot1 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot2 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot3 { get; protected set; }
    public SkillHotbarSlot SkillHotbarSlot4 { get; protected set; }
    
    public bool IsSkillBeingSelected = false;
    protected SkillHotbarSlot selectedSlot;
    protected bool hasActiveTooltip = false;
    
    public override void _Ready() {
        skillAssignmentContainer = GetNode<VBoxContainer>("SkillAssignmentContainer");
        skillsContainer = skillAssignmentContainer.GetNode<HBoxContainer>("SkillsContainer");
        SkillHotbarSlot1 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot1");
        SkillHotbarSlot2 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot2");
        SkillHotbarSlot3 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot3");
        SkillHotbarSlot4 = GetNode<SkillHotbarSlot>("SkillHotbar/SkillHotbarSlot4");

        SkillHotbarSlot1.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot2.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot3.SkillSlotClicked += SkillSlotClicked;
        SkillHotbarSlot4.SkillSlotClicked += SkillSlotClicked;

        SkillHotbarSlot1.SkillSlotEntered += SkillSlotMouseEntered;
        SkillHotbarSlot2.SkillSlotEntered += SkillSlotMouseEntered;
        SkillHotbarSlot3.SkillSlotEntered += SkillSlotMouseEntered;
        SkillHotbarSlot4.SkillSlotEntered += SkillSlotMouseEntered;

        SkillHotbarSlot1.SkillSlotExited += SkillSlotMouseExited;
        SkillHotbarSlot2.SkillSlotExited += SkillSlotMouseExited;
        SkillHotbarSlot3.SkillSlotExited += SkillSlotMouseExited;
        SkillHotbarSlot4.SkillSlotExited += SkillSlotMouseExited;

        SkillHotbarSlot1.UpdateHint("LMB");
        SkillHotbarSlot2.UpdateHint("RMB");
        SkillHotbarSlot3.UpdateHint("Q");
        SkillHotbarSlot4.UpdateHint("E");

        SkillAssignable removeSkill = skillAssignableScene.Instantiate<SkillAssignable>();
        removeSkill.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        removeSkill.SetAssignableSkill(null);
        removeSkill.SkillSelected += AssignableSkillSelected;
        skillAssignmentContainer.AddChild(removeSkill);
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

    public void SkillSlotMouseEntered(SkillHotbarSlot skillSlot) {
        Vector2 anchor = skillSlot.GlobalPosition with { X = skillSlot.GlobalPosition.X + skillSlot.Size.X / 2, Y = skillSlot.GlobalPosition.Y };
        hasActiveTooltip = true;
        PlayerOwner.PlayerHUD.CreateItemTooltip(GetCustomSkillTooltip(skillSlot), anchor, skillSlot.GetGlobalRect(), true);
        //skillSlot.AssignedSkill.DebugPrint();
    }

    public void SkillSlotMouseExited(SkillHotbarSlot skillSlot) {
        if (hasActiveTooltip) {
            PlayerOwner.PlayerHUD.RemoveItemTooltip();
        }
    }

    public void SkillSlotClicked(SkillHotbarSlot slot) {
        IsSkillBeingSelected = true;
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
        if (skillsContainer.GetChildCount() > 0) {
            foreach (var item in skillsContainer.GetChildren()) {
                skillsContainer.RemoveChild(item);
                item.QueueFree();
            }
        }
        
        foreach (Skill skill in PlayerOwner.Skills) {
            SkillAssignable assignableSkill = skillAssignableScene.Instantiate<SkillAssignable>();
            assignableSkill.SetAssignableSkill(skill);

            assignableSkill.SkillSelected += AssignableSkillSelected;

            skillsContainer.AddChild(assignableSkill);
        }

        skillAssignmentContainer.Visible = true;
    }

    public void HideSkillAssignmentMenu() {
        skillAssignmentContainer.Visible = false;
    }

    public void DestroySkillAssignmentMenu() {
        IsSkillBeingSelected = false;
        selectedSlot = null;

        foreach (var item in skillsContainer.GetChildren()) {
            skillsContainer.RemoveChild(item);
            item.QueueFree();
        }

        skillAssignmentContainer.Visible = false;
    }

    public Control GetCustomSkillTooltip(SkillHotbarSlot skillSlot) {
		Skill skill = skillSlot.AssignedSkill;

		SkillTooltip tooltipContent = skillTooltipScene.Instantiate<SkillTooltip>();

		tooltipContent.NameLabel.Text = skill.Name;
		tooltipContent.NameLabel.AddThemeColorOverride("font_color", UILib.ColorSkill);

		tooltipContent.CostLabel.Text = $"{skill.ManaCost}";

		if (skill.Type == ESkillType.Attack) {
			IAttack attack = skill as IAttack;
            // Giver nullptr uden et våben i hånden!
            tooltipContent.TimeLabel.Text = $"{PlayerOwner.MainHand.Weapon.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal:F2}";
		}
        else if (skill.Type == ESkillType.Spell) {
			ISpell spell = skill as ISpell;
            tooltipContent.TimeLabel.Text = $"{spell.BaseCastTime / spell.ActiveCastSpeedModifiers.STotal:F2}";
		}

        if (skill.ActiveDamageModifiers.Physical.SMinTotal > 0) {
            Label physLabel = GenerateAffixLabel($"Deals {skill.ActiveDamageModifiers.Physical.SMinTotal} - {skill.ActiveDamageModifiers.Physical.SMaxTotal} Physical Damage");
            tooltipContent.DamageContainer.AddChild(physLabel);
        }
        if (skill.ActiveDamageModifiers.Fire.SMinTotal > 0) {
            Label fireLabel = GenerateAffixLabel($"Deals {skill.ActiveDamageModifiers.Fire.SMinTotal} - {skill.ActiveDamageModifiers.Fire.SMaxTotal} Fire Damage");
            tooltipContent.DamageContainer.AddChild(fireLabel);
        }
        if (skill.ActiveDamageModifiers.Cold.SMinTotal > 0) {
            Label coldLabel = GenerateAffixLabel($"Deals {skill.ActiveDamageModifiers.Cold.SMinTotal} - {skill.ActiveDamageModifiers.Cold.SMaxTotal} Cold Damage");
            tooltipContent.DamageContainer.AddChild(coldLabel);
        }
        if (skill.ActiveDamageModifiers.Lightning.SMinTotal > 0) {
            Label lightningLabel = GenerateAffixLabel($"Deals {skill.ActiveDamageModifiers.Lightning.SMinTotal} - {skill.ActiveDamageModifiers.Lightning.SMaxTotal} Lightning Damage");
            tooltipContent.DamageContainer.AddChild(lightningLabel);
        }
        if (skill.ActiveDamageModifiers.Chaos.SMinTotal > 0) {
            Label chaosLabel = GenerateAffixLabel($"Deals {skill.ActiveDamageModifiers.Chaos.SMinTotal} - {skill.ActiveDamageModifiers.Chaos.SMaxTotal} Chaos Damage");
            tooltipContent.DamageContainer.AddChild(chaosLabel);
        }
		//Label descriptionLabel = GenerateDescriptionLabel(skillItem.SkillReference.Description);
		//tooltipContent.DescriptionContainer.AddChild(descriptionLabel);

		// Indtil der tilføjes synlige effekter/scaling
		//tooltipContent.EffectSeparator.Visible = false;
		//tooltipContent.EffectContainer.Visible = false;
		
		return tooltipContent;
	}

    protected Label GenerateAffixLabel(string affixText) {
		Label affixTextLabel = new Label();

		affixTextLabel.Text = affixText;
		affixTextLabel.AddThemeFontSizeOverride("font_size", 15);
		affixTextLabel.AddThemeColorOverride("font_color", UILib.ColorBlurple);
		affixTextLabel.HorizontalAlignment = HorizontalAlignment.Center;

		return affixTextLabel;
	}
}
