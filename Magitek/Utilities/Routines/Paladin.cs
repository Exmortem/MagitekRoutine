using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.Account;
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

        public static int RequiescatStackCount => Core.Me.CharacterAuras.GetAuraStacksById(Auras.Requiescat);
        public static bool OnGcd => Spells.FastBlade.Cooldown.TotalMilliseconds > 100;
        public static bool OGCDHold => Spells.FastBlade.Cooldown.TotalMilliseconds < 650 + BaseSettings.Instance.UserLatencyOffset;
        public static bool ToggleAndSpellCheck(bool Toggle, SpellData Spell)
        {
            if (!Toggle)
                return false;

            if (!ActionManager.HasSpell(Spell.Id))
                return false;

            return true;
        }
    }
}