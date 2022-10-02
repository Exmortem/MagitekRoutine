using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class SingleTarget
    {
        public static async Task<bool> HeatedSplitShot()
        {
            //One to disable them all
            if (!MachinistSettings.Instance.UseSplitShotCombo)
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200) && ActionResourceManager.Machinist.Battery <= 80)
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200) && ActionResourceManager.Machinist.Battery <= 80)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            if (!MachinistRoutine.CanContinueComboAfter(Spells.SplitShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            if (!MachinistRoutine.CanContinueComboAfter(Spells.SlugShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistSettings.Instance.UseDrill)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await Spells.Drill.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotAirAnchor()
        {
            if (!MachinistSettings.Instance.UseHotAirAnchor)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            if (MachinistSettings.Instance.UseRookQueen && ActionResourceManager.Machinist.Battery > 80)
                return false;

            return await MachinistRoutine.HotAirAnchor.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GaussRound()
        {
            if (!MachinistSettings.Instance.UseGaussRound)
                return false;

            if (Casting.LastSpell == Spells.Wildfire || Casting.LastSpell == Spells.Hypercharge || Casting.LastSpell == Spells.Ricochet)
                return false;

            if (Spells.Wildfire.IsKnownAndReady() && Spells.Hypercharge.IsKnownAndReady() && Spells.GaussRound.Charges < 1.5f)
                return false;

            if (Core.Me.ClassLevel >= 45)
            {
                if (Spells.GaussRound.Charges < 1.5f && Spells.Wildfire.IsKnownAndReady(2000))
                    return false;

                // Do not run Gauss if an hypercharge is almost ready and not enough charges available for Rico and Gauss
                if (ActionResourceManager.Machinist.Heat > 45 && Spells.Hypercharge.IsKnownAndReady())
                {
                    if (Spells.GaussRound.Charges < 1.5f && Spells.Ricochet.Charges < 0.5f)
                        return false;
                }
            }

            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }
    }
}
