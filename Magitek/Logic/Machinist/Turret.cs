using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;

namespace Magitek.Logic.Machinist
{
    internal static class Turret
    {
        public static async Task<bool> Rook()
        {
            if (Core.Me.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            if (ActionResourceManager.Machinist.Battery < MachinistSettings.Instance.MinBatteryForTurretSummon)
                return false;

            return await Spells.RookAutoturret.Cast(Core.Me);
        }

        public static async Task<bool> RookOverdrive()
        {
            if (Core.Me.Pet.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            return await Spells.RookOverdrive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AutomationQueen()
        {
            if (Core.Me.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            return await Spells.AutomationQueen.Cast(Core.Me);
        }

        public static async Task<bool> QueenOverdrive()
        {
            if (Core.Me.Pet.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            return await Spells.QueenOverdrive.Cast(Core.Me.CurrentTarget);
        }
    }
}
