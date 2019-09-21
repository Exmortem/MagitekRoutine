using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Utilities;

namespace Magitek.Rotations.Dragoon
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            Group.UpdateAllies();

            if (PartyManager.IsInParty)
            {
                // If we're in a party and we die, but our group is still in combat, we don't want to reset the counter
                if (!Utilities.Combat.Enemies.Any())
                    Utilities.Routines.Dragoon.MirageDives = 0;
            }
            else
            {
                Utilities.Routines.Dragoon.MirageDives = 0;
            }

            Utilities.Routines.Dragoon.MirageDives = 0;

            if (await Chocobo.HandleChocobo()) return true;

            await Casting.CheckForSuccessfulCast();        
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            return false;
        }
    }
}