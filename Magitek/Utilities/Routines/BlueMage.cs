using System;
using Magitek.Models.BlueMage;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.AbyssalTransfixion.Cooldown.TotalMilliseconds > 1;
        
    }
}
