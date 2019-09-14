using System;
using System.Threading.Tasks;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;

namespace Magitek.Logic.Ninja
{
    internal static class Aoe
    {
        public static async Task<bool> DeathBlossom()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (!NinjaSettings.Instance.UseDeathBlossom)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.DeathBlossomEnemies)
                return false;

            return await Spells.DeathBlossom.Cast(Core.Me);
        }

        public static async Task<bool> HakkeMujinsatsu()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (Casting.SpellCastHistory.Take(5).All(s => s.Spell != Spells.DeathBlossom))
                return false;

            if (ActionManager.LastSpell == Spells.HakkeMujinsatsu)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.DeathBlossomEnemies)
                return false;

            return await Spells.HakkeMujinsatsu.Cast(Core.Me);
        }

        public static async Task<bool> HellfrogMedium()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (!NinjaSettings.Instance.UseHellfrogMedium)
                return false;


            if(Core.Me.ClassLevel < 68)
                return await Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget);

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 6 + r.CombatReach) < 2)
                return false;

            return await Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget);
        }
    }
}
