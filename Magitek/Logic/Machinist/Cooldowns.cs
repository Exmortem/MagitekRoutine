using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Cooldowns
    {
        public static async Task<bool> BarrelStabilizer()
        {
            if (!MachinistSettings.Instance.UseBarrelStabilizer)
                return false;

            if (!Spells.BarrelStabilizer.IsReady())
                return false;

            if (ActionResourceManager.Machinist.Heat > 50)
                return false;

            if (Spells.Reassemble.IsKnown() && Spells.Reassemble.Charges > 1)
                return false;

            if (Spells.Wildfire.Cooldown.Milliseconds > 0 && Spells.Wildfire.Cooldown.Milliseconds <= 6000)
                return false;

            return await Spells.BarrelStabilizer.Cast(Core.Me);
        }

        public static async Task<bool> Hypercharge()
        {
            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (ActionResourceManager.Machinist.Heat < 50)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Spells.Wildfire.IsKnownAndReady())
                return false;

            //Force cast during wildfire
            if (Spells.Wildfire.IsKnown() && (Casting.LastSpell == Spells.Wildfire || Core.Me.HasAura(Auras.WildfireBuff, true)))
                return await Spells.Hypercharge.Cast(Core.Me);


            if (MachinistSettings.Instance.DelayHypercharge)
            {
                if (Spells.Drill.IsKnown() && Spells.Drill.Cooldown.Seconds <= MachinistSettings.Instance.DelayHyperchargeSeconds)
                    return false;

                if (Spells.AirAnchor.IsKnown() && Spells.AirAnchor.Cooldown.Seconds <= MachinistSettings.Instance.DelayHyperchargeSeconds)
                    return false;

                if (Spells.ChainSaw.IsKnown() && Spells.ChainSaw.Cooldown.Seconds <= MachinistSettings.Instance.DelayHyperchargeSeconds)
                    return false;
            }

            return await Spells.Hypercharge.Cast(Core.Me);
        }

        public static async Task<bool> Wildfire()
        {
            if (!MachinistSettings.Instance.UseWildfire)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true) || Casting.SpellCastHistory.Any(x => x.Spell == Spells.Wildfire))
                return false;

            if (ActionResourceManager.Machinist.Heat < 50 && ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (MachinistSettings.Instance.DelayWildfire) { 
                if (Spells.Drill.IsKnown() && Spells.Drill.Cooldown.Seconds <= MachinistSettings.Instance.DelayWildfireSeconds)
                    return false;

                if (Spells.AirAnchor.IsKnown() && Spells.AirAnchor.Cooldown.Seconds <= MachinistSettings.Instance.DelayWildfireSeconds)
                    return false;

                if (Spells.ChainSaw.IsKnown() && Spells.ChainSaw.Cooldown.Seconds <= MachinistSettings.Instance.DelayWildfireSeconds)
                    return false;
            }

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Reassemble()
        {
            if (!MachinistSettings.Instance.UseReassemble)
                return false;

            if (Spells.Reassemble.Charges < 1)
                return false;

            // Added check for cooldown, gets stuck at lower levels otherwise.
            if (Spells.Reassemble.Charges == 0 && !Spells.Reassemble.IsReady())
                return false;

            if (Core.Me.ClassLevel < 58)
            {
                if (ActionManager.LastSpell != MachinistRoutine.HeatedSlugShot)
                    return false;
            }

            if (Core.Me.ClassLevel >= 58 && Core.Me.ClassLevel < 76)
            {
                if (MachinistSettings.Instance.UseDrill && !Spells.Drill.IsKnownAndReady() && Spells.Drill.Cooldown.TotalMilliseconds - 100 >= MachinistRoutine.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    return false;
            }

            if (Core.Me.ClassLevel >= 76 && Core.Me.ClassLevel < 90)
            {
                if ((MachinistSettings.Instance.UseDrill && !Spells.Drill.IsKnownAndReady() && Spells.Drill.Cooldown.TotalMilliseconds - 100 >= MachinistRoutine.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    && (MachinistSettings.Instance.UseHotAirAnchor && !Spells.AirAnchor.IsKnownAndReady() && Spells.AirAnchor.Cooldown.TotalMilliseconds - 100 >= MachinistRoutine.HeatedSplitShot.Cooldown.TotalMilliseconds))
                    return false;
            }

            if (Core.Me.ClassLevel >= 90)
            {
                if (Spells.Reassemble.Charges >= Spells.Reassemble.MaxCharges)
                {
                    if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsReady(3000)
                        || MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsReady(3000)
                        || MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsReady(3000))
                        return await Spells.Reassemble.Cast(Core.Me);
                } 
                else
                {
                    if (MachinistSettings.Instance.UseChainSaw && !Spells.ChainSaw.IsReady(2000))
                        return false;
                }

            }
            return await Spells.Reassemble.Cast(Core.Me);
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.BarrelStabilizer.IsKnown() && !Spells.BarrelStabilizer.IsReady(5000))
                return false;

            return await PhysicalDps.UsePotion(MachinistSettings.Instance);
        }
    }
}