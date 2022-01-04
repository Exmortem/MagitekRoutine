using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Reaper
    {
        public static int EnemiesAroundPlayer5Yards;
        public static int EnemiesIn8YardCone;
        public static ReaperComboStages CurrentComboStage = ReaperComboStages.Slice;
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Reaper, Spells.Slice);

        // Reaper uses an 8x8 square in front for its "cone". So it can hit something 90* to the side 8y away.
        public static int EnemiesInReaperCone(float maxdistance)
        {
            return Combat.Enemies.Count(r => r.Distance(Core.Me) <= maxdistance + r.CombatReach && r.InCustomRadiantCone(1.57079f));
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