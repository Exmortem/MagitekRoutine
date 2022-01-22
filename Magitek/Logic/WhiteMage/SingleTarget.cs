using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.WhiteMage
{
    internal static class SingleTarget
    {
        public static async Task<bool> Stone()
        {
            if (!WhiteMageSettings.Instance.Stone)
                return false;

            return await Spells.Stone.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> AfflatusMisery()
        {
            if (!WhiteMageSettings.Instance.DoDamage)
                return false;
            if (Core.Me.ClassLevel < Spells.AfflatusMisery.LevelAcquired)
                return false;
            if (!WhiteMageSettings.Instance.UseAfflatusMisery)
                return false;
            if (ActionResourceManager.WhiteMage.BloodLily < 3)
                return false;
            if (!BotManager.Current.IsAutonomous && !MovementManager.IsMoving
                && Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 5 + r.CombatReach) < WhiteMageSettings.Instance.HolyEnemies)
                return false;
            return await Spells.AfflatusMisery.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ForceAfflatusMisery()
        {
            if (!WhiteMageSettings.Instance.ForceAfflatusMisery)
                return false;

            if (!await Spells.AfflatusMisery.Cast(Core.Me.CurrentTarget)) return false;
            WhiteMageSettings.Instance.ForceAfflatusMisery = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> Dots()
        {
            if (Combat.IsMoving(Core.Me) && Core.Me.ClassLevel < 56 || Combat.IsMoving(Core.Me) && WhiteMageSettings.Instance.Dotwhilemoving)
            {
                return await Spells.Dia.Cast(Core.Me.CurrentTarget);
            }

            if (WhiteMageSettings.Instance.UseTimeTillDeathForDots)
            {
                var combatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();

                if (combatTimeLeft > 0 && combatTimeLeft < WhiteMageSettings.Instance.DontDotIfEnemyDyingWithin)
                    return false;
            }
            else
            {
                if (!Core.Me.CurrentTarget.HealthCheck(WhiteMageSettings.Instance.DotHealthMinimum, WhiteMageSettings.Instance.DotHealthMinimumPercent))
                    return false;
            }

            return await Aero();
        }

        private static async Task<bool> Aero()
        {
            if (!WhiteMageSettings.Instance.Aero)
                return false;

            if (Core.Me.ClassLevel < 46)
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Aero, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

                return await Spells.Aero.CastAura(Core.Me.CurrentTarget, Auras.Aero, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000);
            }

            if (Core.Me.ClassLevel < 72)
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Aero2, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000))
                    return false;

                return await Spells.Aero2.CastAura(Core.Me.CurrentTarget, Auras.Aero2, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000);
            }

            else
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Dia, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000))
                    return false;
                if (Core.Me.ClassLevel >= 72 && Spells.Assize.Cooldown.TotalMilliseconds < 4000 && Spells.Assize.Cooldown.TotalMilliseconds > 0)
                    return false;
                return await Spells.Dia.CastAura(Core.Me.CurrentTarget, Auras.Dia, true, WhiteMageSettings.Instance.DotRefreshSeconds * 1000);
            }
        }
    }
}
