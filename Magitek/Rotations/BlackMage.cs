using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.BlackMage;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using Magitek.Utilities.Routines.StateMachine;

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
        TransposeSucceeded
    }

    public static class BlackMage
    {
        private static StateMachine<BlmStateIds> mStateMachine;

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
                                new StateTransition<BlmStateIds>(() => AoEMode && Core.Me.CurrentMana >= 1000, () => Spells.Freeze.Cast(Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => AoEMode,                                () => Spells.Transpose.Cast(Core.Me),            BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                   () => SingleTargetCombat(),                      BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.FreezeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                        BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Sharpcast.Cast(Core.Me),              BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Thunder4.Cast(Core.Me.CurrentTarget), BlmStateIds.Thunder4Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TransposeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                      BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Freeze.Cast(Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Thunder4Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Triplecast.Cast(Core.Me),          BlmStateIds.TriplecastSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Fire3.Cast(Core.Me.CurrentTarget), BlmStateIds.Fire3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TriplecastSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Fire3.Cast(Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.Fire3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Swiftcast.Cast(Core.Me),           BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Flare.Cast(Core.Me.CurrentTarget), BlmStateIds.FirstFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Fire3SucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Flare.Cast(Core.Me.CurrentTarget), BlmStateIds.FirstFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.FirstFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Swiftcast.Cast(Core.Me),           BlmStateIds.FirstFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Flare.Cast(Core.Me.CurrentTarget), BlmStateIds.SecondFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.FirstFlareSucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),                     BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Flare.Cast(Core.Me.CurrentTarget), BlmStateIds.SecondFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.SecondFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => SingleTargetCombat(),           BlmStateIds.SingleTarget),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.ManaFont.Cast(Core.Me),  BlmStateIds.FirstFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => Spells.Transpose.Cast(Core.Me), BlmStateIds.TransposeSucceeded)
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
                    return await Spells.Transpose.Cast(Core.Me);
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

        public static bool AoEMode => BlackMageSettings.Instance.UseAoe && Core.Me.CurrentTarget.EnemiesNearby(10).Count() >= BlackMageSettings.Instance.AoeEnemies;

        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

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
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
