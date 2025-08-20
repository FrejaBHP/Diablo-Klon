using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public abstract class Skill {
    public static readonly PackedScene GenericAreaPersistentScene = GD.Load<PackedScene>("res://scenes/skills/scene_aoe_persistent_generic.tscn");
    public static readonly PackedScene GenericAreaInstantScene = GD.Load<PackedScene>("res://scenes/skills/scene_aoe_instant_generic.tscn");
    public static readonly PackedScene ConeAreaInstantScene = GD.Load<PackedScene>("res://scenes/skills/scene_aoe_instant_cone.tscn");
    protected static readonly PackedScene thrustAttackScene = GD.Load<PackedScene>("res://scenes/skills/scene_thrust.tscn");

    public Actor ActorOwner { get; set; }
    public SkillSlotCluster HousingSkillCluster = null;

    public DamageModifiers BaseDamageModifiers { get; protected set; } = new(); // Kan måske erstattes med nogle mere simple værdier
    public DamageModifiers ActiveDamageModifiers { get; protected set; } = new(); // Som fx bare have en double der hedder "IncreasedPhys" og lægge det til i det relevante sted i et skill
    public StatusEffectModifiers BaseStatusEffectModifiers { get; protected set; } = new();
    public StatusEffectModifiers ActiveStatusEffectModifiers { get; protected set; } = new();
    public double BaseCriticalStrikeChance;
    public double CriticalStrikeMulti;

    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string[] Effects { get; protected set; }

    protected int level = 0;
    public int Level { 
        get => level; 
        set {
            level = value;
            OnSkillLevelChanged();
        }
    }

    public ESkillName SkillName { get; protected set; }
    public ESkillType Type { get; protected set; }
    public ESkillTags Tags { get; protected set; }
    public ESkillDamageTags SkillDamageTags { get; protected set; }
    public EDamageCategory DamageCategory { get; protected set; }
    public Texture2D Texture { get; protected set; }

    public ESkillStatusEffectFlags StatusEffectFlags = new();

    public double ManaCost { get; protected set; } = 0;
    public float ManaCostMultiplier { get; protected set; } = 1f;
    public double AddedDamageModifier { get; protected set; } = 1;
    public double SpeedModifier { get; protected set; } = 1;
    public double Cooldown { get; protected set; } = 0;

    public float CastRange { get; set; } // Mainly intended to be used by the AI to help them walk into range first

    public bool UsesMouseAim { get; protected set; } = true;
    public bool AimsInStraightLine { get; protected set; } = true;
    protected Vector3 mouseAimPosition = Vector3.Zero;

    public abstract void UseSkill();
    protected abstract void OnSkillLevelChanged();
    protected virtual void UpdateEffectStrings() {}
    public virtual string[] GetSecondaryEffectStrings() {
        return null;
    }

    public virtual bool CanUseSkill() {
        if (!ActorOwner.IsIgnoringManaCosts) {
            if (ActorOwner.BasicStats.CurrentMana < ManaCost * ManaCostMultiplier) {
                return false;
            }
        }
        
        // Indsæt tjek for cooldowns når de bliver implementeret

        if (ActorOwner.IsInGroup("Player") && UsesMouseAim) {
            Vector3 pos = Run.Instance.PlayerActor.CreateCameraRaycastAndGetPosition();

            if (pos == Vector3.Zero) {
                return false;
            }

            SetMouseAimPosition(Run.Instance.PlayerActor.CreateCameraRaycastAndGetPosition());
        }

        if (this is IAttack attack) {
            if (ActorOwner.IsIgnoringWeaponRestrictions) {
                return true;
            }
            return attack.AreActorWeaponsCompatible(ActorOwner.MainHand, ActorOwner.OffHandItem);
        }

        return true;
    }

    public void SetMouseAimPosition(Vector3 pos) {
        if (AimsInStraightLine) {
            pos.Y = ActorOwner.GlobalPosition.Y + ActorOwner.OutgoingEffectAttachmentHeight;
        }

        mouseAimPosition = pos;
    }

    public string GetAttackSpeedModifier() {
        return Math.Round(SpeedModifier * 100, 0).ToString();
    }

    public string GetDamageModifier() {
        return Math.Round(AddedDamageModifier * 100, 0).ToString();
    }

    public void DeductManaFromActor() {
        if (!ActorOwner.IsIgnoringManaCosts) {
            ActorOwner.BasicStats.CurrentMana -= ManaCost * ManaCostMultiplier;
        }
    }

    public SkillInfo CalculateOutgoingValuesIntoInfo(bool canCrit) {
        EDamageInfoFlags damageInfoFlags = new();

        bool isCritical = false;

        double physMin = 0;
        double physMax = 0;
        double fireMin = 0;
        double fireMax = 0;
        double coldMin = 0;
        double coldMax = 0;
        double lightningMin = 0;
        double lightningMax = 0;
        double chaosMin = 0;
        double chaosMax = 0;

        if (this is IAttack attack) {
            attack.GetUsedWeaponStats(ActorOwner, out ActorWeaponStats wStats);

            if (canCrit) {
                isCritical = RollForCritical(wStats.CritChance * ActorOwner.CritChanceMod.STotal);
            }

            DamageModifiers.CalculatePreAttackDamageWithType(ActiveDamageModifiers.Physical, wStats.PhysMinDamage, wStats.PhysMaxDamage, AddedDamageModifier, out physMin, out physMax);
            DamageModifiers.CalculatePreAttackDamageWithType(ActiveDamageModifiers.Fire, wStats.FireMinDamage, wStats.FireMaxDamage, AddedDamageModifier, out fireMin, out fireMax);
            DamageModifiers.CalculatePreAttackDamageWithType(ActiveDamageModifiers.Cold, wStats.ColdMinDamage, wStats.ColdMaxDamage, AddedDamageModifier, out coldMin, out coldMax);
            DamageModifiers.CalculatePreAttackDamageWithType(ActiveDamageModifiers.Lightning, wStats.LightningMinDamage, wStats.LightningMaxDamage, AddedDamageModifier, out lightningMin, out lightningMax);
            DamageModifiers.CalculatePreAttackDamageWithType(ActiveDamageModifiers.Chaos, wStats.ChaosMinDamage, wStats.ChaosMaxDamage, AddedDamageModifier, out chaosMin, out chaosMax);
        }
        else if (this is ISpell spell) {
            if (canCrit) {
                isCritical = RollForCritical(BaseCriticalStrikeChance * ActorOwner.CritChanceMod.STotal);
            }

            DamageModifiers.CalculatePreSpellDamageWithType(ActiveDamageModifiers.Physical, AddedDamageModifier, out physMin, out physMax);
            DamageModifiers.CalculatePreSpellDamageWithType(ActiveDamageModifiers.Fire, AddedDamageModifier, out fireMin, out fireMax);
            DamageModifiers.CalculatePreSpellDamageWithType(ActiveDamageModifiers.Cold, AddedDamageModifier, out coldMin, out coldMax);
            DamageModifiers.CalculatePreSpellDamageWithType(ActiveDamageModifiers.Lightning, AddedDamageModifier, out lightningMin, out lightningMax);
            DamageModifiers.CalculatePreSpellDamageWithType(ActiveDamageModifiers.Chaos, AddedDamageModifier, out chaosMin, out chaosMax);
        }

        double basePhysical = DamageModifiers.RollPreDamage(physMin, physMax);
        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Physical, SkillDamageTags, out double increasedPhysMultiplier, out double morePhysMultiplier);
        double hitPhysical = basePhysical * (1 + increasedPhysMultiplier) * morePhysMultiplier;

        double baseFire = DamageModifiers.RollPreDamage(fireMin, fireMax);
        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Fire, SkillDamageTags, out double increasedFireMultiplier, out double moreFireMultiplier);
        double hitFire = baseFire * (1 + increasedFireMultiplier) * moreFireMultiplier;

        double baseCold = DamageModifiers.RollPreDamage(coldMin, coldMax);
        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Cold, SkillDamageTags, out double increasedColdMultiplier, out double moreColdMultiplier);
        double hitCold = baseCold * (1 + increasedColdMultiplier) * moreColdMultiplier;

        double baseLightning = DamageModifiers.RollPreDamage(lightningMin, lightningMax);
        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Lightning, SkillDamageTags, out double increasedLightningMultiplier, out double moreLightningMultiplier);
        double hitLightning = baseLightning * (1 + increasedLightningMultiplier) * moreLightningMultiplier;

        double baseChaos = DamageModifiers.RollPreDamage(chaosMin, chaosMax);
        ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Chaos, SkillDamageTags, out double increasedChaosMultiplier, out double moreChaosMultiplier);
        double hitChaos = baseChaos * (1 + increasedChaosMultiplier) * moreChaosMultiplier;
        
        if (isCritical) {
            damageInfoFlags |= EDamageInfoFlags.Critical;

            hitPhysical *= CriticalStrikeMulti;
            hitFire *= CriticalStrikeMulti;
            hitCold *= CriticalStrikeMulti;
            hitLightning *= CriticalStrikeMulti;
            hitChaos *= CriticalStrikeMulti;
        }

        SkillHitDamageInfo hitDamageInfo = new(hitPhysical, hitFire, hitCold, hitLightning, hitChaos);

        List<AttachedEffect> statusEffects = new();
        /*
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanBleed)) {
            if (ActiveStatusEffectModifiers.Bleed.RollForProc()) {
                // TODO
            }
        }
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanIgnite)) {
            if (ActiveStatusEffectModifiers.Ignite.RollForProc()) {
                statusEffects.Add(new IgniteEffect(null, 1, baseFire));
            }
        }
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanChill)) {
            if (ActiveStatusEffectModifiers.Chill.RollForProc()) {
                // TODO
            }
        }
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanShock)) {
            if (ActiveStatusEffectModifiers.Shock.RollForProc()) {
                // TODO
            }
        }

        GD.Print($"Can Poison: {StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanPoison)}");
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanPoison)) {
            if (ActiveStatusEffectModifiers.Poison.RollForProc()) {
                statusEffects.Add(new PoisonEffect(null, 1, basePhysical + baseChaos));
            }
        }
        if (StatusEffectFlags.HasFlag(ESkillStatusEffectFlags.CanSlow)) {
            if (ActiveStatusEffectModifiers.Slow.RollForProc()) {
                // TODO
            }
        }*/
        if (basePhysical > 0) {
            if (ActiveStatusEffectModifiers.Bleed.RollForProc()) {
                ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Physical, BleedEffect.DamageTags, out double incMult, out double moreMult);
                double damage = basePhysical * (1 + incMult) * moreMult * (1 + ActorOwner.StatusMods.Bleed.SFasterTicking);
                statusEffects.Add(new BleedEffect(null, ActiveStatusEffectModifiers.Bleed.CalculateDurationModifier(), damage));
            }
        }
        if (baseFire > 0) {
            if (ActiveStatusEffectModifiers.Ignite.RollForProc()) {
                ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Fire, IgniteEffect.DamageTags, out double incMult, out double moreMult);
                double damage = baseFire * (1 + incMult) * moreMult * (1 + ActorOwner.StatusMods.Ignite.SFasterTicking);
                statusEffects.Add(new IgniteEffect(null, ActiveStatusEffectModifiers.Ignite.CalculateDurationModifier(), damage));
            }
        }
        if (baseCold > 0) {
            if (ActiveStatusEffectModifiers.Chill.RollForProc()) {
                // TODO
            }
        }
        if (baseLightning > 0) {
            if (ActiveStatusEffectModifiers.Shock.RollForProc()) {
                // TODO
            }
        }

        if ((basePhysical + baseChaos) > 0) {
            if (ActiveStatusEffectModifiers.Poison.RollForProc()) {
                ActiveDamageModifiers.CalculateMultipliersWithType(ActiveDamageModifiers.Chaos, PoisonEffect.DamageTags, out double incMult, out double moreMult);
                double damage = (basePhysical + baseChaos) * (1 + incMult) * moreMult * (1 + ActorOwner.StatusMods.Poison.SFasterTicking);
                statusEffects.Add(new PoisonEffect(null, ActiveStatusEffectModifiers.Poison.CalculateDurationModifier(), damage));
            }
        }

        if (true) {
            if (ActiveStatusEffectModifiers.Slow.RollForProc()) {
                // TODO
            }
        }

        SkillStatusInfo statusInfo = new(statusEffects);

        return new SkillInfo(hitDamageInfo, statusInfo, damageInfoFlags);
    }

    public static bool RollForCritical(double chance) {
        double critRoll = Utilities.RNG.NextDouble();
        return chance >= critRoll;
    }

    public virtual void RecalculateSkillValues() {
        if (ActorOwner != null) {
            ActiveDamageModifiers = ActorOwner.DamageMods + BaseDamageModifiers;
            ActiveStatusEffectModifiers = ActorOwner.StatusMods + BaseStatusEffectModifiers;

            List<SupportGem> supportGems;

            bool isPartOfCluster = HousingSkillCluster != null;
            if (isPartOfCluster) {
                supportGems = HousingSkillCluster.GetSupports();
            }
            else {
                supportGems = [];
            }

            if (this is IAttack atSkill) {
                // Does not contain all variables needed
                atSkill.UpdateAttackSpeedValues(ActorOwner.AttackSpeedMod);
                //attack.UpdateWeaponStats(ActorOwner.MainHandStats, ActorOwner.OffHandStats);

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Attack)) {
                            support.ModifyAttackSkill(atSkill);
                        }
                    } 
                }
            }

            if (this is ISpell spSkill) {
                // Ditto
                spSkill.UpdateCastSpeedValues(ActorOwner.CastSpeedMod);

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Spell)) {
                            support.ModifySpellSkill(spSkill);
                        }
                    }
                }
            }

            if (this is IMeleeSkill mSkill) {
                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Melee)) {
                            support.ModifyMeleeSkill(mSkill);
                        }
                    }  
                }
            }

            if (this is IProjectileSkill pSkill) {
                pSkill.AddedPierces = 0;
                pSkill.AddedProjectiles = 0;
                pSkill.MinimumSpreadAngleOverride = 0;
                pSkill.MaximumSpreadAngleOverride = 0;
                
                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Projectile)) {
                            support.ModifyProjectileSkill(pSkill);
                        }
                    }
                }

                pSkill.TotalPierces = pSkill.BasePierces + pSkill.AddedPierces;
                pSkill.TotalProjectiles = pSkill.BaseProjectiles + pSkill.AddedProjectiles;
            }

            if (this is IAreaSkill aSkill) {
                aSkill.IncreasedArea = ActorOwner.StatDictionary[EStatName.IncreasedAreaOfEffect];
                aSkill.MoreArea = ActorOwner.MultiplicativeStatDictionary[EStatName.MoreAreaOfEffect];

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Area)) {
                            support.ModifyAreaSkill(aSkill);
                        }
                    }
                }

                aSkill.CalculateTotalRadius();
            }

            if (this is IDurationSkill dSkill) {
                dSkill.IncreasedDuration = ActorOwner.StatDictionary[EStatName.IncreasedSkillEffectDuration];
                dSkill.MoreDuration = ActorOwner.MultiplicativeStatDictionary[EStatName.MoreSkillEffectDuration];

                if (isPartOfCluster) {
                    foreach (SupportGem support in supportGems) {
                        if (support.SkillTags.HasFlag(ESkillTags.Duration)) {
                            support.ModifyDurationSkill(dSkill);
                        }
                    }
                }

                dSkill.TotalDuration = dSkill.BaseDuration * (1 + dSkill.IncreasedDuration) * dSkill.MoreDuration;
            }

            if (isPartOfCluster) {
                foreach (SupportGem support in supportGems) {
                    if (support.AffectsDamageModifiers) {
                        support.ApplyToDamageModifiers(ActiveDamageModifiers);
                    }
                    if (support.AffectsStatusModifiers) {
                        support.ApplyToStatusModifiers(ActiveStatusEffectModifiers);
                    }
                }
            }

            CriticalStrikeMulti = ActorOwner.CritMultiplier.STotal;

            /*
            if (ActiveDamageModifiers.Physical.IsNonZero()) {
                StatusEffectFlags |= ESkillStatusEffectFlags.CanBleed;
                StatusEffectFlags |= ESkillStatusEffectFlags.CanPoison;
            }
            else {
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanBleed;
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanPoison;
            }

            if (ActiveDamageModifiers.Fire.IsNonZero()) {
                StatusEffectFlags |= ESkillStatusEffectFlags.CanIgnite;
            }
            else {
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanIgnite;
            }

            if (ActiveDamageModifiers.Cold.IsNonZero()) {
                StatusEffectFlags |= ESkillStatusEffectFlags.CanChill;
            }
            else {
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanChill;
            }

            if (ActiveDamageModifiers.Lightning.IsNonZero()) {
                StatusEffectFlags |= ESkillStatusEffectFlags.CanShock;
            }
            else {
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanShock;
            }

            if (ActiveDamageModifiers.Chaos.IsNonZero()) {
                StatusEffectFlags |= ESkillStatusEffectFlags.CanPoison;
            }
            else {
                StatusEffectFlags &= ~ESkillStatusEffectFlags.CanPoison;
            }
            */
        }
    }

    public void SetSkillCollision(Area3D collisionArea) {
        if (ActorOwner.IsInGroup("Enemy")) {
            collisionArea.CollisionMask += 4;
        }
        else if (ActorOwner.IsInGroup("Player")) {
            collisionArea.CollisionMask += 32;
        }
    }

    public void SetSkillCollision(ShapeCast3D shapeCast) {
        if (ActorOwner.IsInGroup("Enemy")) {
            shapeCast.CollisionMask += 4;
        }
        else if (ActorOwner.IsInGroup("Player")) {
            shapeCast.CollisionMask += 32;
        }
    }

    public void DebugPrint() {
        GD.Print("Player Damage:");
        GD.Print($"P: {ActorOwner.DamageMods.Physical}");
        GD.Print($"F: {ActorOwner.DamageMods.Fire}");
        GD.Print($"C: {ActorOwner.DamageMods.Cold}");
        GD.Print($"L: {ActorOwner.DamageMods.Lightning}");
        GD.Print($"Ch: {ActorOwner.DamageMods.Chaos}");

        GD.Print("Base Damage:");
        GD.Print($"P: {BaseDamageModifiers.Physical}");
        GD.Print($"F: {BaseDamageModifiers.Fire}");
        GD.Print($"C: {BaseDamageModifiers.Cold}");
        GD.Print($"L: {BaseDamageModifiers.Lightning}");
        GD.Print($"Ch: {BaseDamageModifiers.Chaos}");

        GD.Print("Active Damage:");
        GD.Print($"P: {ActiveDamageModifiers.Physical}");
        GD.Print($"F: {ActiveDamageModifiers.Fire}");
        GD.Print($"C: {ActiveDamageModifiers.Cold}");
        GD.Print($"L: {ActiveDamageModifiers.Lightning}");
        GD.Print($"Ch: {ActiveDamageModifiers.Chaos}");
    }
}

