using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.BlueMage;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.SonicBoom.Cooldown.TotalMilliseconds > 1;

        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.BlueMage, Spells.SonicBoom, new List<SpellData>() { Spells.PhantomFlurry });

        public static bool IsSurpanakhaInProgress => Casting.LastSpell == Spells.Surpanakha && Spells.Surpanakha.Charges >= 1.0f;

        public static bool IsMoonFluteTakenActivatedAndWindowReady => ActionManager.HasSpell(Spells.MoonFlute.Id)
            ? BlueMageSettings.Instance.UseMoonFlute ? IsMoonFluteWindowReady : false
            : false;

        public static bool IsMoonFluteWindowReady => !Core.Me.HasAura(Auras.WaxingNocturne)
                && Spells.JKick.Cooldown.TotalMilliseconds < 3000
                && Spells.Surpanakha.Charges > 3.75f
                && Spells.NightBloom.Cooldown.TotalMilliseconds <= 10000;

        public static bool NeedToInterruptCast()
        {
            if (Casting.SpellTarget?.CurrentHealth == 0)
            {
                Logger.Error($"Stopped {Casting.CastingSpell.LocalizedName}: because Target is dead");
                return true;
            }
            return false;
        }
    }
}
