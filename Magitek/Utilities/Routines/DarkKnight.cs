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

        public static bool CanDarkArts(SpellData spell)
        {
            if (!DarkKnightSettings.Instance.DarkArts)
                return false;

            if (!ActionManager.HasSpell(Spells.DarkArts.Id))
                return false;

            if (!Core.Me.HasAura(Auras.Darkside))
                return false;

            if (Core.Me.CurrentManaPercent < DarkKnightSettings.Instance.DarkArtsMinimumMp)
                return false;

            if (Spells.DarkArts.Cost + spell.Cost > Core.Me.CurrentMana)
                return false;

            return Spells.DarkArts.Cooldown == TimeSpan.Zero;       
        }
    }
}