public interface IAttack {
    ESkillWeapons Weapons { get; protected set; }
    Stat BaseAttackSpeedModifiers { get; protected set; }
    Stat ActiveAttackSpeedModifiers { get; protected set; }
    bool CanDualWield { get; protected set; }

    // ===== Virtual functions =====

    // ===== Default functions =====
    public void UpdateAttackSpeedValues(Stat actorAS) {
        ActiveAttackSpeedModifiers = actorAS + BaseAttackSpeedModifiers;
    }

    public string GetAttackSpeedModifier() {
        return Math.Round(ActiveAttackSpeedModifiers.STotal * 100, 0).ToString();
    }

    public void GetUsedWeaponStats(Actor owner, out ActorWeaponStats wStats) {
        if (owner.IsDualWielding) {
            if (owner.IsUsingMainHandDW) {
                wStats = owner.MainHandStats;
            }
            else {
                wStats = owner.OffHandStats;
            }
        }
        else {
            wStats = owner.MainHandStats;
        }
    }

    public bool AreActorWeaponsCompatible(WeaponItem mhWeapon, Item ohItem) {
        EItemWeaponBaseType mhWeaponType = mhWeapon != null ? mhWeapon.ItemWeaponBaseType : EItemWeaponBaseType.Unarmed;
        EItemWeaponBaseType ohWeaponType;

        if (ohItem != null && ohItem.GetType().IsSubclassOf(typeof(WeaponItem))) {
            WeaponItem ohWeapon = ohItem as WeaponItem;
            ohWeaponType = ohWeapon.ItemWeaponBaseType;
        }
        else {
            ohWeaponType = EItemWeaponBaseType.Unarmed;
        }

        switch (mhWeaponType) {
            case EItemWeaponBaseType.Unarmed: 
                return Weapons.HasFlag(ESkillWeapons.Unarmed);
            
            case EItemWeaponBaseType.WeaponMelee1H:
                if (Weapons == ESkillWeapons.MeleeDW && ohWeaponType == EItemWeaponBaseType.Unarmed) {
                    return false;
                }
                return Weapons.HasFlag(ESkillWeapons.Melee1H);

            case EItemWeaponBaseType.WeaponMelee2H:
                return Weapons.HasFlag(ESkillWeapons.Melee2H);
            
            case EItemWeaponBaseType.WeaponRanged1H:
                return Weapons.HasFlag(ESkillWeapons.Ranged1H);
                
            case EItemWeaponBaseType.WeaponRanged2H:
                return Weapons.HasFlag(ESkillWeapons.Ranged2H);

            default:
                return false;
        }
    }
}

