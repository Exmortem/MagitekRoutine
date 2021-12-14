using System.Linq;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Enumerations;
using Magitek.Models.Reaper;


namespace Magitek.Logic.Reaper
{
    internal static class SingleTarget
    {

        public static async Task<bool> Slice()
        {
            if (!ReaperSettings.Instance.UseSlice) return false;
            if (!await Spells.Slice.Cast(Core.Me.CurrentTarget)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.WaxingSlice;
            return true;

        }

        public static async Task<bool> WaxingSlice()
        {
            if (!ReaperSettings.Instance.UseWaxingSlice) return false;
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.WaxingSlice) return false;
            if (ActionManager.ComboTimeLeft <= 0) return false;

            if (!await Spells.WaxingSlice.Cast(Core.Me.CurrentTarget)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.InfernalSlice;
            return true;

        }

        public static async Task<bool> InfernalSlice()
        {
            if (!ReaperSettings.Instance.UseInfernalSlice) return false;
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
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards >= ReaperSettings.Instance.WhorlOfDeathTargetCount) return false;
            if (Core.Me.CurrentTarget.HasAura(2586,true)) return false;

            return await Spells.ShadowOfDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GibbetAndGallows()
        {
            if (!Core.Me.HasAura(2587)) return false;
            if ( (!Core.Me.CurrentTarget.IsBehind && !Core.Me.CurrentTarget.IsFlanking) || ReaperSettings.Instance.EnemyIsOmni )
            {
                if (ReaperSettings.Instance.UseGallows)
                    return await Spells.Gallows.Cast(Core.Me.CurrentTarget);
                if (ReaperSettings.Instance.UseGibbet)
                    return await Spells.Gibbet.Cast(Core.Me.CurrentTarget);
            }
            else if (Core.Me.CurrentTarget.IsBehind)
            {
                if (ReaperSettings.Instance.UseGallows)
                    return await Spells.Gallows.Cast(Core.Me.CurrentTarget);
            }
            else
            {
                if (ReaperSettings.Instance.UseGibbet)
                    return await Spells.Gibbet.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> SoulSlice()
        {
            if (!ReaperSettings.Instance.UseSoulSlice) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards >= ReaperSettings.Instance.SoulScytheTargetCount) return false;

            //Keep SoulSlice/SoulScythe Charges at a maximum
            if (Spells.SoulSlice.Charges <= 1) return false;
            if (Spells.SoulSlice.Cooldown > Spells.Slice.Cooldown) return false;

            if (ActionResourceManager.Reaper.SoulGauge > 50) return false;

            return await Spells.SoulSlice.Cast(Core.Me.CurrentTarget);
        }

    }
}
