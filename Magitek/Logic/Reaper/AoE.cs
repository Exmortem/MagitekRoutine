using System.Linq;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Enumerations;
using Magitek.Models.Reaper;
using static ff14bot.Managers.ActionResourceManager.Reaper;

namespace Magitek.Logic.Reaper
{
    internal static class AoE
    {
        public static async Task<bool> SpinningScythe()
        {
            if (!ReaperSettings.Instance.UseSpinningScythe) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.SpinningScytheTargetCount) return false;
            if (!await Spells.SpinningScythe.Cast(Core.Me)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.NightmareScythe;
            return true;

        }

        public static async Task<bool> NightmareScythe()
        {
            if (!ReaperSettings.Instance.UseNightmareScythe) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.NightmareScytheTargetCount) return false;
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.NightmareScythe) return false;
            if (ActionManager.ComboTimeLeft <= 0) return false;

            if (!await Spells.NightmareScythe.Cast(Core.Me)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.SpinningScythe;
            return true;

        }

        //Expire Check Missing
        //Something like TTK > Current GCD 
        public static async Task<bool> WhorlofDeath()
        {

            if (!ReaperSettings.Instance.UseWhorlOfDeath) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.WhorlOfDeathTargetCount) return false;
            if (!Combat.Enemies.Any(x => !x.HasMyAura(2586) && x.Distance(Core.Me) <= 5 + x.CombatReach)) return false;

            return await Spells.WhorlOfDeath.Cast(Core.Me);
        }

        //Logic for Smart targeting or burst sniping maybe
        public static async Task<bool> Communio()
        {
            if (!ReaperSettings.Instance.UseCommunio) return false;
            if (ActionResourceManager.Reaper.LemureShroud > 1) return false;

            return await Spells.Communio.Cast(Core.Me.CurrentTarget);
        }

    }
}