public interface ISpell {
    double BaseCastTime { get; protected set; }
    Stat BaseCastSpeedModifiers { get; protected set; }
    Stat ActiveCastSpeedModifiers { get; protected set; }

    // ===== Virtual functions =====

    // ===== Default functions =====
    public void UpdateCastSpeedValues(Stat actorCS) {
        ActiveCastSpeedModifiers = actorCS + BaseCastSpeedModifiers;
    }

    public string GetCastSpeedModifier() {
        return Math.Round(ActiveCastSpeedModifiers.STotal * 100, 0).ToString();
    }
}

public interface IMeleeSkill {
    float BaseAttackRange { get; protected set; }

    // ===== Virtual functions =====
    //void ApplyMeleeSkillBehaviourToTarget(Actor target);

    // ===== Default functions =====
}

public interface IProjectileSkill {
    float BaseProjectileSpeed { get; protected set; }
    double BaseProjectileLifetime { get; protected set; }
    ESkillProjectileType ProjectileType { get; protected set; }
    int BasePierces { get; protected set; }
    int AddedPierces { get; set; }
    int TotalPierces { get; set; }
    bool AlwaysPierces { get; set; }
    int BaseProjectiles { get; protected set; }
    int AddedProjectiles { get; set; }
    int TotalProjectiles { get; set; }
    bool CanFireSequentially { get; protected set; }
    bool FiresSequentially { get; set; }

