using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.RedMage;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.Routines;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;
using Auras = Magitek.Utilities.Auras;
using CombatUtil = Magitek.Utilities.Combat;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Rotations
{
    public enum RdmStateIds
    {
        Start,
        RiposteAtMost,
        Aoe,
        AoeSwiftcast,
        AoeFirstWeave,
        AoeSecondWeave,
        FishForProcsUntil60Mana,
        FishForProcsSwiftcast,
        FishForProcsFirstWeave,
        FishForProcsSecondWeave,
        FishForProcsNeitherProcUpDualcast,
        FishForProcsNeitherProcUpNoDualcast,
        FishForProcsVerstoneUpDualcast,
        FishForProcsVerstoneUpNoDualcast,
        FishForProcsVerfireUpDualcast,
        FishForProcsVerfireUpNoDualcast,
        FishForProcsBothProcsUpDualcast,
        FishForProcsBothProcsUpNoDualcast,
        EschewFishingUntil80Mana,
        EschewFishingSwiftcast,
        EschewFishingFirstWeave,
        EschewFishingSecondWeave,
        EschewFishingNeitherProcUpDualcast,
        EschewFishingNeitherProcUpNoDualcast,
        EschewFishingVerstoneUpDualcast,
        EschewFishingVerstoneUpNoDualcast,
        EschewFishingVerfireUpDualcast,
        EschewFishingVerfireUpNoDualcast,
        EschewFishingBothProcsUpDualcast,
        EschewFishingBothProcsUpNoDualcast,
        PrepareProcsForCombo,
        PrepareProcsFirstWeave,
        PrepareProcsSecondWeave,
        PrepareProcsNeitherProcUpNoDualcast,
        PrepareProcsNeitherProcUpDualcast,
        PrepareProcsVerstoneUpDualcast,
        PrepareProcsVerstoneUpNoDualcast,
        PrepareProcsVerfireUpDualcast,
        PrepareProcsVerfireUpNoDualcast,
        PrepareProcsBothProcsUpNoDualcast,
        PrepareProcsBothProcsUpDualcast,
        OverwriteProc,
        ReadyForCombo,
        Zwerchhau,
        Redoublement,
        VerflareOrVerholy,
        Scorch,
        SingleTargetScatter,
        Moving,
        MovingSwiftcast
    }

    //TODO: Add docs explain all this
    //TODO: Fix spacing on all multi-line boolean expressions
    //TODO: Document SM stuff and write an explanation of how to use it
    public static class RedMage
    {
        private static StateMachine<RdmStateIds> mStateMachine;

        #region Constants
        //TODO: Document these and organize them for maximum understandability
        private const int GcdBufferMs = 350;

        private const int EnhancedContreSixteLevelAcquired = 78;
        private const int EnhancedJoltLevelAcquired = 62;

        private const int ManaForFullCombo = 80;
        private const int ManaForZwerchhauCombo = 55;
        private const int ManaForRiposteOnly = 30;
        private const int MinManaToStartManaficationAoeBurst = 50;
        private const int MaxManaToStartManaficationAoeBurst = 65;

        private const double MeleeRange = 3.4;
        private const double LimitedCorpsACorpsRange = 2;
        private const double GenericAoeSpellRange = 25;
        private const double GenericAoeSpellRadius = 5;
        private const double ContreSixteRadius = 6;
        private const double MoulinetRadius = 8;
        private const float MoulinetArc = 1.361f; //Translated from radians, this is ~78 degrees left or right of player heading

        private const int EmboldenSoonTime = 7500; //Used to decide whether to do a single moulinet to avoid overcapping, or wait (up to this amount of time) for embolden to come up
        private const int MaxAccelCooldownToBeReadyForCombo = 7000; //How soon accel must be up when we start a combo to be able to use it before Verflare/Verholy
        private const int MaxManaficationCooldownToBeReadyForCombo = 7000; //Used to determine if manafication will be up by the time we get to the target mana levels

        private const int MinTargetsForAoeMode = 3;
        private const int MinAoeBurstStartEnemies = 3;
        private const int MinMoulinetEnemies = 3; //According to The Balance, melee combo is better against 2 enemies

        private const int MinTargetsToHardcastAoeDuringStPhaseBeforeJolt2Acquired = 2;

        private const int MinTargetsToUseScatterInStPhaseWithImpact = 2;
        private const int MinTargetsToUseScatterInStPhaseWithEnhancedJoltWithoutImpact = 4;
        private const int MinTargetsToUseScatterInStPhaseWithoutEnhancedJolt = 3;

        private const int MaxTargetsToUseStProcInAoePhaseWithEnhancedContreSixte = 2;
        private const int MaxTargetsToUseStProcInAoePhaseWithEnhancedJoltWithoutEnhancedContreSixte = 3;
        private const int MaxTargetsToUseStProcInAoePhaseWithoutEnhancedJolt = 2;

        private const int MaxTargetsToDualcastStInAoePhaseWithImpact = 1;
        private const int MaxTargetsToDualcastStInAoePhaseWithEnhancedJoltWithoutImpact = 3;
        private const int MaxTargetsToDualcastStInAoePhaseWithoutEnhancedJolt = 2;
        #endregion

        #region General status
        private static int Cap(int mana) => Math.Min(100, mana);
        private static int CapLoss(int moreWhite, int moreBlack) => (WhiteMana + BlackMana + moreWhite + moreBlack) - (Cap(WhiteMana + moreWhite) + Cap(BlackMana + moreBlack));

        private static double GcdLeft => Math.Max(Spells.Riposte.Cooldown.TotalMilliseconds - GcdBufferMs, 0);
        #endregion

        #region Skill status
        private static bool Ver2Enabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Ver2;

        private static bool ScatterEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Scatter;
        private static bool SwiftcastScatter => ScatterEnabled && RedMageSettings.Instance.SwiftcastScatter;

        private static bool UseMoulinet => MoulinetEnabled && MoulinetTargets.Count(r => InMoulinetArc(r)) >= MinMoulinetEnemies;
        private static bool BurnMoulinet => UseMoulinet && !EmboldenReadySoon;
        private static bool MoulinetEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.Moulinet;

        private static bool UseReprise => RedMageSettings.Instance.UseReprise && CombatUtil.Enemies.Any(e => e.IsBoss());

        private static bool FlecheEnabled => RedMageSettings.Instance.Fleche;

        private static bool ContreSixteEnabled => RedMageSettings.Instance.UseAoe && RedMageSettings.Instance.UseContreSixte;

        private static bool UseCorpsACorps => RedMageSettings.Instance.UseMelee
                                              && RedMageSettings.Instance.CorpsACorps
                                              && (!RedMageSettings.Instance.CorpsACorpsInMeleeRangeOnly
                                                  || (Core.Me.CurrentTarget != null
                                                      && Core.Me.CurrentTarget.Distance(Core.Me) <= LimitedCorpsACorpsRange + Core.Me.CurrentTarget.CombatReach));

        private static bool EngagementEnabled => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Engagement;

        private static bool DisplacementEnabled => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Displacement;

        private static bool UseLucidDreaming => RedMageSettings.Instance.LucidDreaming && Core.Me.CurrentManaPercent < RedMageSettings.Instance.LucidDreamingManaPercent;

        private static bool UseAccelerationInAoe => RedMageSettings.Instance.Acceleration && DualcastStInAoe() && CastStProcInAoe();

        private static bool EmboldenEnabled => RedMageSettings.Instance.Embolden;
        private static bool EmboldenReadySoon => EmboldenEnabled && SmUtil.SyncedLevel >= Spells.Embolden.LevelAcquired && Spells.Embolden.Cooldown.TotalMilliseconds <= EmboldenSoonTime;

        private static bool ManaficationEnabled => RedMageSettings.Instance.Manafication;
        private static bool ManaficationUpIn(int ms) =>
               ManaficationEnabled
            && SmUtil.SyncedLevel >= Spells.Manafication.LevelAcquired
            && ActionManager.HasSpell(Spells.Manafication.Id)
            && Spells.Manafication.Cooldown.TotalDays <= ms;
        private static bool ManaficationUp => ManaficationUpIn(0);

        private static bool SwiftcastReadySt => RedMageSettings.Instance.SwiftcastVerthunderVeraero && SwiftcastUp;
        private static bool SwiftcastReadyAoe => SwiftcastScatter && SwiftcastUp;
        private static bool SwiftcastUp => SmUtil.SyncedLevel >= Spells.Swiftcast.LevelAcquired && Spells.Swiftcast.Cooldown == TimeSpan.Zero;
        #endregion

        #region Aoe/Single Target determination
        private static bool AoeMode => RedMageSettings.Instance.UseAoe && AoeTargetCount >= MinTargetsForAoeMode && SmUtil.SyncedLevel >= Spells.Verthunder2.LevelAcquired;

        private static bool ShouldVerthunder2St => BlackMana <= WhiteMana ? HardcastAoeInSt() : false;
        private static bool ShouldVeraero2St => WhiteMana <= BlackMana ? HardcastAoeInSt() : false;
        private static bool HardcastAoeInSt()
        {
            if (!Ver2Enabled)
                return false;

            if (SmUtil.SyncedLevel < Spells.Jolt2.LevelAcquired)
                return AoeTargetCount >= MinTargetsToHardcastAoeDuringStPhaseBeforeJolt2Acquired;

            return false;
        }

        private static bool ScatterInSt()
        {
            if (!ScatterEnabled || !MeHasAura(Auras.Dualcast))
                return false;

            if (SmUtil.SyncedLevel >= Spells.Impact.LevelAcquired)
            {
                return AoeTargetCount >= MinTargetsToUseScatterInStPhaseWithImpact;
            }
            else if (SmUtil.SyncedLevel >= EnhancedJoltLevelAcquired) //Enhanced Jolt raises Verthunder and Veraero to 370 potency @ 62
            {
                return AoeTargetCount >= MinTargetsToUseScatterInStPhaseWithEnhancedJoltWithoutImpact;
            }
            else if (SmUtil.SyncedLevel >= Spells.Scatter.LevelAcquired)
            {
                return AoeTargetCount >= MinTargetsToUseScatterInStPhaseWithoutEnhancedJolt;
            }

            return false;
        }

        private static bool ShouldVerfireAoe => (WhiteMana < BlackMana || !MeHasAura(Auras.VerfireReady)) ? false : CastStProcInAoe();
        private static bool ShouldVerstoneAoe => (BlackMana < WhiteMana || !MeHasAura(Auras.VerstoneReady)) ? false : CastStProcInAoe();
        private static bool CastStProcInAoe()
        {
            //If user has disabled Veraero II and Verthunder II, use the procs if available
            if (!Ver2Enabled)
                return true;

            if (SmUtil.SyncedLevel >= EnhancedContreSixteLevelAcquired) //Enhanced Contre Sixte trait raises Verthunder2 and Veraero2 from 100 to 120 potency @ 78
            {
                return AoeTargetCount <= MaxTargetsToUseStProcInAoePhaseWithEnhancedContreSixte;
            }
            else if (SmUtil.SyncedLevel >= EnhancedJoltLevelAcquired) //Enhanced Jolt raises Verfire and Verstone to 300 potency @ 62
            {
                return AoeTargetCount <= MaxTargetsToUseStProcInAoePhaseWithEnhancedJoltWithoutEnhancedContreSixte;
            }
            else
            {
                return AoeTargetCount <= MaxTargetsToUseStProcInAoePhaseWithoutEnhancedJolt;
            }
        }

        private static bool ShouldVerthunderAoe => (MeHasAura(Auras.Dualcast) && (BlackMana <= WhiteMana || (MeHasAura(Auras.VerstoneReady) && !MeHasAura(Auras.VerfireReady)))) ? DualcastStInAoe() : false;
        private static bool ShouldVeraeroAoe => (MeHasAura(Auras.Dualcast) && (WhiteMana <= BlackMana || (MeHasAura(Auras.VerfireReady) && !MeHasAura(Auras.VerstoneReady)))) ? DualcastStInAoe() : false;
        private static bool DualcastStInAoe()
        {
            if (!ScatterEnabled)
                return true;

            if (SmUtil.SyncedLevel >= Spells.Impact.LevelAcquired)
            {
                return AoeTargetCount <= MaxTargetsToDualcastStInAoePhaseWithImpact;
            }
            else if (SmUtil.SyncedLevel >= EnhancedJoltLevelAcquired) //Enhanced Jolt raises Verthunder and Veraero to 370 potency @ 62
            {
                return AoeTargetCount <= MaxTargetsToDualcastStInAoePhaseWithEnhancedJoltWithoutImpact;
            }
            else
            {
                return AoeTargetCount <= MaxTargetsToDualcastStInAoePhaseWithoutEnhancedJolt;
            }
        }
        #endregion

        #region Targeting
        private static bool OutsideComboRange => (Core.Me.CurrentTarget == null || Core.Me.CurrentTarget == Core.Me) ? false : Core.Me.Distance(Core.Me.CurrentTarget) > MeleeRange + Core.Me.CombatReach + Core.Me.CurrentTarget.CombatReach;

        private static BattleCharacter BestAoeTarget => BestTarget(GenericAoeSpellRange, GenericAoeSpellRadius);
        private static BattleCharacter BestContreSixteTarget => BestTarget(GenericAoeSpellRange, ContreSixteRadius);
        private static BattleCharacter BestTarget(double spellRange, double spellRadius)
        {
            if (!RedMageSettings.Instance.UseSmartTargeting)
                return Core.Me.CurrentTarget as BattleCharacter;

            if (Core.Me.CurrentTarget == null || Core.Me.CurrentTarget == Core.Me || !Core.Me.CurrentTarget.InView())
                return null;

            return CombatUtil.Enemies.Where(t =>    t.InView()
                                                 && t.Distance(Core.Me) <= spellRange + Core.Me.CombatReach + t.CombatReach)
                                     .OrderByDescending(t => CombatUtil.Enemies.Where(e => e.Distance(t) <= spellRadius + e.CombatReach).Count())
                                     .ThenByDescending(t => t.CurrentHealthPercent)
                                     .FirstOrDefault();
        }

        private static IEnumerable<GameObject> EnemiesWithinOf(double distance, GameObject target)
        {
            if (target == null)
            {
                return new List<GameObject>();
            }
            return CombatUtil.Enemies.Where(e => target.Distance(e) <= distance + e.CombatReach);
        }
        private static int AoeTargetCount => EnemiesWithinOf(GenericAoeSpellRadius, BestAoeTarget).Count();
        #endregion

        #region Combo
        //TODO: Make sure these all belong here
        private static bool CanComboCurrentTargetSt => !RedMageSettings.Instance.MeleeComboBossesOnly || (Core.Me.CurrentTarget != null && Core.Me.CurrentTarget.IsBoss());
        private static bool CanComboSomeEnemySt => !RedMageSettings.Instance.MeleeComboBossesOnly || CombatUtil.Enemies.Any(e => e.IsBoss());
        private static bool UseRiposte => RedMageSettings.Instance.UseMelee && CanComboCurrentTargetSt;

        private static bool ReadyForComboNoRedoublement => BlackMana >= ManaForZwerchhauCombo && WhiteMana >= ManaForZwerchhauCombo && SmUtil.SyncedLevel < Spells.Redoublement.LevelAcquired && SmUtil.SyncedLevel >= Spells.Zwerchhau.LevelAcquired;
        private static bool ReadyForComboNoZwerchhau => BlackMana >= ManaForRiposteOnly && WhiteMana >= ManaForRiposteOnly && SmUtil.SyncedLevel < Spells.Zwerchhau.LevelAcquired;

        private static int ComboTargetMana =>
               ManaficationUpIn(2500)
            && (BlackMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana
                || WhiteMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana) ? RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana : ManaForFullCombo;

        private static bool UseManaficationSt =>
               ManaficationEnabled
            && CanComboSomeEnemySt
            && WithinManaOfManafication(0)
            && WhiteMana <= RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana
            && BlackMana <= RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana;
        private static bool WithinManaOfManafication(int distance) => WithinManaOf(RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana, distance);
        private static bool WithinManaOf(int distance, int manaLevel) => WhiteMana >= manaLevel - distance && BlackMana >= manaLevel - distance;

        private static bool UseAccelerationInCombo =>
               RedMageSettings.Instance.Acceleration
            && SmUtil.SyncedLevel >= Spells.Acceleration.LevelAcquired
            && Spells.Acceleration.Cooldown.TotalMilliseconds <= MaxAccelCooldownToBeReadyForCombo
            && (WhiteMana == BlackMana
                || (WhiteMana > BlackMana && !MeHasAura(Auras.VerstoneReady) && MeHasAura(Auras.VerfireReady) && WhiteMana - BlackMana <= 9) ///More than 9 apart will overcap mana
                || (BlackMana > WhiteMana && !MeHasAura(Auras.VerfireReady) && MeHasAura(Auras.VerstoneReady) && BlackMana - WhiteMana <= 9));
        //Avoid Acceleration if we'll be casting Manafication soon
        private static bool AvoidAccelerationSt =>
               !RedMageSettings.Instance.Acceleration
            || (ManaficationEnabled
                && SmUtil.SyncedLevel >= Spells.Manafication.LevelAcquired
                && ActionManager.HasSpell(Spells.Manafication.Id)
                && Spells.Manafication.Cooldown.TotalMilliseconds <= MaxManaficationCooldownToBeReadyForCombo
                && (BlackMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana || WhiteMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                && WithinManaOfManafication(10));
        #endregion

        #region AoE finisher
        private static bool DoEmboldenBurst =>
               EmboldenEnabled
            && (   (BlackMana == 100 && WhiteMana == 100) //Embolden, 5 moulinet
                || (BlackMana >= 90 && WhiteMana >= 90 && ManaficationUp) //Embolden, 2 moulinet, Manafication, 5 moulinet
                || MeHasAura(Auras.Manafication)) //Manafication, 5 moulinet
            && CanStartBurst;

        private static bool DoManaficationBurst =>
               ManaficationEnabled
            && BlackMana >= MinManaToStartManaficationAoeBurst
            && WhiteMana >= MinManaToStartManaficationAoeBurst
            && BlackMana <= MaxManaToStartManaficationAoeBurst
            && WhiteMana <= MaxManaToStartManaficationAoeBurst
            && CanStartBurst;

        //We can start the burst if we'll hit enough enemies with Moulinet and they all have enough health to make it worth it
        private static bool CanStartBurst =>
            MoulinetEnabled && MoulinetTargets.Count(r =>    InMoulinetArc(r)
                                                          && r.CurrentHealthPercent >= RedMageSettings.Instance.EmboldenFinisherPercent) >= MinAoeBurstStartEnemies;

        private static IEnumerable<GameObject> MoulinetTargets => EnemiesWithinOf(MoulinetRadius + Core.Me.CombatReach, Core.Me);

        private static bool InMoulinetArc(GameObject target)
        {
            if (target == null)
                return false;

            return target.RadiansFromPlayerHeading() < MoulinetArc;
        }
        #endregion

        #region Aura level sync
        private static Dictionary<uint, int> AuraLevelsAcquiredDict = new Dictionary<uint, int>()
        {
            { Auras.Dualcast,      1 },
            { Auras.VerfireReady,  Spells.Verfire.LevelAcquired },
            { Auras.VerstoneReady, Spells.Verstone.LevelAcquired },
            { Auras.Embolden,      Spells.Embolden.LevelAcquired },
            { Auras.Manafication,  Spells.Manafication.LevelAcquired }
        };

        //These methods pevent the state machine from seeing that we have an aura that we shouldn't have yet at the synced level
        private static bool MeHasAura(uint aura) => SmUtil.SyncedLevel < AuraLevelsAcquiredDict[aura] ? false : Core.Me.HasAura(aura);
        private static bool MeHasAnyAura(List<uint> auras) => Core.Me.HasAnyAura(auras.Where(a => SmUtil.SyncedLevel >= AuraLevelsAcquiredDict[a]).ToList());
        private static bool MeHasAllAuras(List<uint> auras) => auras.Any(a => SmUtil.SyncedLevel < AuraLevelsAcquiredDict[a]) ? false : Core.Me.HasAllAuras(auras.Where(a => SmUtil.SyncedLevel >= AuraLevelsAcquiredDict[a]).ToList());
        #endregion

        static RedMage()
        {
            List<uint> mBothProcs = new List<uint>() { Auras.VerfireReady, Auras.VerstoneReady };
            List<uint> mBothProcsAndDualcast = new List<uint>() { Auras.VerfireReady, Auras.VerstoneReady, Auras.Dualcast };
            List<uint> mVerstoneAndDualcast = new List<uint>() { Auras.VerstoneReady, Auras.Dualcast };
            List<uint> mVerfireAndDualcast = new List<uint>() { Auras.VerfireReady, Auras.Dualcast };
            List<uint> mManaficationAndEmbolden = new List<uint>() { Auras.Manafication, Auras.Embolden };
            
            //Potential improvements:
            //TODO: Don't cast verstone or verfire if proc will expire before it's done (it interrupts the cast)
            //TODO: Use embolden during AoE at low mana if it would sit for a really long time waiting for us to build up mana - need to take Manafication into account, because we might not be sitting for as long as we think we would if that will go off
            //TODO: What if we're moving during AoE? Need an AoeMoving state. It should probably try to swiftcast stuff and also use enchanted moulinet
            //TODO: If you're less than level 70, go right into the combo - don't bother trying to balance it. Maybe if you're 68 or 69 and you can balance it for black?
            //TODO: At levels below Moulinet, when in AoE, should we do the combo instead? Remember combo is faster (1.5s), so it's effective potency is 2/3 higher than reported
            //      Also remember that betwee 18 and 21, we'd need to balance between Verthunder 2 and Veraero (no Veraero 2 until 22)
            mStateMachine = new StateMachine<RdmStateIds>(
                RdmStateIds.Start,
                new Dictionary<RdmStateIds, State<RdmStateIds>>()
                {
                    {
                        RdmStateIds.Start,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel == 1, () => SmUtil.NoOp(), RdmStateIds.RiposteAtMost,           TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => AoeMode,                 () => SmUtil.NoOp(), RdmStateIds.Aoe,                     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                    () => SmUtil.NoOp(), RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.RiposteAtMost,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                            RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.Aoe,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,                                                                   () => SmUtil.NoOp(),                                               RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe,                                                        () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe,                                                           () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ScatterEnabled && MeHasAura(Auras.Dualcast),                                () => SmUtil.SyncedCast(Spells.Scatter, BestAoeTarget),            RdmStateIds.AoeFirstWeave),
                                //Continue a moulinet burst if we've started one
                                new StateTransition<RdmStateIds>(() => MeHasAnyAura(mManaficationAndEmbolden) && UseMoulinet,                      () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),   RdmStateIds.AoeFirstWeave),
                                //Cast moulinet if we'd otherwise cast verstone, but doing so would overcap mana
                                new StateTransition<RdmStateIds>(() => ShouldVerstoneAoe && CapLoss(12,3) > 0 && BurnMoulinet,                     () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),   RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVerstoneAoe,                                                          () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.Aoe),
                                //Cast moulinet if we'd otherwise cast veraero2, but doing so would overcap mana
                                new StateTransition<RdmStateIds>(() => Ver2Enabled && WhiteMana <= BlackMana && CapLoss(10,3) > 0 && BurnMoulinet, () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),   RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => Ver2Enabled && WhiteMana <= BlackMana,                                      () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),           RdmStateIds.Aoe),
                                //Cast moulinet if we'd otherwise cast verfire, but doing so would overcap mana
                                new StateTransition<RdmStateIds>(() => ShouldVerfireAoe && CapLoss(3,12) > 0 && BurnMoulinet,                      () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),   RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVerfireAoe,                                                           () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                //Cast moulinet if we'd otherwise cast verthunder2, but doing so would overcap mana
                                new StateTransition<RdmStateIds>(() => Ver2Enabled && CapLoss(3,10) > 0 && BurnMoulinet,                           () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),   RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => Ver2Enabled,                                                                () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),        RdmStateIds.Aoe),
                                //User disabled veraero2/verthunder2, no procs are up, and we gotta do something, so Jolt it is
                                new StateTransition<RdmStateIds>(() => !Ver2Enabled,                                                               () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.Aoe),
                            })
                    },
                    {
                        RdmStateIds.AoeSwiftcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,                                                 () => SmUtil.NoOp(),                                              RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !SwiftcastReadyAoe,                                       () => SmUtil.NoOp(),                                              RdmStateIds.Aoe,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                            () => SmUtil.NoOp(),                                              RdmStateIds.Aoe,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAnyAura(mManaficationAndEmbolden) && UseMoulinet,    () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),  RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe && CapLoss(0,11) > 0 && BurnMoulinet, () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),  RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe,                                      () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe && CapLoss(11,0) > 0 && BurnMoulinet,    () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),  RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe,                                         () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => CapLoss(3,3) > 0 && BurnMoulinet,                         () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),  RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ScatterEnabled,                                           () => SmUtil.Swiftcast(Spells.Scatter, BestAoeTarget),            RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                     () => SmUtil.NoOp(),                                              RdmStateIds.Aoe,    TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.AoeFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,             () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,       () => SmUtil.NoOp(),                                                 RdmStateIds.AoeSecondWeave,         TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst,  () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,   () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,        () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,  () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseAccelerationInAoe, () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me.CurrentTarget), RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.AoeSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.AoeSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,        () => SmUtil.NoOp(),                                                 RdmStateIds.Aoe,          TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst,  () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,   () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,        () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadyAoe,    () => SmUtil.NoOp(),                                                 RdmStateIds.AoeSwiftcast, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseAccelerationInAoe, () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me),               RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Aoe)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsUntil60Mana,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                        () => SmUtil.NoOp(), RdmStateIds.Aoe,                                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,             () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ManaficationUp && WithinManaOfManafication(20), () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ScatterInSt(),                                  () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcsAndDualcast),           () => SmUtil.NoOp(), RdmStateIds.FishForProcsBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerstoneAndDualcast),            () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerfireAndDualcast),             () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.Dualcast),                      () => SmUtil.NoOp(), RdmStateIds.FishForProcsNeitherProcUpDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoZwerchhau,                       () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoRedoublement,                    () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                       () => SmUtil.NoOp(), RdmStateIds.Moving,                              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcs),                      () => SmUtil.NoOp(), RdmStateIds.FishForProcsBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerfireReady),                  () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerstoneReady),                 () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                           () => SmUtil.NoOp(), RdmStateIds.FishForProcsNeitherProcUpNoDualcast, TransitionType.Immediate),
                            })
                    },
                    {
                        RdmStateIds.FishForProcsSwiftcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                                                   () => SmUtil.NoOp(),                                              RdmStateIds.Aoe,                     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                        () => SmUtil.NoOp(),                                              RdmStateIds.PrepareProcsForCombo,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !SwiftcastReadySt,                                                         () => SmUtil.NoOp(),                                              RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                                             () => SmUtil.NoOp(),                                              RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoZwerchhau,                                                  () => SmUtil.NoOp(),                                              RdmStateIds.ReadyForCombo,           TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoRedoublement,                                               () => SmUtil.NoOp(),                                              RdmStateIds.ReadyForCombo,           TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !MeHasAura(Auras.VerstoneReady) && WhiteMana <= BlackMana,                 () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => !MeHasAura(Auras.VerfireReady) && BlackMana <= WhiteMana,                  () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => !MeHasAura(Auras.VerstoneReady) && Cap(WhiteMana+11) <= Cap(BlackMana+30), () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => !MeHasAura(Auras.VerfireReady) && Cap(BlackMana+11) <= Cap(WhiteMana+30),  () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                                      () => SmUtil.NoOp(),                                              RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60, () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                     () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                    () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => !AvoidAccelerationSt,               () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me.CurrentTarget), RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                      () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                 () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                     () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,                () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                  () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                   () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.FishForProcsSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,        () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => !AvoidAccelerationSt, () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me),               RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,        () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,   () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt,     () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsSwiftcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.FishForProcsUntil60Mana)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana,                               () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                 () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Verthunder.LevelAcquired, () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                 () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,    () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),     RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St, () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.NoOp(),                                         RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerstoneUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+11) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerstoneUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,                      () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),         RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St,                   () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),      RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),     RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                             RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerfireUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+11) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerfireUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,                      () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),        RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St,                   () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),     RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                            RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsBothProcsUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                             RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingUntil80Mana,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                               () => SmUtil.NoOp(), RdmStateIds.Aoe,                                  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                    () => SmUtil.NoOp(), RdmStateIds.PrepareProcsForCombo,                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => (BlackMana < 60 || WhiteMana < 60) && !ManaficationUp, () => SmUtil.NoOp(), RdmStateIds.FishForProcsUntil60Mana,              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 20 || WhiteMana < 20,                      () => SmUtil.NoOp(), RdmStateIds.FishForProcsUntil60Mana,              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ScatterInSt(),                                         () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcsAndDualcast),                  () => SmUtil.NoOp(), RdmStateIds.EschewFishingBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerstoneAndDualcast),                   () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerfireAndDualcast),                    () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.Dualcast),                             () => SmUtil.NoOp(), RdmStateIds.EschewFishingNeitherProcUpDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoZwerchhau,                              () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                        TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoRedoublement,                           () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                        TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                              () => SmUtil.NoOp(), RdmStateIds.Moving,                               TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcs),                             () => SmUtil.NoOp(), RdmStateIds.EschewFishingBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerfireReady),                         () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerstoneReady),                        () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.NoOp(), RdmStateIds.EschewFishingNeitherProcUpNoDualcast, TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingSwiftcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                             () => SmUtil.NoOp(),                                              RdmStateIds.Aoe,                      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                  () => SmUtil.NoOp(),                                              RdmStateIds.PrepareProcsForCombo,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !SwiftcastReadySt,                                   () => SmUtil.NoOp(),                                              RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                       () => SmUtil.NoOp(),                                              RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoZwerchhau,                            () => SmUtil.NoOp(),                                              RdmStateIds.ReadyForCombo,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoRedoublement,                         () => SmUtil.NoOp(),                                              RdmStateIds.ReadyForCombo,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !MeHasAnyAura(mBothProcs) && WhiteMana <= BlackMana, () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => !MeHasAnyAura(mBothProcs) && BlackMana <= WhiteMana, () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                () => SmUtil.NoOp(),                                              RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                    () => SmUtil.NoOp(),                                                 RdmStateIds.PrepareProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => (BlackMana < 60 || WhiteMana < 60) && !ManaficationUp, () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 20 || WhiteMana < 20,                      () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                                        () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseManaficationSt,                                     () => SmUtil.SyncedCast(Spells.Manafication, Core.Me),               RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                       () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                         () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                    () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                        () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,                                   () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                      () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.EschewFishingSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,      () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseManaficationSt,  () => SmUtil.SyncedCast(Spells.Manafication, Core.Me),              RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,    () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,      () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled, () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt,   () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingSwiftcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,     () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,  () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,   () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.EschewFishingUntil80Mana),
                            })
                    },
                    {
                        RdmStateIds.EschewFishingNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+11) > BlackMana && BlackMana >= ComboTargetMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+11) > WhiteMana && WhiteMana >= ComboTargetMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana,                                        () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                          () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                          () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,    () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),     RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St, () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.NoOp(),                                         RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerstoneUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+11) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerstoneUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,                      () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),         RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St,                   () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),      RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),     RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                             RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerfireUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+11) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                   () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerfireUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,                      () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),        RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St,                   () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),     RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                            RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingBothProcsUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                             RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsForCombo,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                              () => SmUtil.NoOp(), RdmStateIds.Aoe,                                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,                     () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ScatterInSt(),                                        () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcsAndDualcast),                 () => SmUtil.NoOp(), RdmStateIds.PrepareProcsBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerstoneAndDualcast),                  () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mVerfireAndDualcast),                   () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.Dualcast),                            () => SmUtil.NoOp(), RdmStateIds.PrepareProcsNeitherProcUpDualcast,   TransitionType.Immediate),
                                //If manafication is up, go right to the combo so we don't drop the buff before we cast Scorch
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.Manafication),                        () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseAccelerationInCombo && !MeHasAllAuras(mBothProcs), () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoZwerchhau,                             () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ReadyForComboNoRedoublement,                          () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                             () => SmUtil.NoOp(), RdmStateIds.Moving,                              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAllAuras(mBothProcs),                            () => SmUtil.NoOp(), RdmStateIds.PrepareProcsBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerfireReady),                        () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerstoneReady),                       () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                                 () => SmUtil.NoOp(), RdmStateIds.PrepareProcsNeitherProcUpNoDualcast, TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80, () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                   () => SmUtil.NoOp(),                                                 RdmStateIds.PrepareProcsSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                  () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                    () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,               () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                   () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,              () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                 () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.PrepareProcsSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                                                                 () => SmUtil.NoOp(),                                                RdmStateIds.PrepareProcsForCombo, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                                                               () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.PrepareProcsForCombo),
                                //If mana's equal and no procs are up, we can swiftcast to get to the combo
                                new StateTransition<RdmStateIds>(() => WhiteMana == BlackMana && SwiftcastReadySt && !MeHasAnyAura(mBothProcs) && CapLoss(11,0) <= 8, () => SmUtil.Swiftcast(Spells.Veraero, Core.Me),                    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                                                                 () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                                                            () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                                                                () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                                                             () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                                                              () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                //Cast the one with less mana, if it'll surpass the larger
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana && Cap(WhiteMana+11) > BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana && Cap(BlackMana+11) > WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                //Otherwise, keep the larger larger
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+11) > BlackMana,                          () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                   () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana == WhiteMana && CapLoss(3, 14) <= 8, () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                          () => SmUtil.NoOp(),                                         RdmStateIds.ReadyForCombo,        TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerstoneUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerstoneUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana > BlackMana,                                             () => SmUtil.NoOp(),                                             RdmStateIds.ReadyForCombo,        TransitionType.Immediate),
                                //If trying to fix procs will waste too much mana or cap us at 100, it doesn't really help, so don't bother and go into the combo
                                new StateTransition<RdmStateIds>(() => CapLoss(9, 11) <= 8 && Cap(WhiteMana+9) + Cap(BlackMana+11) < 200, () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                              () => SmUtil.NoOp(),                                             RdmStateIds.ReadyForCombo,        TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerfireUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                            RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerfireUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana > WhiteMana,                                             () => SmUtil.NoOp(),                                            RdmStateIds.ReadyForCombo,        TransitionType.Immediate),
                                //If trying to fix procs will waste too much mana or cap us at 100, it doesn't really help, so don't bother and go into the combo
                                new StateTransition<RdmStateIds>(() => CapLoss(11, 9) <= 8 && Cap(WhiteMana+11) + Cap(BlackMana+9) < 200, () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                              () => SmUtil.NoOp(),                                            RdmStateIds.ReadyForCombo,        TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsBothProcsUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana && CapLoss(11, 9) <= 8 && Cap(WhiteMana+11) + Cap(BlackMana+9) < 200, () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),  RdmStateIds.OverwriteProc),
                                new StateTransition<RdmStateIds>(() => CapLoss(9, 11) <= 8 && Cap(WhiteMana+9) + Cap(BlackMana+11) < 200,                           () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.OverwriteProc),
                                new StateTransition<RdmStateIds>(() => true,                                                                                        () => SmUtil.NoOp(),                                             RdmStateIds.ReadyForCombo, TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.OverwriteProc,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80, () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !MeHasAura(Auras.Dualcast),       () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !MeHasAnyAura(mBothProcs),        () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerstoneReady),   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => MeHasAura(Auras.VerfireReady),    () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                            })
                    },
                    {
                        RdmStateIds.ReadyForCombo,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                             () => SmUtil.NoOp(),                                                RdmStateIds.Aoe,                  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EmboldenEnabled,                   () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                     () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                    () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                 () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                  () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => UseRiposte,                                          () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget),     RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => UseRiposte && (!OutsideComboRange || GcdLeft >= 50), () => SmUtil.NoOp(),                                                RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse),
                                //We're too far away, so keep casting. First check if the lower mana has a proc. If so, use it
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana,                               () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana,                               () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.PrepareProcsForCombo),
                                //Then just use any procs we can find
                                new StateTransition<RdmStateIds>(() => true,                                                () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.PrepareProcsForCombo),
                                //Then cast Jolt (or an AoE that's more efficient)
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St,                                    () => SmUtil.SyncedCast(Spells.Veraero2, BestAoeTarget),            RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St,                                 () => SmUtil.SyncedCast(Spells.Verthunder2, BestAoeTarget),         RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.PrepareProcsForCombo),
                            })
                    },
                    {
                        RdmStateIds.Zwerchhau,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Zwerchhau.LevelAcquired, () => SmUtil.NoOp(),                                                RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                    () => SmUtil.NoOp(),                                                RdmStateIds.Start, TransitionType.Immediate),
                                //We need to give the action manager time to notice there's a combo going, otherwise we sometimes leave this state immediately
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50 && ActionManager.ComboTimeLeft == 0,    () => SmUtil.NoOp(),                                                RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => OutsideComboRange && GcdLeft < 50,                   () => SmUtil.NoOp(),                                                RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EmboldenEnabled,                   () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                     () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                    () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                 () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                  () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => true,                                                () => SmUtil.SyncedCast(Spells.Zwerchhau, Core.Me.CurrentTarget),   RdmStateIds.Redoublement)
                            })
                    },
                    {
                        RdmStateIds.Redoublement,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Redoublement.LevelAcquired, () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                       () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ActionManager.ComboTimeLeft == 0,                       () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => OutsideComboRange && GcdLeft < 50,                      () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EmboldenEnabled,                      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                        () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                   () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => true,                                                   () => SmUtil.SyncedCast(Spells.Redoublement, Core.Me.CurrentTarget), RdmStateIds.VerflareOrVerholy)
                            })
                    },
                    {
                        RdmStateIds.VerflareOrVerholy,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            { 
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Verflare.LevelAcquired,                                               () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ActionManager.ComboTimeLeft == 0,                                                                 () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseAccelerationInCombo && !MeHasAllAuras(mBothProcs),                           () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me),               RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EmboldenEnabled,                                                                () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                                                                  () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                                                             () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                                                                 () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && DisplacementEnabled,                                                           () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                                                              () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                                                               () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana && !MeHasAura(Auras.VerstoneReady),                                         () => SmUtil.SyncedCast(Spells.Verholy, Core.Me.CurrentTarget),      RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana && !MeHasAura(Auras.VerfireReady),                                          () => SmUtil.SyncedCast(Spells.Verflare, Core.Me.CurrentTarget),     RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => (!MeHasAura(Auras.VerstoneReady) && WhiteMana+21 <= BlackMana+30) || BlackMana+21 > WhiteMana+30, () => SmUtil.SyncedCast(Spells.Verholy, Core.Me.CurrentTarget),      RdmStateIds.Scorch),
                                //It's important that verflare is last, because if we'd like to cast verholy but we don't have it yet, we'll fall through to here, and we need to cast verflare to get out of this state
                                new StateTransition<RdmStateIds>(() => true,                                                                                             () => SmUtil.SyncedCast(Spells.Verflare, Core.Me.CurrentTarget),     RdmStateIds.Scorch),
                            })
                    },
                    {
                        RdmStateIds.Scorch,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Scorch.LevelAcquired, () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ActionManager.ComboTimeLeft == 0,                 () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EmboldenEnabled,                () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                  () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,             () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                 () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && DisplacementEnabled,           () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,              () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,               () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => true,                                             () => SmUtil.SyncedCast(Spells.Scorch, Core.Me.CurrentTarget),       RdmStateIds.Start)
                            })
                    },
                    {
                        RdmStateIds.SingleTargetScatter,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ScatterEnabled && MeHasAura(Auras.Dualcast), () => SmUtil.SyncedCast(Spells.Scatter, BestAoeTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                        () => SmUtil.NoOp(),                                    RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.Moving,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && GcdLeft <= 1400 && SwiftcastReadySt, () => SmUtil.NoOp(),                                                 RdmStateIds.MovingSwiftcast, TransitionType.NextPulse),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                       () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                  () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                      () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && DisplacementEnabled,                () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                   () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                    () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50 && UseReprise,                            () => SmUtil.SyncedCast(Spells.Reprise, Core.Me.CurrentTarget),      RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50 && SwiftcastReadySt,                      () => SmUtil.NoOp(),                                                 RdmStateIds.MovingSwiftcast, TransitionType.NextPulse),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && FlecheEnabled,                         () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && ContreSixteEnabled,                    () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && UseCorpsACorps,                        () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && DisplacementEnabled,                   () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && EngagementEnabled,                     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.NoOp(),                                                 RdmStateIds.Start,           TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.MovingSwiftcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                                                                () => SmUtil.NoOp(),                                              RdmStateIds.Start, TransitionType.NextPulse),
                                new StateTransition<RdmStateIds>(() => !MeHasAnyAura(mBothProcs) && WhiteMana <= BlackMana,                                                               () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => !MeHasAnyAura(mBothProcs) && BlackMana <= WhiteMana,                                                               () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => (BlackMana <= 60 || WhiteMana <= 60) && !MeHasAura(Auras.VerstoneReady) && Cap(WhiteMana+11) <= Cap(BlackMana+30), () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => (BlackMana <= 60 || WhiteMana <= 60) && !MeHasAura(Auras.VerfireReady) && Cap(BlackMana+11) <= Cap(WhiteMana+30),  () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana,                                                                                            () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana,                                                                                            () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true,                                                                                                              () => SmUtil.NoOp(),                                              RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    }
                });
            StateMachineManager.RegisterStateMachine(mStateMachine);
        }

        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Logic.RedMage.Heal.Verraise()) return true;
            return await Logic.RedMage.Heal.Vercure();
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            return await mStateMachine.Pulse();

            if (await Buff.LucidDreaming()) return true;

            if (RedMageRoutines.CanWeave)
            {
                if (await SingleTarget.Fleche()) return true;
                if (await Aoe.ContreSixte()) return true;
            }

            if (RedMageSettings.Instance.UseAoe)
            {
                if (await Aoe.Embolden()) return true;
                if (await Aoe.Manafication()) return true;
                if (await Aoe.Moulinet()) return true;
                if (await Aoe.Scatter()) return true;
                if (await Aoe.Veraero2()) return true;
                if (await Aoe.Verthunder2()) return true;
            }

            if (await Buff.Embolden()) return true;
            if (await Buff.Manafication()) return true;

            if (await SingleTarget.CorpsACorps()) return true;

            if (await SingleTarget.Scorch()) return true;
            if (await SingleTarget.Verholy()) return true;
            if (await SingleTarget.Verflare()) return true;
            if (await SingleTarget.Redoublement()) return true;
            if (await SingleTarget.Zwerchhau()) return true;
            if (await SingleTarget.Riposte()) return true;

            if (await SingleTarget.Displacement()) return true;
            if (await SingleTarget.Engagement()) return true;

            if (await SingleTarget.Verstone()) return true;
            if (await SingleTarget.Verfire()) return true;
            if (await SingleTarget.Veraero()) return true;
            if (await SingleTarget.Verthunder()) return true;
            if (await Buff.Acceleration()) return true;
            if (await SingleTarget.Jolt()) return true;
            return await SingleTarget.Reprise();
        }

        public static async Task<bool> PvP()
        {
            return false;
        }

        public static void RegisterCombatMessages()
        {
            Func<bool> bossPresenceOk = () => CanComboSomeEnemySt && !AoeMode;

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority: Melee combo is ready
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "Combo Ready!",
                                          () =>    mStateMachine.CurrentState == RdmStateIds.ReadyForCombo
                                                && SmUtil.SyncedLevel >= Spells.Redoublement.LevelAcquired));

            //Third priority:
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    WithinManaOf(15, ComboTargetMana)
                                                && bossPresenceOk()));
        }
    }
}
