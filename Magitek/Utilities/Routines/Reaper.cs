using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Reaper
    {
        public static int EnemiesAroundPlayer5Yards;
        public static int EnemiesIn8YardCone;
        public static ReaperComboStages CurrentComboStage = ReaperComboStages.Slice;

        // Reaper uses an 8x8 square in front for its "cone". So it can hit something 90* to the side 8y away.
        public static bool InReaperView(GameObject target)
        {
            if (target == null)
                return false;

            if (target == Core.Me)
                return true;

            return target.RadiansFromPlayerHeading() < 1.57079f; //This is Pi/2 radians, or 90 degrees left or right
        }

        public static int EnemiesInReaperCone(float maxdistance)
        {
            return Combat.Enemies.Count(r => r.Distance(Core.Me) <= maxdistance + r.CombatReach && InReaperView(r));
        }

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesIn8YardCone = EnemiesInReaperCone(8);
            EnemiesAroundPlayer5Yards = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);
        }
    }
}