using System;
using ff14bot.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Machinist
    {
        public static bool OnGcd => Spells.SplitShot.Cooldown.TotalMilliseconds > 650;
        public static double GcdTimeRemaining => Spells.SplitShot.Cooldown.TotalMilliseconds;
    }
}
