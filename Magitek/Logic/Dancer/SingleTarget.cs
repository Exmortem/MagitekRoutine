using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using Magitek.Models.Dancer;
using System.Linq;

namespace Magitek.Logic.Dancer
{
    internal static class SingleTarget
    {

        public static async Task<bool> FanDance()
        {
            if (ActionResourceManager.Dancer.FourFoldFeathers < 4 && !Core.Me.HasAura(Auras.Devilment) && Core.Me.ClassLevel >= 62)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= DancerSettings.Instance.FanDanceTwoEnemies)
                return false;
            return await Spells.FanDance.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fountainfall()
        {
            if (!Core.Me.HasAura(Auras.FlourshingFountain)) return false;

            return await Spells.Fountainfall.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ReverseCascade()
        {
            if (!Core.Me.HasAura(Auras.FlourshingCascade)) return false;

            return await Spells.ReverseCascade.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fountain()
        {
            if (ActionManager.LastSpell != Spells.Cascade) return false;

            return await Spells.Fountain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Cascade()
        {
            return await Spells.Cascade.Cast(Core.Me.CurrentTarget);
        }
    }
}