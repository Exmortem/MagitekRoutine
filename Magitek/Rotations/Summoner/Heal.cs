using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Utilities;

namespace Magitek.Rotations.Summoner
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;
            
            if (Core.Me.IsMounted)
                return true;

            Group.UpdateAllies(Utilities.Routines.Scholar.GroupExtension);
            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance; 
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2) || Core.Me.InCombat;
            Globals.OnPvpMap = Core.Me.OnPvpMap();

            if (Duty.State() == Duty.States.Ended)
                return false;

            if (await CustomOpenerLogic.Opener()) return true;
            if (await GambitLogic.Gambit()) return true;
           
            return await Logic.Summoner.Heal.Physick();
        }
    }
}