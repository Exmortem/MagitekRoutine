using System.Collections.Generic;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Utilities.Routines
{
    internal static class Machinist
    {
        public static bool OnGcd => Spells.SplitShot.Cooldown.TotalMilliseconds > 650;

        private static List<SpellData> _ogcdSpells = new List<SpellData>
        {
            Spells.GaussRound
        };
    }
}
