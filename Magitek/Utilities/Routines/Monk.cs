using ff14bot;
using Magitek.Extensions;
using System;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Monk
    {
        public static bool OnGcd => Spells.Bootshine.Cooldown.TotalMilliseconds > 400;
        public static int PBStage = 0;
        public static int AoeEnemies8Yards;
        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();
        }
    }
}
