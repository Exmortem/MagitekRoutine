using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Utilities;

namespace Magitek.Rotations.Samurai
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();
            
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            if (await GambitLogic.Gambit()) return true;
            return false;
        }
    }
}