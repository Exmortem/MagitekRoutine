using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Summoner
    {
        public static bool OnGcd => Spells.Ruin.Cooldown.TotalMilliseconds > 100;

        public static uint[] BioAuras = { Auras.Bio, Auras.Bio2, Auras.Bio3 };
        public static uint[] MiasmaAuras = { Auras.Miasma, Auras.Miasma3 };
        public static HashSet<int> DemiSummonIds = new HashSet<int> { 10, 14 };

        public static bool NeedToInterruptCast()
        {
            /*if (Casting.CastingTankBuster || Casting.SpellTarget == null)
                return false;*/

            if (ActionResourceManager.Summoner.TranceTimer <= 1 &&
                Core.Me.IsCasting)
            {
                Logger.Error($"Stopped {Casting.CastingSpell.LocalizedName}: so we don't lose Deathflare");
            }

            if (Casting.CastingSpell == Spells.Resurrection && Casting.SpellTarget?.CurrentHealth > 1)
            {
                Logger.Error("Stopped Resurrection: Unit is now alive");
                return true;
            }

            return false;
        }
    }
}
