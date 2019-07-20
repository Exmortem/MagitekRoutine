using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Gunbreaker;
using Magitek.Logic.Roles;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;

namespace Magitek.Rotations.Gunbreaker
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;
            
            if (await Defensive.ExecuteTankBusters()) return true;
            
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
            }

            if (await SingleTarget.RoughDivide()) return true;
            if (await Tank.Interrupt(GunbreakerSettings.Instance)) return true;
            
            if (Utilities.Routines.Gunbreaker.OnGcd)
            {
                if (await Defensive.Execute()) return true;
                if (await Buff.RoyalGuard()) return true;
                if (await Buff.Bloodfest()) return true;
                if (await Buff.NoMercy()) return true;
                if (await SingleTarget.EyeGouge()) return true;
                if (await SingleTarget.AbdomenTear()) return true;
                if (await SingleTarget.JugularRip()) return true;
                if (await SingleTarget.DangerZone()) return true;
                if (await SingleTarget.BlastingZone()) return true;

                if(GunbreakerSettings.Instance.UseAoe)
                    if (await Aoe.BowShock()) return true;
            }

            if (GunbreakerSettings.Instance.UseAoe)
            {
                if (await Aoe.FatedCircle()) return true;
                if (await Aoe.DemonSlaughter()) return true;
                if (await Aoe.DemonSlice()) return true;
            }

            if (await SingleTarget.LightningShot()) return true;
            if (await SingleTarget.WickedTalon()) return true;
            if (await SingleTarget.SavageClaw()) return true;
            if (await SingleTarget.GnashingFang()) return true;
            if (await SingleTarget.SonicBreak()) return true;
            if (await SingleTarget.BurstStrike()) return true;
            if (await SingleTarget.SolidBarrel()) return true;
            if (await SingleTarget.BrutalShell()) return true;
            return await SingleTarget.KeenEdge();
        }
    }
}