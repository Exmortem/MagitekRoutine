using System;
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
        //Expire Check Missing
        //Something like TTK > Current GCD 
        public static async Task<bool> ShadowOfDeath()
        {
            if (!ReaperSettings.Instance.UseShadowOfDeath) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards >= ReaperSettings.Instance.WhorlOfDeathTargetCount) return false;
            if (Core.Me.CurrentTarget.HasAura(2586, true)) return false;

            return await Spells.ShadowOfDeath.Cast(Core.Me.CurrentTarget);
        }

        #region SoulGaugeGenerator
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

        #endregion

        #region SoulShroudGenerator
        public static async Task<bool> GibbetAndGallows()
        {
            if (!Core.Me.HasAura(2587)) return false;
            if ((!Core.Me.CurrentTarget.IsBehind && !Core.Me.CurrentTarget.IsFlanking) || ReaperSettings.Instance.EnemyIsOmni)
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

        #endregion

        #region SoulGaugeSpender
        public static async Task<bool> BloodStalk()
        {
            if (!ReaperSettings.Instance.UseBloodStalk) return false;
            if (Spells.Gluttony.Cooldown.Ticks == 0 || (Spells.Gluttony.AdjustedCooldown - Spells.Gluttony.Cooldown <= Spells.Slice.AdjustedCooldown)) return false;
            if (Core.Me.HasAura(2587)) return false;

            return await Spells.BloodStalk.Cast(Core.Me.CurrentTarget);
        }

        #endregion

        #region LemureShroudSpender

        public static async Task<bool> VoidAndCrossReaping()
        {

            if (ActionResourceManager.Reaper.LemureShroud < 2) return false;
            if (!Core.Me.HasMyAura(2591))
            {
                return await Spells.VoidReaping.Cast(Core.Me.CurrentTarget);
            }
            else
            {
                return await Spells.CrossReaping.Cast(Core.Me.CurrentTarget);
            }
            

        }

        public static async Task<bool> VoidReaping()
        {
            if (!ReaperSettings.Instance.UseVoidReaping) return false;
            if (ActionResourceManager.Reaper.LemureShroud < 2) return false;
            if (ActionResourceManager.Reaper.LemureShroud != 7 && !Core.Me.HasMyAura(2590)) return false;

            return await Spells.VoidReaping.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CrossReaping()
        {
            if (!ReaperSettings.Instance.UseCrossReaping) return false;
            if (ActionResourceManager.Reaper.LemureShroud < 2) return false;
            if (!Core.Me.HasMyAura(2591)) return false;

            return await Spells.CrossReaping.Cast(Core.Me.CurrentTarget);
        }

        #endregion

        #region VoidShroudSpender

        public static async Task<bool> LemuresSlice()
        {
            if (!ReaperSettings.Instance.UseLemuresSlice) return false;
            if (ActionResourceManager.Reaper.VoidShroud < 2) return false;

            return await Spells.LemuresSlice.Cast(Core.Me.CurrentTarget);
        }

        #endregion

    }
}
