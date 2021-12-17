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

        public static void RefreshVars()
        {

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesIn8YardCone = Core.Me.EnemiesInCone(8);
            EnemiesAroundPlayer5Yards = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);

        }

    }
}