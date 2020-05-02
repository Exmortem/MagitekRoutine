using System;

namespace Magitek.Utilities.Routines
{
    internal static class RedMage
    {
        public static bool CanWeave => Spells.Riposte.Cooldown.TotalMilliseconds >= 700;
    }
}
