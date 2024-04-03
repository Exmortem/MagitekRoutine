using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using System;
using System.Collections.Generic;
using System.Linq;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Utilities.Routines
{
    internal static class RedMage
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.RedMage, Spells.Jolt);

        #region Constants
        public const int GcdBufferMs = 350;

        public const int MinTargetsForAoeMode = 3;
        public const int MinAoeBurstStartEnemies = 3;
        public const int MinMoulinetEnemies = 3; //According to The Balance, melee combo is better against 2 enemies
        #endregion

        #region General status
        public static int Cap(int mana) => Math.Min(100, mana);
        //How much mana would be lost to the cap if we added more white and black mana in the specified amounts
        public static int CapLoss(int moreWhite, int moreBlack) => (WhiteMana + BlackMana + moreWhite + moreBlack) - (Cap(WhiteMana + moreWhite) + Cap(BlackMana + moreBlack));

        //Weave time remaining
        public static double GcdLeft => Math.Max(Spells.Riposte.Cooldown.TotalMilliseconds - GcdBufferMs, 0);
        #endregion

        #region Skill status
        public static bool Ver2Enabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Ver2;

        public static bool ScatterEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Scatter;
        public static bool SwiftcastScatter => ScatterEnabled && RedMageSettings.Instance.SwiftcastScatter;

        //Use moulinet if it's enabled and it would hit enough enemies
        public static bool UseMoulinet => MoulinetEnabled && EnemiesWithinOf(8 + Core.Me.CombatReach, Core.Me).Count(r => InMoulinetArc(r)) >= MinMoulinetEnemies;
        //Burn a moulinet if we can use it, and we're not about to start an embolden burst
        public static bool BurnMoulinet => UseMoulinet && !EmboldenReadySoon;
        public static bool MoulinetEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Moulinet;

        //Use reprise only in boss fights
        public static bool UseReprise => RedMageSettings.Instance.UseReprise && Combat.Enemies.Any(e => e.IsBoss());

        public static bool FlecheEnabled => RedMageSettings.Instance.Fleche;

        public static bool ContreSixteEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.UseContreSixte;

        //If the user has selected to use Corps-A-Corps only in melee range, make sure we're close enough
        public static bool UseCorpsACorps => RedMageSettings.Instance.UseMelee
                                             && RedMageSettings.Instance.CorpsACorps
                                             && !(BotManager.Current.IsAutonomous && RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                                             && (!RedMageSettings.Instance.CorpsACorpsInMeleeRangeOnly
                                                 || (Core.Me.CurrentTarget != null
                                                     && Core.Me.CurrentTarget.Distance(Core.Me) <= 2 + Core.Me.CurrentTarget.CombatReach));

        public static bool EngagementEnabled => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Engagement;

        public static bool DisplacementEnabled => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Displacement;

        public static bool UseLucidDreaming => RedMageSettings.Instance.LucidDreaming && Core.Me.CurrentManaPercent < RedMageSettings.Instance.LucidDreamingManaPercent;

        //We want to use acceleration during AoE if we happen to both want to use single target procs and single target dualcast, which is possible for 3 enemies at specific level ranges
        public static bool UseAccelerationInAoe => RedMageSettings.Instance.Acceleration && UseStDualcastInAoe() && UseProcInAoe();

        public static bool EmboldenEnabled => RedMageSettings.Instance.Embolden;
        public static bool EmboldenReadySoon => EmboldenEnabled && SmUtil.SyncedLevel >= Spells.Embolden.LevelAcquired && Spells.Embolden.Cooldown.TotalMilliseconds <= 7500;

        public static bool ManaficationEnabled => RedMageSettings.Instance.Manafication;
        public static bool ManaficationUp =>
               ManaficationEnabled
            && SmUtil.SyncedLevel >= Spells.Manafication.LevelAcquired
            && ActionManager.HasSpell(Spells.Manafication.Id)
            && Spells.Manafication.Cooldown == TimeSpan.Zero;

        public static bool SwiftcastReadySt => RedMageSettings.Instance.SwiftcastVerthunderVeraero && SwiftcastUp;
        public static bool SwiftcastReadyAoe => SwiftcastScatter && SwiftcastUp;
        public static bool SwiftcastUp => SmUtil.SyncedLevel >= Spells.Swiftcast.LevelAcquired && Spells.Swiftcast.Cooldown == TimeSpan.Zero;
        #endregion

        #region Aoe/Single Target determination
        //Go into AoE mode if there are enough enemies and we're high enough level to have our first AoE spell
        public static bool AoeMode => RedMageSettings.Instance.UseAoe && AoeTargets >= MinTargetsForAoeMode && SmUtil.SyncedLevel >= Spells.Verthunder2.LevelAcquired;

        public static bool ShouldVerthunder2St => (BlackMana <= WhiteMana) ? HardcastAoeInSt() : false;
        public static bool ShouldVeraero2St => (WhiteMana <= BlackMana) ? HardcastAoeInSt() : false;
        //At certain level ranges, it's more efficient to use the AoE hardcast spells for fewer than three enemies
        public static bool HardcastAoeInSt()
        {
            if (!Ver2Enabled)
                return false;

            if (SmUtil.SyncedLevel < Spells.Jolt2.LevelAcquired)
                return AoeTargets >= 2;

            return false;
        }

        //At certain level ranges, it's more efficient to use Scatter/Impact for fewer than three enemies
        public static bool ScatterInSt()
        {
            if (!ScatterEnabled)
                return false;

            if (!MeHasAura(Auras.Dualcast))
                return false;

            if (SmUtil.SyncedLevel >= Spells.Impact.LevelAcquired)
            {
                return AoeTargets >= 2;
            }
            else if (SmUtil.SyncedLevel >= Spells.Jolt2.LevelAcquired)
            {
                return AoeTargets >= 4;
            }
            else if (SmUtil.SyncedLevel >= Spells.Scatter.LevelAcquired)
            {
                return AoeTargets >= 3;
            }
            else
            {
                return false;
            }
        }

        public static bool ShouldVerfireAoe => (WhiteMana < BlackMana || !MeHasAura(Auras.VerfireReady)) ? false : UseProcInAoe();
        public static bool ShouldVerstoneAoe => (BlackMana < WhiteMana || !MeHasAura(Auras.VerstoneReady)) ? false : UseProcInAoe();
        //At certain level ranges, it's more efficient to use the Verfire and Verstone even when in AoE mode, for some numbers of enemies
        public static bool UseProcInAoe()
        {
            //If user has disabled Veraero II and Verthunder II, use the procs if available
            if (!Ver2Enabled)
                return true;

            if (SmUtil.SyncedLevel >= 78) //Enhanced Contre Sixte trait raises Verthunder2 and Veraero2 from 100 to 120 potency @ 78
            {
                return AoeTargets <= 2;
            }
            else if (SmUtil.SyncedLevel >= Spells.Jolt2.LevelAcquired)
            {
                return AoeTargets <= 3;
            }
            else
            {
                return AoeTargets <= 2;
            }
        }

        public static bool ShouldVerthunderAoe => (BlackMana <= WhiteMana || (MeHasAura(Auras.VerstoneReady) && !MeHasAura(Auras.VerfireReady))) ? UseStDualcastInAoe() : false;
        public static bool ShouldVeraeroAoe => (WhiteMana <= BlackMana || (MeHasAura(Auras.VerfireReady) && !MeHasAura(Auras.VerstoneReady))) ? UseStDualcastInAoe() : false;
        //At certain level ranges, it's more efficient to use the Verthunder and Veraero even when in AoE mode, for some numbers of enemies
        public static bool UseStDualcastInAoe()
        {
            if (!MeHasAura(Auras.Dualcast))
                return false;

            if (!ScatterEnabled)
                return true;

            if (SmUtil.SyncedLevel >= Spells.Impact.LevelAcquired)
            {
                return AoeTargets <= 1;
            }
            else if (SmUtil.SyncedLevel >= Spells.Jolt2.LevelAcquired)
            {
                return AoeTargets <= 3;
            }
            else
            {
                return AoeTargets <= 2;
            }
        }
        #endregion

        #region Targeting
        public static bool CurrentTargetIsBoss => Core.Me.CurrentTarget != null && Core.Me.CurrentTarget.IsBoss();
        public static bool BossIsPresent => Combat.Enemies.Any(e => e.IsBoss());

        public static bool OutsideComboRange => (Core.Me.CurrentTarget == null || Core.Me.CurrentTarget == Core.Me) ? false : Core.Me.Distance(Core.Me.CurrentTarget) > 3.4 + Core.Me.CombatReach + Core.Me.CurrentTarget.CombatReach;

        public static BattleCharacter BestAoeTarget => BestTarget(25, 5);
        public static BattleCharacter BestContreSixteTarget => BestTarget(25, 6);
        //Find the AoE target that will hit the most others for a given spell range and radius. If there's a tie, pick the one with the most health left.
        public static BattleCharacter BestTarget(double spellRange, double spellRadius)
        {
            if (!RedMageSettings.Instance.UseSmartTargeting)
                return Core.Me.CurrentTarget as BattleCharacter;

            if (Core.Me.CurrentTarget == null || Core.Me.CurrentTarget == Core.Me || !Core.Me.CurrentTarget.InView())
                return null;

            return Combat.Enemies.Where(t => t.InView()
                                             && t.Distance(Core.Me) <= spellRange + Core.Me.CombatReach + t.CombatReach)
                                 .OrderByDescending(t => Combat.Enemies.Where(e => e.Distance(t) <= spellRadius + e.CombatReach).Count())
                                 .ThenByDescending(t => t.CurrentHealthPercent)
                                 .FirstOrDefault();
        }

        //Number of enemies within the given distance of the given target. This method takes into account each enemy's combat reach.
        public static IEnumerable<GameObject> EnemiesWithinOf(double distance, GameObject target)
        {
            if (target == null)
            {
                return new List<GameObject>();
            }
            return Combat.Enemies.Where(e => target.Distance(e) <= distance + e.CombatReach);
        }
        public static int AoeTargets => EnemiesWithinOf(5, BestTarget(25, 5)).Count();
        #endregion

        #region Combo
        //Can we start a combo?
        public static bool UseRiposte => RedMageSettings.Instance.UseMelee && (!RedMageSettings.Instance.MeleeComboBossesOnly || CurrentTargetIsBoss);

        //Ready to start a combo (below level for Redoublement)
        public static bool ReadyForComboNoRedoublement => BlackMana >= 55 && WhiteMana >= 55 && SmUtil.SyncedLevel < Spells.Redoublement.LevelAcquired && SmUtil.SyncedLevel >= Spells.Zwerchhau.LevelAcquired;
        //Ready to start a combo (below level for Zwerchhau)
        public static bool ReadyForComboNoZwerchhau => BlackMana >= 30 && WhiteMana >= 30 && SmUtil.SyncedLevel < Spells.Zwerchhau.LevelAcquired;

        //How much mana do we need to start a combo? This returns 50 if manafication is down, or the user's configured value for using manafication if it's up.
        public static int ComboTargetMana =>
               ManaficationUp
            && (BlackMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana
                || WhiteMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana) ? RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana : 50;

        public static bool UseManaficationSt =>
               ManaficationEnabled
            && (!RedMageSettings.Instance.MeleeComboBossesOnly || BossIsPresent)
            && WithinManaOfManafication(0)
            && WhiteMana <= RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana
            && BlackMana <= RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana;
        public static bool WithinManaOfManafication(int distance) => WithinManaOf(distance, RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana);
        public static bool WithinManaOf(int distance, int target) => WhiteMana >= target - distance && BlackMana >= target - distance;

        //Can we use acceleration in the combo to get a proc from Verflare or Verholy that we otherwise wouldn't get?
        public static bool UseAccelerationInCombo =>
               RedMageSettings.Instance.Acceleration
            && SmUtil.SyncedLevel >= Spells.Acceleration.LevelAcquired
            && Spells.Acceleration.Cooldown.TotalMilliseconds <= 3500
            && (WhiteMana == BlackMana
                || (WhiteMana > BlackMana && !MeHasAura(Auras.VerstoneReady) && MeHasAura(Auras.VerfireReady) && WhiteMana - BlackMana <= 9)
                || (BlackMana > WhiteMana && !MeHasAura(Auras.VerfireReady) && MeHasAura(Auras.VerstoneReady) && BlackMana - WhiteMana <= 9));
        //Avoid using Acceleration if we'll be casting Manafication soon, because we'll waste it while the combo's going on
        public static bool AvoidAccelerationSt =>
               !RedMageSettings.Instance.Acceleration
            || (ManaficationEnabled
                && SmUtil.SyncedLevel >= Spells.Manafication.LevelAcquired
                && ActionManager.HasSpell(Spells.Manafication.Id)
                && Spells.Manafication.Cooldown.TotalMilliseconds <= 7000
                && (BlackMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana || WhiteMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                && WithinManaOfManafication(10));
        #endregion

        #region AoE finisher
        public static bool DoEmboldenBurst =>
               EmboldenEnabled
            && ((BlackMana == 100 && WhiteMana == 100) //Embolden, 5 moulinet
                || (BlackMana >= 90 && WhiteMana >= 90 && ManaficationUp) //Embolden, 2 moulinet, Manafication, 5 moulinet
                || MeHasAura(Auras.Manafication)) //Manafication, 5 moulinet
            && EnoughEnemiesToStartBurst;

        public static bool DoManaficationBurst =>
               ManaficationEnabled
            && BlackMana >= 50
            && WhiteMana >= 50
            && BlackMana <= 65
            && WhiteMana <= 65
            && EnoughEnemiesToStartBurst;

        //We can start the burst if we'll hit enough enemies with Moulinet and they all have enough health to make it worth it
        public static bool EnoughEnemiesToStartBurst =>
               MoulinetEnabled
            && EnemiesWithinOf(8 + Core.Me.CombatReach, Core.Me).Count(r => InMoulinetArc(r)
                                                                            && r.CurrentHealthPercent >= RedMageSettings.Instance.EmboldenFinisherPercent) >= MinAoeBurstStartEnemies;
        public static bool InMoulinetArc(GameObject target)
        {
            if (target == null)
                return false;

            return target.RadiansFromPlayerHeading() < 1.361f; //Translated from radians, this is ~78 degrees left or right;
        }
        #endregion

        #region Aura level sync
        //This code is to prevent the state machine from seeing that we have an aura that we shouldn't have yet at the synced level
        public static Dictionary<uint, int> AuraLevelsAcquiredDict = new Dictionary<uint, int>()
        {
            { Auras.Dualcast,      1 },
            { Auras.VerfireReady,  Spells.Verfire.LevelAcquired },
            { Auras.VerstoneReady, Spells.Verstone.LevelAcquired },
            { Auras.Embolden,      Spells.Embolden.LevelAcquired },
            { Auras.Manafication,  Spells.Manafication.LevelAcquired }
        };

        public static bool MeHasAura(uint aura) => SmUtil.SyncedLevel < AuraLevelsAcquiredDict[aura] ? false : Core.Me.HasAura(aura);
        public static bool MeHasAnyAura(List<uint> auras) => Core.Me.HasAnyAura(auras.Where(a => SmUtil.SyncedLevel >= AuraLevelsAcquiredDict[a]).ToList());
        public static bool MeHasAllAuras(List<uint> auras) => auras.Any(a => SmUtil.SyncedLevel < AuraLevelsAcquiredDict[a]) ? false : Core.Me.HasAllAuras(auras.Where(a => SmUtil.SyncedLevel >= AuraLevelsAcquiredDict[a]).ToList());
        #endregion

        public static bool CanContinueComboAfter(SpellData LastSpellExecuted)
        {
            if (ActionManager.ComboTimeLeft <= 0)
                return false;

            if (ActionManager.LastSpell.Id != LastSpellExecuted.Id)
                return false;

            return true;
        }
    }
}
