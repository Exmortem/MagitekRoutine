using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;
using MonkRoutine = Magitek.Utilities.Routines.Monk;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Monk
{
    internal static class Aoe
    {

        public static async Task<bool> Enlightenment()
        {
            if (!MonkSettings.Instance.UseEnlightenment)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Core.Me.ClassLevel < 40)
                return false;

            if (MonkRoutine.EnemiesInCone < MonkSettings.Instance.AoeEnemies)
                return false;

            if (ActionResourceManager.Monk.ChakraCount < 5)
                return false;

            return await Spells.HowlingFist.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MasterfulBlitz()
        {
            if (!MonkSettings.Instance.UseMasterfulBlitz)
                return false;

            if (Core.Me.ClassLevel < 60)
                return false;

            if (!Spells.MasterfulBlitz.IsKnownAndReady())
                return false;

            return await Spells.MasterfulBlitz.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Rockbreaker()
        {
            if (Core.Me.ClassLevel < 30)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.CoeurlForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            return await Spells.Rockbreaker.Cast(Core.Me);
        }

        public static async Task<bool> FourPointStrike()
        {
            if (Core.Me.ClassLevel < 45)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);

            return await Spells.FourPointFury.Cast(Core.Me);
        }

        public static async Task<bool> ArmOfDestroyer()
        {
            if (Core.Me.ClassLevel < 26)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies)
                return false;

            return await Spells.ArmOfTheDestroyer.Cast(Core.Me);
        }
    }
}