    double MinimumSpreadAngle { get; protected set; }
    double MaximumSpreadAngle { get; protected set; }
    double MinimumSpreadAngleOverride { get; set; }
    double MaximumSpreadAngleOverride { get; set; }

    static readonly double[] projectileMinAngles = [
        0,                  // 1 projectile, no spread
        Math.Tau / 12,      // 2 projectiles, 30 degrees
        Math.Tau / 9,       // 3 projectiles, 40 degrees
        Math.Tau / 7.5,     // 4 projectiles, 48 degrees
        Math.Tau / 6,       // 5 projectiles, 60 degrees
        Math.Tau / 4.5,     // 6+ projectiles, 80 degrees
    ];

    static readonly double[] projectileMaxAngles = [
        0,                  // 1 projectile, no spread
        Math.Tau / 8,       // 2 projectiles, 45 degrees
        Math.Tau / 6,       // 3 projectiles, 60 degrees
        Math.Tau / 5,       // 4 projectiles, 72 degrees
        Math.Tau / 4,       // 5 projectiles, 90 degrees
        Math.Tau / 3,       // 6+ projectiles, 120 degrees
    ];

    // ===== Virtual functions =====
    void ApplyProjectileSkillBehaviourToTarget(Actor target);

    // ===== Default functions =====
    public static float GetSpreadCoefficient(Vector3 startPos, Vector3 endPos) {
        float dist = startPos.DistanceTo(endPos);
        float coefficient = Math.Clamp(dist / 8f, 0f, 1f);
        return coefficient;
    }

