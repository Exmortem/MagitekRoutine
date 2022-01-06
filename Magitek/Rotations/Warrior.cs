﻿using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Roles;
using Magitek.Logic.Warrior;
using Healing = Magitek.Logic.Warrior.Heal;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Warrior
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();

            return await Buff.Defiance();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
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

            return await GambitLogic.Gambit();
        }
        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }
        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            //Utility
            if (await Tank.Interrupt(WarriorSettings.Instance)) return true;
            if (await Buff.Defiance()) return true;

            if (WarriorRoutine.GlobalCooldown.CanWeave())
            {
                //Defensive Buff
                if (await Defensive.Holmgang()) return true;
                if (await Healing.Equilibrium()) return true;
                if (await Healing.ThrillOfBattle()) return true;
                if (await Defensive.BloodWhetting()) return true;
                if (await Defensive.Reprisal()) return true;
                if (await Defensive.Rampart()) return true;
                if (await Defensive.Vengeance()) return true;
                if (await Defensive.ShakeItOff()) return true;
                if (await Buff.NascentFlash()) return true;

                //Cooldowns
                if (await Buff.InnerRelease()) return true;
                if (await Buff.Infuriate()) return true;

                //oGCD
                if (await Aoe.Orogeny()) return true;
                if (await SingleTarget.Upheaval()) return true;
                if (await SingleTarget.Onslaught()) return true;
            }

            //Spell to use with Nascent Chaos
            if (await Aoe.ChaoticCyclone()) return true;
            if (await SingleTarget.InnerChaos()) return true;

            //Spell to spam inside Inner Release
            if (await Aoe.PrimalRend()) return true;
            if (await Aoe.Decimate()) return true;
            if (await SingleTarget.FellCleave()) return true;

            //Use On CD
            if (await SingleTarget.TomahawkOnLostAggro()) return true;
            if (await Aoe.MythrilTempest()) return true;
            if (await Aoe.Overpower()) return true;

            //Storm Eye Combo + Filler
            if (await SingleTarget.StormsEye()) return true;
            if (await SingleTarget.StormsPath()) return true;
            if (await SingleTarget.Maim()) return true;
            if (await SingleTarget.HeavySwing()) return true;

            return await SingleTarget.Tomahawk();
        }
        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }
    }
}
