using Godot;
using System;

public partial class SkillHotbar : Control {
    protected readonly PackedScene skillAssignableScene = GD.Load<PackedScene>("res://scenes/gui/hud_skill_assignable.tscn");
    protected readonly PackedScene skillTooltipScene = GD.Load<PackedScene>("res://scenes/gui/hud_skill_tooltip.tscn");

    public Player PlayerOwner;

    protected VBoxContainer skillAssignmentContainer;
    protected HBoxContainer skillsContainer;
    public SkillHotbarSlot[] SkillHotbarSlots = new SkillHotbarSlot[4];
    
    public bool IsSkillBeingSelected = false;
    protected SkillHotbarSlot selectedSlot;
    protected bool hasActiveTooltip = false;
    
    public override void _Ready() {
        skillAssignmentContainer = GetNode<VBoxContainer>("SkillAssignmentContainer");
        skillsContainer = skillAssignmentContainer.GetNode<HBoxContainer>("SkillsContainer");

        for (int i = 0; i < SkillHotbarSlots.Length; i++) {
            SkillHotbarSlots[i] = GetNode<SkillHotbarSlot>($"SkillHotbar/SkillHotbarSlot{i + 1}");
            SkillHotbarSlots[i].SkillSlotClicked += SkillSlotClicked;
            SkillHotbarSlots[i].SkillSlotEntered += SkillSlotMouseEntered;
            SkillHotbarSlots[i].SkillSlotExited += SkillSlotMouseExited;
        }

        SkillHotbarSlots[0].UpdateHint("LMB");
        SkillHotbarSlots[1].UpdateHint("RMB");
        SkillHotbarSlots[2].UpdateHint("Q");
        SkillHotbarSlots[3].UpdateHint("E");

        SkillAssignable removeSkill = skillAssignableScene.Instantiate<SkillAssignable>();
        removeSkill.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        removeSkill.SetAssignableSkill(null);
        removeSkill.SkillSelected += AssignableSkillSelected;
        skillAssignmentContainer.AddChild(removeSkill);
    }

