using Magitek.Models.BlueMage;
using System;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.AbyssalTransfixion.Cooldown.TotalMilliseconds > 1;
    }
}
