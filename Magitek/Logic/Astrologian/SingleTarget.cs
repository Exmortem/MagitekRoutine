using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

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
            if (AstrologianSettings.Instance.UseTTDForCombust)
            {
                var combatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();

                if (combatTimeLeft > 0 && combatTimeLeft < AstrologianSettings.Instance.DontCombustIfEnemyDyingWithin)
                    return false;
            }
            else
            {
                if (!Core.Me.CurrentTarget.HealthCheck(AstrologianSettings.Instance.CombustHealthMinimum, AstrologianSettings.Instance.CombustHealthMinimumPercent))
                    return false;
            }
            
            return await Combust();
        }

        public static async Task<bool> CombustMultipleTargets()
        {
            if (!AstrologianSettings.Instance.Combust)
                return false;

            if (!AstrologianSettings.Instance.CombustMultipleTargets)
                return false;

            var combustTarget = Combat.Enemies.FirstOrDefault(NeedsCombust);

            if (combustTarget == null)
                return false;

            return await Spells.Combust.Cast(combustTarget);

            bool NeedsCombust(BattleCharacter unit)
            {
                if (!CanCombust(unit))
                    return false;

                if (Core.Me.ClassLevel < 26)
                    return !unit.HasAura(Auras.Combust, true, AstrologianSettings.Instance.CombustRefreshSeconds * 1000);

                if (Core.Me.ClassLevel < 72)
                    return !unit.HasAura(Auras.Combust2, true, AstrologianSettings.Instance.CombustRefreshSeconds * 1000);

                return !unit.HasAura(Auras.Combust3, true, AstrologianSettings.Instance.CombustRefreshSeconds * 1000);
            }

            bool CanCombust(GameObject unit)
            {
                if (!AstrologianSettings.Instance.UseTTDForCombust)
                    return true;

                return unit.CombatTimeLeft() >= AstrologianSettings.Instance.DontCombustIfEnemyDyingWithin;
            }
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

            if (Core.Me.CurrentTarget.HasAura(combustAura, true, AstrologianSettings.Instance.CombustRefreshSeconds * 1000))
                    return false;

            return await Spells.Combust.CastAura(Core.Me.CurrentTarget, combustAura, true, AstrologianSettings.Instance.CombustRefreshSeconds * 1000);
        }
    }
}