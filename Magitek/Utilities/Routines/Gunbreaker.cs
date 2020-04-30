using System;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class Gunbreaker

    {
        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.Camouflage,
            Auras.Nebula,
            Auras.Aurora,
            Auras.Superbolide,
            Auras.HeartofLight,
            Auras.HeartofStone,
        };

        public static bool OnGcd => Spells.KeenEdge.Cooldown > TimeSpan.FromMilliseconds(500);
    }
}