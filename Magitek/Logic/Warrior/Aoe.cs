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

			if (!Core.Me.HasAura(Auras.Defiance))
				return false;

			if (ActionResourceManager.Warrior.BeastGauge < 50 && !Core.Me.HasAura(Auras.InnerRelease))
				return false;

			if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies) 
				return false;

			if (Core.Me.HasAura(Auras.NascentChaos) && ActionResourceManager.Warrior.BeastGauge < 50) 
				return await Spells.ChaoticCyclone.Cast(Core.Me);

			return await Spells.Decimate.Cast(Core.Me);
		}


		internal static async Task<bool> Decimate()
        {
            if (!WarriorSettings.Instance.UseDecimate)
                return false;

            if (!Core.Me.HasAura(Auras.Deliverance))
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < 25)
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < 50)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < WarriorSettings.Instance.DecimateMinimumEnemies)
                return false;

            return await Spells.Decimate.Cast(Core.Me);
        }

       internal static async Task<bool> Overpower()
		
		
        {
            if (!WarriorSettings.Instance.UseOverpower) 
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            if (!Globals.InParty)
                return false;

            if (WarriorSettings.Instance.OverpowerNeverInterruptCombo)
            {
                if (Casting.LastSpell == Spells.HeavySwing)
                    return false;

                if (Casting.LastSpell == Spells.Maim)
                    return false;
            }
			if (ActionManager.LastSpell == Spells.MythrilTempest && WarriorSettings.Instance.UseOverpower && Core.Me.ClassLevel >= 40)
			{
				return await Spells.MythrilTempest.Cast(Core.Me);
			}
			return await Spells.Overpower.Cast(Core.Me.CurrentTarget);

			if (!ActionManager.HasSpell(Spells.Overpower.Id))
                return false;


            if (Core.Me.Distance(Core.Me.CurrentTarget) <= 8 + Core.Me.CombatReach && Combat.CombatTime.Elapsed.Seconds < 20 && Utilities.Routines.Warrior.PullOverpower < WarriorSettings.Instance.OverpowersOnPull)
            {
                if (Core.Me.CurrentTarget.EnemiesNearbyOoc(8 + Core.Me.CombatReach).Count() >= WarriorSettings.Instance.OverpowerMinimumEnemies)
                {
                    if (!await Spells.Overpower.Cast(Core.Me.CurrentTarget))
                        return true;

                    Utilities.Routines.Warrior.PullOverpower++;
                    Utilities.Routines.Warrior.LastOverpower = DateTime.Now;
                }
            }

            if (Combat.Enemies.Count(x => x.IsTargetable && x.InView() && x.Distance(Core.Me) <= 8 + x.CombatReach) < WarriorSettings.Instance.OverpowerMinimumEnemies)
                return false;

            if (Combat.Enemies.Count(x => x.IsTargetable && x.InView() && x.Distance(Core.Me) <= 8 + x.CombatReach && x.TargetGameObject != Core.Me) >= WarriorSettings.Instance.OverpowerMinimumEnemies)
            {
                return await Spells.Overpower.Cast(Core.Me.CurrentTarget);
            }

            if (Combat.CombatTime.Elapsed.Seconds < 20 && Utilities.Routines.Warrior.PullOverpower < WarriorSettings.Instance.OverpowersOnPull)
            {
                if (!await Spells.Overpower.Cast(Core.Me.CurrentTarget))
                    return false;

                Utilities.Routines.Warrior.PullOverpower++;
                Logger.WriteInfo($@"Using Overpower On Pull [{Utilities.Routines.Warrior.PullOverpower}]");
                Utilities.Routines.Warrior.LastOverpower = DateTime.Now;
            }

            if (!WarriorSettings.Instance.UseOverpowerInterval)
                return false;

            if (DateTime.Now <
                Utilities.Routines.Warrior.LastOverpower.AddSeconds(WarriorSettings.Instance.OverpowerIntervalSeconds))
                return false;

            if (!await Spells.Overpower.Cast(Core.Me.CurrentTarget))
                return false;

            Logger.WriteInfo($@"Using Overpower On Interval");

            Utilities.Routines.Warrior.LastOverpower = DateTime.Now;
            return true;
        }
    }
}