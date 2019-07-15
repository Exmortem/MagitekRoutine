using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

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
                if (await CheckParty()) return true;
            }
            else
            {
                if (await CheckSelf()) return true;
            }

            return await CheckPet();
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

            dispelTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.HasAnyDispellableAura());

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

            if (!ScholarSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                return false;

            if (Core.Me.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth)
                return false;

            if (!Core.Me.HasAnyDispellableAura())
                return false;

            return await Spells.Esuna.Cast(Core.Me);
        }

        private static async Task<bool> CheckPet()
        {
            if (!ScholarSettings.Instance.DispelPet)
                return false;

            if (Core.Me.Pet == null)
                return false;

            var petAsCharacter = Core.Me.Pet as Character;

            // Check if we're in a party since we can be in both with a pet
            if (Globals.InParty)
            {
                if (petAsCharacter.NeedsDispel(true))
                {
                    return await Spells.Esuna.Cast(petAsCharacter);
                }

                if (!ScholarSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                    return false;

                // Check if anyone in our party needs a heal first
                if (ScholarSettings.Instance.DispelOnlyAbove && Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth))
                    return false;

                if (!petAsCharacter.HasAnyDispellableAura())
                    return false;

                return await Spells.Esuna.Cast(petAsCharacter);
            }
            else
            {
                if (petAsCharacter.NeedsDispel(true))
                {
                    return await Spells.Esuna.Cast(petAsCharacter);
                }

                if (!ScholarSettings.Instance.AutomaticallyDispelAnythingThatsDispellable)
                    return false;

                // Check if we need to be healed first
                if (Core.Me.CurrentHealthPercent < ScholarSettings.Instance.DispelOnlyAboveHealth)
                    return false;

                if (!petAsCharacter.HasAnyDispellableAura())
                    return false;

                return await Spells.Esuna.Cast(petAsCharacter);
            }
        }
    }
}
