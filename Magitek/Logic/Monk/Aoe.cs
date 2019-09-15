using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;

namespace Magitek.Logic.Monk
{
    internal static class Aoe
    {

        public static async Task<bool> Enlightenment()
        {
            if (!MonkSettings.Instance.UseEnlightenment)
                return false;

            if (Core.Me.ClassLevel < 74)
                return false;

            if (ActionResourceManager.Monk.FithChakra < 5)
                return false;

            Logger.Write($@"[Magitek] Enlightenment Check: We have {Utilities.Routines.Monk.EnemiesInCone} Enemies in range we need {MonkSettings.Instance.EnlightenmentEnemies} and Enlightenment is {MonkSettings.Instance.UseEnlightenment}");

            if (Utilities.Routines.Monk.EnemiesInCone > MonkSettings.Instance.EnlightenmentEnemies)
                return await Spells.Enlightenment.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Rockbreaker()
        {
            if (Core.Me.ClassLevel < 30)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < MonkSettings.Instance.RockbreakerEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.CoeurlForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            return await Spells.Rockbreaker.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FourPointStrike()
        {
            if (Core.Me.ClassLevel < 45)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < MonkSettings.Instance.RockbreakerEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (!Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);

            return await Spells.FourPointFury.Cast(Core.Me);
        }

        public static async Task<bool> ArmOfDestroyer()
        {
            if (Core.Me.ClassLevel < 26)
                return false;

            if (!MonkSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < MonkSettings.Instance.RockbreakerEnemies)
                return false;

            return await Spells.ArmOfTheDestroyer.Cast(Core.Me);
        }
    }
}
