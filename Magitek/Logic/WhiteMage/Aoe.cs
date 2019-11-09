using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;

namespace Magitek.Logic.WhiteMage
{
    internal static class Aoe
    {
        public static async Task<bool> Holy()
        {
            if (!WhiteMageSettings.Instance.Holy)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 8 + r.CombatReach) < WhiteMageSettings.Instance.HolyEnemies)
                return false;

            if (WhiteMageSettings.Instance.ThinAirBeforeHoly && await Buff.ThinAir(true))
            {
                await Coroutine.Wait(3000, () => Core.Me.HasAura(Auras.ThinAir));
            }

            if (WhiteMageSettings.Instance.PresenceOfMindBeforeHoly)
            {
                if (await Spells.PresenceofMind.Cast(Core.Me))
                {
                    await Coroutine.Wait(3000, () => Core.Me.HasAura(Auras.PresenceOfMind));
                }               
            }

            return await Spells.Holy.Cast(Core.Me);
        }
        
        public static async Task<bool> AssizeDamage()
        {
            if (!WhiteMageSettings.Instance.Assize)
                return false;
            if (Spells.Assize.Cooldown.TotalMilliseconds > 1)
                return false;

            if (!WhiteMageSettings.Instance.AssizeDamage)
                return false;

            if (WhiteMageSettings.Instance.AssizeHealOnly)
                return false;

            if (WhiteMageSettings.Instance.AssizeOnlyBelow90Mana && Core.Me.CurrentManaPercent >= 90)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= (8 + r.CombatReach)) < WhiteMageSettings.Instance.AssizeEnemies)
                return false;
            if (Casting.LastSpell == Spells.Dia)
                return await Spells.Assize.Cast(Core.Me);
            if (!Core.Me.CurrentTarget.HasAura(Auras.Dia, true, 5500))
                return false;
            return await Spells.Assize.Cast(Core.Me);
        }
    }
}