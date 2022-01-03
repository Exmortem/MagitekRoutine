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
        public static async Task<bool> Dosis()
        {
            if (!SageSettings.Instance.Dosis)
                return false;

            if (Core.Me.HasAura(Auras.Eukrasia, true))
                return false; 

            return await Spells.Dosis.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> EukrasianDosis()
        {
            //Also this
            if (Combat.CurrentTargetCombatTimeLeft <= SageSettings.Instance.DontDotIfEnemyDyingWithin)
                return false;

            if (!SageSettings.Instance.EukrasianDosis)
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(DotAuras, true, msLeft: SageSettings.Instance.DotRefreshMSeconds))
                return false;

            await UseEukrasia();

            async Task UseEukrasia()
            {
                if (!SageSettings.Instance.Eukrasia)
                    return;
                if (!await Spells.Eukrasia.Cast(Core.Me))
                    return;
                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Eukrasia)))
                    return;
                //await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Dosis.Id, Core.Me.CurrentTarget));
            }

            if (Core.Me.ClassLevel < 72)
            {
                return await Spells.EukrasianDosis.Cast(Core.Me.CurrentTarget);
            }
            if (Core.Me.ClassLevel < 82)
            {
                return await Spells.EukrasianDosisII.Cast(Core.Me.CurrentTarget);
            }
            return await Spells.EukrasianDosisIII.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> DotMultipleTargets()
        {
            if (!SageSettings.Instance.EukrasianDosis)
                return false;

            if (!SageSettings.Instance.DotMultipleTargets)
                return false;

            var DotTarget = Combat.Enemies.FirstOrDefault(NeedsDot);

            if (DotTarget == null)
                return false;

            if (!await Spells.EukrasianDosis.Cast(DotTarget))
                return false;

            await UseEukrasia();

            async Task UseEukrasia()
            {
                if (!SageSettings.Instance.Eukrasia)
                    return;
                if (!await Spells.Eukrasia.Cast(Core.Me))
                    return;
                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Eukrasia)))
                    return;
                //await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Dosis.Id, DotTarget));
            }
            if (Core.Me.ClassLevel < 72)
            {
                return await Spells.EukrasianDosis.Cast(DotTarget);
            }
            if (Core.Me.ClassLevel < 82)
            {
                return await Spells.EukrasianDosisII.Cast(DotTarget);
            }
            return await Spells.EukrasianDosisIII.Cast(DotTarget);

            bool NeedsDot(BattleCharacter unit)
            {
                if (!CanDot(unit))
                    return false;
                return !unit.HasAnyAura(DotAuras, true, msLeft: SageSettings.Instance.DotRefreshMSeconds);
            }
            bool CanDot(GameObject unit)
            {
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
