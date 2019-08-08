using System.Linq;
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
        public static async Task<bool> ApexArrow()
        {
            if (!BardSettings.Instance.UseApexArrow)
                return false;

            if (BardSettings.Instance.UseBuffedApexArrow 
                && ActionResourceManager.Bard.SoulVoice >= BardSettings.Instance.UseBuffedApexArrowWithAtLeastXSoulVoice)
            {
                if (Utilities.Routines.Bard.CheckCurrentDamageIncrease(BardSettings.Instance.UseBuffedApexArrowWithAtLeastXBonusDamage))
                    return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
            }

            if (ActionResourceManager.Bard.SoulVoice < BardSettings.Instance.UseApexArrowWithAtLeastXSoulVoice)
                return false;

            return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeathDuringMagesBallard()
        {
            if (!BardSettings.Instance.UseAoe)
                return false;

            if (!BardSettings.Instance.UseRainOfDeath)
                return false;

            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.MagesBallad)
                return false;

            if (Utilities.Routines.Bard.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeath()
        {
            if (!BardSettings.Instance.UseRainOfDeath)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Utilities.Routines.Bard.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShadowBite()
        {
            if (!BardSettings.Instance.UseShadowBite)
                return false;

            if (!ActionManager.HasSpell(Spells.Shadowbite.Id))
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() <= 1)
                return false;

            return await Spells.Shadowbite.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> QuickNock()
        {
            if (!BardSettings.Instance.UseQuickNock)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Utilities.Routines.Bard.EnemiesInCone < BardSettings.Instance.QuickNockEnemiesInCone)
                return false;

            return await Spells.QuickNock.Cast(Core.Me.CurrentTarget);
        }
    }
}