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
        FirstFlareSucceeded,
        FirstFlareSucceededAvoidSwiftcast,
        SecondFlareSucceeded,
        ManaFontSucceeded,
        TransposeSucceeded,
        FireUntilLowMana
    }

    public static class BlackMage
    {
        public const int SyncedLevel = 65;

        public static void smTransition()
        {
            mStateMachine.Transition();
        }

        public static async Task<bool> DebugCast(SpellData spell, GameObject target)
        {
            Logger.WriteInfo($"Trying to cast {spell.Name}: {spell.LevelAcquired}/{SyncedLevel}");
            if (spell.LevelAcquired >= SyncedLevel)
                return false;

            return await spell.Cast(target);
        }

        public static int SyncedClassLevel()
        {
            return Math.Min((int)SyncedLevel, (int)Core.Me.ClassLevel);
        }

        private static StateMachine<BlmStateIds> mStateMachine;
        
        private static bool LongEnoughSinceManaFont()
        {
            SpellCastHistoryItem mf = Casting.SpellCastHistory.FirstOrDefault(s => s.Spell == Spells.ManaFont);

            if (mf == null)
                return true;

            if (DateTime.UtcNow.Subtract(mf.TimeCastUtc).TotalMilliseconds > 2500)
                return true;

            return false;
        }

        static BlackMage()
        {
            //TODO: Handle getting out of sync (i.e., we're in a state and we can't cast any of the spells in it for some reason, what do we do to recover?)
            mStateMachine = new StateMachine<BlmStateIds>(
                BlmStateIds.SingleTarget,
                new Dictionary<BlmStateIds, State<BlmStateIds>>()
                {
                    {
                        BlmStateIds.SingleTarget,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.HasAura(Auras.ThunderCloud), () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.CurrentMana >= 1000,         () => DebugCast(Spells.Freeze, Core.Me.CurrentTarget),   BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => AoEMode,                                        () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                           () => SingleTargetCombat(),                        BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.FreezeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),              () => DebugCast(Spells.Enochian, Core.Me),               BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Sharpcast, Core.Me),              BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TransposeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                      BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => DebugCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Thunder4Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 2000,          () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Triplecast, Core.Me),             BlmStateIds.TriplecastSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Fire3, Core.Me.CurrentTarget),    BlmStateIds.Fire3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TriplecastSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks < 3,  () => DebugCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks == 3, () => DebugCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FirstFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.Fire3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                  () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                       () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Fire3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                 () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => SyncedClassLevel() >= 68,                                                                  () => DebugCast(Spells.Swiftcast, Core.Me),              BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => SyncedClassLevel() >= 68,                                                                  () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FirstFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && Core.Me.CurrentMana >= Spells.Fire4.AdjustedSpellCostBlm() + 800,  () => DebugCast(Spells.Fire4, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && Core.Me.CurrentMana >= Spells.Fire2.AdjustedSpellCostBlm() + 800, () => DebugCast(Spells.Fire2, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                      () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.SecondFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.FireUntilLowMana,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                  () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                       () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Fire3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                 () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),                                                             
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && Core.Me.CurrentMana >= Spells.Fire4.AdjustedSpellCostBlm() + 800 && ActionResourceManager.BlackMage.StackTimer >= Spells.Fire4.AdjustedCastTime + Spells.Flare.AdjustedCastTime,  () => DebugCast(Spells.Fire4, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && Core.Me.CurrentMana >= Spells.Fire2.AdjustedSpellCostBlm() + 800 && ActionResourceManager.BlackMage.StackTimer >= Spells.Fire2.AdjustedCastTime + Spells.Flare.AdjustedCastTime, () => DebugCast(Spells.Fire2, Core.Me.CurrentTarget),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && (Core.Me.CurrentMana < Spells.Fire4.AdjustedSpellCostBlm() + 800 || ActionResourceManager.BlackMage.StackTimer < Spells.Fire4.AdjustedCastTime + Spells.Flare.AdjustedCastTime),  () => DebugCast(Spells.Swiftcast, Core.Me),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget >= 5 && (Core.Me.CurrentMana < Spells.Fire2.AdjustedSpellCostBlm() + 800 || ActionResourceManager.BlackMage.StackTimer < Spells.Fire2.AdjustedCastTime + Spells.Flare.AdjustedCastTime),  () => DebugCast(Spells.Swiftcast, Core.Me),    BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                      () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.SecondFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Fire3SucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.FirstFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.FirstFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                               () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                    () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.FirstFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && LongEnoughSinceManaFont(), () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                   () => DebugCast(Spells.Swiftcast, Core.Me),              BlmStateIds.FirstFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,                                                   () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.SecondFlareSucceeded)
                                
                            })
                    },
                    {
                        BlmStateIds.FirstFlareSucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                               () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                    () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.FirstFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && LongEnoughSinceManaFont(), () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                   () => DebugCast(Spells.Flare, Core.Me.CurrentTarget),    BlmStateIds.SecondFlareSucceeded)
                                
                            })
                    },
                    {
                        BlmStateIds.SecondFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => DebugCast(Spells.Thunder4, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.ManaFont, Core.Me),               BlmStateIds.FirstFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => DebugCast(Spells.Transpose, Core.Me),              BlmStateIds.TransposeSucceeded)
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
                    return await DebugCast(Spells.Transpose, Core.Me);
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
                return await DebugCast(Spells.Fire3, Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
