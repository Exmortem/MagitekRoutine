using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Sage;

namespace Magitek.Logic.Sage
{
    internal static class AoE
    {
        public static async Task<bool> Phlegma()
        {
            if (!SageSettings.Instance.AoE)
                return false;

            if (Core.Me.ClassLevel < Spells.Phlegma.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= Spells.Phlegma.Radius) < SageSettings.Instance.AoEEnemies)
                return false;

            if (Spells.Phlegma.Charges == 0)
                return false;

            return await Spells.Phlegma.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Dyskrasia()
        {
            if (!SageSettings.Instance.AoE)
                return false;

            if (Core.Me.ClassLevel < Spells.Dyskrasia.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            //Prioritize Toxikon over Dyskrasia
            if (Addersting > 0
                && Core.Me.ClassLevel >= Spells.Toxikon.LevelAcquired)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.Dyskrasia.Radius) < SageSettings.Instance.AoEEnemies)
                return false;

            return await Spells.Dyskrasia.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Toxikon()
        {
            if (!SageSettings.Instance.AoE)
                return false;

            if (Core.Me.ClassLevel < Spells.Toxikon.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= Spells.Toxikon.Radius) < SageSettings.Instance.AoEEnemies)
                return false;

            if (Addersting == 0)
                return false;

            return await Spells.Toxikon.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Pneuma()
        {
            if (!SageSettings.Instance.AoE)
                return false;

            if (!SageSettings.Instance.Pneuma)
                return false;

            if (Core.Me.ClassLevel < Spells.Pneuma.LevelAcquired)
                return false;

            if (!SageSettings.Instance.PneumaHealOnly)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= Spells.Pneuma.Radius) < SageSettings.Instance.AoEEnemies)
                return false;

            if (Spells.Pneuma.Cooldown != TimeSpan.Zero)
                return false;

            return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);
        }
    }
}
