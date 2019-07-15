using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;

namespace Magitek.Logic.RedMage
{
    internal static class Aoe
    {
        public static async Task<bool> Scatter()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (!RedMageSettings.Instance.Scatter)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.ScatterEnemies)
                return false;

            return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ContreSixte()
        {
            if (!RedMageSettings.Instance.UseContreSixte)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            return await Spells.ContreSixte.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Moulinet()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Moulinet)
                return false;

            if (ActionResourceManager.RedMage.BlackMana < 30 || ActionResourceManager.RedMage.WhiteMana < 30)
                return false;

            if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
        }
    }
}
