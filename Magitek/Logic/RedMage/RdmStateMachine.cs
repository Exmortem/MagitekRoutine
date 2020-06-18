using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using static ff14bot.Managers.ActionResourceManager.RedMage;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using Magitek.Utilities.Routines;
using static Magitek.Utilities.Routines.RedMage;

namespace Magitek.Logic.RedMage
{
    public enum RdmStateIds
    {
        Start,
        RiposteAtMost,
        Aoe,
        AoeFirstWeave,
        AoeSecondWeavePart1,
        AoeSecondWeavePart2,
        AoeSecondWeaveSwiftcast,
        FishForProcsUntil60Mana,
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

    internal static class RdmStateMachine
    {
        private static StateMachine<RdmStateIds> mStateMachine;
        public static StateMachine<RdmStateIds> StateMachine
        {
            get
            {
                if (mStateMachine == null)
                {
                    mStateMachine = CreateStateMachine();
                }
                return mStateMachine;
            }
        }

        private static StateMachine<RdmStateIds> CreateStateMachine()
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
            return new StateMachine<RdmStateIds>(
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
                    //Used only at level 1, when Riposte is the only spell available
                    {
                        RdmStateIds.RiposteAtMost,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                            RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
                    //AoE mode
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
                    //First weave during AoE mode
                    {
                        RdmStateIds.AoeFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => !AoeMode,             () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,       () => SmUtil.NoOp(),                                                 RdmStateIds.AoeSecondWeavePart1,    TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,      () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst,  () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,   () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,        () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,  () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => UseAccelerationInAoe, () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me.CurrentTarget), RdmStateIds.AoeSecondWeavePart1),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.AoeSecondWeavePart1)
                            })
                    },
                    //Second weave during AoE mode, high priority (higher priority than swiftcast)
                    {
                        RdmStateIds.AoeSecondWeavePart1,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,       () => SmUtil.NoOp(),                                                 RdmStateIds.Aoe,          TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => DoEmboldenBurst,     () => SmUtil.SyncedCast(Spells.Embolden, Core.Me.CurrentTarget),     RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => DoManaficationBurst, () => SmUtil.SyncedCast(Spells.Manafication, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,  () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,       () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadyAoe,   () => SmUtil.NoOp(),                                                 RdmStateIds.AoeSecondWeaveSwiftcast, TransitionType.Immediate),
                            })
                    },
                    //Second weave during AoE mode, swiftcast - this is its own state to allow us to jump out to the lower priority OGCDs if we shouldn't cast swiftcast
                    {
                        RdmStateIds.AoeSecondWeaveSwiftcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                //If we're in a moulinet burst, don't swiftcast
                                new StateTransition<RdmStateIds>(() => MeHasAnyAura(mManaficationAndEmbolden) && UseMoulinet,                         () => SmUtil.NoOp(),                                              RdmStateIds.AoeSecondWeavePart2, TransitionType.Immediate),
                                //If we should moulinet to avoid overcapping on verthunder, don't swiftcast
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe && CapLoss(0,11) > 0 && UseMoulinet && !EmboldenReadySoon, () => SmUtil.NoOp(),                                              RdmStateIds.AoeSecondWeavePart2, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ShouldVerthunderAoe,                                                           () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.AoeFirstWeave),
                                //If we should moulinet to avoid overcapping on veraero, don't swiftcast
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe && CapLoss(11,0) > 0 && UseMoulinet && !EmboldenReadySoon,    () => SmUtil.NoOp(),                                              RdmStateIds.AoeSecondWeavePart2, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ShouldVeraeroAoe,                                                              () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.AoeFirstWeave),
                                //If we should moulinet to avoid overcapping on Scatter, don't swiftcast
                                new StateTransition<RdmStateIds>(() => CapLoss(3,3) > 0 && UseMoulinet && !EmboldenReadySoon,                         () => SmUtil.NoOp(),                                              RdmStateIds.AoeSecondWeavePart2, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ScatterEnabled,                                                                () => SmUtil.Swiftcast(Spells.Scatter, BestAoeTarget),            RdmStateIds.AoeFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                                                          () => SmUtil.NoOp(),                                              RdmStateIds.AoeSecondWeavePart2, TransitionType.Immediate)
                            })
                    },
                    //Second weave during AoE mode, low priority (lower priority than swiftcast)
                    {
                        RdmStateIds.AoeSecondWeavePart2,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,       () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,    () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseAccelerationInAoe, () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me),              RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,     () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.Aoe),
                                new StateTransition<RdmStateIds>(() => true,                 () => SmUtil.NoOp(),                                                RdmStateIds.AoeSecondWeavePart1, TransitionType.NextPulse)
                            })
                    },
                    //Single target, building mana toward 60/60
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
                    //Single target, building mana toward 60/60, first weave
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
                    //Single target, building mana toward 60/60, second weave
                    {
                        RdmStateIds.FishForProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                                                            () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                                                                 () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsUntil60Mana,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                                                               () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => !AvoidAccelerationSt,                                                                          () => SmUtil.SyncedCast(Spells.Acceleration, Core.Me),               RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                                                                 () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                                                            () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAura(Auras.VerstoneReady) && WhiteMana <= BlackMana,                 () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAura(Auras.VerfireReady) && BlackMana <= WhiteMana,                  () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAura(Auras.VerstoneReady) && Cap(WhiteMana+11) <= Cap(BlackMana+30), () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAura(Auras.VerfireReady) && Cap(BlackMana+11) <= Cap(WhiteMana+30),  () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                                                                () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                                                             () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                                                              () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.FishForProcsUntil60Mana)
                            })
                    },
                    //Single target, building mana toward 60/60, dualcast is up, neither proc is up
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
                    //Single target, building mana toward 60/60, we need to hardcast, neither proc is up
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
                    //Single target, building mana toward 60/60, dualcast is up, verstone proc is up
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
                    //Single target, building mana toward 60/60, we need to hardcast, verstone proc is up
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
                    //Single target, building mana toward 60/60, dualcast is up, verfire proc is up
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
                    //Single target, building mana toward 60/60, we need to hardcast, verfire proc is up
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
                    //Single target, building mana toward 60/60, dualcast is up, both procs are up (shouldn't really get here, but acounting for it just in case)
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
                    //Single target, building mana toward 60/60, we need to hardcast, both procs are up
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
                    //Single target, building mana toward 80/80
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
                    //Single target, building mana toward 80/80, first weave
                    {
                        RdmStateIds.EschewFishingFirstWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                    () => SmUtil.NoOp(),                                                 RdmStateIds.PrepareProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => (BlackMana < 60 || WhiteMana < 60) && !ManaficationUp, () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => BlackMana < 20 || WhiteMana < 20,                      () => SmUtil.NoOp(),                                                 RdmStateIds.FishForProcsFirstWeave,   TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 1400,                                        () => SmUtil.NoOp(),                                                 RdmStateIds.EschewFishingSecondWeave, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                       () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                         () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                    () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                        () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => DisplacementEnabled,                                   () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingSecondWeave),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                      () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.EschewFishingSecondWeave)
                            })
                    },
                    //Single target, building mana toward 80/80, second weave
                    {
                        RdmStateIds.EschewFishingSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                      () => SmUtil.NoOp(),                                                RdmStateIds.PrepareProcsSecondWeave,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                                           () => SmUtil.NoOp(),                                                RdmStateIds.EschewFishingUntil80Mana, TransitionType.Immediate),
                                //Use Manafication only in the second weave slot to ensure mana values have had time to update before we do this check
                                new StateTransition<RdmStateIds>(() => UseManaficationSt,                                                       () => SmUtil.SyncedCast(Spells.Manafication, Core.Me),              RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                                         () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                                           () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                                      () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAnyAura(mBothProcs) && WhiteMana <= BlackMana, () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),      RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => SwiftcastReadySt && !MeHasAnyAura(mBothProcs) && BlackMana <= WhiteMana, () => SmUtil.Swiftcast(Spells.Verthunder, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingFirstWeave),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                                          () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                                       () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                                        () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.EschewFishingUntil80Mana),
                            })
                    },
                    //Single target, building mana toward 80/80, dualcast is up, neither proc is up
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
                    //Single target, building mana toward 80/80, we need to hardcast, neither proc is up
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
                    //Single target, building mana toward 80/80, dualcast is up, verstone proc is up
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
                    //Single target, building mana toward 80/80, we need to hardcast, verstone proc is up
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
                    //Single target, building mana toward 80/80, dualcast is up, verfire proc is up
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
                    //Single target, building mana toward 80/80, we need to hardcast, verfire proc is up
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
                    //Single target, building mana toward 80/80, dualcast is up, both procs are up (shouldn't really get here, but acounting for it just in case)
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
                    //Single target, building mana toward 80/80, we need to hardcast, both procs are up
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
                    //Single target, mana at 80/80, get procs set up properly for combo
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
                    //Single target, mana at 80/80, get procs set up properly for combo, first weave
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
                    //Single target, mana at 80/80, get procs set up properly for combo, second weave
                    {
                        RdmStateIds.PrepareProcsSecondWeave,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft < 700,                                                                                 () => SmUtil.NoOp(),                                                RdmStateIds.PrepareProcsForCombo, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => EmboldenEnabled,                                                                               () => SmUtil.SyncedCast(Spells.Embolden, Core.Me),                  RdmStateIds.PrepareProcsForCombo),
                                //If mana's equal and no procs are up, we can swiftcast to get to the combo
                                new StateTransition<RdmStateIds>(() => WhiteMana == BlackMana && SwiftcastReadySt && !MeHasAnyAura(mBothProcs) && CapLoss(11,0) <= 8, () => SmUtil.Swiftcast(Spells.Veraero, Core.Me.CurrentTarget),      RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => FlecheEnabled,                                                                                 () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),      RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => ContreSixteEnabled,                                                                            () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseCorpsACorps,                                                                                () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => EngagementEnabled,                                                                             () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),  RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => UseLucidDreaming,                                                                              () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),             RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    //Single target, mana at 80/80, get procs set up properly for combo, dualcast is up, neither proc is up
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
                    //Single target, mana at 80/80, get procs set up properly for combo, we need to hardcast, neither proc is up
                    {
                        RdmStateIds.PrepareProcsNeitherProcUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                //If mana's equal and we have space to fix it, try to fix it
                                new StateTransition<RdmStateIds>(() => BlackMana == WhiteMana && CapLoss(3, 14) <= 8, () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                //Otherwise, we're ready for the combo
                                new StateTransition<RdmStateIds>(() => true,                                          () => SmUtil.NoOp(),                                         RdmStateIds.ReadyForCombo,        TransitionType.Immediate)
                            })
                    },
                    //Single target, mana at 80/80, get procs set up properly for combo, dualcast is up, verstone proc is up
                    {
                        RdmStateIds.PrepareProcsVerstoneUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    //Single target, mana at 80/80, get procs set up properly for combo, we need to hardcast, verstone proc is up
                    {
                        RdmStateIds.PrepareProcsVerstoneUpNoDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => WhiteMana > BlackMana,                                             () => SmUtil.NoOp(),                                             RdmStateIds.ReadyForCombo,        TransitionType.Immediate),
                                //If trying to fix procs will waste too much mana or cap both at 100, it doesn't really help, so don't bother and go into the combo
                                new StateTransition<RdmStateIds>(() => CapLoss(9, 11) <= 8 && Cap(WhiteMana+9) + Cap(BlackMana+11) < 200, () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                              () => SmUtil.NoOp(),                                             RdmStateIds.ReadyForCombo,        TransitionType.Immediate)
                            })
                    },
                    //Single target, mana at 80/80, get procs set up properly for combo, dualcast is up, verfire proc is up
                    {
                        RdmStateIds.PrepareProcsVerfireUpDualcast,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(),                                            RdmStateIds.PrepareProcsForCombo, TransitionType.NextPulse)
                            })
                    },
                    //Single target, mana at 80/80, get procs set up properly for combo, we need to hardcast, verfire proc is up
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
                    //Single target, mana at 80/80, get procs set up properly for combo, dualcast is up, verfire proc is up (shouldn't really get here, but acounting for it just in case)
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
                    //Single target, mana at 80/80, get procs set up properly for combo, we need to hardcast, both procs are up
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
                    //Single target, mana at 80/80, get procs set up properly for combo, dualcast is up, we need to overwrite a proc before we start the combo
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
                    //Single target, we're ready to start the combo! It's possible the player won't be close enough, so we need to keep doing stuff in the meantime.
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
                    //Combo in progress, cast Zwerchhau next
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
                    //Combo in progress, cast Redoublement next
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
                    //Combo in progress, cast Verflare or Verholy next
                    {
                        RdmStateIds.VerflareOrVerholy,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => SmUtil.SyncedLevel < Spells.Verflare.LevelAcquired,                                               () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => ActionManager.ComboTimeLeft == 0,                                                                 () => SmUtil.NoOp(),                                                 RdmStateIds.Start,  TransitionType.Immediate),
                                //Use acceleration before Verflare or Verholy if we need it to get the proc
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
                    //Combo in progress, cast Scorch next
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
                                new StateTransition<RdmStateIds>(() => true,                                             () => SmUtil.SyncedCast(Spells.Scorch, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsFirstWeave)
                            })
                    },
                    //Single target mode, dualcast up, use Scatter because there are enough targets to make it worth it (but not enough to go to AoE mode)
                    {
                        RdmStateIds.SingleTargetScatter,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => ScatterEnabled && MeHasAura(Auras.Dualcast), () => SmUtil.SyncedCast(Spells.Scatter, BestAoeTarget), RdmStateIds.PrepareProcsFirstWeave),
                                new StateTransition<RdmStateIds>(() => true,                                        () => SmUtil.NoOp(),                                    RdmStateIds.Start, TransitionType.NextPulse)
                            })
                    },
                    //Single target mode, user is moving. Try to mitigate as best we can.
                    {
                        RdmStateIds.Moving,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && GcdLeft <= 1400 && SwiftcastReadySt, () => SmUtil.NoOp(),                                                 RdmStateIds.MovingSwiftcast, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && FlecheEnabled,                       () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && ContreSixteEnabled,                  () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseCorpsACorps,                      () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 1500 && DisplacementEnabled,                () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && EngagementEnabled,                   () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft >= 700 && UseLucidDreaming,                    () => SmUtil.SyncedCast(Spells.LucidDreaming, Core.Me),              RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50 && UseReprise,                            () => SmUtil.SyncedCast(Spells.Reprise, Core.Me.CurrentTarget),      RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft < 50 && SwiftcastReadySt,                      () => SmUtil.NoOp(),                                                 RdmStateIds.MovingSwiftcast, TransitionType.Immediate),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && FlecheEnabled,                         () => SmUtil.SyncedCast(Spells.Fleche, Core.Me.CurrentTarget),       RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && ContreSixteEnabled,                    () => SmUtil.SyncedCast(Spells.ContreSixte, BestContreSixteTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && UseCorpsACorps,                        () => SmUtil.SyncedCast(Spells.CorpsACorps, Core.Me.CurrentTarget),  RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && DisplacementEnabled,                   () => SmUtil.SyncedCast(Spells.Displacement, Core.Me.CurrentTarget), RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => GcdLeft == 0 && EngagementEnabled,                     () => SmUtil.SyncedCast(Spells.Engagement, Core.Me.CurrentTarget),   RdmStateIds.Start),
                                new StateTransition<RdmStateIds>(() => true,                                                  () => SmUtil.NoOp(),                                                 RdmStateIds.Start,           TransitionType.NextPulse)
                            })
                    },
                    //Single target mode, user is moving, use a swiftcast if available to maintain uptime
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
        }
    }
}
