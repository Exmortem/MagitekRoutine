using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.RedMage
{
    internal static class Heal
    {
        public static async Task<bool> Vercure()
        {
            if (CustomOpenerLogic.InOpener)
                return false;

            if (!RedMageSettings.Instance.Vercure)
                return false;

            if (Core.Me.ClassLevel < 54)
                return false;

            if (RedMageSettings.Instance.VercureOnlyDualCast)
            {
                if (!Core.Me.HasAura(Auras.Dualcast))
                    return false;
            }

            if (Globals.InParty)
            {
                if (!Core.Me.InCombat)
                    return false;

                var vercureTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanVercure);

                if (vercureTarget == null)
                    return false;

                return await Spells.Vercure.Cast(vercureTarget);

                bool CanVercure(GameObject unit)
                {
                    if (unit.CurrentHealthPercent > RedMageSettings.Instance.VercureHealthPercent)
                        return false;

                    if (RedMageSettings.Instance.VercureTank && unit.IsTank())
                    {
                        return true;
                    }

                    if (RedMageSettings.Instance.VercureHealer && unit.IsHealer())
                    {
                        return true;
                    }

                    if (RedMageSettings.Instance.VercureSelf && unit == Core.Me)
                    {
                        return true;
                    }

                    return RedMageSettings.Instance.VercureDps && unit.IsDps();
                }
            }

            if (Core.Me.CurrentHealthPercent > RedMageSettings.Instance.VercureHealthPercent)
                return false;
            
            return await Spells.Vercure.Cast(Core.Me);
        }

        public static async Task<bool> Verraise()
        {
            if (CustomOpenerLogic.InOpener)
                return false;

            if (!RedMageSettings.Instance.Verraise)
                return false;

            if (Core.Me.ClassLevel < 64)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (!Globals.InParty)
                return false;
            
            if (!Core.Me.InCombat)
                return false;

            var verraiseTarget = Group.DeadAllies.FirstOrDefault(CanVerraise);

            if (verraiseTarget == null)
                return false;
            
            return await Spells.Verraise.Cast(verraiseTarget);

            bool CanVerraise(GameObject unit)
            {
                if (unit.HasAura(Auras.Raise))
                    return false;

                if (unit.Distance(Core.Me) > 30)
                    return false;

                if (RedMageSettings.Instance.VerraiseTank && unit.IsTank())
                {
                    return true;
                }

                if (RedMageSettings.Instance.VerraiseHealer && unit.IsHealer())
                {
                    return true;
                }

                return RedMageSettings.Instance.VerraiseDps && unit.IsDps();
            }          
        }
    }
}
