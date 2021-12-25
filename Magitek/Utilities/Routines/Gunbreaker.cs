using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Models.Gunbreaker;
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
            Auras.HeartOfCorundum
        };

        public static SpellData HeartOfCorundum => Core.Me.ClassLevel < 82
                                            ? Spells.HeartofStone
                                            : Spells.HeartOfCorundum;

        public static SpellData BlastingZone => Core.Me.ClassLevel < 80
                                            ? Spells.DangerZone
                                            : Spells.BlastingZone;

        public static int MaxCartridge => Core.Me.ClassLevel < 88 ? 2 : 3;
        public static int AmountCartridgeFromBloodfest => Core.Me.ClassLevel < 88 ? 2 : 3;
        public static int RequiredCartridgeForGnashingFang => 1;
        public static int RequiredCartridgeForDoubleDown => 2;
        public static int RequiredCartridgeForBurstStrike => 1;
        public static int RequiredCartridgeForFatedCircle => 1;

        public static bool ToggleAndSpellCheck(bool Toggle, SpellData Spell)
        {
            if (!Toggle)
                return false;

            if (!ActionManager.HasSpell(Spell.Id))
                return false;

            return true;
        }

        public static bool IsAurasForComboActive()
        {
            return Core.Me.HasAura(Auras.ReadytoRip) || Core.Me.HasAura(Auras.ReadytoTear) || Core.Me.HasAura(Auras.ReadytoGouge) || Core.Me.HasAura(Auras.ReadytoBlast);
        }

        public static bool CanContinueComboAfter(SpellData LastSpellExecuted)
        {
            if (ActionManager.ComboTimeLeft <= 0)
                return false;

            if (ActionManager.LastSpell.Id != LastSpellExecuted.Id)
                return false;

            return true;
        }

    }
}