    /// <summary>
    /// Calculates the rotation that each projectile should have applied to form a cone.
    /// </summary>
    /// <param name="noOfProjectiles"></param>
    /// <param name="spreadCoefficient"></param>
    /// <returns>An array with the individual angle modifications in radians.</returns>
    float[] GetMultipleProjectileAngles(int noOfProjectiles, float spreadCoefficient) {
        float[] angles = new float[noOfProjectiles];

        if (noOfProjectiles > 1) {
            double usedMinSpread;
            double usedMaxSpread;
            float finalSpread;

            if (MinimumSpreadAngleOverride != 0) {
                usedMinSpread = MinimumSpreadAngleOverride;
            }
            else if (MinimumSpreadAngle != 0) {
                usedMinSpread = MinimumSpreadAngle;
            }
            else {
                if ((noOfProjectiles - 1) >= projectileMinAngles.Length) {
                    usedMinSpread = projectileMinAngles.Last();
                }
                else {
                    usedMinSpread = projectileMinAngles[noOfProjectiles - 1];
                }
            }

            if (MaximumSpreadAngleOverride != 0) {
                usedMaxSpread = MaximumSpreadAngleOverride;
            }
            else if (MaximumSpreadAngle != 0) {
                usedMaxSpread = MaximumSpreadAngle;
            }
            else {
                if ((noOfProjectiles - 1) >= projectileMaxAngles.Length) {
                    usedMaxSpread = projectileMaxAngles.Last();
                }
                else {
                    usedMaxSpread = projectileMaxAngles[noOfProjectiles - 1];
                }
            }

            finalSpread = (float)(usedMaxSpread - ((usedMaxSpread - usedMinSpread) * spreadCoefficient));
            float spreadSegment = finalSpread / (noOfProjectiles - 1);
            
            // if number of projectiles is even
            if (noOfProjectiles % 2 == 0) {
                for (int i = 0; i < noOfProjectiles; i++) {
                    angles[i] = (-finalSpread / 2) + (spreadSegment * i);
                }
            }
            // if number of projectiles is odd
            else {
                for (int i = 0; i < noOfProjectiles; i++) {
                    angles[i] = (-finalSpread / 2) + (spreadSegment * i);
                }
            }
        }
        else {
            angles[0] = 0;
        }

        return angles;
    }

