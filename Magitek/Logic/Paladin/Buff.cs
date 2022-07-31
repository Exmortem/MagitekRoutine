using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
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

            if (Core.Me.HasAura(Auras.Requiescat, true))
                return false;

            if (Casting.LastSpell == Spells.Requiescat)
                return false;

            // If we're in an aoe situation we want to FoF even if our target doesn't have goring. This is typically in dungeons.
            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies)
            {
                if (ActionManager.LastSpell != Spells.FastBlade)
                    return false;
            } else
            {
                if (Spells.Requiescat.IsKnown() && Spells.Requiescat.IsReady())
                    return false;
            }

            if (Spells.FastBlade.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
                return false;

            return await Spells.FightorFlight.Cast(Core.Me);

        }
 
    }
}