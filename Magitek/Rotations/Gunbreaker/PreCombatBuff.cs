using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic.Gunbreaker;
using Magitek.Utilities;

namespace Magitek.Rotations.Gunbreaker
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
            return await Buff.RoyalGuard();
        }
    }
}