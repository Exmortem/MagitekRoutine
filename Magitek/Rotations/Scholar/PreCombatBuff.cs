using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Logic.Scholar;
using Magitek.Utilities;

namespace Magitek.Rotations.Scholar
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies(Utilities.Routines.Scholar.GroupExtension);

            if (Core.Me.IsCasting)
                return true;

            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();

            await Casting.CheckForSuccessfulCast();
            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            if (Core.Me.IsMounted)
                return false;

            if (CustomOpenerLogic.InOpener) return false;

            if (await Buff.SummonPet()) return true;
            return await Buff.Aetherflow();
        }
    }
}
