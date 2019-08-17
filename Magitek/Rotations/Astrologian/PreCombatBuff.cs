using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using Magitek.Logic.Astrologian;

namespace Magitek.Rotations.Astrologian
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (Duty.State() == Duty.States.Ended)
                return false;

            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies(Utilities.Routines.Astrologian.GroupExtension);

            if (Core.Me.IsCasting)
                return true;

            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
            await Casting.CheckForSuccessfulCast();

            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance; 
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            if (Globals.OnPvpMap)
                return false;
            
            if(ActionResourceManager.Astrologian.Arcana == ActionResourceManager.Astrologian.AstrologianCard.None)
                await Spells.Draw.Cast(Core.Me);

            return await Buff.Sect();
        }
    }
}
