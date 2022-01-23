using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Sage
{
    internal static class SingleTarget
    {
        private static async Task<bool> UseEukrasianDosis(GameObject target)
        {
            var spell = Spells.EukrasianDosisIII;
            var aura = Auras.EukrasianDosisIII;

            if (Core.Me.ClassLevel < 82)
            {
                spell = Spells.EukrasianDosisII;
                aura = Auras.EukrasianDosisII;
            }
            if (Core.Me.ClassLevel < 72)
            {
                spell = Spells.EukrasianDosis;
                aura = Auras.EukrasianDosis;
            }

            if (!await Heal.UseEukrasia(spell.Id, target))
                return false;

            return await spell.CastAura(target, (uint)aura);
        }
        public static async Task<bool> Dosis()
        {
            if (!SageSettings.Instance.DoDamage)
                return false;

            if (!SageSettings.Instance.Dosis)
                return false;

            // By the time the routine is casting Dosis, if it has a stored eukrasia, there was
            // nothing else to do with it, so refresh the dot so it can proceed to throwing out
            // regular dosis instead of stopping completely.
            if (Core.Me.HasAura(Auras.Eukrasia, true))
                return await UseEukrasianDosis(Core.Me.CurrentTarget);

            var spell = Spells.DosisIII;

            if (Core.Me.ClassLevel < 82)
            {
                spell = Spells.DosisII;
            }
            if (Core.Me.ClassLevel < 72)
            {
                spell = Spells.Dosis;
            }

            return await spell.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> EukrasianDosis()
        {
            if (!SageSettings.Instance.DoDamage)
                return false;

            if (SageSettings.Instance.UseTTDForDots && Combat.CurrentTargetCombatTimeLeft <= SageSettings.Instance.DontDotIfEnemyDyingWithin)
                return false;

            if (!SageSettings.Instance.EukrasianDosis)
                return false;

            if (!Heal.IsEukrasiaReady())
                return false;

            var targetChar = Core.Me.CurrentTarget as Character;

            if (targetChar != null && targetChar.CharacterAuras.Count() >= 25)
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(DotAuras, true, msLeft: SageSettings.Instance.DotRefreshMSeconds))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25 + Core.Me.CurrentTarget.CombatReach)
                return false;

            return await UseEukrasianDosis(Core.Me.CurrentTarget);
        }
        public static async Task<bool> DotMultipleTargets()
        {
            if (!SageSettings.Instance.DoDamage)
                return false;

            if (!SageSettings.Instance.EukrasianDosis)
                return false;

            if (!SageSettings.Instance.DotMultipleTargets)
                return false;

            if (!Heal.IsEukrasiaReady())
                return false;

            if (Combat.Enemies.Count < 2)
                return false;

            var DotTarget = Combat.Enemies.Where(NeedsDot).Where(CanDot).FirstOrDefault();

            if (DotTarget == null)
                return false;

            return await UseEukrasianDosis(DotTarget);

            bool NeedsDot(BattleCharacter unit)
            {
                if (unit.CharacterAuras.Count() >= 25)
                    return false;
                return !unit.HasAnyAura(DotAuras, true, msLeft: SageSettings.Instance.DotRefreshMSeconds);
            }
            bool CanDot(GameObject unit)
            {
                // Check dosis since no eukrasia buff yet.
                if (!Spells.Dosis.CanCast(unit))
                    return false;
                if (unit.Distance(Core.Me) > 25 + unit.CombatReach)
                    return false;
                if (!SageSettings.Instance.UseTTDForDots)
                    return true;
                return unit.CombatTimeLeft() >= SageSettings.Instance.DontDotIfEnemyDyingWithin;
            }
        }
        private static readonly uint[] DotAuras =
        {
            Auras.EukrasianDosis,
            Auras.EukrasianDosisII,
            Auras.EukrasianDosisIII
        };
    }

}
