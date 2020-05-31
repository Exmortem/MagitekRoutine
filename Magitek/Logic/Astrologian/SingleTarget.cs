using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
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

                return !unit.HasAnyAura(CombustAuras, true, msLeft: AstrologianSettings.Instance.CombustRefreshMSeconds);
            }

            bool CanCombust(GameObject unit)
            {
                if (!AstrologianSettings.Instance.UseTTDForCombust)
                    return true;

                return unit.CombatTimeLeft() >= AstrologianSettings.Instance.DontCombustIfEnemyDyingWithin;
            }
        }

        public static async Task<bool> Combust()
        {
            //Why this wasn't here before, I have no idea
            if (!AstrologianSettings.Instance.UseTTDForCombust)
                return false;

            //Also this
            if (Combat.CurrentTargetCombatTimeLeft <= AstrologianSettings.Instance.DontCombustIfEnemyDyingWithin)
                return false;

            if (!AstrologianSettings.Instance.Combust)
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(CombustAuras, true, msLeft: AstrologianSettings.Instance.CombustRefreshMSeconds))
                return false;

            return await Spells.Combust.Cast(Core.Me.CurrentTarget);
        }

        private static readonly uint[] CombustAuras =
        {
            Auras.Combust,
            Auras.Combust2,
            Auras.Combust3
        };
    }
}
