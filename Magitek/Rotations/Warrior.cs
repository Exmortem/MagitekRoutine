using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Roles;
using Magitek.Logic.Warrior;
using Magitek.Models.Account;
using Magitek.Models.Warrior;
using Magitek.Utilities;

namespace Magitek.Rotations
{
    public static class Warrior
    {
        public static async Task<bool> Rest()
        {
            return false;
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
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 4);
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
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 4);
            }

            if (await Tank.Interrupt(WarriorSettings.Instance)) return true;
            if (await Buff.Defiance()) return true;

            if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.HeavySwing.Cooldown.TotalMilliseconds > 800 + BaseSettings.Instance.UserLatencyOffset)
            {
                if (await Defensive.ExecuteTankBusters()) return true;
                if (await Defensive.Defensives()) return true;
                if (await Buff.Beserk()) return true;
                if (await Buff.InnerRelease()) return true;
                if (await Buff.Infuriate()) return true;
                if (await Buff.Equilibrium()) return true;
                if (await SingleTarget.Onslaught()) return true;
                if (await SingleTarget.Upheaval()) return true;
            }

            if (WarriorSettings.Instance.UseDefiance)
            {
                if (await Tank.Provoke(WarriorSettings.Instance)) return true;
                if (await SingleTarget.TomahawkOnLostAggro()) return true;
            }

            if (await Aoe.SteelCyclone()) return true;
            if (await Aoe.Decimate()) return true;
            if (await Aoe.InnerReleaseDecimateSpam()) return true;
            if (await Aoe.Overpower()) return true;
            if (await SingleTarget.InnerBeast()) return true;
            if (await SingleTarget.FellCleave()) return true;
            if (await SingleTarget.InnerReleaseFellCleaveSpam()) return true;

            // Main Rotation Part

            if (await SingleTarget.StormsEye()) return true;
            if (await SingleTarget.StormsPath()) return true;
            if (await SingleTarget.Maim()) return true;
            if (await SingleTarget.HeavySwing()) return true;
            return await SingleTarget.Tomahawk();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
