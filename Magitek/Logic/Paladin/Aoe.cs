using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;


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
            if (!PaladinSettings.Instance.AoE)
                return false;

            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.HolyCircle, Spells.HolyCircle))
                return false;

            if (PaladinRoutine.RequiescatStackCount <= 1)
                return false;

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies)
                return false;

            return await Spells.HolyCircle.Cast(Core.Me);
        }

        public static async Task<bool> Confiteor()
        {
            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.AoE, Spells.Confiteor))
                return false;

            if (ActionManager.CanCast(Spells.BladeOfFaith.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfFaith.Cast(Core.Me.CurrentTarget);

            if (ActionManager.CanCast(Spells.BladeOfTruth.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfTruth.Cast(Core.Me.CurrentTarget);

            if (ActionManager.CanCast(Spells.BladeOfValor.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfValor.Cast(Core.Me.CurrentTarget);

            if (PaladinRoutine.RequiescatStackCount > 1)
                return false;

            return await Spells.Confiteor.Cast(Core.Me.CurrentTarget);
        }
    }
}