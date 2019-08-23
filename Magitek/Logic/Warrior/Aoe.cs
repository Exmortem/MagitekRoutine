using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;

namespace Magitek.Logic.Warrior
{
    internal static class Aoe
    {
        internal static async Task<bool> SteelCyclone()
        {
			if (!WarriorSettings.Instance.UseDecimate)
				return false;

			if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
				return false;

			if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies) 
				return false;

			if (Core.Me.HasAura(Auras.NascentChaos) && ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50) 
				return await Spells.ChaoticCyclone.Cast(Core.Me);

			return await Spells.SteelCyclone.Cast(Core.Me);
		}


		internal static async Task<bool> Decimate()
        {
            if (!WarriorSettings.Instance.UseDecimate)
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos) && Core.Me.ClassLevel < 80)
            {
                return await Spells.Decimate.Cast(Core.Me);
            }

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies)
                return false;

            return await Spells.SteelCyclone.Cast(Core.Me);
        }
        public static async Task<bool> InnerReleaseDecimateSpam()
        {
            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (!WarriorSettings.Instance.UseDecimate)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies)
                return false;

            if (Casting.LastSpell == Spells.Decimate)
            {   //If Onslaught is allowed  //If Upheaval is allowed
                if (WarriorSettings.Instance.UseOnslaught && await Spells.Onslaught.Cast(Core.Me.CurrentTarget)) return true;
                if (WarriorSettings.Instance.UseUpheaval && await Spells.Upheaval.Cast(Core.Me.CurrentTarget)) return true;
            }

            await Spells.SteelCyclone.Cast(Core.Me.CurrentTarget);

            // Keep returning true as long as we have Inner Release
            return true;
        }

        public static async Task<bool> Overpower()
        {
            if (!WarriorSettings.Instance.UseOverpower)
                return false;

            if (!ActionManager.HasSpell(Spells.Overpower.Id))
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 8 + r.CombatReach) < WarriorSettings.Instance.OverpowerMinimumEnemies)
                return false;

            if (ActionManager.LastSpell == Spells.Overpower && Core.Me.ClassLevel >= 40)
            {
                return await Spells.MythrilTempest.Cast(Core.Me);
            }

            return await Spells.Overpower.Cast(Core.Me.CurrentTarget);
        }
    }
}