﻿using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Pet
    {
        public static async Task<bool> RookQueen()
        {
            if (!MachinistSettings.Instance.UseRookQueen)
                return false;

            if (Core.Me.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            if (ActionResourceManager.Machinist.Battery < MachinistSettings.Instance.MinBatteryForPetSummon)
                return false;

            return await MachinistGlobals.RookQueenPet.Cast(Core.Me);
        }

        public static async Task<bool> RookQueenOverdrive()
        {
            if (!MachinistSettings.Instance.UseRookQueenOverdrive)
                return false;

            if (Core.Me.Distance2D(Core.Me.CurrentTarget) > 30)
                return false;

            return await MachinistGlobals.RookQueenOverdrive.Cast(Core.Me.CurrentTarget);
        }
    }
}
