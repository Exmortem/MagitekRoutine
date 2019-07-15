using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;

namespace Magitek.Logic.Ninja
{
    internal static class Buff
    {
        public static async Task<bool> ShadeShift()
        {
            if (!NinjaSettings.Instance.UseShadeShift)
                return false;

            if (Core.Me.CurrentHealthPercent > NinjaSettings.Instance.ShadeShiftHealthPercent)
                return false;

            return await Spells.ShadeShift.Cast(Core.Me);
        }

        public static async Task<bool> Kassatsu()
        {
            if (!NinjaSettings.Instance.UseKassatsu)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            if (Spells.TrickAttack.Cooldown.Seconds > 20000)
                return await Spells.Kassatsu.Cast(Core.Me);

            if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
                return await Spells.Kassatsu.Cast(Core.Me);

            return false;
        }
        
        public static async Task<bool> Bunshin()
        {
            if (!NinjaSettings.Instance.UseBunshin)
                return false;

            return await Spells.Bunshin.Cast(Core.Me);
        }
    }
}