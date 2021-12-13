using Magitek.Models.BlueMage;
using ff14bot;
using ff14bot.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.SonicBoom.Cooldown.TotalMilliseconds > 1;
        
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
