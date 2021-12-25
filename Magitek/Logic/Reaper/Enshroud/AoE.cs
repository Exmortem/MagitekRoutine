using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Reaper;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Reaper.Enshroud
{
    internal static class AoE
    {
        public static async Task<bool> GrimReaping()
        {
            if (!ReaperSettings.Instance.UseGrimReaping || Core.Me.ClassLevel < Spells.GrimReaping.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.LemureShroud < 2 && Core.Me.ClassLevel >= Spells.Communio.LevelAcquired) return false;

            if (ReaperSettings.Instance.EfficientAoEPotencyCalculation)
            {
                if (Core.Me.HasAura(Auras.EnhancedVoidReaping) || Core.Me.HasAura(Auras.EnhancedCrossReaping))
                {
                    if (Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 >= 520)
                        return await Spells.GrimReaping.Cast(Core.Me.CurrentTarget);
                }
                else
                {
                    if (Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 >= 460)
                        return await Spells.GrimReaping.Cast(Core.Me.CurrentTarget);
                }
            }
            else
            {
                if (Utilities.Routines.Reaper.EnemiesIn8YardCone >= ReaperSettings.Instance.GrimReapingTargetCount)
                    return await Spells.GrimReaping.Cast(Core.Me.CurrentTarget);
            }

            return false;

        }

        public static async Task<bool> LemuresScythe()
        {
            if (!ReaperSettings.Instance.UseLemuresScythe || Core.Me.ClassLevel < Spells.LemuresScythe.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.VoidShroud < 2 && Core.Me.ClassLevel >= Spells.Communio.LevelAcquired) return false;

            if (ReaperSettings.Instance.EfficientAoEPotencyCalculation)
            {
                if (Utilities.Routines.Reaper.EnemiesIn8YardCone * 100 >= 200)
                    return await Spells.LemuresScythe.Cast(Core.Me.CurrentTarget);
            }
            else
            {
                if (Utilities.Routines.Reaper.EnemiesIn8YardCone >= ReaperSettings.Instance.LemuresScytheTargetCount)
                    return await Spells.LemuresScythe.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        //Logic for Smart targeting or burst sniping maybe
        public static async Task<bool> Communio()
        {
            if (!ReaperSettings.Instance.UseCommunio || Core.Me.ClassLevel < Spells.Communio.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.LemureShroud > 1 || (Core.Me.HasAura(Auras.Enshrouded) && !Core.Me.HasAura(Auras.Enshrouded, true, 3000)))
                return false;

            return await Spells.Communio.Cast(Core.Me.CurrentTarget);
        }
    }
}
