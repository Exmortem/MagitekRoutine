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
    internal static class SingleTarget
    {

        public static async Task<bool> Slice()
        {
            if (!await Spells.Slice.Cast(Core.Me.CurrentTarget)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.WaxingSlice;
            return true;

        }

        public static async Task<bool> WaxingSlice()
        {
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.WaxingSlice) return false;
            if (ActionManager.ComboTimeLeft <= 0)  return false;

            if (!await Spells.WaxingSlice.Cast(Core.Me.CurrentTarget)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.InfernalSlice;
            return true;

        }

        public static async Task<bool> InfernalSlice()
        {
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.InfernalSlice) return false;
            if (ActionManager.ComboTimeLeft <= 0) return false;

            if (!await Spells.InfernalSlice.Cast(Core.Me.CurrentTarget)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.Slice;
            return true;

        }

        //Expire Check Missing
        //Something like TTK > Current GCD 
        public static async Task<bool> ShadowOfDeath()
        {
            if (!ReaperSettings.Instance.UseShadowOfDeath) return false;
            if (!(Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards >= ReaperSettings.Instance.WhorlOfDeathTargetCount)) return false;
            if (!Core.Me.CurrentTarget.HasAura(2586,true)) return false;

            return await Spells.ShadowOfDeath.Cast(Core.Me.CurrentTarget);
        }

    }
}
