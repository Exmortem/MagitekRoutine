using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using System;
using System.Diagnostics;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class RedMage
    {
        public static bool OnGcd => Spells.Jolt.Cooldown > TimeSpan.FromMilliseconds(650);
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies6Yards;
        public static bool InMeleeRange = false;
        public static float Range;
        internal static Stopwatch moveTime { get; } = new Stopwatch();
        public static bool Moving3;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(16);
            AoeEnemies5Yards = Combat.Enemies.Count(x => x.WithinSpellRange(10));
            AoeEnemies6Yards = Combat.Enemies.Count(x => x.WithinSpellRange(12));
            Range = Core.Me.Distance(Core.Me.CurrentTarget);
            InMeleeRange = (Range <= 4.0);

            if (MovementManager.IsMoving)
            {
                moveTime.Restart();
                if (moveTime.Elapsed.TotalMilliseconds > 2500)
                {
                    Moving3 = true;
                }
                Moving3 = false;
            }
        }
    }
}
