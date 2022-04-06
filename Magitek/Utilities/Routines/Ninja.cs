using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Extensions;
using System;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Ninja
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Ninja, Spells.SpinningEdge);

        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static int TCJState = 0;
        public static double ninki;
        public static bool OnGcd => Spells.SpinningEdge.Cooldown > TimeSpan.FromMilliseconds(100);
        public static bool CanCastNinjutsu => SpellDataExtensions.CanCast(Spells.Ninjutsu, null);

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();
            ninki = ActionResourceManager.Ninja.NinkiGauge;
        }
    }
}
