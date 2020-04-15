using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class SingleTarget
    {
        public static async Task<bool> HeatedSplitShot()
        {
            //One to disable them all
            if (!MachinistSettings.Instance.UseSplitShotCombo)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;
            */

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistGlobals.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            //Logger.WriteInfo($@"Last Spell ActionManager: {ActionManager.LastSpell} | Last Spell Cast: {Casting.LastSpell}");
            if (ActionManager.LastSpell != Spells.SplitShot)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistGlobals.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            //Logger.WriteInfo($@"Last Spell ActionManager: {ActionManager.LastSpell} | Last Spell Cast: {Casting.LastSpell}");
            if (ActionManager.LastSpell != Spells.SlugShot)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            return await MachinistGlobals.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistSettings.Instance.UseDrill)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            return await Spells.Drill.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotAirAnchor()
        {
            if (!MachinistSettings.Instance.UseHotAirAnchor)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            if (ActionResourceManager.Machinist.Battery >= 80)
                return false;

            return await MachinistGlobals.HotAirAnchor.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {

            if(Casting.LastSpell == Spells.Hypercharge)
                return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);

            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GaussRound()
        {
            if (!MachinistSettings.Instance.UseGaussRound)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (Casting.LastSpell == Spells.Wildfire)
                return false;

            if (Core.Me.ClassLevel > 45)
            {
                if (Spells.Wildfire.Cooldown.Seconds < 2)
                    return false;

                // Do not run Gauss if an hypercharge is almost ready and not enough charges available for Rico and Gauss
                if (ActionResourceManager.Machinist.Heat > 45 && Spells.Hypercharge.Cooldown == TimeSpan.Zero)
                {
                    if (Spells.GaussRound.Charges < 1.0f && Spells.Ricochet.Charges < 1.5f)
                        return false;
                }
            }

            /*add some mor precise logic for pooling/dumping
            if (Spells.GaussRound.Charges < 1.8f)
                return false;*/

            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }
    }
}