    List<Projectile> BasicProjectileSkillBehaviour(Skill skill, Vector3 mouseAimPosition) {
        List<Projectile> projectiles = new();

        float coefficient = GetSpreadCoefficient(skill.ActorOwner.GlobalPosition, mouseAimPosition);
        Vector3 diffVector = mouseAimPosition - skill.ActorOwner.GlobalPosition;

        float[] angleOffsets = GetMultipleProjectileAngles(TotalProjectiles, coefficient);
        HashSet<Actor> ActorsHitByThisInstance = new();

        for (int i = 0; i < TotalProjectiles; i++) {
            Projectile proj = ProjectileDatabase.GetProjectile(ProjectileType);
            Run.Instance.AddChild(proj);
            skill.SetSkillCollision(proj.Hitbox);
            proj.TargetHit += ApplyProjectileSkillBehaviourToTarget;
            proj.ActorsHitThisGroup = ActorsHitByThisInstance;

            proj.GlobalPosition = proj.Position with { 
                X = skill.ActorOwner.GlobalPosition.X, 
                Y = skill.ActorOwner.GlobalPosition.Y + skill.ActorOwner.OutgoingEffectAttachmentHeight, 
                Z = skill.ActorOwner.GlobalPosition.Z 
            };

            if (mouseAimPosition != Vector3.Zero) {
                if (!FiresSequentially) {
                    Vector3 newPos = diffVector.Rotated(Vector3.Up, angleOffsets[i]) + skill.ActorOwner.GlobalPosition;
                    proj.SetFacing(newPos);
                }
                else {
                    proj.SetFacing(mouseAimPosition);
                }
            }
            else {
                if (!FiresSequentially) {
                    Vector3 rotation = skill.ActorOwner.GlobalRotation.Rotated(Vector3.Up, angleOffsets[i]);
                    proj.SetFacing(rotation.Y);
                }
                else {
                    proj.SetFacing(skill.ActorOwner.GlobalRotation.Y);
                }
            }

            int effectivePierces;
            if (AlwaysPierces) {
                effectivePierces = 999;
            }
            else {
                effectivePierces = TotalPierces;
            }
            
            proj.SetProperties(BaseProjectileSpeed, -1, effectivePierces, 15f, false);
            projectiles.Add(proj);
        }

        skill.DeductManaFromActor();
        return projectiles;
    }
}

