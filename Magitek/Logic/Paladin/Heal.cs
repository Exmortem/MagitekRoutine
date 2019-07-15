using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;

namespace Magitek.Logic.Paladin
{
    internal static class Heal
    {
        public static async Task<bool> Clemency()
        {
            if (!PaladinSettings.Instance.UseClemency)
                return false;

            if (Casting.LastSpell == Spells.Clemency)
                return false;

            if (PaladinSettings.Instance.UseClemencySelf)
            {
                if (Core.Me.CurrentHealthPercent < PaladinSettings.Instance.UseClemencySelfHp)
                {
                    return await Spells.Clemency.Cast(Core.Me);
                }
            }

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentManaPercent < PaladinSettings.Instance.MinMpClemency)
                return false;

            var anyHealers = Group.CastableAlliesWithin30.Any(r => r.IsHealer());

            if (PaladinSettings.Instance.UseClemencyHealer && anyHealers)
            {
                var healerTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsHealer() && r.CurrentHealthPercent < PaladinSettings.Instance.UseClemencyHealerHp);

                if (healerTarget != null)
                {
                    return await Spells.Clemency.Cast(healerTarget);
                }
            }

            var anyDps = Group.CastableAlliesWithin30.Any(r => r.IsDps());

            if (PaladinSettings.Instance.UseClemencyDps && anyDps)
            {
                var dpsTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsDps() && r.CurrentHealthPercent < PaladinSettings.Instance.UseClemencyDpsHp);

                if (dpsTarget != null)
                {
                    return await Spells.Clemency.Cast(dpsTarget);
                }
            }

            if (!PaladinSettings.Instance.UseClemencySelf || anyHealers) 
                return false;

            if (Core.Me.CurrentHealthPercent < PaladinSettings.Instance.UseClemencySelfHp)
            {
                return await Spells.Clemency.Cast(Core.Me);
            }

            return false;
        }
    }
}