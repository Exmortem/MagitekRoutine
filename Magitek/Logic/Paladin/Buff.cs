using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

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

            if (Casting.LastSpell == Spells.FastBlade)
                return false;

            if (Casting.LastSpell == Spells.RiotBlade)
                return false;

            //Force Delay CD
            if (Spells.FastBlade.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
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