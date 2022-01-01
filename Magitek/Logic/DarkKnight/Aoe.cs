using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.DarkKnight
{
    internal static class Aoe
    {
        public static async Task<bool> AbyssalDrain()
        {
            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (!DarkKnightSettings.Instance.UseAbyssalDrain)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 5);
            if (enemyCount < DarkKnightSettings.Instance.AbyssalDrainEnemies)
                return false;

            return await Spells.AbyssalDrain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SaltedEarth()
        {
            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (!DarkKnightSettings.Instance.UseSaltedEarth)
                return false;

            if (MovementManager.IsMoving)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5);
            if (enemyCount < DarkKnightSettings.Instance.SaltedEarthEnemies)
                return false;

            return await Spells.SaltedEarth.Cast(Core.Me);
        }

        public static async Task<bool> Unleash()
        {
            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (!DarkKnightSettings.Instance.UseUnleash)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);
            if (enemyCount < DarkKnightSettings.Instance.UnleashEnemies)
                return false;

            return await Spells.Unleash.Cast(Core.Me);
        }

        public static async Task<bool> StalwartSoul()
        {
            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (ActionManager.LastSpell != Spells.Unleash)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            var enemyCount = (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach));
            if (enemyCount < DarkKnightSettings.Instance.UnleashEnemies)
                return false;

            return await Spells.StalwartSoul.Cast(Core.Me);
        }

        public static async Task<bool> Quietus()
        {

            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (!DarkKnightSettings.Instance.UseQuietus)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);
            if (enemyCount < DarkKnightSettings.Instance.QuietusEnemies)
                return false;

            return await Spells.Quietus.Cast(Core.Me);
        }

        public static async Task<bool> FloodofDarknessShadow()
        {
            if (!DarkKnightSettings.Instance.UseAoe)
                return false;

            if (!DarkKnightSettings.Instance.UseFloodDarknessShadow)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 10 + r.CombatReach);
            if (enemyCount < DarkKnightSettings.Instance.FloodEnemies)
                return false;

            if (Core.Me.CurrentMana < 6000 && DarkKnightSettings.Instance.UseTheBlackestNight)
                return false;

            if (Core.Me.CurrentMana < DarkKnightSettings.Instance.SaveXMana + 3000)
                return false;

            return await Spells.FloodofDarkness.Cast(Core.Me.CurrentTarget);
        }
    }
}