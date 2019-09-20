using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Machinist;
using Magitek.Logic.Roles;
using Magitek.Models.Machinist;
using Magitek.Models.Account;
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

            if (MachinistSettings.Instance.UseFlamethrower && Core.Me.HasAura(Auras.Flamethrower))
            {
                if (!MachinistSettings.Instance.UseFlamethrower)
                    return true;

                if (MovementManager.IsMoving)
                    return false;

                if (Core.Me.EnemiesInCone(8) >= MachinistSettings.Instance.FlamethrowerEnemyCount)
                    return true;
            }

            //oGCDs

            //Utility
            if (await Utility.Tactician()) return true;
            if (await PhysicalDps.ArmsLength(MachinistSettings.Instance)) return true;
            if (await PhysicalDps.SecondWind(MachinistSettings.Instance)) return true;
            if (await Utility.HeadGraze()) return true;
            //Cooldowns
            if (await Cooldowns.Reassemble()) return true;
            if (await Cooldowns.Wildfire()) return true;
            if (await Cooldowns.Hypercharge()) return true;
            if (await Cooldowns.BarrelStabilizer()) return true;
            if (await Pet.RookQueen()) return true;
            if (await Pet.RookQueenOverdrive()) return true;
            //oGCDs
            if (await SingleTarget.GaussRound()) return true;
            if (await MultiTarget.Ricochet()) return true;
            



            //GCDs - Top Hypercharge Priority
            if (await MultiTarget.AutoCrossbow()) return true;
            if (await SingleTarget.HeatBlast()) return true;

            //Use On CD
            if (await MultiTarget.BioBlaster()) return true;
            if (await SingleTarget.Drill()) return true;
            if (await SingleTarget.HotAirAnchor()) return true;
            if (await MultiTarget.Flamethrower()) return true;

            //Default Combo
            if (await MultiTarget.SpreadShot()) return true;
            if (await SingleTarget.HeatedCleanShot()) return true;
            if (await SingleTarget.HeatedSlugShot()) return true;
            return await SingleTarget.HeatedSplitShot();
        }
    }
}