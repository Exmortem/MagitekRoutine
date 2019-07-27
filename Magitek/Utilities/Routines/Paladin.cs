using System;
using System.Collections.Generic;
using Magitek.Utilities;
using Magitek.Models.Paladin;

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

        public static bool OnGcd => Spells.FastBlade.Cooldown.TotalMilliseconds > 100;
        public static bool OGCDHold => Spells.FastBlade.Cooldown.TotalMilliseconds < 650 + PaladinSettings.Instance.PingValue;
    }
}