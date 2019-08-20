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

            //oGCDs
            if (await Cooldowns.Wildfire()) return true;
            if (await Cooldowns.Hypercharge()) return true;

            //GCDs
            if (await SingleTarget.HeatBlast()) return true;    //Top HyperCharge Prio

            //Use On CD
            if (await SingleTarget.Drill()) return true;
            if (await SingleTarget.HotAirAnchor()) return true;

            //Default Combo
            if (await SingleTarget.HeatedCleanShot()) return true;
            if (await SingleTarget.HeatedSlugShot()) return true;
            return await SingleTarget.HeatedSplitShot();
        }
    }
}