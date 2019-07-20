using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Logic.Bard
{
    public class Aoe
    {
        public static async Task<bool> QuickNock()
        {
            if (!BardSettings.Instance.QuickNock)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (!BardSettings.Instance.UseAoeBeforeDots)
            {
                // We don't have Windbite
                if (Core.Me.ClassLevel < 30)
                {
                    // If the target doesn't have Venomous Bite on return false
                    if (!Core.Me.CurrentTarget.HasAura(Auras.VenomousBite, true, BardSettings.Instance.DotRefreshTime * 1000))
                        return false;
                }
                else
                {
                    // Return false if we don't have both DOTs on target
                    if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.DotRefreshTime * 1000))
                        return false;
                }
            }

            if (Utilities.Routines.Bard.EnemiesInCone < BardSettings.Instance.QuickNockEnemiesInCone)
                return false;

            return await Spells.QuickNock.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeath()
        {
            if (!BardSettings.Instance.RainOfDeath)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Utilities.Routines.Bard.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ApexArrow()
        {
            if (!BardSettings.Instance.ApexArrow)
                return false;

            if (ActionResourceManager.Bard.SoulVoice < BardSettings.Instance.ApexArrowMinimumSoulVoice)
                return false;

            return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
        }
    }
}