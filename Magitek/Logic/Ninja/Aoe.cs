using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class Aoe
    {
        public static async Task<bool> DeathBlossom()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            return await Spells.DeathBlossom.Cast(Core.Me);
        }

        public static async Task<bool> HakkeMujinsatsu()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            if (ActionManager.LastSpell != Spells.DeathBlossom)
                return false;

            return await Spells.HakkeMujinsatsu.Cast(Core.Me);
        }

        public static async Task<bool> HellfrogMedium()
        {
            if (Core.Me.ClassLevel < 62)
                return false;

            if (!NinjaSettings.Instance.UseHellfrogMedium)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 6 + r.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            if (Spells.Mug.IsKnownAndReady(1000))
                return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));

            if (Spells.Bunshin.IsKnownAndReady())
                return false;

            if (Spells.TrickAttack.IsKnownAndReady(14000))
                return false;

            return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));
        }

        public static async Task<bool> PhantomKamaitachi()
        {
            if (Core.Me.ClassLevel < 82)
                return false;

            if (!NinjaSettings.Instance.UsePhantomKamaitachi)
                return false;

            if (!Spells.PhantomKamaitachi.IsKnownAndReady())
                return false;

            return await Spells.PhantomKamaitachi.Cast(Core.Me.CurrentTarget);

        }
    }
}
