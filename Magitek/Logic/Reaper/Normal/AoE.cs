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

        //Expire Check Missing
        //Something like TTK > Current GCD 
        public static async Task<bool> WhorlofDeath()
        {

            if (!ReaperSettings.Instance.UseWhorlOfDeath)
                return false;

            if (Core.Me.HasAura(Auras.SoulReaver))
                return false;

            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.WhorlOfDeathTargetCount)
                return false;

            if (!Combat.Enemies.Any(x => (!x.HasMyAura(Auras.DeathsDesign) || (x.HasMyAura(Auras.DeathsDesign) && !x.HasAura(Auras.DeathsDesign, true, Spells.Slice.AdjustedCooldown.Milliseconds)))
                                         && x.Distance(Core.Me) <= 5 + x.CombatReach))
                return false;

            return await Spells.WhorlOfDeath.Cast(Core.Me);
        }

        public static async Task<bool> HarvestMoon()
        {

            if (!ReaperSettings.Instance.UseHarvestMoon)
                return false;

            if (!Core.Me.HasAura(Auras.Soulsow))
                return false;

            if (Core.Me.HasAura(Auras.SoulReaver))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.DeathsDesign, true))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() >= ReaperSettings.Instance.HarvestMoonTargetCount)
                return false;

            return await Spells.HarvestMoon.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WhorlofDeathIdle()
        {

            if (!ReaperSettings.Instance.UseWhorlOfDeath)
                return false;

            if (Core.Me.HasAura(Auras.SoulReaver))
                return false;

            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.WhorlOfDeathTargetCount)
                return false;

            if (!Combat.Enemies.Any(x => (!x.HasMyAura(Auras.DeathsDesign) || (x.HasMyAura(Auras.DeathsDesign) && !x.HasAura(Auras.DeathsDesign, true, 30000 - Spells.Slice.AdjustedCooldown.Milliseconds)))
                                         && x.Distance(Core.Me) <= 5 + x.CombatReach))
                return false;

            return await Spells.WhorlOfDeath.Cast(Core.Me);
        }

        #region SoulGaugeGenerator

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
            //Add level check so it doesn't hang here
            if (Core.Me.ClassLevel < Spells.NightmareScythe.LevelAcquired)
                return false;
            if (!ReaperSettings.Instance.UseNightmareScythe) return false;
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.NightmareScytheTargetCount) return false;
            if (Utilities.Routines.Reaper.CurrentComboStage != ReaperComboStages.NightmareScythe) return false;
            if (ActionManager.ComboTimeLeft <= 0) return false;

            if (!await Spells.NightmareScythe.Cast(Core.Me)) return false;
            Utilities.Routines.Reaper.CurrentComboStage = ReaperComboStages.SpinningScythe;
            return true;

        }

        public static async Task<bool> SoulScythe()
        {
            if (!ReaperSettings.Instance.UseSoulScythe) return false;
            if (!ReaperSettings.Instance.UseSoulSlice 
                || Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < ReaperSettings.Instance.SoulScytheTargetCount) 
                return false;

            if (ActionResourceManager.Reaper.SoulGauge >= 50) return false;

            return await Spells.SoulScythe.Cast(Core.Me);
        }

        #endregion

        #region SoulGaugeSpender

        public static async Task<bool> GrimSwathe()
        {
            //Add level check so it doesn't hang here
            if (Core.Me.ClassLevel < Spells.GrimSwathe.LevelAcquired)
                return false;
            if (!ReaperSettings.Instance.UseGrimSwathe) return false;
            if (Utilities.Routines.Reaper.EnemiesIn8YardCone < ReaperSettings.Instance.GrimSwatheTargetCount) return false;
            if (Core.Me.HasAura(Auras.SoulReaver)) return false;
            if (Spells.Gluttony.Cooldown.Ticks == 0 || (Spells.Gluttony.AdjustedCooldown - Spells.Gluttony.Cooldown <= Spells.Slice.AdjustedCooldown)) return false;
            if (!Core.Me.CurrentTarget.HasAura(Auras.DeathsDesign, true)) return false;

            return await Spells.GrimSwathe.Cast(Core.Me.CurrentTarget);
        }

        #endregion

        #region SoulShroudGenerator

        public static async Task<bool> Guillotine()
        {
            //Add level check so it doesn't hang here
            if (Core.Me.ClassLevel < Spells.Guillotine.LevelAcquired)
                return false;
            if (!ReaperSettings.Instance.UseGuillotine) return false;
            if (!Core.Me.HasAura(Auras.SoulReaver)) return false;
            if (Utilities.Routines.Reaper.EnemiesIn8YardCone < ReaperSettings.Instance.GuillotineTargetCount) return false;
            return await Spells.Guillotine.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PlentifulHarvest()
        {
            if (!ReaperSettings.Instance.UsePlentifulHarvest || Core.Me.ClassLevel < Spells.PlentifulHarvest.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.BloodsownCircle))
                return false;

            if (ActionResourceManager.Reaper.ShroudGauge > 50)
                return false;
            
            return await Spells.PlentifulHarvest.Cast(Core.Me.CurrentTarget);
        }

        #endregion

        }
}