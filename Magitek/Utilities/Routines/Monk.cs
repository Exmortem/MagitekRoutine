using System;

namespace Magitek.Utilities.Routines
{
    internal static class Monk
    {
        public static bool OnGcd => Spells.Bootshine.Cooldown.TotalMilliseconds > 400;
        public static int PBStage = 0;
    }
}
