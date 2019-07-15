using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Rotations.WhiteMage
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies(Utilities.Routines.WhiteMage.GroupExtension);
            
            if (Core.Me.IsCasting)
                return true;

            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
            await Casting.CheckForSuccessfulCast();

            if (Core.Me.OnPvpMap())
            {
                return await Heal.PvpRotation();
            }

            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance; 
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            
            if (Core.Me.IsMounted)
                return false;

            if (Duty.State() == Duty.States.Ended)
                return false;
            
            return false;
        }
    }
}
