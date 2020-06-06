using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.RedMage;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.Routines;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Rotations
{
    public enum RdmStateIds
    {
        Start,
        FishForProcsUntil60Mana,
        FishForProcsBothUp,
        FishForProcsVerstoneUp,
        FishForProcsVerfireUp,
        EschewFishingUntil80Mana,
        EschewFishingBothUp,
        EschewFishingVerstoneUp,
        EschewFishingVerfireUp,
        PrepareProcsForCombo,
        PrepareProcsBothUp,
        PrepareProcsVerstoneUp,
        PrepareProcsVerfireUp,
        RiposteComplete,
        ZwerchhauComplete
    }

    public static class RedMage
    {
        private static StateMachine<RdmStateIds> mStateMachine;
        private static int Cap(int mana) => Math.Min(100, mana);

        static RedMage()
        {
            List<uint> mBothProcs = new List<uint>() { Auras.VerfireReady, Auras.VerstoneReady };

            mStateMachine = new StateMachine<RdmStateIds>(
                RdmStateIds.Start,
                new Dictionary<RdmStateIds, State<RdmStateIds>>()
                {
                    {
                        RdmStateIds.Start,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => true, () => SmUtil.NoOp(), RdmStateIds.FishForProcsUntil60Mana, true)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsUntil60Mana,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                        () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),                           () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsBothUp,       true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),                      () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsVerstoneUp,   true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),                       () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsVerfireUp,    true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                           () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                      () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana)
                            })
                    },
                    {
                        RdmStateIds.FishForProcsBothUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                        () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAllAuras(mBothProcs),                          () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                           () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana,                                    () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                      () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsUntil60Mana),
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerstoneUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                                           () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerstoneReady) || Core.Me.HasAura(Auras.VerfireReady), () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && Cap(BlackMana+11) <= Cap(WhiteMana+30),    () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30),                                        () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana),
                            })
                    },
                    {
                        RdmStateIds.FishForProcsVerfireUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 60 && WhiteMana >= 60,                                           () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerfireReady) || Core.Me.HasAura(Auras.VerstoneReady), () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && Cap(WhiteMana+11) <= Cap(BlackMana+30),    () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30),                                        () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.FishForProcsUntil60Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.FishForProcsUntil60Mana),
                            })
                    },
                    {
                        RdmStateIds.EschewFishingUntil80Mana,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                               () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     true),
                                new StateTransition<RdmStateIds>(() => BlackMana < 60 || WhiteMana < 60,                                                 () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),                                                  () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingBothUp,      true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),                                             () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingVerstoneUp,  true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),                                              () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingVerfireUp,   true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana + 11 > WhiteMana && WhiteMana >= 80, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && WhiteMana + 11 > BlackMana && BlackMana >= 80, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana,                        () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                                  () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                                             () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingUntil80Mana)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingBothUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                        () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     true),
                                new StateTransition<RdmStateIds>(() => BlackMana < 60 || WhiteMana < 60,                          () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAllAuras(mBothProcs),                          () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                           () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana,                                    () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                      () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingUntil80Mana)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerstoneUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                           () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     true),
                                new StateTransition<RdmStateIds>(() => BlackMana < 60 || WhiteMana < 60,                                             () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerstoneReady) || Core.Me.HasAura(Auras.VerfireReady), () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && Cap(BlackMana+11) <= Cap(WhiteMana+30),    () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => WhiteMana - 1 <= BlackMana && WhiteMana >= 71,                                () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Cap(WhiteMana+9) <= Cap(BlackMana+30),                                        () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingUntil80Mana)
                            })
                    },
                    {
                        RdmStateIds.EschewFishingVerfireUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana >= 80 && WhiteMana >= 80,                                           () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo,     true),
                                new StateTransition<RdmStateIds>(() => BlackMana < 60 || WhiteMana < 60,                                             () => SmUtil.NoOp(),                                               RdmStateIds.FishForProcsUntil60Mana,  true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerfireReady) || Core.Me.HasAura(Auras.VerstoneReady), () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && Cap(WhiteMana+11) <= Cap(BlackMana+30),    () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => BlackMana - 1 <= WhiteMana && BlackMana >= 71,                                () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => Cap(BlackMana+9) <= Cap(WhiteMana+30),                                        () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.EschewFishingUntil80Mana),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Jolt, Core.Me.CurrentTarget),       RdmStateIds.EschewFishingUntil80Mana)
                            })
                    },
                    //TODO: Figure out the acceleration trick
                    {
                        RdmStateIds.PrepareProcsForCombo,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,                                                        () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAllAuras(mBothProcs),                                                         () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsBothUp,       true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerstoneReady),                                                    () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsVerstoneUp,   true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.VerfireReady),                                                     () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsVerfireUp,    true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana && BlackMana + 11 > WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && WhiteMana <= BlackMana && WhiteMana + 11 > BlackMana, () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana > WhiteMana,                                () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && WhiteMana > BlackMana,                                () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                                         () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                                                    () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget),    RdmStateIds.RiposteComplete)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsBothUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,                          () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAllAuras(mBothProcs),                          () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast) && BlackMana <= WhiteMana, () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                           () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => BlackMana <= WhiteMana,                                    () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget),    RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => true,                                                      () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),   RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerstoneUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,                                             () => SmUtil.NoOp(),                                               RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerstoneReady) || Core.Me.HasAura(Auras.VerfireReady), () => SmUtil.NoOp(),                                               RdmStateIds.PrepareProcsForCombo, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Verthunder, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => WhiteMana > BlackMana,                                                        () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget),       RdmStateIds.RiposteComplete),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Verstone, Core.Me.CurrentTarget),       RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    {
                        RdmStateIds.PrepareProcsVerfireUp,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 80 || WhiteMana < 80,                                             () => SmUtil.NoOp(),                                            RdmStateIds.EschewFishingUntil80Mana, true),
                                new StateTransition<RdmStateIds>(() => !Core.Me.HasAura(Auras.VerfireReady) || Core.Me.HasAura(Auras.VerstoneReady), () => SmUtil.NoOp(),                                            RdmStateIds.PrepareProcsForCombo, true),
                                new StateTransition<RdmStateIds>(() => Core.Me.HasAura(Auras.Dualcast),                                              () => SmUtil.SyncedCast(Spells.Veraero, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo),
                                new StateTransition<RdmStateIds>(() => BlackMana > WhiteMana,                                                        () => SmUtil.SyncedCast(Spells.Riposte, Core.Me.CurrentTarget), RdmStateIds.RiposteComplete),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Verfire, Core.Me.CurrentTarget), RdmStateIds.PrepareProcsForCombo)
                            })
                    },
                    {
                        RdmStateIds.RiposteComplete,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                                             () => SmUtil.NoOp(),                                            RdmStateIds.Start, true),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Zwerchhau, Core.Me.CurrentTarget), RdmStateIds.ZwerchhauComplete)
                            })
                    },
                    {
                        RdmStateIds.ZwerchhauComplete,
                        new State<RdmStateIds>(
                            new List<StateTransition<RdmStateIds>>()
                            {
                                new StateTransition<RdmStateIds>(() => BlackMana < 25 || WhiteMana < 25,                                             () => SmUtil.NoOp(),                                            RdmStateIds.Start, true),
                                new StateTransition<RdmStateIds>(() => true,                                                                         () => SmUtil.SyncedCast(Spells.Redoublement, Core.Me.CurrentTarget), RdmStateIds.Start)
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
