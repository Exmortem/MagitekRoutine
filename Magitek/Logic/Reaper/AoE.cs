using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Enumerations;
using static ff14bot.Managers.ActionResourceManager.Reaper;

namespace Magitek.Logic.Reaper
{
    internal static class AoE
    {
        public static async Task<bool> SpinningScythe()
        {
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < 2) return false;
            if (!await Spells.SpinningScythe.Cast(Core.Me)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.NightmareScythe;
            return true;

        }

        public static async Task<bool> NightmareScythe()
        {
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < 2) return false;
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.NightmareScythe) return false;
            if (ActionManager.ComboTimeLeft <= 0) return false;

            if (!await Spells.NightmareScythe.Cast(Core.Me)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.SpinningScythe;
            return true;

        }

    }
}