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
    internal static class Cooldown
    {

        public static async Task<bool> Gluttony()
        {
            //Add level check so it doesn't hang here
            if (Core.Me.ClassLevel < Spells.Gluttony.LevelAcquired)
                return false;
            if (!ReaperSettings.Instance.UseGluttony) return false;
            if (Core.Me.HasAura(Auras.SoulReaver)) return false;
            if (Spells.Slice.Cooldown > new TimeSpan(Spells.Slice.AdjustedCooldown.Ticks / 2)) return false;
            if (!Core.Me.CurrentTarget.HasAura(Auras.DeathsDesign, true)) return false;

            return await Spells.Gluttony.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Enshroud()
        {
            //Add level check so it doesn't hang here
            if (Core.Me.ClassLevel < Spells.Enshroud.LevelAcquired)
                return false;
            if (Core.Me.HasAura(Auras.SoulReaver))
                return false;
            if (!ReaperSettings.Instance.UseEnshroud) return false;
            if (ActionResourceManager.Reaper.ShroudGauge < 50) return false;
            if (!Core.Me.CurrentTarget.HasAura(Auras.DeathsDesign, true)) return false;

            return await Spells.Enshroud.Cast(Core.Me);
        }

        public static async Task<bool> ArcaneCircle()
        {
            if (!ReaperSettings.Instance.UseArcaneCircle || Core.Me.ClassLevel < Spells.ArcaneCircle.LevelAcquired)
                return false;

            // Prevent blowing arcane circle before reaching target.
            if (Utilities.Routines.Reaper.EnemiesAroundPlayer5Yards < 1)
                return false;

            if (Core.Me.HasAura(Auras.ArcaneCircle))
                return false;

            return await Spells.ArcaneCircle.Cast(Core.Me);
        }

    }
}
