using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.DarkKnight
{
    internal static class Aoe
    {
        public static async Task<bool> AbyssalDrain()
        {
            if (!DarkKnightSettings.Instance.UseAbyssalDrain)
                return false;

            // Find the enemy that has the most enemies around them
            GameObject abyssalTarget = null;
            var enemiesInRange = 0;

            foreach (var enemy in Combat.Enemies)
            {
                if (enemy.Distance(Core.Me) > 15)
                    continue;

                var enemyCount = Combat.Enemies.Count(r => r.Distance(enemy) <= 5 + r.CombatReach);

                if (enemyCount < DarkKnightSettings.Instance.AbyssalDrainEnemies)
                    continue;

                if (enemyCount <= enemiesInRange)
                    continue;

                enemiesInRange = enemyCount;
                abyssalTarget = enemy;
            }

            if (abyssalTarget == null || enemiesInRange < DarkKnightSettings.Instance.AbyssalDrainEnemies)
                return false;

            return await Spells.AbyssalDrain.Cast(abyssalTarget);
        }

        public static async Task<bool> SaltedEarth()
        {
            if (!DarkKnightSettings.Instance.UseSaltedEarth)
                return false;

            if (DarkKnightSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < DarkKnightSettings.Instance.SaltedEarthEnemies)
                return false;

            // We could be gathering enemies?
            if (MovementManager.IsMoving)
                return false;

            return await Spells.SaltedEarth.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Unleash()
        {
            if (!DarkKnightSettings.Instance.UseUnleash)
                return false;

            if (DarkKnightSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < DarkKnightSettings.Instance.UnleashEnemies)
                return false;

            return await Spells.Unleash.Cast(Core.Me);
        }

        public static async Task<bool> StalwartSoul()
        {
            if (ActionManager.LastSpell != Spells.Unleash)
                return false;

            return await Spells.StalwartSoul.Cast(Core.Me);
        }

        public static async Task<bool> Quietus()
        {
            if (!DarkKnightSettings.Instance.UseQuietus)
                return false;

            if (DarkKnightSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < DarkKnightSettings.Instance.QuietusEnemies)
                return false;

            if (ActionResourceManager.DarkKnight.BlackBlood >= 50 || Core.Me.HasAura(Auras.Delirium))
                return await Spells.Quietus.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> FloodofDarknessShadow()
        {
            if (!DarkKnightSettings.Instance.UseFloodDarknessShadow)
                return false;

            if (Core.Me.CurrentMana < DarkKnightSettings.Instance.SaveXMana + 3000)
                return false;

            if (Core.Me.CurrentMana < 6000 && DarkKnightSettings.Instance.UseTheBlackestNight)
                return false;

            if (DarkKnightSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= 10 + r.CombatReach && r.InView()) < DarkKnightSettings.Instance.FloodEnemies)
                return false;

            if (Core.Me.HasDarkArts())
                return await Spells.FloodofDarkness.Cast(Core.Me.CurrentTarget);

            return await Spells.FloodofDarkness.Cast(Core.Me.CurrentTarget);
        }
    }
}