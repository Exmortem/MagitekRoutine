using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.BlackMage;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using Magitek.Utilities.Routines.StateMachine;
using CombatUtil = Magitek.Utilities.Combat;
using Auras = Magitek.Utilities.Auras;
using Buddy.Coroutines;

namespace Magitek.Rotations
{
    public enum BlmStateIds
    {
        SingleTarget,
        AoEStart,
        FreezeSucceeded,
        Thunder4Succeeded,
        TriplecastSucceeded,
        Fire3Succeeded,
        Fire3SucceededAvoidSwiftcast,
        InitialFlareSucceeded,
        InitialFlareSucceededAvoidSwiftcast,
        FinalFlareSucceeded,
        ManaFontSucceeded,
        TransposeSucceeded,
        FireUntilLowMana,
        FreezeSpam
    }

    public static class BlackMage
    {
        #region Forced Level Sync
        //Pretend we're level synced to test out rotations without having to go into a dungeon
        private const int ForcedSyncLevel = 80;
        private static int ForcedSyncClassLevel => Math.Min(ForcedSyncLevel, (int)Core.Me.ClassLevel);

        private static async Task<bool> ForcedSyncCast(SpellData spell, GameObject target)
        {
            if (spell.LevelAcquired > ForcedSyncClassLevel)
                return false;

            //Logger.WriteInfo($"Trying to cast {spell.Name}");
            return await spell.Cast(target);
        }
        #endregion

        private static StateMachine<BlmStateIds> mStateMachine;

        private static async Task<bool> SwiftcastAndPause()
        {
            if (await ForcedSyncCast(Spells.Swiftcast, Core.Me))
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

        private static bool SpamFreeze()
        {
            Aura thunder2 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder2 && x.CasterId == Core.Player.ObjectId);
            Aura thunder4 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder4 && x.CasterId == Core.Player.ObjectId);

            if (thunder2 != null && thunder2.TimespanLeft.TotalMilliseconds - Spells.Freeze.AdjustedCastTime.TotalMilliseconds > Spells.Thunder2.AdjustedCastTime.TotalMilliseconds)
                return true;

            if (thunder4 != null && thunder4.TimespanLeft.TotalMilliseconds - Spells.Freeze.AdjustedCastTime.TotalMilliseconds > Spells.Thunder4.AdjustedCastTime.TotalMilliseconds)
                return true;

            return false;
        }

        static BlackMage()
        {
            mStateMachine = new StateMachine<BlmStateIds>(
                BlmStateIds.SingleTarget,
                new Dictionary<BlmStateIds, State<BlmStateIds>>()
                {
                    {
                        BlmStateIds.SingleTarget,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.HasAura(Auras.ThunderCloud),                      () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.CurrentMana >= 1000 && ForcedSyncClassLevel < 50, () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),   BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.CurrentMana >= 1000,                              () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),   BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                             () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                                () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.FreezeSpam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),    
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => ForcedSyncCast(Spells.Thunder2, Core.Me.CurrentTarget), BlmStateIds.FreezeSpam),                                
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => SpamFreeze(),                        () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),   BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Thunder2, Core.Me.CurrentTarget), BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.FreezeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                              () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                                () => ForcedSyncCast(Spells.Enochian, Core.Me),               BlmStateIds.FreezeSucceeded),
                                //TODO: What if SharpCast is up? Should we go to FreezeSucceeded so that we can sharpcast Thunder 4?
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => ForcedSyncCast(Spells.Thunder2, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                             () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Foul, Core.Me.CurrentTarget),     BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Sharpcast, Core.Me),              BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Thunder2, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded)
                            })
                    },                    
                    {
                        BlmStateIds.TransposeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                                 BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => ForcedSyncCast(Spells.Foul, Core.Me.CurrentTarget),   BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Thunder4Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 2000,          () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Triplecast, Core.Me),             BlmStateIds.TriplecastSucceeded),                                
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget),    BlmStateIds.Fire3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TriplecastSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SingleTargetCombat(),                                BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks < 3,  () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                //TODO: Why are we checking if we can go straight to Flare here? Under what scenario is that possible?
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks == 3, () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.Fire3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                   () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                        () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Fire3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                  () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 68,                                                                 () => SwiftcastAndPause(),                                    BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 68,                                                                 () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && Core.Me.CurrentMana >= Spells.Fire4.AdjustedSpellCostBlm() + 800,  () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && Core.Me.CurrentMana >= Spells.Fire2.AdjustedSpellCostBlm() + 800, () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                       () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.FireUntilLowMana,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                                      () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                //TODO delay thundercloud if it will push flare outside of astral fire
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                           () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                                     () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && CanCastAnother(Spells.Fire4, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),   () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && CanCastAnother(Spells.Fire2, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),  () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && !CanCastAnother(Spells.Fire4, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),  () => SwiftcastAndPause(),                                    BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && !CanCastAnother(Spells.Fire2, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds), () => SwiftcastAndPause(),                                    BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                          () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Fire3SucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.InitialFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => SwiftcastAndPause(),                                    BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,                                              () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.FinalFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                                   BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => ForcedSyncCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.ManaFont, Core.Me),               BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded)
                            })
                    }});
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
                    return await ForcedSyncCast(Spells.Transpose, Core.Me);
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

        public static bool AoEMode => BlackMageSettings.Instance.UseAoe && EnemiesNearTarget >= BlackMageSettings.Instance.AoeEnemies;

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
                return await ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
