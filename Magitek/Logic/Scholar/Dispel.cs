using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Scholar
{
    internal static class Dispel
    {
        public static async Task<bool> Execute()
        {
            if (!ScholarSettings.Instance.Dispel || !ActionManager.HasSpell(Spells.Esuna.Id))
                return false;

            if (Globals.InParty)
            {
                return await CheckParty();
            }

            return await CheckSelf();
        }

        private static async Task<bool> CheckParty()
        {
            // First we look for High Priority Dispels
            var dispelTarget = Group.CastableAlliesWithin30.Where(a => a.NeedsDispel(true)).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget != null)
            {
                return await Spells.Esuna.Cast(dispelTarget);
            }

            if (!ScholarSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                return false;

            // Check to see if we need to heal people before we Dispel anyone
            if (ScholarSettings.Instance.DispelOnlyAbove && Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth))
                return false;

            if(Casting.LastSpell == Spells.Esuna)
                dispelTarget = Group.CastableAlliesWithin30.Where(a => a.HasAnyDispellableAura() && Casting.LastSpellTarget != a).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            else
                dispelTarget = Group.CastableAlliesWithin30.Where(a => a.HasAnyDispellableAura()).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget == null)
                return false;

            return await Spells.Esuna.Cast(dispelTarget);
        }

        private static async Task<bool> CheckSelf()
        {
            if (Core.Me.NeedsDispel(true))
            {
                return await Spells.Esuna.Cast(Core.Me);
            }

            if (Casting.LastSpell == Spells.Esuna && Casting.LastSpellTarget == Core.Me)
                return false;

            if (!ScholarSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                return false;

            if (Core.Me.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth)
                return false;

            if (!Core.Me.HasAnyDispellableAura())
                return false;

            return await Spells.Esuna.Cast(Core.Me);
        }
    }
}
