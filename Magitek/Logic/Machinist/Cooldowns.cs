using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Cooldowns
    {

        public static async Task<bool> Hypercharge()
        {
            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (Spells.Wildfire.Cooldown != TimeSpan.Zero && MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds > 600 + MachinistGlobals.AnimationLock)
                return false;

            if (Spells.Drill.Cooldown.TotalMilliseconds < 8000 || MachinistGlobals.HotAirAnchor.Cooldown.TotalMilliseconds < 8000)
                return false;

            //add check for dropping combo

            double gcdsUntilNextWildfire = (Spells.Wildfire.Cooldown.TotalMilliseconds -
                                            MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds) / 3000;

            double drillCastsUntilNextWildFire = (Spells.Wildfire.Cooldown.TotalMilliseconds -
                                                  Spells.Drill.Cooldown.TotalMilliseconds) / Spells.Drill.AdjustedCooldown.TotalMilliseconds;

            double airAnchorCastsUntilNextWildFire = (Spells.Wildfire.Cooldown.TotalMilliseconds -
                                                      MachinistGlobals.HotAirAnchor.Cooldown.TotalMilliseconds) / MachinistGlobals.HotAirAnchor.AdjustedCooldown.TotalMilliseconds;

            //if (Math.Truncate(drillCastsUntilNextWildFire) * Spells.Drill.AdjustedCooldown.TotalMilliseconds > 8000)
            //    drillCastsUntilNextWildFire -= Math.Truncate(drillCastsUntilNextWildFire);
            //else
            //    drillCastsUntilNextWildFire -= Math.Truncate(drillCastsUntilNextWildFire) + 1;

            //if (Math.Truncate(airAnchorCastsUntilNextWildFire) * MachinistGlobals.HotAirAnchor.AdjustedCooldown.TotalMilliseconds > 8000)
            //    airAnchorCastsUntilNextWildFire -= Math.Truncate(airAnchorCastsUntilNextWildFire);
            //else
            //    airAnchorCastsUntilNextWildFire -= Math.Truncate(airAnchorCastsUntilNextWildFire) + 1;

            drillCastsUntilNextWildFire = Math.Truncate(drillCastsUntilNextWildFire);
            airAnchorCastsUntilNextWildFire = Math.Truncate(airAnchorCastsUntilNextWildFire);

            int heatGeneratinGCDs = (int)(gcdsUntilNextWildfire - drillCastsUntilNextWildFire 
                                                                - airAnchorCastsUntilNextWildFire);

            if (Spells.Wildfire.Cooldown != TimeSpan.Zero && heatGeneratinGCDs * 5 < 50)
                return false;

            return await Spells.Hypercharge.Cast(Core.Me);
        }

        public static async Task<bool> Wildfire()
        {
            if (!MachinistSettings.Instance.UseWildfire)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (Spells.Drill.Cooldown.TotalMilliseconds < 8000 || MachinistGlobals.HotAirAnchor.Cooldown.TotalMilliseconds < 8000)
                return false;

            //add check for dropping combo

            if (ActionResourceManager.Machinist.Heat < 50 && ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }
        
    }
}