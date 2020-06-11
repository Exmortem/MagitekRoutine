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
        Moving,
        FishForProcsUntil60Mana,
        FishForProcsFirstWeave,
        FishForProcsSecondWeave,
        FishForProcsNeitherProcUpDualcast,
        FishForProcsNeitherProcUpNoDualcast,
        FishForProcsBothProcsUpDualcast,
        FishForProcsBothProcsUpNoDualcast,
        FishForProcsVerstoneUpDualcast,
        FishForProcsVerstoneUpNoDualcast,
        FishForProcsVerfireUpDualcast,
        FishForProcsVerfireUpNoDualcast,
        EschewFishingUntil80Mana,
        EschewFishingFirstWeave,
        EschewFishingSecondWeave,
        EschewFishingNeitherProcUpDualcast,
        EschewFishingNeitherProcUpNoDualcast,
        EschewFishingBothProcsUpDualcast,
        EschewFishingBothProcsUpNoDualcast,
        EschewFishingVerstoneUpDualcast,
        EschewFishingVerstoneUpNoDualcast,
        EschewFishingVerfireUpDualcast,
        EschewFishingVerfireUpNoDualcast,
        PrepareProcsForCombo,
        PrepareProcsFirstWeave,
        PrepareProcsSecondWeave,
        PrepareProcsNeitherProcUpNoDualcast,
        PrepareProcsNeitherProcUpDualcast,
        PrepareProcsBothProcsUpNoDualcast,
        PrepareProcsBothProcsUpDualcast,
        PrepareProcsVerstoneUpDualcast,
        PrepareProcsVerstoneUpNoDualcast,
        PrepareProcsVerfireUpDualcast,
        PrepareProcsVerfireUpNoDualcast,
        OverwriteProc,
        ReadyForCombo,
        Zwerchhau,
        Redoublement,
        VerflareOrVerholy,
        Scorch,
        SingleTargetScatter,
        Aoe,
        AoeFirstWeave,
        AoeSecondWeave
    }

    public static class RedMage
    {
        public const int MinAoeTargets = 3;
        public const double MinBurstHealth = 40;

        private static StateMachine<RdmStateIds> mStateMachine;

        private static int Cap(int mana) => Math.Min(100, mana);
        private static int CapLoss(int moreWhite, int moreBlack) => (WhiteMana + BlackMana + moreWhite + moreBlack) - (Cap(WhiteMana + moreWhite) + Cap(BlackMana + moreBlack));

        private const int GcdBufferMs = 350;
        private static double GcdLeft => Math.Max(Spells.Riposte.Cooldown.TotalMilliseconds - GcdBufferMs, 0);

        private static Stopwatch mComboTimer = new Stopwatch();
        private static async Task<bool> CastComboSpell(SpellData spell, GameObject target)
        {
            if (await SmUtil.SyncedCast(spell, target))
            {
                mComboTimer.Restart();
                return true;
            }
            return false;
        }
        private static bool ComboUp => mComboTimer.ElapsedMilliseconds <= 15000;

        private static bool OutsideComboRange => Core.Me.CurrentTarget == null ? false : Core.Me.Distance(Core.Me.CurrentTarget) > 3.4 + Core.Me.CombatReach + Core.Me.CurrentTarget.CombatReach;

        private static IEnumerable<GameObject> EnemiesWithinOf(double distance, GameObject target)
        {
            if (target == null)
            {
                return new List<GameObject>();
            }
            return CombatUtil.Enemies.Where(e => target.Distance(e) <= distance + e.CombatReach);
        }

        private static int AoeTargets => EnemiesWithinOf(5, Core.Me.CurrentTarget).Count();

        //TODO: Will we ever be in single target mode with more than 2 targets? If not, this can be simplified.
        private static bool UseScatterSt()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (SmUtil.SyncedLevel >= Spells.Impact.LevelAcquired)
            {
                return AoeTargets >= 2;
            }
            else if (SmUtil.SyncedLevel >= Spells.Jolt2.LevelAcquired)
            {
                return AoeTargets >= 4;
            }
            else
            {
                return AoeTargets >= 3;
            }
        }

        private static bool ShouldVerthunder2St()
        {
            if (BlackMana <= WhiteMana)
                return HardcastAoeInSt();

            return false;
        }
        private static bool ShouldVeraero2St()
        {
            if (WhiteMana <= BlackMana)
                return HardcastAoeInSt();

            return false;
        }
        private static bool HardcastAoeInSt()
        {
            if (SmUtil.SyncedLevel < Spells.Jolt2.LevelAcquired)
            {
                return AoeTargets >= 2;
            }
            return false;
        }


        private static bool ManaficationUp => SmUtil.SyncedLevel >= Spells.Manafication.LevelAcquired &&  Spells.Manafication.Cooldown == TimeSpan.Zero;
        private static bool InManaficationRangeSt => WhiteMana >= 40 && BlackMana >= 40 && WhiteMana <= 60 && BlackMana <= 60;

        private static int TargetMana => ManaficationUp && (BlackMana <= 40 || WhiteMana <= 40) ? 40 : 80;

        private static bool AoeMode => AoeTargets >= MinAoeTargets;

        private static bool EmboldenReadySoon => SmUtil.SyncedLevel >= Spells.Embolden.LevelAcquired && Spells.Embolden.Cooldown.TotalMilliseconds <= 7500;
        private static bool DoEmboldenBurst => ((BlackMana == 100 && WhiteMana == 100) || (BlackMana >= 90 && WhiteMana >= 90 && ManaficationUp)  || Core.Me.HasAura(Auras.Manafication)) && CanDoBurst;
        private static bool DoManaficationBurst => BlackMana >= 50 && WhiteMana >= 50 && BlackMana <= 65 && WhiteMana <= 65 && CanDoBurst;
        //We can do the burst if we'll hit 3 or more enemies with Moulinet
        private static bool CanDoBurst => EnemiesWithinOf(8 + Core.Me.CombatReach, Core.Me).Count(r =>    InMoulinetArc(r)
                                                                                                       && r.CurrentHealthPercent >= MinBurstHealth) >= MinAoeTargets;

        private static bool InMoulinetArc(GameObject target)
        {
            if (target == null)
                return false;

            return target.RadiansFromPlayerHeading() < 1.361f; //Translated from radians, this is ~78 degrees left or right;
        }

        private static bool UseEngagement  => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Engagement;
        private static bool UseDisplacement => RedMageSettings.Instance.UseMelee && RedMageSettings.Instance.Displacement;

        private static bool ShouldVerfireAoe()
        {
            if (WhiteMana < BlackMana || !Core.Me.HasAura(Auras.VerfireReady))
                return false;
            return UseProcInAoe();
        }
        private static bool ShouldVerstoneAoe()
        {
            if (BlackMana < WhiteMana || !Core.Me.HasAura(Auras.VerstoneReady))
                return false;
            return UseProcInAoe();
        }
        //TODO: Will we ever be in AoE mode with fewer than 3 targets? If not, this can be simplified
        private static bool UseProcInAoe()
        {
            if (SmUtil.SyncedLevel >= 78) //Enhanced Contre Sixte trait raises Verthunder2 and Veraero2 from 100 to 120 potency @ 78
            {
                return AoeTargets <= 2;
            }
            else
            {
                return AoeTargets <= 3;
            }
        }

        private static bool ShouldVerthunderAoe()
        {
            if (BlackMana <= WhiteMana || (Core.Me.HasAura(Auras.VerstoneReady) && !Core.Me.HasAura(Auras.VerfireReady)))
                return UseStDualcastInAoe();

            return false;
        }
        private static bool ShouldVeraeroAoe()
        {
            if (WhiteMana <= BlackMana || (Core.Me.HasAura(Auras.VerfireReady) && !Core.Me.HasAura(Auras.VerstoneReady)))
                return UseStDualcastInAoe();

            return false;
        }
        //TODO: Could there ever be fewer than 3 targets in AoE mode? If not, this could be simplified quite a bit
        private static bool UseStDualcastInAoe()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

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

        static RedMage()
        {
            List<uint> mBothProcs = new List<uint>() { Auras.VerfireReady, Auras.VerstoneReady };
            List<uint> mBothProcsAndDualcast = new List<uint>() { Auras.VerfireReady, Auras.VerstoneReady, Auras.Dualcast };
            List<uint> mVerstoneAndDualcast = new List<uint>() { Auras.VerstoneReady, Auras.Dualcast };
            List<uint> mVerfireAndDualcast = new List<uint>() { Auras.VerfireReady, Auras.Dualcast };
            List<uint> mManaficationAndEmbolden = new List<uint>() { Auras.Manafication, Auras.Embolden };

            //TODO: Use embolden during AoE at low mana if it would sit for a really long time waiting for us to build up mana - need to take Manafication into account, because we might not be sitting for as long as we think we would if that will go off
            //TODO: Lower level AoE rotations
            //TODO: Incorporate all of the settings to enable/disable various skills & spells
            //TODO: Check what happens if spells acquired through class quests are missing even if you're high enough level that the rotation expects them
            //TODO: If you're less than level 70, go right into the combo - don't bother trying to balance it. Maybe if you're 68 or 69 and you can balance it for black?
            //TODO: Smart targeting
            mStateMachine = new StateMachine<RdmStateIds>(
                RdmStateIds.Start,
                new Dictionary<RdmStateIds, State<RdmStateIds>>()
                {
                    {
                        RdmStateIds.Start,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode, () => SmUtil.NoOp(), RdmStateIds.Aoe,                     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,    () => SmUtil.NoOp(), RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate)
                            })
                    },
                    //TODO: What if we're moving during AoE? Need an AoeMoving state. It should probably try to swiftcast stuff and also use enchanted moulinet
                    {
                        RdmStateIds.Aoe,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,                                                                        () => SmUtil.NoOp(),                                                RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe(),                                                           () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget),  RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe(),                                                              () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),     RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                                 () => SmUtil.SyncedCast(Spells.Scatter, Core.Me.CurrentTarget),     RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAnyAura(mManaficationAndEmbolden) && CanDoBurst,                      () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),    RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => ShouldVerstoneAoe() && CapLoss(12,3) > 0 && CanDoBurst && !EmboldenReadySoon,    () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => ShouldVerstoneAoe(),                                                             () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana && CapLoss(10,3) > 0 && CanDoBurst && !EmboldenReadySoon, () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => WhiteMana <= BlackMana,                                                          () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => ShouldVerfireAoe() && CapLoss(3,12) > 0 && CanDoBurst && !EmboldenReadySoon,     () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => ShouldVerfireAoe(),                                                              () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => CapLoss(3,10) > 0 && CanDoBurst && !EmboldenReadySoon,                           () => SmUtil.SyncedCast(Spells.Moulinet, Core.Me.CurrentTarget),    RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => true,                                                                            () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                            })
                    },
                    {
                        RdmStateIds.AoeFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,            () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,      () => SmUtil.NoOp(),                                                 RdmStateIds.AoeSecondWeave,         TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,     () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst, () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseDisplacement,     () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseEngagement,       () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.AoeSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.AoeSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.AoeSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,       () => SmUtil.NoOp(),                                                 RdmStateIds.Aoe, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,     () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst, () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseEngagement,       () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => true,                () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Aoe)
                            })
                    },
                    //TODO: We should probably have swiftcast in here
                    {
                        RdmStateIds.Moving,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                     () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                     () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && UseDisplacement, () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50,                       () => SmUtil.SyncedCast(Spells.Reprise, Core.Me.CurrentTarget),      RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true,                               () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsUntil60Mana,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                              () => SmUtil.NoOp(), RdmStateIds.Aoe,                                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                   () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ManaficationUp && BlackMana >= 20 && WhiteMana >= 20, () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseScatterSt(),                                       () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcsAndDualcast),           () => SmUtil.NoOp(), RdmStateIds.FishForProcsBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerstoneAndDualcast),            () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerfireAndDualcast),             () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                      () => SmUtil.NoOp(), RdmStateIds.FishForProcsNeitherProcUpDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                             () => SmUtil.NoOp(), RdmStateIds.Moving,                              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),                      () => SmUtil.NoOp(), RdmStateIds.FishForProcsBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),                 () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),                  () => SmUtil.NoOp(), RdmStateIds.FishForProcsVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                                 () => SmUtil.NoOp(), RdmStateIds.FishForProcsNeitherProcUpNoDualcast, TransitionType.Immediate),
                            })
                    },
                    {
                        RdmStateIds.FishForProcsFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60, () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                     () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                               () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                               () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                               () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseDisplacement,                    () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => UseEngagement,                      () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                               () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.FishForProcsSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,   () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsUntil60Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => UseEngagement,   () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.FishForProcsUntil60Mana)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(), () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.NoOp(),                                                RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
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
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),                    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(),                 () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                                RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
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
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),                    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(),                 () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                                RdmStateIds.FishForProcsUntil60Mana, TransitionType.NextPulse)
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
                                new StateTransition<RdmStateIds>(() => UseScatterSt(),                                        () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcsAndDualcast),            () => SmUtil.NoOp(), RdmStateIds.EschewFishingBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerstoneAndDualcast),             () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerfireAndDualcast),              () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                       () => SmUtil.NoOp(), RdmStateIds.EschewFishingNeitherProcUpDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                              () => SmUtil.NoOp(), RdmStateIds.Moving,                               TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),                       () => SmUtil.NoOp(), RdmStateIds.EschewFishingBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),                  () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),                   () => SmUtil.NoOp(), RdmStateIds.EschewFishingVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.NoOp(), RdmStateIds.EschewFishingNeitherProcUpNoDualcast, TransitionType.Immediate)
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
                                new StateTransition<RdmStateIds>(() => InManaficationRangeSt,                                 () => SmUtil.SyncedCast(Spells.Manafication, Core.Me),               RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseDisplacement,                                       () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => UseEngagement,                                         () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.EschewFishingSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,         () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => InManaficationRangeSt, () => SmUtil.SyncedCast(Spells.Manafication, Core.Me),               RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => UseEngagement,         () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.EschewFishingUntil80Mana),
                            })
                    },
                    {
                        RdmStateIds.EschewFishingNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+11) > WhiteMana && WhiteMana >= TargetMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+11) > BlackMana && BlackMana >= TargetMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana,                                   () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                     () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                     () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(), () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                  () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingFirstWeave),
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
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30), () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),                    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(),                 () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
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
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30), () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),                    () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(),                 () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                  () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingUntil80Mana, TransitionType.NextPulse)
                            })
                    },
                    //TODO: Figure out the acceleration trick
                    {
                        RdmStateIds.PrepareProcsForCombo,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                                    () => SmUtil.NoOp(), RdmStateIds.Aoe,                                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,           () => SmUtil.NoOp(), RdmStateIds.EschewFishingUntil80Mana,            TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => UseScatterSt(),                             () => SmUtil.NoOp(), RdmStateIds.SingleTargetScatter,                 TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcsAndDualcast), () => SmUtil.NoOp(), RdmStateIds.PrepareProcsBothProcsUpDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerstoneAndDualcast),  () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerstoneUpDualcast,      TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mVerfireAndDualcast),   () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerfireUpDualcast,       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),            () => SmUtil.NoOp(), RdmStateIds.PrepareProcsNeitherProcUpDualcast,   TransitionType.Immediate),
                                //If manafication is up, go right to the combo so we don't drop the buff before we cast Scorch
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Manafication),        () => SmUtil.NoOp(), RdmStateIds.ReadyForCombo,                       TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => MovementManager.IsMoving,                   () => SmUtil.NoOp(), RdmStateIds.Moving,                              TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),            () => SmUtil.NoOp(), RdmStateIds.PrepareProcsBothProcsUpNoDualcast,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),       () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerstoneUpNoDualcast,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),        () => SmUtil.NoOp(), RdmStateIds.PrepareProcsVerfireUpNoDualcast,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                                       () => SmUtil.NoOp(), RdmStateIds.PrepareProcsNeitherProcUpNoDualcast, TransitionType.Immediate)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80, () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                   () => SmUtil.NoOp(),                                                 RdmStateIds.PrepareProcsSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,                             () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                             () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                             () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseDisplacement,                  () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseEngagement,                    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.PrepareProcsSecondWeave),
                                new StateTransition<RdmStateIds>(() => true,                             () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.PrepareProcsSecondWeave)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,   () => SmUtil.NoOp(),                                                 RdmStateIds.PrepareProcsForCombo, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseEngagement,   () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,            () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsNeitherProcUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                //Cast the one with less mana, if it'll surpass the larger
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana && Cap(BlackMana+11) > WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana && Cap(WhiteMana+11) > BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                //Otherwise, keep the larger larger
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+11) > WhiteMana,                          () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
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
                        RdmStateIds.PrepareProcsBothProcsUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                   () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
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
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,     () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.Dualcast),     () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAnyAura(mBothProcs),      () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady), () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),  () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
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
                        RdmStateIds.ReadyForCombo,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => AoeMode,                             () => SmUtil.NoOp(),                                                RdmStateIds.Aoe,                  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                      () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                      () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget), RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                      () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.ReadyForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                () => CastComboSpell(Spells.Riposte, Core.Me.CurrentTarget),        RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => !OutsideComboRange || GcdLeft >= 50, () => SmUtil.NoOp(),                                                RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse),
                                //We're too far away, so keep casting. First check if the lower mana has a proc. If so, use it
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana,               () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana,               () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.PrepareProcsForCombo),
                                //Then just use any procs we can find
                                new StateTransition<RdmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),     RdmStateIds.PrepareProcsForCombo),
                                //Then cast Jolt (or an AoE that's more efficient)
                                new StateTransition<RdmStateIds>(() => ShouldVeraero2St(),                  () => SmUtil.SyncedCast(Spells.Veraero2, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunder2St(),               () => SmUtil.SyncedCast(Spells.Verthunder2, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),        RdmStateIds.PrepareProcsForCombo),
                            })
                    },
                    //TODO: If we're not in combo range, we just stop
                    //TODO: If some other spell is cast in the middle (especially reprise), it tries to finish the combo. Either using un-combo melee, or hardcasting verthunder/veraero/jolt ii
                    {
                        RdmStateIds.Zwerchhau,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Zwerchhau.LevelAcquired, () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                    () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !ComboUp,                                            () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => OutsideComboRange && GcdLeft < 50,                   () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                      () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                      () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,                     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                      () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Zwerchhau),
                                new StateTransition<RdmStateIds>(() => true,                                                () => CastComboSpell(Spells.Zwerchhau, Core.Me.CurrentTarget),       RdmStateIds.Redoublement)
                            })
                    },
                    {
                        RdmStateIds.Redoublement,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Redoublement.LevelAcquired, () => SmUtil.NoOp(),                                                  RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                       () => SmUtil.NoOp(),                                                  RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !ComboUp,                                               () => SmUtil.NoOp(),                                                  RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => OutsideComboRange && GcdLeft < 50,                      () => SmUtil.NoOp(),                                                  RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                         () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                    RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                         () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),        RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                         () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),   RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,                        () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),    RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                         () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),               RdmStateIds.Redoublement),
                                new StateTransition<RdmStateIds>(() => true,                                                   () => CastComboSpell(Spells.Redoublement, Core.Me.CurrentTarget),     RdmStateIds.VerflareOrVerholy)
                            })
                    },
                    {
                        RdmStateIds.VerflareOrVerholy,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            { 
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Verflare.LevelAcquired, () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !ComboUp,                                           () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                     () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                     () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                     () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && UseDisplacement,                 () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,                    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.VerflareOrVerholy),
                                new StateTransition<RdmStateIds>(() => WhiteMana < BlackMana,                              () => CastComboSpell(Spells.Verholy, Core.Me.CurrentTarget),         RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => BlackMana < WhiteMana,                              () => CastComboSpell(Spells.Verflare, Core.Me.CurrentTarget),        RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerstoneReady),              () => CastComboSpell(Spells.Verholy, Core.Me.CurrentTarget),         RdmStateIds.Scorch),
                                //It's important that verflare is last, because if we'd like to cast verholy but we don't have it yet, we'll fall through to here, and we need to cast verflare to get out of this state
                                new StateTransition<RdmStateIds>(() => true,                                               () => CastComboSpell(Spells.Verflare, Core.Me.CurrentTarget),        RdmStateIds.Scorch),
                            })
                    },
                    {
                        RdmStateIds.Scorch,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Scorch.LevelAcquired, () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => !ComboUp,                                         () => SmUtil.NoOp(),                                                 RdmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                   () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                   () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                   () => SmUtil.SyncedCast(Spells.ContreSixte, Core.Me.CurrentTarget),  RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && UseDisplacement,               () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseEngagement,                  () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700,                                   () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Scorch),
                                new StateTransition<RdmStateIds>(() => true,                                             () => SmUtil.SyncedCast(Spells.Scorch, Core.Me.CurrentTarget),       RdmStateIds.Start)
                            })
                    },
                    {
                        RdmStateIds.SingleTargetScatter,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast), () => SmUtil.SyncedCast(Spells.Scatter, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                            () => SmUtil.NoOp(),                                            RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
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
            Func<bool> bossPresenceOk = () => !RedMageSettings.Instance.MeleeComboBossesOnly || Utilities.Combat.Enemies.Any(e => e.IsBoss());

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority: Melee combo is ready
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "Combo Ready!",
                                          () =>    SingleTarget.ReadyForCombo()
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    SingleTarget.ReadyForCombo(BlackMana + 9, WhiteMana + 9)
                                                && !SingleTarget.ComboInProgress
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon, but based on different conditions
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    SingleTarget.ReadyForManaficationComboSoon
                                                && !SingleTarget.ComboInProgress
                                                && bossPresenceOk()
                                                && RedMageSettings.Instance.Manafication
                                                && Spells.Manafication.Cooldown.TotalMilliseconds <= 5000));
        }
    }
}
