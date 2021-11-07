using Magitek.Models.BlueMage;
using ff14bot;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.SonicBoom.Cooldown.TotalMilliseconds > 1;
        
        public static bool IsSurpanakhaInProgress => Casting.LastSpell == Spells.Surpanakha && Spells.Surpanakha.Charges >= 1.0f;

        public static bool IsMoonFluteWindowReady => BlueMageSettings.Instance.UseMoonFlute ?
            !Core.Me.HasAura(Auras.WaxingNocturne)
            && Spells.JKick.Cooldown.TotalMilliseconds < 3000 
            && Spells.Surpanakha.Charges > 3.75f
            && Spells.NightBloom.Cooldown.TotalMilliseconds <= 10000
            : true;
    }
}
