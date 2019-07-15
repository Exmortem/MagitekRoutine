using System;

namespace Magitek.Utilities.Routines
{
    internal static class RedMage
    {
        public static bool OnGcd => Spells.Riposte.Cooldown > TimeSpan.FromMilliseconds(300);
    }
}
