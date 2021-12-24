using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class Warrior
    {

        public static SpellData FellCleave => Core.Me.ClassLevel < 54
                                            ? Spells.InnerBeast
                                            : Spells.FellCleave;

        public static SpellData Decimate => Core.Me.ClassLevel < 60
                                            ? Spells.SteelCyclone
                                            : Spells.Decimate;

        public static SpellData InnerRelease => Core.Me.ClassLevel < 70
                                            ? Spells.Berserk
                                            : Spells.InnerRelease;

        public static SpellData Bloodwhetting => Core.Me.ClassLevel < 82
                                            ? Spells.RawIntuition
                                            : Spells.Bloodwhetting;

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
