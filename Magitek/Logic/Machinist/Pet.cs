using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using System;
using System.Threading.Tasks;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Pet
    {
        public static async Task<bool> RookQueen()
        {
            if (!MachinistSettings.Instance.UseRookQueen)
                return false;

            if (ActionResourceManager.Machinist.SummonRemaining > TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Machinist.Battery == 100) // For 2 min burst, let's use Queen at 100 battery
                return await MachinistRoutine.RookQueenPet.Cast(Core.Me);

            if (Spells.ChainSaw.IsKnownAndReady() && Spells.Drill.IsKnownAndReady() && Spells.AirAnchor.IsKnownAndReady())
                return await MachinistRoutine.RookQueenPet.Cast(Core.Me);

            if (Spells.ChainSaw.IsKnown() && !Spells.ChainSaw.IsReady(3000))
                return false;

            //Logger.WriteInfo($@"[RookQueen] used with {ActionResourceManager.Machinist.Battery} Battery");
            return await MachinistRoutine.RookQueenPet.Cast(Core.Me);
        }

        public static async Task<bool> RookQueenOverdrive()
        {
            if (!MachinistSettings.Instance.UseRookQueenOverdrive)
                return false;

            if (Core.Me.CurrentTarget.CombatTimeLeft() <= 2 && Core.Me.CurrentTarget.CurrentHealthPercent < 2)
                return await MachinistRoutine.RookQueenOverdrive.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}
