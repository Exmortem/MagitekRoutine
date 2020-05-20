using System;

namespace Magitek.Utilities.Routines
{
    internal static class RedMage
    {
        //TODO: Can we take lag into account here?
        public static bool CanWeave => Spells.Riposte.Cooldown.TotalMilliseconds >= Globals.AnimationLockMs;
    }
}
