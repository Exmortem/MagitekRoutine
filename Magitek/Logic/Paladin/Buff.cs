using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;

namespace Magitek.Logic.Paladin
{
    internal static class Buff
    {

        public static async Task<bool> IronWill()
        {
            if (!PaladinSettings.Instance.UseIronWill)
            {
                if (Core.Me.HasAura(Auras.IronWill, true))
                {
                    return await Spells.IronWill.Cast(Core.Me);
                }
                return false;
            }

            if (Core.Me.HasAura(Auras.IronWill, true))
                return false;

            return await Spells.IronWill.Cast(Core.Me);
        }

        public static async Task<bool> FightOrFlight()
        {
            if (!PaladinSettings.Instance.UseFightOrFlight)
                return false;

            if (Spells.Requiescat.IsKnown() && !Spells.Requiescat.IsReady(1000))
                return false;

            //if you're not in Range for your burst (Req Range, do not launch it
            if (!Core.Me.CurrentTarget.WithinSpellRange(Spells.Requiescat.Range))
                return false;

            if (!PaladinRoutine.GlobalCooldown.CanDoubleWeave() || !PaladinRoutine.GlobalCooldown.CanWeave(2))
                return false;
 
            return await Spells.FightorFlight.Cast(Core.Me);

        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.FightorFlight.IsKnown() && !Spells.FightorFlight.IsReady(3000))
                return false;

            return await Tank.UsePotion(PaladinSettings.Instance);
        }

    }
}