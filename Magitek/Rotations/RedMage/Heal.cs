using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Utilities;

namespace Magitek.Rotations.RedMage
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            Group.UpdateAllies();

            if (await Chocobo.HandleChocobo()) return true;

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();
            
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            if (await GambitLogic.Gambit()) return true;
            if (await Logic.RedMage.Heal.Verraise()) return true;
            return await Logic.RedMage.Heal.Vercure();
        }
    }
}