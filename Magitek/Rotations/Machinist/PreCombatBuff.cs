using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic.Roles;
using Magitek.Models.Machinist;
using Magitek.Utilities;

namespace Magitek.Rotations.Machinist
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies();

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            
            return await PhysicalDps.Peloton(MachinistSettings.Instance);
        }
    }
}