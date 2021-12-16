using System.Linq;
using System.Runtime.Remoting.Messaging;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Enumerations;
using Magitek.Models.Reaper;

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

        #region SoulGaugeSpender

        public static async Task<bool> GrimSwathe()
        {
            if (!ReaperSettings.Instance.UseGrimSwathe) return false;
            if (Utilities.Routines.Reaper.AoeEnemies8Yards < ReaperSettings.Instance.GrimSwatheTargetCount) return false;
            if (Core.Me.HasAura(2587)) return false;
            if (Spells.Gluttony.Cooldown.Ticks == 0 || (Spells.Gluttony.AdjustedCooldown - Spells.Gluttony.Cooldown <= Spells.Slice.AdjustedCooldown)) return false;

            return await Spells.GrimSwathe.Cast(Core.Me);
        }

        #endregion

        #region SoulShroudGenerator

        public static async Task<bool> Guillotine()
        {
            if (!ReaperSettings.Instance.UseGuillotine) return false;
            if (!Core.Me.HasAura(2587)) return false;
            if (Utilities.Routines.Reaper.AoeEnemies8Yards < ReaperSettings.Instance.GuillotineTargetCount) return false;
            return await Spells.Guillotine.Cast(Core.Me);
        }

        #endregion

        #region Enshroud

        public static async Task<bool> GrimReaping()
        {
            if (!ReaperSettings.Instance.UseGrimReaping) return false;
            if (ActionResourceManager.Reaper.LemureShroud <= 2) return false;
            if (Utilities.Routines.Reaper.AoeEnemies8Yards < ReaperSettings.Instance.GrimReapingTargetCount) return false;

            return await Spells.GrimReaping.Cast(Core.Me);
        }

        public static async Task<bool> LemuresScythe()
        {
            if (!ReaperSettings.Instance.UseLemuresScythe) return false;
            if (ActionResourceManager.Reaper.VoidShroud <= 2) return false;
            if (Utilities.Routines.Reaper.AoeEnemies8Yards < ReaperSettings.Instance.LemuresScytheTargetCount) return false;

            return await Spells.LemuresScythe.Cast(Core.Me);
        }

        //Logic for Smart targeting or burst sniping maybe
        public static async Task<bool> Communio()
        {
            if (!ReaperSettings.Instance.UseCommunio) return false;
            if (ActionResourceManager.Reaper.LemureShroud > 1) return false;

            return await Spells.Communio.Cast(Core.Me.CurrentTarget);
        }

        #endregion

    }
}