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

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 9000)
                return await Spells.Kassatsu.Cast(Core.Me);

            return false;
        }
        
        public static async Task<bool> Bunshin()
        {
            if (!NinjaSettings.Instance.UseBunshin)
                return false;
            if (Spells.SpinningEdge.Cooldown.TotalMilliseconds < 850)
                return false;
            return await Spells.Bunshin.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorth()
        {
            if (!NinjaSettings.Instance.UseTrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            //Do we need to for TA?
            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }

        public static async Task<bool> Meisui()
        {

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if (Spells.TrickAttack.Cooldown.Seconds < 30 && Spells.TenChiJin.Cooldown.Seconds < 15 || !Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
                return false;

            return await Spells.Meisui.Cast(Core.Me);
        }

    }
}