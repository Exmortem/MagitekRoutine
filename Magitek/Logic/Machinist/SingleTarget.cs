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

            if (Core.Me.EnemiesInCone(12) > MachinistSettings.Instance.SpreadShotEnemyCount
                && MachinistSettings.Instance.UseSpreadShot)
                return false;

            return await MachinistGlobals.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            if (ActionManager.LastSpell != Spells.SplitShot)
                return false;

            if (Core.Me.EnemiesInCone(12) > MachinistSettings.Instance.SpreadShotEnemyCount
                && MachinistSettings.Instance.UseSpreadShot)
                return false;

            return await MachinistGlobals.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            if (ActionManager.LastSpell != Spells.SlugShot)
                return false;

            if (Core.Me.EnemiesInCone(12) > MachinistSettings.Instance.SpreadShotEnemyCount
                && MachinistSettings.Instance.UseSpreadShot)
                return false;

            return await MachinistGlobals.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistSettings.Instance.UseDrill)
                return false;

            if (Core.Me.EnemiesInCone(12) > MachinistSettings.Instance.BioBlasterEnemyCount
                && MachinistSettings.Instance.UseBioBlaster)
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

            if (Core.Me.EnemiesInCone(12) > MachinistSettings.Instance.AutoCrossbowEnemyCount
                && MachinistSettings.Instance.UseAutoCrossbow)
                return false;

            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GaussRound()
        {
            if (!MachinistSettings.Instance.UseGaussRound)
                return false;

            //add some mor precise logic for pooling/dumping
            if (Spells.GaussRound.Charges < 1.8f)
                return false;

            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }
    }
}
