using ff14bot.Enums;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class DarkKnight
    {
        public static WeaveWindow GlobalCooldown
            = new WeaveWindow(ClassJobType.DarkKnight, Spells.HardSlash);

        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.LivingDead,
            Auras.ShadowWall,
            Auras.Rampart,
            Auras.DarkMissionary,
            Auras.BlackestNight,
            Auras.DarkMind,
            Auras.Oblation
        };
    }
}