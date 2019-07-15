using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic.DarkKnight;
using Magitek.Utilities;

namespace Magitek.Rotations.DarkKnight
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies();
            
            if (Core.Me.IsCasting)
                return true;
            
            await Casting.CheckForSuccessfulCast();
            
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            Utilities.Routines.DarkKnight.PullUnleash = 0;
            return await Buff.Grit();
        }
    }
}