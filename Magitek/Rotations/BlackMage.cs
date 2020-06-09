using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Buddy.Coroutines;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.BlackMage;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using Magitek.Utilities.Routines;
using CombatUtil = Magitek.Utilities.Combat;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Rotations
{
    public enum BlmStateIds
    {
        Start,
        SingleTarget,
        AoE,
        FreezeSucceeded,
        ThunderSucceeded,
        TriplecastSucceeded,
        Fire3Succeeded,
        Fire3SucceededAvoidSwiftcast,
        InitialFlareSucceeded,
        InitialFlareSucceededAvoidSwiftcast,
        FinalFlareSucceeded,
        ManaFontSucceeded,
        TransposeSucceeded,
        FireUntilLowMana,
        FreezeSpam,
        FreezeSpamNoThunder,
        Blizzard2Spam,
        Blizzard2SpamNoThunder,
        Fire2Spam,
        Fire2SpamNoThunder
    }
    
    public static class BlackMage
    {
        private static StateMachine<BlmStateIds> mStateMachine;

        private static async Task<bool> SwiftcastAndPause()
        {
            if (await SmUtil.SyncedCast(Spells.Swiftcast, Core.Me))
            {
                await Coroutine.Wait(300, () => false);
                return true;
            }
            return false;
        }
        
        private static bool TimeSince(SpellData spell, int minimumMs)
        {
            SpellCastHistoryItem mf = Casting.SpellCastHistory.FirstOrDefault(s => s.Spell == spell);

            if (mf == null)
                return true;

            if (DateTime.UtcNow.Subtract(mf.TimeCastUtc).TotalMilliseconds >= minimumMs)
                return true;

            return false;
        }

        private static bool ManaFontDelayElapsed => TimeSince(Spells.ManaFont, 2500);

        private static bool CanCastAnother(SpellData spell, int remainingManaNeeded, double remainingTimeNeededMs)
        {
            return Core.Me.CurrentMana >= spell.AdjustedSpellCostBlm() + remainingManaNeeded
                   && ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds >= spell.AdjustedCastTime.TotalMilliseconds + remainingTimeNeededMs;
        }

        private static bool CanCastFoul()
        {
            return ActionManager.HasSpell(Spells.Foul.Id) && ActionResourceManager.BlackMage.PolyglotCount > 0;
        }

        private static bool SpamSpell(SpellData spell)
        {
            Aura thunder2 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder2 && x.CasterId == Core.Player.ObjectId);
            Aura thunder4 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder4 && x.CasterId == Core.Player.ObjectId);

            if (thunder2 != null && thunder2.TimespanLeft.TotalMilliseconds - spell.AdjustedCastTime.TotalMilliseconds > Spells.Thunder2.AdjustedCastTime.TotalMilliseconds)
                return true;

            if (thunder4 != null && thunder4.TimespanLeft.TotalMilliseconds - spell.AdjustedCastTime.TotalMilliseconds > Spells.Thunder4.AdjustedCastTime.TotalMilliseconds)
                return true;

            return false;
        }

        private static bool HasMyAura(GameObject target, uint aura)
        {
            return (target as Character)?.Auras.Any(x => x.Id == aura && x.CasterId == Core.Player.ObjectId) ?? false;
        }

        private static async Task<bool> CastThunder(GameObject target)
        {
            if (await SmUtil.SyncedCast(Spells.Thunder4, target))
                return true;

            return await SmUtil.SyncedCast(Spells.Thunder2, target);
        }

        static BlackMage()
        {
            mStateMachine = new StateMachine<BlmStateIds>(
                BlmStateIds.Start,
                new Dictionary<BlmStateIds, State<BlmStateIds>>()
                {
                    {
                        BlmStateIds.Start,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode, () => SmUtil.NoOp(), BlmStateIds.AoE,          TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => true,    () => SmUtil.NoOp(), BlmStateIds.SingleTarget, TransitionType.Immediate),                                
                            })
                    },
                    {
                        BlmStateIds.SingleTarget,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode, () => SmUtil.NoOp(),               BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => true,    () => SingleTargetCombat(), BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.AoE,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                 () => SmUtil.NoOp(),                                                  BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                      () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.AoE),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel < 35 && Core.Me.CurrentMana >= 3400, () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel < 35 && Core.Me.CurrentMana >= 1200, () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel < 35,                                () => SmUtil.SyncedCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel < 50 && Core.Me.CurrentMana >= 1400, () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.FreezeSpamNoThunder),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel < 50,                                () => SmUtil.SyncedCast(Spells.Freeze, Core.Me.CurrentTarget),    BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                                     () => SmUtil.SyncedCast(Spells.Freeze, Core.Me.CurrentTarget),    BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks > 0,         () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),               BlmStateIds.AoE),                                 
                            })
                    },
                    {
                        BlmStateIds.Blizzard2Spam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                                  BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= 9000,         () => SmUtil.SyncedCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Blizzard2),         () => SmUtil.SyncedCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => true,                                () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2SpamNoThunder)
                            })
                    },
                    {
                        BlmStateIds.Blizzard2SpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                                  BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= 9000,         () => SmUtil.SyncedCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Fire2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                            })
                    },
                    {
                        BlmStateIds.Fire2Spam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                  () => SmUtil.NoOp(),                                                  BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                       () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire2.AdjustedSpellCostBlm(), () => SmUtil.SyncedCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Fire2),                                   () => SmUtil.SyncedCast(Spells.Fire2, Core.Me.CurrentTarget),     BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => true,                                                      () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2SpamNoThunder)
                            })
                    },
                    {
                        BlmStateIds.Fire2SpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                  () => SmUtil.NoOp(),                                                  BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                       () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire2.AdjustedSpellCostBlm(), () => SmUtil.SyncedCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                                      () => SmUtil.SyncedCast(Spells.Fire2, Core.Me.CurrentTarget),     BlmStateIds.Fire2Spam),
                            })
                    },
                    {
                        BlmStateIds.FreezeSpam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                               BlmStateIds.Start, TransitionType.Immediate),    
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpam), 
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Freeze),            () => SmUtil.SyncedCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpamNoThunder)
                            })
                    },
                    {
                        BlmStateIds.FreezeSpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                               BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSpam),
                            })
                    },
                    {
                        BlmStateIds.FreezeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                              () => SmUtil.NoOp(),                                             BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                                () => SmUtil.SyncedCast(Spells.Enochian, Core.Me),           BlmStateIds.FreezeSucceeded),
                                //TODO: Should we sharpcast before every thundercloud proc?
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => SmUtil.SyncedCast(Spells.Sharpcast, Core.Me),          BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => CastThunder(Core.Me.CurrentTarget),                 BlmStateIds.ThunderSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                             () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),          BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.Foul, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => SmUtil.SyncedCast(Spells.Sharpcast, Core.Me),          BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => CastThunder(Core.Me.CurrentTarget),                 BlmStateIds.ThunderSucceeded)
                            })
                    },                    
                    {
                        BlmStateIds.TransposeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SmUtil.NoOp(),                                               BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => true,     () => SmUtil.SyncedCast(Spells.Foul, Core.Me.CurrentTarget),   BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => SmUtil.SyncedCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded)
                            })
                    },
                    {
                        BlmStateIds.ThunderSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.ThunderSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 2000,          () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Triplecast, Core.Me),          BlmStateIds.TriplecastSucceeded),                                
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TriplecastSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks < 3,  () => SmUtil.SyncedCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast)                                
                            })
                    },
                    {
                        BlmStateIds.Fire3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                                                  () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                                       () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Fire3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                                                 () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel >= 68,                                                                                                () => SwiftcastAndPause(),                                 BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => SmUtil.SyncedLevel >= 68,                                                                                                () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && Core.Me.CurrentMana >= Spells.Fire4.AdjustedSpellCostBlm() + 800,                                 () => SmUtil.SyncedCast(Spells.Fire4, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => (EnemiesNearTarget >= 5 || SmUtil.SyncedLevel < 60) && Core.Me.CurrentMana >= Spells.Fire2.AdjustedSpellCostBlm() + 800, () => SmUtil.SyncedCast(Spells.Fire2, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                      () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.FireUntilLowMana,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                                                                                                               () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),                                                                                                            
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && ActionResourceManager.BlackMage.StackTimer >= Spells.Thunder2.AdjustedCooldown + Spells.Flare.AdjustedCastTime + TimeSpan.FromMilliseconds(150), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                                                                                                              () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && CanCastAnother(Spells.Fire4, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                                                            () => SmUtil.SyncedCast(Spells.Fire4, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => (EnemiesNearTarget >= 5 || SmUtil.SyncedLevel < 60) && CanCastAnother(Spells.Fire2, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                            () => SmUtil.SyncedCast(Spells.Fire2, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && !CanCastAnother(Spells.Fire4, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                                                           () => SwiftcastAndPause(),                                 BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => (EnemiesNearTarget >= 5 || SmUtil.SyncedLevel < 60) && !CanCastAnother(Spells.Fire2, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                           () => SwiftcastAndPause(),                                 BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                                                                                   () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Fire3SucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => SwiftcastAndPause(),                                 BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,                                              () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SmUtil.NoOp(),                                              BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => SmUtil.SyncedCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => SmUtil.SyncedCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.FinalFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SmUtil.NoOp(),                                    BlmStateIds.Start, TransitionType.Immediate),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),        BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.ManaFont, Core.Me),  BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => SmUtil.SyncedCast(Spells.Transpose, Core.Me), BlmStateIds.TransposeSucceeded)
                            })
                    }});
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


            if (ActionResourceManager.BlackMage.AstralStacks > 0 && ActionResourceManager.BlackMage.UmbralStacks == 0)
            {
                if (Core.Me.CurrentManaPercent < 70 && Spells.Transpose.Cooldown == TimeSpan.Zero)
                {
                    return await SmUtil.SyncedCast(Spells.Transpose, Core.Me);
                }
            }

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();


            if (await GambitLogic.Gambit()) return true;

            // if (await Buff.TransposeMovement()) return true;

            return false;
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static bool AoEMode => BlackMageSettings.Instance.UseAoe && EnemiesNearTarget >= BlackMageSettings.Instance.AoeEnemies && SmUtil.SyncedLevel >= 18;

        public static int EnemiesNearTarget => CombatUtil.Enemies.Where(e => e.Distance(Core.Me.CurrentTarget) <= 5 + e.CombatReach).Count();

        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            await Casting.CheckForSuccessfulCast();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;
            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;
            if (await CustomOpenerLogic.Opener()) return true;

            return await mStateMachine.Pulse();
        }

        public static async Task<bool> SingleTargetCombat()
        {
            //DON'T CHANGE THE ORDER OF THESE
            if (await Buff.Enochian()) return true;
            if (await Buff.Triplecast()) return true;
            if (await Buff.Sharpcast()) return true;
            if (await Buff.ManaFont()) return true;
            if (await Buff.LeyLines()) return true;
            if (await Buff.UmbralSoul()) return true;

            if (await SingleTarget.Blizzard4()) return true;
            if (await SingleTarget.Fire()) return true;
            if (await SingleTarget.Thunder3()) return true;
            if (await SingleTarget.Xenoglossy()) return true;
            if (await SingleTarget.Fire4()) return true;
            if (await SingleTarget.Despair()) return true;

            if (await SingleTarget.Fire3()) return true;

            if (await SingleTarget.Blizzard()) return true;
            if (await SingleTarget.Blizzard3()) return true;

            if (Core.Me.ClassLevel < 80)
                return await SmUtil.SyncedCast(Spells.Fire3, Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
