using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;

namespace Magitek.Logic.Astrologian
{
    internal static class SingleTarget
    {
        public static async Task<bool> Malefic()
        {
            if (!AstrologianSettings.Instance.Malefic)
                return false;

            return await Spells.Malefic.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> Dots()
        {
            if (AstrologianSettings.Instance.UseTimeTillDeathForDots)
            {
                var combatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();

                if (combatTimeLeft > 0 && combatTimeLeft < AstrologianSettings.Instance.DontDotIfEnemyDyingWithin)
                    return false;
            }
            else
            {
                if (!Core.Me.CurrentTarget.HealthCheck(AstrologianSettings.Instance.DotHealthMinimum, AstrologianSettings.Instance.DotHealthMinimumPercent))
                    return false;
            }
            
            return await Combust();
        }

        private static async Task<bool> Combust()
        {
            if (!AstrologianSettings.Instance.Combust)
                return false;

            var classLevel = Core.Me.ClassLevel;
            uint combustAura = Auras.Combust;

            if (classLevel >= 72)
                combustAura = Auras.Combust3;

            if (classLevel < 72 && classLevel >= 46)
                combustAura = Auras.Combust2;

            if (Core.Me.CurrentTarget.HasAura(combustAura, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

            return await Spells.Combust.CastAura(Core.Me.CurrentTarget, combustAura, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000);
        }
    }
}