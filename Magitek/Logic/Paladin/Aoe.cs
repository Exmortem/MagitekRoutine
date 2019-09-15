using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using static Magitek.Utilities.Routines.Paladin;

namespace Magitek.Logic.Paladin
{
    internal static class Aoe
    {
        public static async Task<bool> CircleofScorn()
        {
            if (!PaladinSettings.Instance.CircleOfScorn)
                return false;

            if (MovementManager.IsMoving)
                return false;

            var enemiesHealth = Combat.Enemies.Any(r => r.Distance(Core.Me) <= 5 + r.CombatReach && r.HealthCheck(PaladinSettings.Instance.HealthSetting, PaladinSettings.Instance.HealthSettingPercent));

            if (!enemiesHealth)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 25 && r.InCombat);
            var cosCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);

            var canCoS = cosCount >= enemyCount || cosCount > 2;

            if (Casting.LastSpell == Spells.FightorFlight)
                return false;

            if (!canCoS)
                return false;

            if (Spells.FightorFlight.Cooldown.Seconds <= 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 8000))
            {
                //Right we want to check if we want to hold CoS.
                if (Casting.LastSpell == Spells.FastBlade)
                    return false;

                if (Casting.LastSpell == Spells.RiotBlade)
                    return false;

                if (Casting.LastSpell == Spells.Confiteor)
                    return false;

                if (Casting.LastSpell == Spells.HolySpirit)
                    return false;

                if (Casting.LastSpell == Spells.Atonement)
                    return false;

                if (Casting.LastSpell == Spells.Intervene)
                    return false;

                return await Spells.CircleofScorn.Cast(Core.Me);
            }

            return await Spells.CircleofScorn.Cast(Core.Me);
        }

        public static async Task<bool> TotalEclipse()
        {
            if (!PaladinSettings.Instance.TotalEclipse)
                return false;

            if (!PaladinSettings.Instance.AoE)
                return false;

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies)
                return false;

            if (ActionManager.LastSpell == Spells.TotalEclipse && PaladinSettings.Instance.Prominance && Core.Me.ClassLevel >= 40)
            {
                return await Spells.Prominance.Cast(Core.Me);
            }

            return await Spells.TotalEclipse.Cast(Core.Me);
        }

        public static async Task<bool> HolyCircle()
        {
            if (!PaladinSettings.Instance.HolyCircle)
                return false;

            if (!PaladinSettings.Instance.Requiescat)
                return false;

            if (!PaladinSettings.Instance.AoE)
                return false;

            if (Core.Me.ClassLevel < 72)
                return false;

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat))
                return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel == 80 && Core.Me.CurrentMana <= 4000)
                return false;

            return await Spells.HolyCircle.Cast(Core.Me);
        }

        public static async Task<bool> Confiteor()
        {
            if (Core.Me.ClassLevel < 80)
                return false;
            // This is a sanity check, when client is slow it doesn't trigger the aura quick enough the next check will just cast Confiteor away since it can't check the timer.
            if (Core.Me.CurrentMana > 9000)
                return false;

            if (ActionManager.LastSpell == Spells.Requiescat)
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat, true,
                3000))
                return await Spells.Confiteor.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentMana > 4000)
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat))
                return false;

            return await Spells.Confiteor.Cast(Core.Me.CurrentTarget);
        }
    }
}