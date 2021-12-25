using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
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


        public static bool ToggleAndSpellCheck(bool Toggle, SpellData Spell)
        {
            if (!Toggle)
                return false;

            if (!ActionManager.HasSpell(Spell.Id))
                return false;

            return true;
        }

        public static bool CanContinueComboAfter(SpellData LastSpellExecuted)
        {
            if (ActionManager.ComboTimeLeft <= 0)
                return false;

            if (ActionManager.LastSpell.Id != LastSpellExecuted.Id)
                return false;

            return true;
        }

        public static readonly List<uint> Defensives = new List<uint>()
        {
            Auras.Rampart,
            Auras.RawIntuition,
            Auras.Bloodwhetting,
            Auras.Vengeance,
            Auras.Holmgang,
            Auras.ThrillOfBattle
        };
    }
}
