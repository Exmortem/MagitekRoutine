using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Ninja
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Ninja, Spells.SpinningEdge, new List<SpellData>() { Spells.Ten, Spells.Jin, Spells.Chi, Spells.Ninjutsu });

        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;

        public static List<SpellData> UsedMudras = new List<SpellData>();

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat && UsedMudras.Count() > 0 && !Core.Me.HasMyAura(Auras.Mudra))
            {
                UsedMudras.Clear();
            }

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

        }
    }
}
