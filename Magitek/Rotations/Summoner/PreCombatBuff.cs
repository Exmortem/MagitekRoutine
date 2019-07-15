using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Logic.Summoner;
using Magitek.Utilities;

namespace Magitek.Rotations.Summoner
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            if (Core.Me.IsCasting)
                return true;
            
            await Casting.CheckForSuccessfulCast();
            SpellQueueLogic.SpellQueue.Clear();
            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance; 
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            return await Pets.Summon();
        }
    }
}