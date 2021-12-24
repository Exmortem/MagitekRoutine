using System.Linq;
using System.Runtime.Remoting.Messaging;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Enumerations;
using Magitek.Models.Reaper;

namespace Magitek.Logic.Reaper.Enshroud
{
    internal static class SingleTarget
    {
        public static async Task<bool> VoidReaping()
        {
            if (!ReaperSettings.Instance.UseVoidReaping || Core.Me.ClassLevel < Spells.VoidReaping.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.LemureShroud < 2 && Core.Me.ClassLevel >= Spells.Communio.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.EnhancedCrossReaping))
                return false;

            if (Core.Me.HasAura(Auras.EnhancedVoidReaping))
            {
                if (!ReaperSettings.Instance.EfficientAoEPotencyCalculation || Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 < 520)
                    return await Spells.VoidReaping.Cast(Core.Me.CurrentTarget);
            }
            else
            {
                if (!ReaperSettings.Instance.EfficientAoEPotencyCalculation || Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 < 460)
                    return await Spells.VoidReaping.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> CrossReaping()
        {
            if (!ReaperSettings.Instance.UseVoidReaping || Core.Me.ClassLevel < Spells.CrossReaping.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.LemureShroud < 2 && Core.Me.ClassLevel >= Spells.Communio.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.EnhancedVoidReaping))
                return false;

            if (Core.Me.HasAura(Auras.EnhancedCrossReaping))
            {
                if (!ReaperSettings.Instance.EfficientAoEPotencyCalculation || Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 < 520)
                    return await Spells.CrossReaping.Cast(Core.Me.CurrentTarget);
            }
            else
            {
                if (!ReaperSettings.Instance.EfficientAoEPotencyCalculation || Utilities.Routines.Reaper.EnemiesIn8YardCone * 200 < 460)
                    return await Spells.CrossReaping.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> LemuresSlice()
        {
            //Add level check so it doesn't hang here
            if (!ReaperSettings.Instance.UseLemuresSlice || Core.Me.ClassLevel < Spells.LemuresSlice.LevelAcquired)
                return false;

            if (ActionResourceManager.Reaper.VoidShroud < 2) 
                return false;

            if (!ReaperSettings.Instance.EfficientAoEPotencyCalculation || Utilities.Routines.Reaper.EnemiesIn8YardCone * 100 < 200)
                return await Spells.LemuresSlice.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}
