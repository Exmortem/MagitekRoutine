using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class Cooldowns
    {
        public static async Task<bool> BarrelStabilizer()
        {
            if (!Core.Me.HasTarget)
                return false;

            if (!MachinistSettings.Instance.UseBarrelStabilizer)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            /*
            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;
            */

            if (ActionResourceManager.Machinist.Heat > 45 && Spells.Wildfire.Cooldown.TotalMilliseconds <= 6000)
                return false;

            if (ActionResourceManager.Machinist.Heat >= 35 && Spells.Wildfire.Cooldown.TotalMilliseconds > 6000)
                return false;

            if (Spells.Hypercharge.Cooldown.TotalMilliseconds > 8000 && Casting.LastSpell == Spells.HeatBlast && Spells.GaussRound.Charges < 1.2f && Spells.Ricochet.Charges < 1.2f)
                return await Spells.BarrelStabilizer.Cast(Core.Me);

            if (ActionResourceManager.Machinist.Heat < 50)
            {
                if (MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown.TotalMilliseconds < 8000 && Spells.Wildfire.Cooldown.TotalMilliseconds < 4000)
                    return false;

                if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.Cooldown.TotalMilliseconds < 8000 && Spells.Wildfire.Cooldown.TotalMilliseconds < 4000)
                    return false;
            }

            Logger.Write($@"Using BarrelStabilizer with {ActionResourceManager.Machinist.Heat} Heat.");
            return await Spells.BarrelStabilizer.Cast(Core.Me);
        }

        public static async Task<bool> Hypercharge()
        {

            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (ActionResourceManager.Machinist.Heat < 50)
                return false;

            if (Core.Me.ClassLevel >= 45)
            {
                if (Core.Me.HasAura(Auras.WildfireBuff, true) && Spells.Wildfire.Cooldown.TotalMilliseconds > 117200)
                    return await Spells.Hypercharge.Cast(Core.Me);

                if (!MachinistSettings.Instance.UseWildfire || !ActionManager.CurrentActions.Values.Contains(Spells.Wildfire))
                    return await Spells.Hypercharge.Cast(Core.Me);

                if (MachinistSettings.Instance.UseWildfire && Spells.Wildfire.Cooldown.TotalMilliseconds > 0 && Spells.Wildfire.Cooldown.TotalMilliseconds <= 25000)
                    return false;
            }

            if (Core.Me.ClassLevel >= 58 && ActionManager.HasSpell(Spells.Drill.Id) && MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (Core.Me.ClassLevel >= 76 && ActionManager.HasSpell(Spells.AirAnchor.Id) && MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (Core.Me.ClassLevel >= 90 && ActionManager.HasSpell(Spells.ChainSaw.Id) && MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (Spells.Ricochet.Charges >= 2.5f || Spells.GaussRound.Charges >= 2.5f)
                return false;

            //Force Delay CD
            if (Spells.SplitShot.Cooldown.TotalMilliseconds > 800 + BaseSettings.Instance.UserLatencyOffset)
                return false;

            return await Spells.Hypercharge.Cast(Core.Me);
        }
        
        public static async Task<bool> Wildfire()
        {
            if (!MachinistSettings.Instance.UseWildfire)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true) || Casting.SpellCastHistory.Any(x => x.Spell == Spells.Wildfire))
                return false;

            if (Core.Me.ClassLevel >= 58 && ActionManager.HasSpell(Spells.Drill.Id) && MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown.TotalMilliseconds < 9000)
                return false;

            if (Core.Me.ClassLevel >= 76 && ActionManager.HasSpell(Spells.AirAnchor.Id) && MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.Cooldown.TotalMilliseconds < 9000)
                return false;

            if (Core.Me.ClassLevel >= 90 && ActionManager.HasSpell(Spells.ChainSaw.Id) && MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (ActionResourceManager.Machinist.Heat < 50 && ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Reassemble()
        {
            if (!MachinistSettings.Instance.UseReassemble)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (Spells.Reassemble.Charges < 1)
                return false;

            //If we're in AoE logic, use Reassemble for SpreadShot
            if (MachinistSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 12 + r.CombatReach) >= 4)
                return await Spells.Reassemble.Cast(Core.Me);

            if (Core.Me.ClassLevel < 58)
            {
                if (ActionManager.LastSpell != MachinistGlobals.HeatedSlugShot)
                    return false;
            }

            if (Core.Me.ClassLevel >= 58 && Core.Me.ClassLevel < 76)
            {
                if (ActionManager.HasSpell(Spells.Drill.Id) && MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown != TimeSpan.Zero && Spells.Drill.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    return false;
            }

            if (Core.Me.ClassLevel >= 76 && Core.Me.ClassLevel < 90)
            {
                if ( (ActionManager.HasSpell(Spells.Drill.Id) && MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown != TimeSpan.Zero && Spells.Drill.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    && (ActionManager.HasSpell(Spells.AirAnchor.Id) && MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.Cooldown != TimeSpan.Zero && Spells.AirAnchor.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds) )
                    return false;
            }

            if (Core.Me.ClassLevel >= 90)
            {
                if ((ActionManager.HasSpell(Spells.Drill.Id) && MachinistSettings.Instance.UseDrill && Spells.Drill.Cooldown != TimeSpan.Zero && Spells.Drill.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    && (ActionManager.HasSpell(Spells.AirAnchor.Id) && MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.Cooldown != TimeSpan.Zero && Spells.AirAnchor.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds)
                    && (ActionManager.HasSpell(Spells.ChainSaw.Id) && MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.Cooldown != TimeSpan.Zero && Spells.ChainSaw.Cooldown.TotalMilliseconds - 100 >= MachinistGlobals.HeatedSplitShot.Cooldown.TotalMilliseconds))
                    return false;
            }

            return await Spells.Reassemble.Cast(Core.Me);
        }
        
    }
}