public interface IAreaSkill {
    float BaseAreaRadius { get; protected set; }
    double IncreasedArea { get; set; }
    double MoreArea { get; set; }
    float TotalAreaRadius { get; protected set; }
    bool IsNovaSkill { get; protected set; }

    // ===== Virtual functions =====
    void ApplyAreaSkillBehaviourToTargets(List<Actor> targets);

    // ===== Default functions =====
    void BasicAreaSweepSkillBehaviour(Skill skill, string animationName, float animationPixelSize) {
        const int casts = 1; // Might allow skills to repeat in the future, but until then, this is here to make sure they only cast once
        Vector3 targetPosition;

        if (IsNovaSkill) {
            targetPosition = skill.ActorOwner.GlobalPosition;
        }
        else {
            targetPosition = skill.ActorOwner.GlobalPosition; // temp
        }

        for (int i = 0; i < casts; i++) {
            AreaOfEffectInstant area = Skill.GenericAreaInstantScene.Instantiate<AreaOfEffectInstant>();
            Run.Instance.AddChild(area);
            skill.SetSkillCollision(area.ShapeCastSweep);

            area.TargetsSwept += ApplyAreaSkillBehaviourToTargets;
            area.GlobalPosition = targetPosition;
            area.SetProperties(TotalAreaRadius, animationName, animationPixelSize, TotalAreaRadius / BaseAreaRadius);
            area.Sweep();
        }

        skill.DeductManaFromActor();
    }

