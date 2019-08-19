using System;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class Warrior
    {
        public static bool OnGcd => Spells.HeavySwing.Cooldown > TimeSpan.FromMilliseconds(500);

        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.Rampart,
            Auras.RawIntuition,
            Auras.Vengeance,
            Auras.Holmgang
        };
    }
}
