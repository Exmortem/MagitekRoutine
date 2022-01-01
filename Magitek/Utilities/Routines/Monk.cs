using ff14bot;
using ff14bot.Enums;
using Magitek.Extensions;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Monk
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Monk, Spells.Bootshine);
        public static int EnemiesInCone;
        public static int AoeEnemies8Yards;
        public static int UseToast = 9;
        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();
            EnemiesInCone = Core.Me.EnemiesInCone(40);
        }
    }
}
