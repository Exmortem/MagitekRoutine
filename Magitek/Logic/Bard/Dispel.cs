using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.Bard
{
    internal static class Dispel
    {
        public static async Task<bool> Execute()
        {
            // We don't have Leeches if we're below level 40
            if (!BardSettings.Instance.Dispel || !ActionManager.HasSpell(Spells.TheWardensPaean.Id))
                return false;

            if (Globals.InParty)
            {
                if (await CheckParty()) return true;
            }
            else
            {
                if (await CheckSelf()) return true;
            }

            return false;
        }
        
        private static async Task<bool> CheckParty()
        {
            // First we look for High Priority Dispels
            var dispelTarget = Group.CastableAlliesWithin30.Where(a => a.NeedsDispel(true)).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget != null)
            {
                return await Spells.TheWardensPaean.Cast(dispelTarget);
            }

            // Check to see if we need to heal people before we Dispel anyone
            if (ScholarSettings.Instance.DispelOnlyAbove && Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth))
                return false;

            dispelTarget = Group.CastableAlliesWithin30.Where(a => a.NeedsDispel()).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget == null)
                return false;

            return await Spells.TheWardensPaean.Cast(dispelTarget);
        }

        private static async Task<bool> CheckSelf()
        {
            if (Core.Me.NeedsDispel(true))
            {
                return await Spells.TheWardensPaean.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth)
                return false;

            if (!Core.Me.NeedsDispel())
                return false;

            return await Spells.TheWardensPaean.Cast(Core.Me);
        }
    }
}