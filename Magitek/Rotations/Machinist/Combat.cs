using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Machinist;
using Magitek.Logic.Roles;
using Magitek.Models.Machinist;
using Magitek.Utilities;

namespace Magitek.Rotations.Machinist
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
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

            if (await CustomOpenerLogic.Opener()) return true;

            if (MachinistSettings.Instance.UseFlameThrower && Core.Me.HasAura(Auras.Flamethrower))
            {
                // Did someone use this manually? Make sure we don't cancel it...
                if (!MachinistSettings.Instance.UseFlameThrower)
                    return true;

                if (MovementManager.IsMoving)
                    return false;

                // Keep using it for the AOE benefit if we need to
                if (Core.Me.EnemiesInCone(8) >= MachinistSettings.Instance.FlamethrowerEnemies)
                    return true;
            }

            if (await PhysicalDps.SecondWind(MachinistSettings.Instance)) return true;
            if (await PhysicalDps.Interrupt(MachinistSettings.Instance)) return true;

            if (Utilities.Routines.Machinist.OnGcd)
            {
                if (await Buff.BarrelStabilizer()) return true;
                if (await Buff.Wildfire()) return true;
                if (await Buff.Hypercharge()) return true;
                if (await Turret.AutomationQueen()) return true;
                if (await Turret.Rook()) return true;
                if (await Aoe.Flamethrower()) return true;
                if (await Buff.Reassemble()) return true;
                if (await SingleTarget.GaussRound()) return true;

                if (MachinistSettings.Instance.UseAoe)
                    if (await Aoe.Ricochet()) return true;
                
                return await Buff.Tactician();
            }

            if (MachinistSettings.Instance.UseAoe)
                if (await Aoe.Bioblaster()) return true;
                
            if (await SingleTarget.Drill()) return true;
            if (await SingleTarget.AirAnchor()) return true;
            if (await SingleTarget.HeatBlast()) return true;

            if (MachinistSettings.Instance.UseAoe)
            {
                if (await Aoe.AutoCrossbow()) return true;
                if (await Aoe.SpreadShot()) return true;
            }

            if (await SingleTarget.HotShot()) return true;
            if (await SingleTarget.CleanShot()) return true;
            if (await SingleTarget.SlugShot()) return true;
            return await SingleTarget.SplitShot();
        }
    }
}