    void BasicConeSweepSkillBehaviour(Skill skill, string animationName, float animationPixelSize, Vector3 direction, float coneDotMinimum) {
        const int casts = 1; // Might allow skills to repeat in the future, but until then, this is here to make sure they only cast once
        Vector3  targetPosition = skill.ActorOwner.GlobalPosition;

        for (int i = 0; i < casts; i++) {
            AreaOfEffectInstantCone area = Skill.ConeAreaInstantScene.Instantiate<AreaOfEffectInstantCone>();
            Run.Instance.AddChild(area);
            skill.SetSkillCollision(area.ShapeCastSweep);

            area.TargetsSwept += ApplyAreaSkillBehaviourToTargets;
            area.GlobalPosition = targetPosition;
            area.SetProperties(TotalAreaRadius, animationName, animationPixelSize, TotalAreaRadius / BaseAreaRadius, skill.ActorOwner.OutgoingEffectAttachmentHeight, direction, coneDotMinimum);
            area.Sweep();
        }

        skill.DeductManaFromActor();
    }

    /// <summary>
    /// Calculates and sets the TotalAreaRadius with modifiers to the area.
    /// </summary>
    void CalculateTotalRadius() {
        // In lieu of applying increases to Area of Effect directly to the radius, they are instead applied to the circle's calculated area, 
        // where that circle's new radius is derived and set as the skill's total radius to avoid crazy scaling.
        // This results in calculations such as these for a skill with a base radius of 2,50:
        //  50% Increased Area of Effect = 2,50 -> 3,06, and not 3,75. (Area = 19,63 -> 29,45)
        // 100% Increased Area of Effect = 2,50 -> 3,54, and not 5,00. (Area = 19,63 -> 39,27)
        // 200% Increased Area of Effect = 2,50 -> 4,33, and not 7,50. (Area = 19,63 -> 58,91)

        double area = Math.PI * Math.Pow(BaseAreaRadius, 2);
        area *= (1 + IncreasedArea) * MoreArea;

        TotalAreaRadius = (float)Math.Pow(area / Math.PI, 0.5);
    }
}

public interface IDurationSkill {
    double BaseDuration { get; protected set; }
    double IncreasedDuration { get; set; }
    double MoreDuration { get; set; }
    double TotalDuration { get; set; }

    // ===== Virtual functions =====

    // ===== Default functions =====
}
