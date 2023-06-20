using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
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

        private static bool TenChiJin = false;

        public static List<SpellData> UsedMudras = new List<SpellData>();

        public static void RefreshVars()
        {

            switch (UsedMudras.Count)
            {
                case 0:
                    break; 

                case 1:

                    if (Core.Me.HasMyAura(Auras.Mudra))
                        break;

                    if (!Core.Me.HasMyAura(Auras.Mudra) && new List<SpellData>() { Spells.Ten, Spells.Chi, Spells.Jin }.Contains(Casting.SpellCastHistory.First().Spell))
                        break;

                    UsedMudras.Clear();
                    break;

                case 2:

                case 3:

                    if (Core.Me.HasMyAura(Auras.Mudra))
                        break;

                    UsedMudras.Clear();
                    break;

                default:
                    break;
            }

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            if (!TenChiJin && Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell == Spells.TenChiJin)
            {
                TenChiJin = true;
            }
            if (TenChiJin && Core.Me.HasMyAura(Auras.TenChiJin))
            {
                TenChiJin = false;
            }

            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

        }
    }
}
