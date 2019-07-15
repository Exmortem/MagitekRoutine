using System;
using System.Collections.Generic;
using ff14bot.Objects;

namespace Magitek.Utilities.Routines
{
    internal static class Dragoon
    {
        public static bool OnGcd => Spells.TrueThrust.Cooldown > TimeSpan.FromMilliseconds(500);
        public static DateTime CanBloodOfTheDragonAgain = DateTime.Now;
        public static int EnemiesInView;
        public static int MirageDives;

        public static List<SpellData> Jumps = new List<SpellData>()
        {
            Spells.Jump,
            Spells.SpineshatterDive,
            Spells.DragonfireDive,
            Spells.MirageDive,
            Spells.Stardiver
        };
    }
}
