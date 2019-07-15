using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic.Monk;
using Magitek.Utilities;

namespace Magitek.Rotations.Monk
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            await Casting.CheckForSuccessfulCast();        
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            if (await Buff.FistsOf()) return true;

            return false;
        }
    }
}