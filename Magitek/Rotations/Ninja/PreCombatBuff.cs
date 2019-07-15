using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Logic.Ninja;
using Magitek.Models.Ninja;
using Magitek.Utilities;

namespace Magitek.Rotations.Ninja
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            await Casting.CheckForSuccessfulCast();        
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!NinjaSettings.Instance.UseHutonOutsideOfCombat)
                return false;

            if (NinjaSettings.Instance.UseHutonOutsideOfCombat)
            {
                if (!DutyManager.InInstance)
                    return false;
            }

            return Ninjutsu.Huton();
        }
    }
}