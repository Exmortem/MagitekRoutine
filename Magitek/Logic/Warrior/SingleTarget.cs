using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Warrior
{
    internal static class SingleTarget
    {
        public static async Task<bool> Tomahawk()
        {
            if (WarriorSettings.Instance.UseTomahawkToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                {
                    return await Spells.Tomahawk.Cast(pullTarget);
                }
            }

            if (!WarriorSettings.Instance.UseTomahawk)
            {
                if (WarriorSettings.Instance.UseTomahawkToPull && !Core.Me.InCombat)
                {
                    return await Spells.Tomahawk.Cast(Core.Me.CurrentTarget);
                }

                return false;
            }

            return await Spells.Tomahawk.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TomahawkOnLostAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (!WarriorSettings.Instance.UseDefiance)
                return false;

            if (!WarriorSettings.Instance.UseTomahawkOnLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var tomahawkTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (tomahawkTarget == null)
                return false;

            if (tomahawkTarget.TargetGameObject == null)
                return false;

            if (!await Spells.Tomahawk.Cast(tomahawkTarget))
                return false;

            Logger.Write($@"[Magitek] Tomahawk On {tomahawkTarget.Name} To Pull Aggro");
            return true;
        }

        
        internal static async Task<bool> Upheaval()
        {
            int upheavalReplaced = 1;

            if (Spells.HeavySwing.Cooldown.TotalMilliseconds < 600)
                return false;

            if (Spells.Upheaval.Cooldown.TotalMilliseconds > 0)
                return false;

            if (Core.Me.ClassLevel >= 70 && Spells.InnerRelease.Cooldown.TotalMilliseconds > 25000)
                upheavalReplaced = 0; 

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (!WarriorSettings.Instance.UseUpheaval)
                return false;

            // If we have Inner Release & CD for it is 25s or less, don't cast upheaval
            if (Core.Me.ClassLevel >= 70 && Spells.InnerRelease.Cooldown.TotalMilliseconds < 25000)
            {
                if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) < 1)
                {
                    upheavalReplaced = 1;
                    if (await Spells.Onslaught.Cast(Core.Me.CurrentTarget))
                        return true;
                }
                
                return false;
            }
            if (upheavalReplaced != 0)
                return false;

            if (Casting.LastSpell == Spells.InnerRelease)
                return false;

            return await Spells.Upheaval.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> InnerBeast()//Becomes Fell Cleave
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel > 54)
                return false;

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
                return false;

            return await Spells.InnerBeast.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> FellCleave()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel < 54)
                return false;

            if (Core.Me.ClassLevel < 80 && Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
                return false;

            return await Spells.FellCleave.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> StormsPath()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.Maim)
                return false;

            return await Spells.StormsPath.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> StormsEye()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.Maim)
                return false;

            // If we have Inner Release
            if (Core.Me.ClassLevel > 70)
            {
                // If Inner Release as 10 seconds or less left on cooldown
                if (Spells.InnerRelease.Cooldown.TotalMilliseconds < 10000)
                {
                    int refreshTime = 19000 + (int)Spells.InnerRelease.Cooldown.TotalMilliseconds; 
                    // If we don't have Storm's Eye aura for at least 10 seconds + Inner Release cooldown time
                    if (!Core.Me.HasAura(Auras.StormsEye, true, refreshTime))
                    {
                        // Use Storm's Eye
                        return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
                    }
                }
            }

            if (Core.Me.HasAura(Auras.StormsEye, true, 7000))
                return false;

            return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> Maim()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.HeavySwing)
                return false;

            return await Spells.Maim.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> HeavySwing()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            return await Spells.HeavySwing.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Onslaught()
        {
            if (!WarriorSettings.Instance.UseOnslaught)
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) < 1)
                return false;

            if (Casting.LastSpell == Spells.InnerRelease)
                return false;

            return await Spells.Onslaught.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> InnerReleaseFellCleaveSpam()
        {
            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (Casting.LastSpell == Spells.FellCleave)
            {   //If Onslaught is allowed
                if (WarriorSettings.Instance.UseOnslaught && await Spells.Onslaught.Cast(Core.Me.CurrentTarget)) return true;
                if (WarriorSettings.Instance.UseUpheaval && await Spells.Upheaval.Cast(Core.Me.CurrentTarget)) return true;
            }

            await Spells.FellCleave.Cast(Core.Me.CurrentTarget);

            // Keep returning true as long as we have Inner Release
            return true;
        }
    }
}