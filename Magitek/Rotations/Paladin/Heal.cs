using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Utilities;

namespace Magitek.Rotations.Paladin
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies();
            if (await GambitLogic.Gambit()) return true;
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();
            
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            
            return await Logic.Paladin.Heal.Clemency();
        }
    }
}