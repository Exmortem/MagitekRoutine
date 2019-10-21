using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Logic.Monk;
using Magitek.Models.Monk;
using Magitek.Utilities;

namespace Magitek.Rotations.Monk
{
    internal static class PreCombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Chocobo.HandleChocobo()) return true;

            await Casting.CheckForSuccessfulCast();        
            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);

            if (await Buff.FistsOf()) return true;
            if (await Buff.Meditate()) return true;

            if (MonkSettings.Instance.UsePositionalToasts && Utilities.Routines.Monk.UseToast == 9)
            {
                Logger.Write($@"[Magitek] Initiated Toast for MNK");
                Thread T = new Thread(() => PositionalToast.PositionalLogic());
                Utilities.Routines.Monk.UseToast = 0;
                PositionalToast.SendToast("Toast Overlay Initiated", 5);
                T.Start();
            }

            return false;
        }
    }
}