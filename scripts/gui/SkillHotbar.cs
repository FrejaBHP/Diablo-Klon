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

            if (PlayerOwner.MainHand != null) {
                tooltipContent.TimeLabel.Text = $"{PlayerOwner.MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal:F2}";
            }
            else {
                tooltipContent.TimeLabel.Text = $"{1 / attack.ActiveAttackSpeedModifiers.STotal:F2}";
            }

            VBoxContainer mhDmgCon = GenerateDamageContainer();

            // Indsæt logik både her og i Tooltip scene, der kan bruge begge våben
            if (skill.ActiveDamageModifiers.Physical.SMinBase > 0 || skill.ActiveDamageModifiers.Physical.SMinAdded > 0 || PlayerOwner.MainHandStats.PhysMinDamage > 0) {
                skill.ActiveDamageModifiers.Physical.CalculateTotalWithBase(PlayerOwner.MainHandStats.PhysMinDamage, PlayerOwner.MainHandStats.PhysMaxDamage, out double tMin, out double tMax);
                Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Physical Damage");
                mhDmgCon.AddChild(physLabel);
            }
            if (skill.ActiveDamageModifiers.Fire.SMinBase > 0 || skill.ActiveDamageModifiers.Fire.SMinAdded > 0 || PlayerOwner.MainHandStats.FireMinDamage > 0) {
                skill.ActiveDamageModifiers.Fire.CalculateTotalWithBase(PlayerOwner.MainHandStats.FireMinDamage, PlayerOwner.MainHandStats.FireMaxDamage, out double tMin, out double tMax);
                Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Fire Damage");
                mhDmgCon.AddChild(fireLabel);
            }
            if (skill.ActiveDamageModifiers.Cold.SMinBase > 0 || skill.ActiveDamageModifiers.Cold.SMinAdded > 0 || PlayerOwner.MainHandStats.ColdMinDamage > 0) {
                skill.ActiveDamageModifiers.Cold.CalculateTotalWithBase(PlayerOwner.MainHandStats.ColdMinDamage, PlayerOwner.MainHandStats.ColdMaxDamage, out double tMin, out double tMax);
                Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Cold Damage");
                mhDmgCon.AddChild(coldLabel);
            }
            if (skill.ActiveDamageModifiers.Lightning.SMinBase > 0 || skill.ActiveDamageModifiers.Lightning.SMinAdded > 0 || PlayerOwner.MainHandStats.LightningMinDamage > 0) {
                skill.ActiveDamageModifiers.Lightning.CalculateTotalWithBase(PlayerOwner.MainHandStats.LightningMinDamage, PlayerOwner.MainHandStats.LightningMaxDamage, out double tMin, out double tMax);
                Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Lightning Damage");
                mhDmgCon.AddChild(lightningLabel);
            }
            if (skill.ActiveDamageModifiers.Chaos.SMinBase > 0 || skill.ActiveDamageModifiers.Chaos.SMinAdded > 0 || PlayerOwner.MainHandStats.ChaosMinDamage > 0) {
                skill.ActiveDamageModifiers.Chaos.CalculateTotalWithBase(PlayerOwner.MainHandStats.ChaosMinDamage, PlayerOwner.MainHandStats.ChaosMaxDamage, out double tMin, out double tMax);
                Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Chaos Damage");
                mhDmgCon.AddChild(chaosLabel);
            }

            tooltipContent.DamageContainer.AddChild(mhDmgCon);

            if (PlayerOwner.IsDualWielding) {
                VBoxContainer ohDmgCon = GenerateDamageContainer();

                // Indsæt logik både her og i Tooltip scene, der kan bruge begge våben
                if (skill.ActiveDamageModifiers.Physical.SMinBase > 0 || skill.ActiveDamageModifiers.Physical.SMinAdded > 0 || PlayerOwner.OffHandStats.PhysMinDamage > 0) {
                    skill.ActiveDamageModifiers.Physical.CalculateTotalWithBase(PlayerOwner.OffHandStats.PhysMinDamage, PlayerOwner.OffHandStats.PhysMaxDamage, out double tMin, out double tMax);
                    Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Physical Damage");
                    ohDmgCon.AddChild(physLabel);
                }
                if (skill.ActiveDamageModifiers.Fire.SMinBase > 0 || skill.ActiveDamageModifiers.Fire.SMinAdded > 0 || PlayerOwner.OffHandStats.FireMinDamage > 0) {
                    skill.ActiveDamageModifiers.Fire.CalculateTotalWithBase(PlayerOwner.OffHandStats.FireMinDamage, PlayerOwner.OffHandStats.FireMaxDamage, out double tMin, out double tMax);
                    Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Fire Damage");
                    ohDmgCon.AddChild(fireLabel);
                }
                if (skill.ActiveDamageModifiers.Cold.SMinBase > 0 || skill.ActiveDamageModifiers.Cold.SMinAdded > 0 || PlayerOwner.OffHandStats.ColdMinDamage > 0) {
                    skill.ActiveDamageModifiers.Cold.CalculateTotalWithBase(PlayerOwner.OffHandStats.ColdMinDamage, PlayerOwner.OffHandStats.ColdMaxDamage, out double tMin, out double tMax);
                    Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Cold Damage");
                    ohDmgCon.AddChild(coldLabel);
                }
                if (skill.ActiveDamageModifiers.Lightning.SMinBase > 0 || skill.ActiveDamageModifiers.Lightning.SMinAdded > 0 || PlayerOwner.OffHandStats.LightningMinDamage > 0) {
                    skill.ActiveDamageModifiers.Lightning.CalculateTotalWithBase(PlayerOwner.OffHandStats.LightningMinDamage, PlayerOwner.OffHandStats.LightningMaxDamage, out double tMin, out double tMax);
                    Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Lightning Damage");
                    ohDmgCon.AddChild(lightningLabel);
                }
                if (skill.ActiveDamageModifiers.Chaos.SMinBase > 0 || skill.ActiveDamageModifiers.Chaos.SMinAdded > 0 || PlayerOwner.OffHandStats.ChaosMinDamage > 0) {
                    skill.ActiveDamageModifiers.Chaos.CalculateTotalWithBase(PlayerOwner.OffHandStats.ChaosMinDamage, PlayerOwner.OffHandStats.ChaosMaxDamage, out double tMin, out double tMax);
                    Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Chaos Damage");
                    ohDmgCon.AddChild(chaosLabel);
                }

                tooltipContent.DamageContainer.AddChild(ohDmgCon);
            }

		}
        else if (skill.Type == ESkillType.Spell) {
			ISpell spell = skill as ISpell;

            tooltipContent.TimeLabel.Text = $"{spell.BaseCastTime / spell.ActiveCastSpeedModifiers.STotal:F2}";

            VBoxContainer spellDmgCon = GenerateDamageContainer();

            if (skill.ActiveDamageModifiers.Physical.SMinBase > 0 || skill.ActiveDamageModifiers.Physical.SMinAdded > 0) {
                skill.ActiveDamageModifiers.Physical.CalculateTotal(out double tMin, out double tMax);
                Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Physical Damage");
                spellDmgCon.AddChild(physLabel);
            }
            if (skill.ActiveDamageModifiers.Fire.SMinBase > 0 || skill.ActiveDamageModifiers.Fire.SMinAdded > 0) {
                skill.ActiveDamageModifiers.Fire.CalculateTotal(out double tMin, out double tMax);
                Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Fire Damage");
                spellDmgCon.AddChild(fireLabel);
            }
            if (skill.ActiveDamageModifiers.Cold.SMinBase > 0 || skill.ActiveDamageModifiers.Cold.SMinAdded > 0) {
                skill.ActiveDamageModifiers.Cold.CalculateTotal(out double tMin, out double tMax);
                Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Cold Damage");
                spellDmgCon.AddChild(coldLabel);
            }
            if (skill.ActiveDamageModifiers.Lightning.SMinBase > 0 || skill.ActiveDamageModifiers.Lightning.SMinAdded > 0) {
                skill.ActiveDamageModifiers.Lightning.CalculateTotal(out double tMin, out double tMax);
                Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Lightning Damage");
                spellDmgCon.AddChild(lightningLabel);
            }
            if (skill.ActiveDamageModifiers.Chaos.SMinBase > 0 || skill.ActiveDamageModifiers.Chaos.SMinAdded > 0) {
                skill.ActiveDamageModifiers.Chaos.CalculateTotal(out double tMin, out double tMax);
                Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, 0)} - {Math.Round(tMax, 0)} Chaos Damage");
                spellDmgCon.AddChild(chaosLabel);
            }

            tooltipContent.DamageContainer.AddChild(spellDmgCon);
		}

		return tooltipContent;
	}

    protected VBoxContainer GenerateDamageContainer() {
        VBoxContainer dmgContainer = new VBoxContainer();
        dmgContainer.Alignment = BoxContainer.AlignmentMode.Begin;
        dmgContainer.AddThemeConstantOverride("separation", -1);
        dmgContainer.MouseFilter = MouseFilterEnum.Ignore;
        dmgContainer.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;

        return dmgContainer;
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
