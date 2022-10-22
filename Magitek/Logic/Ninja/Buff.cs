using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Threading.Tasks;
using ff14bot.Managers;
using static ff14bot.Managers.ActionResourceManager.Ninja;
using Magitek.Logic.Roles;
using Magitek.Models.Samurai;

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
            
            return await Spells.Bunshin.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorth()
        {
       
            if (!NinjaSettings.Instance.UseTrueNorth
                || Core.Me.HasAura(Auras.TrueNorth))
                return false;

                if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 3000 && !Core.Me.CurrentTarget.IsBehind)
                    return await Spells.TrueNorth.Cast(Core.Me);

                if (ActionManager.LastSpell == Spells.GustSlash 
                                    && !Core.Me.CurrentTarget.IsFlanking 
                                    && HutonTimer.TotalMilliseconds <= 30000 
                                    && 
                                       (Spells.TrueNorth.Charges == 2 
                                    || (Spells.TrueNorth.Charges > 1 && Spells.TrueNorth.Charges < 2
                                       && Spells.TrickAttack.Cooldown.TotalMilliseconds > Spells.TrueNorth.Cooldown.TotalMilliseconds)))

                    return await Spells.TrueNorth.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> Meisui()
        {

            if (!Core.Me.HasAura(Auras.Suiton) 
                || NinkiGauge > 40)
                return false;                     
        
            return await Spells.Meisui.Cast(Core.Me);
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.TenChiJin.IsKnown() && !Spells.TenChiJin.IsReady(4000))
                return false;

            return await PhysicalDps.UsePotion(NinjaSettings.Instance);
        }

    }
}