    public void ClearInvalidSkills(ESkillName skillName) {
        for (int i = 0; i < SkillHotbarSlots.Length; i++) {
            if (SkillHotbarSlots[i].AssignedSkill != null && SkillHotbarSlots[i].GetSkillName() == skillName) {
                SkillHotbarSlots[i].AssignSkillToSlot(null);
            }
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
        const int digitsToRoundTo = 0;

        SkillTooltip tooltipContent = skillTooltipScene.Instantiate<SkillTooltip>();
		Skill skill = skillSlot.AssignedSkill;

		tooltipContent.NameLabel.Text = skill.Name;
		tooltipContent.NameLabel.AddThemeColorOverride("font_color", UILib.ColorSkill);

		tooltipContent.CostLabel.Text = $"{skill.ManaCost} Mana";

		if (skill.Type == ESkillType.Attack) {
			IAttack attack = skill as IAttack;

            tooltipContent.TimeDescLabel.Text = "Attack time";

            if (PlayerOwner.MainHand != null) {
                tooltipContent.TimeLabel.Text = $"{PlayerOwner.MainHandStats.AttackSpeed / attack.ActiveAttackSpeedModifiers.STotal:F2}";
            }
            else {
                tooltipContent.TimeLabel.Text = $"{1 / attack.ActiveAttackSpeedModifiers.STotal:F2}";
            }

            VBoxContainer mhDmgCon = GenerateDamageContainer();
            
            if (skill.ActiveDamageModifiers.Physical.SMore > 0) {
                skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Physical, skill.SkillDamageTags, 
                    PlayerOwner.MainHandStats.PhysMinDamage, PlayerOwner.MainHandStats.PhysMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                if (tMin > 0) {
                    Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Physical Damage");
                    mhDmgCon.AddChild(physLabel);
                }
            }
            if (skill.ActiveDamageModifiers.Fire.SMore > 0) {
                skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Fire, skill.SkillDamageTags, 
                    PlayerOwner.MainHandStats.FireMinDamage, PlayerOwner.MainHandStats.FireMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                if (tMin > 0) {
                    Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Fire Damage");
                    mhDmgCon.AddChild(fireLabel);
                }
            }
            if (skill.ActiveDamageModifiers.Cold.SMore > 0) {
                skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Cold, skill.SkillDamageTags, 
                    PlayerOwner.MainHandStats.ColdMinDamage, PlayerOwner.MainHandStats.ColdMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                if (tMin > 0) {
                    Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Cold Damage");
                    mhDmgCon.AddChild(coldLabel);
                }
            }
            if (skill.ActiveDamageModifiers.Lightning.SMore > 0) {
                skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Lightning, skill.SkillDamageTags, 
                    PlayerOwner.MainHandStats.LightningMinDamage, PlayerOwner.MainHandStats.LightningMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                if (tMin > 0) {
                    Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Lightning Damage");
                    mhDmgCon.AddChild(lightningLabel);
                }
            }
            if (skill.ActiveDamageModifiers.Chaos.SMore > 0) {
                skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Chaos, skill.SkillDamageTags, 
                    PlayerOwner.MainHandStats.ChaosMinDamage, PlayerOwner.MainHandStats.ChaosMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                if (tMin > 0) {
                    Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Chaos Damage");
                    mhDmgCon.AddChild(chaosLabel);
                }
            }

            tooltipContent.DamageContainer.AddChild(mhDmgCon);

            if (PlayerOwner.IsDualWielding) {
                VBoxContainer ohDmgCon = GenerateDamageContainer();

                if (skill.ActiveDamageModifiers.Physical.SMore > 0) {
                    skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Physical, skill.SkillDamageTags, 
                        PlayerOwner.OffHandStats.PhysMinDamage, PlayerOwner.OffHandStats.PhysMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                    if (tMin > 0) {
                        Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Physical Damage");
                        ohDmgCon.AddChild(physLabel);
                    }
                }
                if (skill.ActiveDamageModifiers.Fire.SMore > 0) {
                    skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Fire, skill.SkillDamageTags, 
                        PlayerOwner.OffHandStats.FireMinDamage, PlayerOwner.OffHandStats.FireMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                    if (tMin > 0) {
                        Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Fire Damage");
                        ohDmgCon.AddChild(fireLabel);
                    }
                }
                if (skill.ActiveDamageModifiers.Cold.SMore > 0) {
                    skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Cold, skill.SkillDamageTags, 
                        PlayerOwner.OffHandStats.ColdMinDamage, PlayerOwner.OffHandStats.ColdMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                    if (tMin > 0) {
                        Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Cold Damage");
                        ohDmgCon.AddChild(coldLabel);
                    }
                }
                if (skill.ActiveDamageModifiers.Lightning.SMore > 0) {
                    skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Lightning, skill.SkillDamageTags, 
                        PlayerOwner.OffHandStats.LightningMinDamage, PlayerOwner.OffHandStats.LightningMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                    if (tMin > 0) {
                        Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Lightning Damage");
                        ohDmgCon.AddChild(lightningLabel);
                    }
                }
                if (skill.ActiveDamageModifiers.Chaos.SMore > 0) {
                    skill.ActiveDamageModifiers.CalculateTotalAttackDamageWithType(skill.ActiveDamageModifiers.Chaos, skill.SkillDamageTags, 
                        PlayerOwner.OffHandStats.ChaosMinDamage, PlayerOwner.OffHandStats.ChaosMaxDamage, skill.AddedDamageModifier, out double tMin, out double tMax);
                    if (tMin > 0) {
                        Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Chaos Damage");
                        ohDmgCon.AddChild(chaosLabel);
                    }
                }

                tooltipContent.DamageContainer.AddChild(ohDmgCon);
            }

		}
        else if (skill.Type == ESkillType.Spell) {
			ISpell spell = skill as ISpell;

            tooltipContent.TimeDescLabel.Text = "Cast time";
            tooltipContent.TimeLabel.Text = $"{spell.BaseCastTime / spell.ActiveCastSpeedModifiers.STotal:F2}";

            VBoxContainer spellDmgCon = GenerateDamageContainer();

            if (skill.ActiveDamageModifiers.Physical.IsNonZero()) {
                skill.ActiveDamageModifiers.CalculateTotalSpellDamageWithType(skill.ActiveDamageModifiers.Physical, skill.SkillDamageTags, 
                    skill.AddedDamageModifier, out double tMin, out double tMax);
                Label physLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Physical Damage");
                spellDmgCon.AddChild(physLabel);
            }
            if (skill.ActiveDamageModifiers.Fire.IsNonZero()) {
                skill.ActiveDamageModifiers.CalculateTotalSpellDamageWithType(skill.ActiveDamageModifiers.Fire, skill.SkillDamageTags, 
                    skill.AddedDamageModifier, out double tMin, out double tMax);
                Label fireLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Fire Damage");
                spellDmgCon.AddChild(fireLabel);
            }
            if (skill.ActiveDamageModifiers.Cold.IsNonZero()) {
                skill.ActiveDamageModifiers.CalculateTotalSpellDamageWithType(skill.ActiveDamageModifiers.Cold, skill.SkillDamageTags, 
                    skill.AddedDamageModifier, out double tMin, out double tMax);
                Label coldLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Cold Damage");
                spellDmgCon.AddChild(coldLabel);
            }
            if (skill.ActiveDamageModifiers.Lightning.IsNonZero()) {
                skill.ActiveDamageModifiers.CalculateTotalSpellDamageWithType(skill.ActiveDamageModifiers.Lightning, skill.SkillDamageTags, 
                    skill.AddedDamageModifier, out double tMin, out double tMax);
                Label lightningLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Lightning Damage");
                spellDmgCon.AddChild(lightningLabel);
            }
            if (skill.ActiveDamageModifiers.Chaos.IsNonZero()) {
                skill.ActiveDamageModifiers.CalculateTotalSpellDamageWithType(skill.ActiveDamageModifiers.Chaos, skill.SkillDamageTags, 
                    skill.AddedDamageModifier, out double tMin, out double tMax);
                Label chaosLabel = GenerateAffixLabel($"{Math.Round(tMin, digitsToRoundTo)} - {Math.Round(tMax, digitsToRoundTo)} Chaos Damage");
                spellDmgCon.AddChild(chaosLabel);
            }

            tooltipContent.DamageContainer.AddChild(spellDmgCon);
		}

        if (skill.GetSecondaryEffectStrings() != null) {
            foreach (string effectString in skill.GetSecondaryEffectStrings()) {
                tooltipContent.EffectContainer.AddChild(GenerateAffixLabel(effectString));
            }
        }

        if (skill.Tags.HasFlag(ESkillTags.Projectile)) {
            IProjectileSkill ps = skill as IProjectileSkill;

            if (ps.AlwaysPierces) {
                tooltipContent.EffectContainer.AddChild(GenerateAffixLabel("Pierces all targets"));
            }
            else if (ps.TotalPierces != 0) {
                string pierceString;

                if (ps.TotalPierces == 1) {
                    pierceString = $"Pierces {ps.TotalPierces} target";
                }
                else {
                    pierceString = $"Pierces {ps.TotalPierces} targets";
                }

                tooltipContent.EffectContainer.AddChild(GenerateAffixLabel(pierceString));
            }

            string projString;
            if (ps.TotalProjectiles == 1) {
                projString = $"Fires {ps.TotalProjectiles} projectile";
            }
            else {
                projString = $"Fires {ps.TotalProjectiles} projectiles";
            }

            tooltipContent.EffectContainer.AddChild(GenerateAffixLabel(projString));
        }

        if (skill.Tags.HasFlag(ESkillTags.Area)) {
            IAreaSkill aSkill = skill as IAreaSkill;

            tooltipContent.EffectContainer.AddChild(GenerateAffixLabel($"Radius is {aSkill.TotalAreaRadius:F2} metres"));
        }

        if (skill.Tags.HasFlag(ESkillTags.Duration)) {
            IDurationSkill dSkill = skill as IDurationSkill;

            tooltipContent.EffectContainer.AddChild(GenerateAffixLabel($"Duration is {dSkill.TotalDuration:F2} seconds"));
        }

        if (skill.ActiveStatusEffectModifiers.Ignite.CalculateTotalChance() != 0) {
            tooltipContent.EffectContainer.AddChild(GenerateAffixLabel($"{skill.ActiveStatusEffectModifiers.Ignite.CalculateTotalChance():P0} chance to Ignite on Hit"));
        }

        if (skill.ActiveStatusEffectModifiers.Poison.CalculateTotalChance() != 0) {
            tooltipContent.EffectContainer.AddChild(GenerateAffixLabel($"{skill.ActiveStatusEffectModifiers.Poison.CalculateTotalChance():P0} chance to Poison on Hit"));
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
