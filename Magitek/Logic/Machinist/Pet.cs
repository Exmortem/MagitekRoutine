using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using System;
using System.Threading.Tasks;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Pet
    {
        public static async Task<bool> RookQueen()
        {
            if (!MachinistSettings.Instance.UseRookQueen)
                return false;

            if (Spells.Hypercharge.IsKnown() && Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (ActionResourceManager.Machinist.Battery < MachinistSettings.Instance.MinBatteryForPetSummon)
                return false;

            if (MachinistSettings.Instance.UseBuffedRookQueen)
            {
                if (!Utilities.Routines.Machinist.CheckCurrentDamageIncrease(MachinistSettings.Instance.UseRookQueenWithAtLeastXBonusDamage) && ActionResourceManager.Machinist.Battery < 80)
                    return false;
            }

            return await MachinistGlobals.RookQueenPet.Cast(Core.Me);
        }

        public static async Task<bool> RookQueenOverdrive()
        {
            if (!MachinistSettings.Instance.UseRookQueenOverdrive)
                return false;

            if (Core.Me.CurrentTarget.CombatTimeLeft() <= 2 && Core.Me.CurrentTarget.CurrentHealthPercent < 2)
                return await MachinistGlobals.RookQueenOverdrive.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}
