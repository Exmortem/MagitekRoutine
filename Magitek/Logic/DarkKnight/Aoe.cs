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
        public static async Task<bool> Unleash()
        {
            if (!DarkKnightSettings.Instance.Unleash)
                return false;

            if (Math.Abs(ActionManager.ComboTimeLeft) > 0)
                return false;
            
            if (Core.Me.OnPvpMap())
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            if (!Globals.InParty)
                return false;

            if (DarkKnightSettings.Instance.UnleashAlwaysUseProc && Core.Me.HasAura(Auras.EnhancedUnleash))
                return await Spells.Unleash.Cast(Core.Me);

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.WithinSpellRange(5)) < DarkKnightSettings.Instance.UnleashEnemies)
                return false;

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach && r.TargetGameObject != Core.Me) >= DarkKnightSettings.Instance.UnleashEnemies)
            {
                return await Spells.Unleash.Cast(Core.Me);
            }
            
            if (Combat.CombatTime.Elapsed.Seconds < 15 && Utilities.Routines.DarkKnight.PullUnleash < DarkKnightSettings.Instance.UnleashOnGroupPull)
            {
                if (!await Spells.Unleash.Cast(Core.Me))
                    return false;
                
                Utilities.Routines.DarkKnight.PullUnleash++;
                Utilities.Routines.DarkKnight.LastUnleash = DateTime.Now;
                return true;
            }

            if (!DarkKnightSettings.Instance.UnleashOnInterval) 
                return false;

            if (DateTime.Now < Utilities.Routines.DarkKnight.LastUnleash.AddSeconds(DarkKnightSettings.Instance.UnleashIntervalSeconds))
                return false;

            if (!await Spells.Unleash.Cast(Core.Me))
                return false;
            
            Utilities.Routines.DarkKnight.LastUnleash = DateTime.Now;
            return true;
        }

        public static async Task<bool> SaltedEarth()
        {
            if (!DarkKnightSettings.Instance.SaltedEarth)
                return false;

            // We could be gathering enemies?
            if (MovementManager.IsMoving)
                return false;
            
            //var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 25 && r.TaggerType == 2);

            //if (enemyCount < DarkKnightSettings.Instance.SaltedEarthEnemies)
            //    return false;
            
            //// Lets look at how many enemies are in combat and compare it to how many are in range of us
            //var saltedEarthCount = Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 15 + r.CombatReach);

            //if (saltedEarthCount < DarkKnightSettings.Instance.SaltedEarthEnemies)
            //    return false;
            
            return await Spells.SaltedEarth.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AbyssalDrain()
        {         
            if (!DarkKnightSettings.Instance.AbyssalDrain)
                return false;

            if (!Core.Me.HasAura(Auras.Grit))
                return false;

            if (Core.Me.CurrentManaPercent < DarkKnightSettings.Instance.AbyssalDrainMinimumMp)
                return false;

            if (Spells.AbyssalDrain.Cooldown != TimeSpan.Zero)
                return false;

            // If we have Blood Price we should Dark Arts this ability
            if (Core.Me.HasBloodWeapon())
            {
                if (!Utilities.Routines.DarkKnight.CanDarkArts(Spells.AbyssalDrain))
                    return false;
            }

            // Find the enemy that has the most enemies around them
            GameObject abyssalTarget = null;
            var enemiesInRange = 0;

            foreach (var enemy in Combat.Enemies)
            {
                if (enemy.Distance(Core.Me) < 15)
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

            if (!Core.Me.HasBloodWeapon())
                return await Spells.AbyssalDrain.Cast(abyssalTarget);

            return await Spells.AbyssalDrain.Cast(abyssalTarget);
        }

        public static async Task<bool> Quietus()
        {
            if (!DarkKnightSettings.Instance.Quietus)
                return false;

            if (Core.Me.ClassLevel < 64)
                return false;

            if (ActionResourceManager.DarkKnight.BlackBlood < 50)
                return false;

            if (Spells.Quietus.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < DarkKnightSettings.Instance.QuietusEnemies)
                return false;

            if (!Utilities.Routines.DarkKnight.CanDarkArts(Spells.Quietus))
            {
                if (ActionResourceManager.DarkKnight.BlackBlood > 90)
                {
                    return await Spells.Quietus.Cast(Core.Me);
                }

                return false;
            }

            return await Spells.Quietus.Cast(Core.Me);
        }
    }
}