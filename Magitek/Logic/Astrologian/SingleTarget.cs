using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
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
        
        public static async Task<bool> LordofCrowns()
        {
            if (!AstrologianSettings.Instance.LordofCrowns)
                return false;

            if (!ActionManager.HasSpell(Spells.LordofCrowns.Id)) return false;
            
            if (ActionResourceManager.Astrologian.Arcana != ActionResourceManager.Astrologian.AstrologianCard.LordofCrowns) return false;
            
            if (Utilities.Routines.Astrologian.OnGcd) return false;
            
            return await Spells.LordofCrowns.Cast(Core.Me.CurrentTarget);
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

            if (ActionManager.HasSpell(Spells.Combust3.Id))
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Combust3, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

                return await Spells.Combust3.CastAura(Core.Me.CurrentTarget, Auras.Combust3, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000);
            }

            if (ActionManager.HasSpell(Spells.Combust2.Id))
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Combust2, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

                return await Spells.Combust2.CastAura(Core.Me.CurrentTarget, Auras.Combust2, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000);
            }
            if (Core.Me.CurrentTarget.HasAura(Auras.Combust, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

            return await Spells.Combust.CastAura(Core.Me.CurrentTarget, Auras.Combust, true, AstrologianSettings.Instance.DotRefreshSeconds * 1000);
        }
    }
}