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
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseSplitShotCombo, MachinistRoutine.HeatedSplitShot))
                return false; 

            if (MachinistSettings.Instance.UseDrill && Spells.IsAvailableAndReadyInLessThanXMs(Spells.Drill, 200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.IsAvailableAndReadyInLessThanXMs(Spells.AirAnchor, 200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.IsAvailableAndReadyInLessThanXMs(Spells.ChainSaw, 200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            if (!ActionManager.HasSpell(MachinistRoutine.HeatedSlugShot.Id))
                return false;

            if (!MachinistRoutine.CanContinueComboAfter(Spells.SplitShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.IsAvailableAndReadyInLessThanXMs(Spells.Drill, 200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.IsAvailableAndReadyInLessThanXMs(Spells.AirAnchor, 200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.IsAvailableAndReadyInLessThanXMs(Spells.ChainSaw, 200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            if (!ActionManager.HasSpell(MachinistRoutine.HeatedCleanShot.Id))
                return false;

            if (!MachinistRoutine.CanContinueComboAfter(Spells.SlugShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.IsAvailableAndReadyInLessThanXMs(Spells.Drill, 200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.IsAvailableAndReadyInLessThanXMs(Spells.AirAnchor, 200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.IsAvailableAndReadyInLessThanXMs(Spells.ChainSaw, 200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistRoutine.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseDrill, Spells.Drill))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            return await Spells.Drill.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotAirAnchor()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseHotAirAnchor, MachinistRoutine.HotAirAnchor))
                return false; 

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            if (ActionResourceManager.Machinist.Battery >= 80)
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

            if (!MachinistRoutine.IsInWeaveingWindow)
                return false;

            if (Casting.LastSpell == Spells.Wildfire || Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Spells.IsAvailableAndReady(Spells.Wildfire) && Spells.IsAvailableAndReady(Spells.Hypercharge) && Spells.GaussRound.Charges < 1.5f)
                return false;

            if (Core.Me.ClassLevel >= 45)
            {
                if (Spells.GaussRound.Charges < 1.5f && Spells.IsAvailableAndReadyInLessThanXMs(Spells.Wildfire, 2000))
                    return false;

                // Do not run Gauss if an hypercharge is almost ready and not enough charges available for Rico and Gauss
                if (ActionResourceManager.Machinist.Heat > 45 && Spells.IsAvailableAndReady(Spells.Hypercharge))
                {
                    if (Spells.GaussRound.Charges < 1.5f && Spells.Ricochet.Charges < 0.5f)
                        return false;
                }
            }

            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }
    }
}
