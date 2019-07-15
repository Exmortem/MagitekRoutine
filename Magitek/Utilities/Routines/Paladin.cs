using System;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class Paladin
    {
        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.HallowedGround,
            Auras.Rampart,
            Auras.Sentinel
        };
        
        public static bool OnGcd => Spells.FastBlade.Cooldown > TimeSpan.FromMilliseconds(500);
    }
}