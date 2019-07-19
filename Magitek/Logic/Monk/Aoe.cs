using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;

namespace Magitek.Logic.Monk
{
    internal static class Aoe
    {

        public static async Task<bool> Rockbreaker()
        {
            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.InView()) < MonkSettings.Instance.RockbreakerEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.CoeurlForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            return await Spells.Rockbreaker.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FourPointStrike()
        {
            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.InView()) < MonkSettings.Instance.RockbreakerEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            return await Spells.FourPointFury.Cast(Core.Me);
        }



        public static async Task<bool> ElixerField()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (!MonkSettings.Instance.UseElixerField)
                return false;

            if (Core.Me.EnemiesNearby(5).Count() < MonkSettings.Instance.ElixerFieldEnemies)
                return false;

            return await Spells.ElixirField.Cast(Core.Me);
        }
    }
}
