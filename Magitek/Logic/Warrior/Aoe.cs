using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;

namespace Magitek.Logic.Warrior
{
    internal static class Aoe
    {
        public static async Task<bool> ChaoticCyclone()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (!Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.ChaoticCycloneMinimumEnemies)
                return false;

            return await Spells.ChaoticCyclone.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> Decimate()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (!WarriorSettings.Instance.UseDecimate)
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies)
                return false;

            return await WarriorRoutine.Decimate.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> Overpower()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 8 + r.CombatReach) < WarriorSettings.Instance.OverpowerMinimumEnemies)
                return false;

            return await Spells.Overpower.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MythrilTempest()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (!WarriorRoutine.CanContinueComboAfter(Spells.Overpower))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < WarriorSettings.Instance.MythrilTempestMinimumEnemies)
                return false;

            return await Spells.MythrilTempest.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Orogeny()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (!Spells.Orogeny.IsReady())
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < WarriorSettings.Instance.OrogenyMinimumEnemies)
                return false;

            return await Spells.Orogeny.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PrimalRend()
        {
            if (!WarriorSettings.Instance.UseAoe)
                return false;

            if (!WarriorSettings.Instance.UsePrimalRend)
                return false;

            if (!Core.Me.HasAura(Auras.PrimalRendReady))
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < WarriorSettings.Instance.PrimalRendMinimumEnemies)
                return false;

            return await Spells.PrimalRend.Cast(Core.Me.CurrentTarget);
        }
    }
}