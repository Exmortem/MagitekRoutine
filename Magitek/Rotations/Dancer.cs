using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Dancer;
using Magitek.Logic.Roles;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using DancerRoutine = Magitek.Utilities.Routines.Dancer;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Dancer
    {
        public static Task<bool> Rest()
        {
            var needRest = Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50;
            return Task.FromResult(needRest);
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

            if (WorldManager.InSanctuary)
                return false;

            if (await Buff.DancePartner()) return true;

            return await PhysicalDps.Peloton(DancerSettings.Instance);
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            return false;
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

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
            if (await GambitLogic.Gambit()) return true;

            if (await Buff.DancePartner()) return true;

            if (await Aoe.StarfallDance()) return true;
            if (await Dances.Tillana()) return true;
            if (await Dances.StandardStep()) return true;
            if (await Dances.TechnicalStep()) return true;
            if (await Dances.DanceStep()) return true;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (DancerRoutine.GlobalCooldown.CanWeave())
            {
                if (await PhysicalDps.Interrupt(DancerSettings.Instance)) return true;
                if (await PhysicalDps.SecondWind(DancerSettings.Instance)) return true;
                if (await Buff.CuringWaltz()) return true;
                if (await Buff.PreTechnicalDevilment()) return true;
                if (await Aoe.FanDance3()) return true;
                if (await Aoe.FanDance2()) return true;
                if (await SingleTarget.FanDance()) return true;
                if (await Aoe.FanDance4()) return true;
                if (await Buff.Flourish()) return true;
                if (await Buff.Devilment()) return true;
                if (await Buff.Improvisation()) return true;
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

        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }
    }
}
