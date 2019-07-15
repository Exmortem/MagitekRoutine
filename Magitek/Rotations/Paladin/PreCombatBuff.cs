using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic.Paladin;
using Magitek.Utilities;

namespace Magitek.Rotations.Paladin
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
            
            if (Core.Me.IsMounted)
                return false;
            
            return await Buff.Oath();
        }
    }
}