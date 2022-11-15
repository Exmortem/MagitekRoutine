using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Threading.Tasks;
using ff14bot.Managers;
using static ff14bot.Managers.ActionResourceManager.Ninja;
using Magitek.Logic.Roles;

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

        public static async Task<bool> Meisui()
        {
            if (Core.Me.ClassLevel < 72)
                return false;

            if (!NinjaSettings.Instance.UseMeisui)
                return false;

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if(Spells.TrickAttack.IsKnownAndReady(20000))
                return false;

            return await Spells.Meisui.Cast(Core.Me);
        }

        public static async Task<bool> Bunshin()
        {
            if (Core.Me.ClassLevel < 80)
                return false;

            if (!NinjaSettings.Instance.UseBunshin)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.TrickAttack) && !Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityUp))
                return false;

            return await Spells.Bunshin.Cast(Core.Me);
        }

        public static async Task<bool> Kassatsu()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (!NinjaSettings.Instance.UseKassatsu)
                return false;

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            return await Spells.Kassatsu.Cast(Core.Me);
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.Mug.IsKnown() && !Spells.Mug.IsReady(4000))
                return false;

            return await PhysicalDps.UsePotion(NinjaSettings.Instance);
        }

    }
}