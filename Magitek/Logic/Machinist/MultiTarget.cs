using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class MultiTarget
    {
        public static async Task<bool> SpreadShot()
        {
            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.SpreadShotEnemyCount
                || !MachinistSettings.Instance.UseSpreadShot)
                return false;

            return await Spells.SpreadShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioBlaster()
        {
            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.BioBlasterEnemyCount 
                || !MachinistSettings.Instance.UseBioBlaster)
                return false;

            return await Spells.Bioblaster.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AutoCrossbow()
        {
            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AutoCrossbowEnemyCount
                || !MachinistSettings.Instance.UseAutoCrossbow)
                return false;

            return await Spells.AutoCrossbow.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Ricochet()
        {
            if (!MachinistSettings.Instance.UseRicochet)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            //add some mor precise logic for pooling/dumping
            if (Spells.Ricochet.Charges < 1.8f)
                return false;

            return await Spells.Ricochet.Cast(Core.Me.CurrentTarget);
        }

    }
}
