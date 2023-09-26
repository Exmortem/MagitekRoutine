using ff14bot;
using ff14bot.Enums;
using Magitek.Converters;
using Magitek.Extensions;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Monk
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Monk, Spells.Bootshine);
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int UseToast = 9;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;


            EnemiesInCone = Core.Me.EnemiesInCone(40);
            AoeEnemies5Yards = Combat.Enemies.Count(x => x.WithinSpellRange(5) && x.IsTargetable && x.IsValid && !x.HasAnyAura(Auras.Invincibility) && x.NotInvulnerable());
        }
    }
}
