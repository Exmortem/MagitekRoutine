using System;
using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.DarkKnight;
using Magitek.Models.QueueSpell;

namespace Magitek.Utilities.Routines
{
    internal static class DarkKnight
    {
        public static DateTime LastUnleash;
        public static int PullUnleash;

        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.Foresight,
            Auras.LivingDead,
            Auras.DarkDance,
            Auras.Shadowskin,
            Auras.ShadowWall
        };
        
        public static bool OnGcd => Spells.HardSlash.Cooldown > TimeSpan.FromMilliseconds(500);
    }
}