using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using System;
using System.Runtime.CompilerServices;

namespace Magitek.Extensions
{
    internal static class PetSpellDataExtensions
    {
        public static void PetCast(this PetSpellData spell, GameObject tar, [CallerMemberName] string name = "")
        {
            if (Core.Me.Pet == null)
                return;

            if (tar == null)
                return;

            if (Core.Me.Pet.IsCasting)
                return;

            if (spell.Cooldown != TimeSpan.Zero)
                return;

            if (!PetManager.CanCast(spell, tar))
                return;

            PetManager.DoAction(spell.LocalizedName, tar);
        }
    }
}
