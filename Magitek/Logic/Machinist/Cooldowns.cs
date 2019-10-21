using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
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
            if (!MachinistSettings.Instance.UseBarrelStabilizer)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (ActionResourceManager.Machinist.Heat >= 50)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true) || Casting.SpellCastHistory.Any(x => x.Spell == Spells.Wildfire))
                return false;

            return await Spells.BarrelStabilizer.Cast(Core.Me);
        }

        public static async Task<bool> Hypercharge()
        {
            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (Spells.Drill.Cooldown.TotalMilliseconds < 8000 || MachinistGlobals.HotAirAnchor.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (Spells.Ricochet.Charges >= 2.0f || Spells.GaussRound.Charges >= 2.0f)
                return false;

            //Force Delay CD
            if (Spells.SplitShot.Cooldown.TotalMilliseconds > 700 + BaseSettings.Instance.UserLatencyOffset)
                return false;

            if (Core.Me.ClassLevel > 45)
            {

                if (Spells.Wildfire.Cooldown.Seconds > 10 || Spells.Wildfire.Cooldown.Seconds < 1)
                    return await Spells.Hypercharge.Cast(Core.Me);

                if (!MachinistSettings.Instance.UseWildfire || !ActionManager.CurrentActions.Values.Contains(Spells.Wildfire))
                    return await Spells.Hypercharge.Cast(Core.Me);

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

            if (Spells.Drill.Cooldown.TotalMilliseconds < 8000 || MachinistGlobals.HotAirAnchor.Cooldown.TotalMilliseconds < 8000)
                return false;

            if (ActionResourceManager.Machinist.Heat < 50 && ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            //Force Delay CD
            if (Spells.SplitShot.Cooldown.TotalMilliseconds > 1000 + BaseSettings.Instance.UserLatencyOffset)
            return false;

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }
        /* OLD LOGIC FOR REFERENCE
        public static async Task<bool> Hypercharge()
        {
            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (!MachinistSettings.Instance.UseWildfire || !ActionManager.CurrentActions.Values.Contains(Spells.Wildfire))
                return await Spells.Hypercharge.Cast(Core.Me);

            if (Spells.Wildfire.Cooldown.Seconds > 10 || Spells.Wildfire.Cooldown.Seconds < 1)
                return await Spells.Hypercharge.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> Wildfire()
        {
            if (!MachinistSettings.Instance.UseWildfire)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true))
                return false;

            if (!MachinistSettings.Instance.UseHypercharge)
                if (ActionResourceManager.Machinist.Heat <= 45 && Casting.SpellCastHistory.Take(5).All(s => s.Spell != Spells.Hypercharge))
                    return false;

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }*/

        public static async Task<bool> Reassemble()
        {
            if (!MachinistSettings.Instance.UseReassemble)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            //If we're in AoE logic, use Reassemble for SpreadShot
            if (MachinistSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 12 + r.CombatReach) >= 4)
                return await Spells.Reassemble.Cast(Core.Me);

            if (Core.Me.ClassLevel < 58)
            {
                if (ActionManager.LastSpell != MachinistGlobals.HeatedSlugShot)
                    return false;
            }

            if (Core.Me.ClassLevel > 58 && Core.Me.ClassLevel < 76)
            {
                if (Spells.Drill.Cooldown != TimeSpan.Zero && Spells.Drill.Cooldown >= MachinistGlobals.HeatedSplitShot.Cooldown)
                    return false;
            }

            if (Core.Me.ClassLevel >= 76)
            {
                //If Drill Cooldown isn't up & Drill Cooldown is longer than GCD, don't use Reassemble
                if ((Spells.Drill.Cooldown != TimeSpan.Zero && Spells.Drill.Cooldown >= MachinistGlobals.HeatedSplitShot.Cooldown) &&
                    (Spells.AirAnchor.Cooldown != TimeSpan.Zero && Spells.AirAnchor.Cooldown >= MachinistGlobals.HeatedSplitShot.Cooldown))
                    return false;
            }

            return await Spells.Reassemble.Cast(Core.Me);
        }
        
    }
}