using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.Astrologian
{
    internal static class Dispel
    {
        public static async Task<bool> Execute()
        {
            if (!AstrologianSettings.Instance.Dispel || !ActionManager.HasSpell(Spells.Esuna.Id))
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

            if (!AstrologianSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                return false;

            // Check to see if we need to heal people before we Dispel anyone
            if (AstrologianSettings.Instance.DispelOnlyAbove && Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < AstrologianSettings.Instance.DispelOnlyAboveHealth))
                return false;

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

            if (!AstrologianSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                return false;

            if (Core.Me.CurrentHealthPercent < AstrologianSettings.Instance.DispelOnlyAboveHealth)
                return false;

            if (!Core.Me.HasAnyDispellableAura())
                return false;

            return await Spells.Esuna.Cast(Core.Me);
        }
    }
}