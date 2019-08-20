using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Navigation;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class SingleTarget
    {
        public static async Task<bool> HeatedSplitShot()
        {
            //One to disable them all
            if (!MachinistSettings.Instance.UseSplitShotCombo)
                return false;

            return await MachinistGlobals.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            if (ActionManager.LastSpell != MachinistGlobals.HeatedSplitShot)
                return false;

            return await MachinistGlobals.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            if (ActionManager.LastSpell != MachinistGlobals.HeatedSlugShot)
                return false;

            return await MachinistGlobals.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistSettings.Instance.UseDrill)
                return false;

            return await Spells.Drill.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotAirAnchor()
        {
            if (!MachinistSettings.Instance.UseHotAirAnchor)
                return false;

            return await MachinistGlobals.HotAirAnchor.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

    }
}
