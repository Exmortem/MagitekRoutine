using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Models.Scholar;
using Magitek.Toggles;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class Utility
    {

        public static async Task<bool> Troubadour()
        {

            if (!BardSettings.Instance.ForceTroubadour)
                return false;

            if (!await Spells.Troubadour.Cast(Core.Me)) return false;
            BardSettings.Instance.ForceTroubadour = false;
            TogglesManager.ResetToggles();
            return true;

        }

        public static async Task<bool> RepellingShot()
        {
            if (!BardSettings.Instance.RepellingShot)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (BardSettings.Instance.RepellingShotOnlyWhenTargeted)
            {
                var repellingShotTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) <= 5 + r.CombatReach &&
                                                                             r.InView() &&
                                                                             ActionManager.CanCast(Spells.RepellingShot.Id, r));

                if (repellingShotTarget == null)
                    return false;

                return await Spells.RepellingShot.Cast(repellingShotTarget);
            }
            else
            {
                if (Core.Me.CurrentTarget == null)
                    return false;

                if (Core.Me.CurrentTarget.Distance(Core.Me) > 5 + Core.Me.CurrentTarget.CombatReach)
                    return false;

                return await Spells.RepellingShot.Cast(Core.Me.CurrentTarget);
            }
        }

        public static async Task<bool> NaturesMinne()
        {
            if (!BardSettings.Instance.NaturesMinne)
                return false;

            if (!Globals.InParty)
            {
                if (Core.Me.CurrentHealthPercent > BardSettings.Instance.NaturesMinneHealthPercent)
                    return false;

                return await Spells.NaturesMinne.Cast(Core.Me);
            }

            var naturesMinneTarget = Group.CastableAlliesWithin30.FirstOrDefault(r =>
                r.CurrentHealthPercent <= BardSettings.Instance.NaturesMinneHealthPercent && (
                    BardSettings.Instance.NaturesMinneTanks && r.IsTank() ||
                    BardSettings.Instance.NaturesMinneDps && r.IsDps() ||
                    BardSettings.Instance.NaturesMinneHealers && r.IsHealer()));

            if (naturesMinneTarget == null)
                return false;

            return await Spells.NaturesMinne.Cast(naturesMinneTarget);
        }

        public static async Task<bool> WardensPaean()
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