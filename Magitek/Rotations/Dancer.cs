using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Dancer;
using Magitek.Logic.Roles;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Dancer
    {
        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50;
        }

        public static async Task<bool> PreCombatBuff()
        {


            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();


            //Openers.OpenerCheck();

            if (Core.Me.HasTarget && Core.Me.CurrentTarget.CanAttack)
            {
                return false;
            }

            if (Globals.OnPvpMap)
                return false;

            if (await Buff.DancePartner()) return true;

            return await PhysicalDps.Peloton(DancerSettings.Instance);
        }

        public static async Task<bool> Pull()
        {
            if (!BotManager.Current.IsAutonomous)
            {
                return await Combat();
            }

            Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);

            return await SingleTarget.Cascade();
        }
        public static async Task<bool> Heal()
        {


            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            return false;
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (await GambitLogic.Gambit()) return true;


            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (await Buff.DancePartner()) return true;

            if (await Dances.StartTechnicalDance()) return true;
            if (Dances.TechnicalStep()) return true;

            if (await Dances.StartStandardDance()) return true;
            if (Dances.StandardStep()) return true;


            if (Utilities.Routines.Dancer.OnGcd)
            {
                //Only cast spells that are instant/off gcd
                if (await PhysicalDps.Interrupt(DancerSettings.Instance)) return true;
                if (await Buff.PreTechnicalDevilment()) return true;
                if (await Aoe.FanDance3()) return true;
                if (await Aoe.FanDance2()) return true;
                if (await SingleTarget.FanDance()) return true;
                if (await Buff.Flourish()) return true;
                if (await Buff.Devilment()) return true;
                if (await Buff.CuringWaltz()) return true;
                if (await Buff.Improvisation()) return true;
                if (await PhysicalDps.SecondWind(DancerSettings.Instance)) return true;
            }

            if (await Aoe.SaberDance()) return true;

            if (await Aoe.Bloodshower()) return true;
            if (await Aoe.RisingWindmill()) return true;

            if (await SingleTarget.Fountainfall()) return true;
            if (await SingleTarget.ReverseCascade()) return true;

            if (await Aoe.Bladeshower()) return true;
            if (await Aoe.Windmill()) return true;

            if (await SingleTarget.Fountain()) return true;

            return await SingleTarget.Cascade();
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
