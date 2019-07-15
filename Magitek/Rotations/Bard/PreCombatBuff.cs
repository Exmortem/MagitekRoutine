using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Rotations.Bard
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies();
            
            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            
            //Openers.OpenerCheck();

            if (Core.Me.HasTarget && Core.Me.CurrentTarget.CanAttack)
            {
                return false;
            }

            if (Core.Me.OnPvpMap())
                return false;

            return await PhysicalDps.Peloton(BardSettings.Instance);
        }
    }
}